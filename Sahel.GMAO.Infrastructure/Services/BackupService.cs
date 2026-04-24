using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Sahel.GMAO.Core.Interfaces;
using Serilog;

namespace Sahel.GMAO.Infrastructure.Services
{
    public class BackupService : IBackupService
    {
        private readonly string _connectionString;
        private readonly string _dbName;

        public BackupService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("DefaultConnection not found");
            
            // Extract DB name from connection string
            var builder = new SqlConnectionStringBuilder(_connectionString);
            _dbName = builder.InitialCatalog;
        }

        public async Task<string> CreateBackupAsync(string? customPath = null)
        {
            string backupDir = !string.IsNullOrEmpty(customPath) ? customPath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);

            string fileName = $"SahelGmao_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string filePath = Path.Combine(backupDir, fileName);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = $@"BACKUP DATABASE [{_dbName}] TO DISK = @path WITH FORMAT, MEDIANAME = 'GmaoBackup', NAME = 'Full Backup of SahelGmao'";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@path", filePath);
            command.CommandTimeout = 300; // 5 minutes

            await command.ExecuteNonQueryAsync();
            
            Log.Information(">>> GMAO: Database backup created at {FilePath}", filePath);
            return filePath;
        }

        public async Task<string> ExportToSqliteAsync()
        {
            string exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
            if (!Directory.Exists(exportDir)) Directory.CreateDirectory(exportDir);

            string sqliteFileName = $"SahelGmao_Export_{DateTime.Now:yyyyMMdd_HHmmss}.db";
            string sqlitePath = Path.Combine(exportDir, sqliteFileName);

            try
            {
                SqliteConnection.ClearAllPools();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                using var sqlConn = new SqlConnection(_connectionString);
                await sqlConn.OpenAsync();

                using var sqliteConn = new SqliteConnection($"Data Source={sqlitePath}");
                await sqliteConn.OpenAsync();

                // List of tables to copy from GmaoDbContext
                string[] tables = { 
                    "Users", "Equipements", "DemandesTravail", "ArticlesPdr", 
                    "ConsommableUsages", "InterventionRoles", "MaintenancePreventives", 
                    "PointageInterventions", "RapportsIncidents", "DemandesFabrication", 
                    "NaturesTravail", "BonsDeConsignation", "FichesEntretienPreventif", 
                    "TachesEntretien", "MatieresFabrication", "IntervenantsFabrication", 
                    "PointagesMachinesFabrication", "Notifications", "InterventionLogs"
                };

                foreach (var table in tables)
                {
                    await CopyTableAsync(sqlConn, sqliteConn, table);
                }

                sqliteConn.Close();
                sqlConn.Close();

                SqliteConnection.ClearAllPools();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Log.Information(">>> GMAO: Database exported to SQLite at {FilePath}", sqlitePath);
                return sqliteFileName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ">>> GMAO: Error during SQLite export");
                if (File.Exists(sqlitePath)) File.Delete(sqlitePath);
                throw;
            }
        }

        private async Task CopyTableAsync(SqlConnection sqlConn, SqliteConnection sqliteConn, string tableName)
        {
            // 1. Check if table exists in SQL Server
            using var checkCmd = new SqlCommand($@"IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') SELECT 1 ELSE SELECT 0", sqlConn);
            var exists = (int)(await checkCmd.ExecuteScalarAsync() ?? 0);
            if (exists == 0) return;

            // 2. Get Schema
            using var cmd = new SqlCommand($"SELECT TOP 0 * FROM [{tableName}]", sqlConn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            var createSql = $"CREATE TABLE [{tableName}] (";
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var type = reader.GetFieldType(i);
                string sqliteType = type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) ? "INTEGER" :
                                    type == typeof(float) || type == typeof(double) || type == typeof(decimal) ? "REAL" : 
                                    type == typeof(DateTime) ? "TEXT" : "TEXT";
                
                createSql += $"[{name}] {sqliteType}, ";
                columnNames.Add(name);
            }
            createSql = createSql.TrimEnd(',', ' ') + ")";
            
            using (var createCmd = new SqliteCommand(createSql, sqliteConn)) await createCmd.ExecuteNonQueryAsync();
            reader.Close();

            // 3. Copy Data
            using var selectCmd = new SqlCommand($"SELECT * FROM [{tableName}]", sqlConn);
            using var dataReader = await selectCmd.ExecuteReaderAsync();

            using var transaction = sqliteConn.BeginTransaction();
            var insertSql = $"INSERT INTO [{tableName}] ({string.Join(",", columnNames.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", columnNames.Select(c => $"@{c}"))})";

            while (await dataReader.ReadAsync())
            {
                using var insertCmd = new SqliteCommand(insertSql, sqliteConn, transaction);
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    var val = dataReader.GetValue(i);
                    if (val is string s) val = s.TrimEnd();
                    if (val is DateTime dt) val = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    insertCmd.Parameters.AddWithValue($"@{columnNames[i]}", val ?? DBNull.Value);
                }
                await insertCmd.ExecuteNonQueryAsync();
            }
            transaction.Commit();
        }

        public async Task RestoreBackupAsync(string backupFilePath)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            builder.InitialCatalog = "master"; // Connect to master to restore target DB
            
            using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            // Set to single user to kick off connections, then restore, then set back to multi user
            string restoreQuery = $@"
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [{_dbName}] FROM DISK = @path WITH REPLACE;
                ALTER DATABASE [{_dbName}] SET MULTI_USER;";

            using var command = new SqlCommand(restoreQuery, connection);
            command.Parameters.AddWithValue("@path", backupFilePath);
            command.CommandTimeout = 600; // 10 minutes for large restores

            await command.ExecuteNonQueryAsync();
            
            // Clear EF pools to avoid using stale connections
            SqlConnection.ClearAllPools();
            Log.Information(">>> GMAO: Database restored from {FilePath}", backupFilePath);
        }
    }
}
