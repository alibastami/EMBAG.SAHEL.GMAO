using System.Threading.Tasks;

namespace Sahel.GMAO.Core.Interfaces
{
    public interface IBackupService
    {
        /// <summary>
        /// Creates a SQL Server backup (.bak) of the current database.
        /// </summary>
        /// <returns>The path to the generated backup file.</returns>
        Task<string> CreateBackupAsync(string? customPath = null);

        /// <summary>
        /// Exports the current SQL Server database to a portable SQLite (.db) file.
        /// </summary>
        /// <returns>The filename of the generated SQLite file.</returns>
        Task<string> ExportToSqliteAsync();
        
        /// <summary>
        /// Restores a SQL Server backup (.bak) to the current database.
        /// </summary>
        /// <param name="backupFilePath">The path to the .bak file.</param>
        Task RestoreBackupAsync(string backupFilePath);
    }
}
