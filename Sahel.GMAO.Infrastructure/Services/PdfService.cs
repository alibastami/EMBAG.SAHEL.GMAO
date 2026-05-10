using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;

namespace Sahel.GMAO.Infrastructure.Services;

public class PdfService : IPdfService
{
    static PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateDtPdfAsync(DemandeTravail dt)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeFormHeader);

                page.Content().PaddingTop(15).Column(column =>
                {
                    column.Spacing(0);
                    
                    // Main Bordered Table
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1);
                        });

                        // Row 1: DEMANDE DE TRAVAIL N°
                        table.Cell().Border(1).Padding(2).AlignCenter().Text($"DEMANDE DE TRAVAIL          N°  {dt.NumeroDT}").SemiBold();
                        
                        // Row 2: DEMANDEUR header
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("DEMANDEUR").Bold();

                        // Row 3: Demande details
                        table.Cell().Border(1).Row(row =>
                        {
                            row.RelativeItem(3).Padding(2).Column(c =>
                            {
                                c.Item().Text(t =>
                                {
                                    t.Span("Equipement : ").Bold();
                                    t.Span($"{dt.Equipement.Designation} ..... ");
                                    t.Span("Code : ").Bold();
                                    t.Span($"{dt.Equipement.Code} ..... ");
                                    t.Span("Organe : ").Bold();
                                    t.Span($"{dt.OrganePartie} .....");
                                });
                                c.Item().Text("Travail demandé : (préciser toutes informations utiles).").Bold().Underline();
                                c.Item().Text(dt.TravailDemande);
                                c.Item().Text("........................................................................................................................................................");
                                c.Item().Text("........................................................................................................................................................");
                            });
                            row.RelativeItem(1).BorderLeft(1).Padding(2).Column(c =>
                            {
                                c.Item().Text("Emission DT").Bold().Underline();
                                c.Item().Text($"Date: {dt.DateEmission:dd/MM/yy}   A {dt.DateEmission:HH}H{dt.DateEmission:mm}");
                                c.Item().Text($"Mr : {dt.Demandeur?.FullName ?? "........................"}");
                                c.Item().Text("Visa");
                            });
                        });

                        // Row 4: PREPARATION header
                        table.Cell().Border(1).Row(row =>
                        {
                            row.RelativeItem(1).Padding(2).Text($"Temps de marche : {dt.TempsDeMarcheHeures?.ToString() ?? ".........."} H").Bold();
                            row.RelativeItem(3).AlignCenter().PaddingTop(2).Text("PREPARATION").Bold();
                        });

                        // Row 5: Consignes / Reception
                        table.Cell().Border(1).Row(row =>
                        {
                            row.RelativeItem(3).Padding(2).Column(c =>
                            {
                                c.Item().Text("Consignes :").Bold().Underline();
                                c.Item().Text(dt.InstructionsPreparation ?? "");
                                c.Item().Text("........................................................................................................................................................");
                                c.Item().Text("........................................................................................................................................................");
                            });
                            row.RelativeItem(1).BorderLeft(1).Padding(2).Column(c =>
                            {
                                c.Item().Text("Réception DT").Bold().Underline();
                                c.Item().Text("Date.../.../....  A ...H....");
                                c.Item().Text("Visa");
                            });
                        });

                        // Row 6: INTERVENTION header
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("INTERVENTION").Bold();

                        // Row 7: Execution Details
                        table.Cell().Border(1).Row(row =>
                        {
                            row.RelativeItem(3).Padding(2).Column(c =>
                            {
                                c.Item().Text("Travail exécuté :").Bold().Underline();
                                c.Item().Text(dt.TravailExecute ?? "");
                                c.Item().Text("........................................................................................................................................................");
                                c.Item().Text("........................................................................................................................................................");
                                c.Item().Text("........................................................................................................................................................");
                                c.Item().Text("........................................................................................................................................................");
                            });
                            row.RelativeItem(1).BorderLeft(1).Column(c =>
                            {
                                c.Item().Padding(2).AlignCenter().Text("Début du travail").Bold();
                                c.Item().Padding(2).AlignCenter().Text(dt.DateExecutionDebut.HasValue ? $"Date: {dt.DateExecutionDebut.Value:dd/MM/yy}   A {dt.DateExecutionDebut.Value:HH}H{dt.DateExecutionDebut.Value:mm}" : "Date.../.../....  A ...H....");
                                c.Item().BorderTop(1).Padding(2).AlignCenter().Text("Fin du travail").Bold();
                                c.Item().Padding(2).AlignCenter().Text(dt.DateExecutionFin.HasValue ? $"Date: {dt.DateExecutionFin.Value:dd/MM/yy}   A {dt.DateExecutionFin.Value:HH}H{dt.DateExecutionFin.Value:mm}" : "Date.../.../....  A ...H....");
                                c.Item().BorderTop(1).Padding(2).AlignCenter().Text("Arrêt de production").Bold();
                                c.Item().Padding(2).AlignCenter().Text($"{dt.DureeArretProductionHeures} H");
                                c.Item().BorderTop(1).Padding(2).Text("Visa intervention").Bold();
                                c.Item().PaddingBottom(15);
                            });
                        });

                    }); // End main table
                    
                    // Timesheet Grid at bottom of Page 1
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(80); // Noms
                            cols.ConstantColumn(15); // Q
                            for(int i=0; i<31; i++) cols.RelativeColumn();
                            cols.ConstantColumn(25); // H.H
                        });

                        table.Cell().Border(1).Padding(1).Text("Noms").FontSize(8);
                        table.Cell().Border(1).Padding(1).Text("Q").FontSize(8);
                        for(int i=1; i<=31; i++) {
                            table.Cell().Border(1).Padding(1).AlignCenter().Text(i.ToString()).FontSize(6);
                        }
                        table.Cell().Border(1).Padding(1).Text("H.H").FontSize(8);

                        var intervenants = dt.Intervenants?.ToList() ?? new List<InterventionRole>();
                        
                        for(int r = 0; r < intervenants.Count; r++)
                        {
                            var inv = r < intervenants.Count ? intervenants[r] : null;

                            table.Cell().Border(1).Padding(1).Text(inv?.Intervenant?.FullName ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(1).AlignCenter().Text(AbbreviateQualification(inv?.Qualification ?? "")).FontSize(8);
                            
                            for(int i=1; i<=31; i++) {
                                var pointage = inv?.Pointages?.FirstOrDefault(p => p.JourDuMois == i);
                                string val = pointage != null ? pointage.HeuresTravaillees.ToString() : "";
                                table.Cell().Border(1).Padding(1).AlignCenter().Text(val).FontSize(6);
                            }
                            table.Cell().Border(1).Padding(1).AlignCenter().Text(inv != null && inv.HeuresTravaillees > 0 ? inv.HeuresTravaillees.ToString() : "").FontSize(8);
                        }
                    });
                    
                    // PAGE 2 BREAK
                    column.Item().PageBreak();

                    // PAGE 2 Header (PREPARATION at the top of the content in Page 2)
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("PREPARATION").Bold();
                        table.Cell().Border(1).BorderBottom(0).Padding(2).Text("COUT PIECES ET MATIERES CONSOMMABLES").Bold();
                    });

                    // Cost PIECES
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(50); // N° BSM
                            cols.RelativeColumn(3); // DESIGNATION
                            cols.ConstantColumn(40); // QTE
                            cols.RelativeColumn(1); // PRIX UNITAIRE
                            cols.RelativeColumn(1); // PRIX TOTAL
                            cols.RelativeColumn(2); // OBSERVATION
                        });

                        Action<string> drawHeader = (text) => table.Cell().Border(1).Padding(2).Text(text).FontSize(8);
                        drawHeader("N° BSM"); drawHeader("DESIGNATION"); drawHeader("QTE"); 
                        drawHeader("PRIX UNITAIRE"); drawHeader("PRIX TOTAL"); drawHeader("OBSERVATION");

                        var consommables = dt.Consommables?.ToList() ?? new List<ConsommableUsage>();

                        for(int r=0; r < consommables.Count; r++)
                        {
                            var c = r < consommables.Count ? consommables[r] : null;
                            table.Cell().Border(1).Padding(2).Text(c?.N_BSM ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c?.ArticlePdr?.Designation ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? c.Quantite.ToString() : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? c.PrixUnitaireApplique.ToString("N2") : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? ((decimal)c.Quantite * c.PrixUnitaireApplique).ToString("N2") : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c?.Observation ?? "").FontSize(8);
                        }
                    });

                    // COUT TOTAL pieces
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(1); });
                        table.Cell().BorderLeft(1).Padding(2).AlignRight().Text("COUT TOTAL").Bold().FontSize(8);
                        table.Cell().Border(2).Padding(2).AlignCenter().Text(dt.TotalCoutPieces > 0 ? dt.TotalCoutPieces.ToString("N2") : "").Bold().FontSize(8);
                        table.Cell().BorderRight(1).Padding(2).Text("");
                    });

                    // COUT MAIN D'OEUVRE 
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).BorderBottom(0).Padding(2).Text("COUT MAIN D'OEUVRE").Bold();
                    });

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3); // NOMS ET PRENOMS
                            cols.RelativeColumn(2); // QUALIFICATION
                            cols.RelativeColumn(1); // NBRE HEURES
                            cols.RelativeColumn(1); // COUT HORAIRE
                            cols.RelativeColumn(1); // TOTAL
                        });

                        Action<string> drawHeader = (text) => table.Cell().Border(1).Padding(2).AlignCenter().Text(text).FontSize(8);
                        drawHeader("NOMS ET PRENOMS"); drawHeader("QUALIFICATION"); drawHeader("NBRE HEURES"); 
                        drawHeader("COUT HORAIRE"); drawHeader("TOTAL");

                        var intervenantsMO = dt.Intervenants?.ToList() ?? new List<InterventionRole>();

                        for(int r=0; r < intervenantsMO.Count; r++)
                        {
                            var inv = r < intervenantsMO.Count ? intervenantsMO[r] : null;
                            table.Cell().Border(1).Padding(2).Text(inv?.Intervenant?.FullName ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(2).AlignCenter().Text(AbbreviateQualification(inv?.Qualification ?? "")).FontSize(8);
                            table.Cell().Border(1).Padding(2).AlignCenter().Text(inv != null && inv.HeuresTravaillees > 0 ? inv.HeuresTravaillees.ToString() : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).AlignCenter().Text(inv != null && inv.TauxHoraire > 0 ? inv.TauxHoraire.ToString("N2") : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).AlignCenter().Text(inv != null && inv.HeuresTravaillees > 0 ? (inv.HeuresTravaillees * (double)inv.TauxHoraire).ToString("N2") : "").FontSize(8);
                        }
                    });

                    // COUT TOTAL main oeuvre
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => { cols.RelativeColumn(3); cols.RelativeColumn(1); });
                        table.Cell().BorderLeft(1).BorderBottom(1).Padding(2).AlignRight().Text("COUT TOTAL").Bold().FontSize(8);
                        table.Cell().Border(2).Padding(2).AlignCenter().Text(dt.TotalCoutMainOeuvre > 0 ? dt.TotalCoutMainOeuvre.ToString("N2") : "").Bold().FontSize(8);
                    });

                    // SUMMARY FOOTER Table
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(1); });
                        table.Cell().Border(1).Padding(2).Column(c =>
                        {
                            c.Item().Text("Remarques :").Bold().Underline();
                            c.Item().Text("................................................................................................");
                            c.Item().Text("................................................................................................");
                            c.Item().Text("................................................................................................");
                            c.Item().Text("................................................................................................");
                            c.Item().Text("................................................................................................");
                        });
                        table.Cell().Border(1).Padding(2).Column(c =>
                        {
                            c.Item().Text("COUT TOTAL DE L'OPERATION").Bold().FontSize(8);
                            c.Item().PaddingTop(5).AlignCenter().Text($"{(dt.TotalCoutPieces + dt.TotalCoutMainOeuvre):N2} DA").Bold().Underline();
                            c.Item().BorderTop(1).PaddingTop(5).Text("Visa préparation").Bold().FontSize(8);
                            c.Item().Text("Date.......................................").FontSize(8);
                            c.Item().Text("Nom :.......................................").FontSize(8);
                            c.Item().Text("Visa :").FontSize(8);
                            c.Item().PaddingBottom(20);
                        });
                    });
                }); // End Column
            });
        });

        return document.GeneratePdf();
    }

    private string AbbreviateQualification(string qualification)
    {
        if (string.IsNullOrWhiteSpace(qualification)) return "";

        qualification = qualification.Trim().ToUpper();

        if (qualification.Contains("TECHNICIEN")) return "TE";
        if (qualification.Contains("ELECTRICIEN") || qualification.Contains("ÉLECTRICIEN")) return "EL";
        if (qualification.Contains("MECANICIEN") || qualification.Contains("MÉCANICIEN")) return "ME";
        if (qualification.Contains("SOUDEUR")) return "SO";
        if (qualification.Contains("TOURNEUR")) return "TO";
        if (qualification.Contains("AJUSTEUR")) return "AJ";
        if (qualification.Contains("FRAISEUR")) return "FR";
        if (qualification.Contains("MANOEUVRE") || qualification.Contains("MANŒUVRE")) return "MA";
        if (qualification.Contains("CONTREMAITRE")) return "CM";
        if (qualification.Contains("INGENIEUR")) return "IN";
        if (qualification.Contains("AIDE")) return "AI";

        if (qualification.Length <= 2) return qualification;
        return qualification.Substring(0, 2);
    }

    private void ComposeFormHeader(IContainer container)
    {
        ComposeFormHeader(container, "DEMANDE DE TRAVAIL", "01");
    }

    private void ComposeFormHeader(IContainer container, string docTitle, string docCode, string version = "01")
    {
        container.Border(1).BorderColor("#553C7B").Row(row =>
        {
            // Left Box: Logo area
            row.ConstantItem(120).Padding(5).Column(col => 
            {
                col.Item().Text(t => 
                {
                    t.Span("GIPEC").FontColor("#008000").Bold().FontSize(16);
                });
                col.Item().PaddingTop(5).Text("EMBAG-SPA").Bold().Italic().FontSize(9);
            });
            
            // Middle: Title
            row.RelativeItem().PaddingTop(10).AlignCenter().Text(docTitle).Bold().FontSize(12);

            // Right Box: Versioning
            row.ConstantItem(100).BorderLeft(1).BorderColor("#553C7B").Padding(5).Column(col => 
            {
                col.Item().AlignCenter().Text($"IMP MAIN {docCode}").Bold().FontSize(9);
                col.Item().AlignCenter().Text($"Version: {version}").Bold().FontColor("#D81B60").FontSize(9);
            });
        });
    }

    public async Task<byte[]> GenerateFabricationPdfAsync(DemandeFabrication df)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(ComposeFormHeader); // Use the same logo design as DT

                page.Content().PaddingTop(10).Column(column =>
                {
                    // Page 1: Order details and machine tracking
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).Padding(2).AlignCenter().Text($"DEMANDE DE FABRICATION          N° {df.NumeroFabrication}").Bold();
                        
                        table.Cell().Border(1).Row(row => {
                            row.RelativeItem(3).Padding(2).Column(c => {
                                c.Item().Text(t => {
                                    t.Span("Destination : ").Bold();
                                    t.Span($"{df.Equipement?.Code} - {df.Equipement?.Designation}");
                                });
                                c.Item().PaddingTop(5).Text("Travail demandé :").Bold().Underline();
                                c.Item().Text(df.DesignationPiece);
                                c.Item().Text(df.Observations ?? "");
                            });
                            row.RelativeItem(1).BorderLeft(1).Padding(2).Column(c => {
                                c.Item().Text("Emission DF").Bold().Underline();
                                c.Item().Text($"Date: {df.DateEmission:dd/MM/yy}");
                                c.Item().Text("Visa");
                            });
                        });

                        // PREPARATION section
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("PREPARATION").Bold();
                        table.Cell().Border(1).Row(row => {
                            row.RelativeItem(1).BorderRight(1).Padding(2).Text($"Réf Pièce: {df.ReferencePiece}");
                            row.RelativeItem(1).BorderRight(1).Padding(2).Text($"Code Art: {df.CodeArticle}");
                            row.RelativeItem(1).BorderRight(1).Padding(2).Text($"Qté: {df.Quantite}");
                            row.RelativeItem(1).BorderRight(1).Padding(2).Text($"N° Dessin: {df.NDessin}");
                            row.RelativeItem(1).Padding(2).Text($"Matière: {df.Matiere}");
                        });

                        // EXECUTION section
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("EXECUTION").Bold();
                        
                        // Machine Grid (Simplified for PDF)
                        table.Cell().Border(1).Table(t => {
                            t.ColumnsDefinition(c => {
                                c.RelativeColumn(2); // Agents
                                for(int i=0; i<8; i++) c.RelativeColumn(); // Machines
                                c.RelativeColumn(); // H.H
                            });
                            
                            t.Cell().Border(1).Padding(1).Text("Agents").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("T200").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("T250").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("T300").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("Frais.").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("Et.Lim").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("Aff.").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("Soud.").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("Autres").FontSize(7);
                            t.Cell().Border(1).Padding(1).Text("H.H").FontSize(7);

                            foreach(var p in df.PointagesMachines) {
                                t.Cell().Border(1).Padding(1).Text(p.Intervenant?.FullName ?? "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.Tour200Heures > 0 ? p.Tour200Heures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.Tour250Heures > 0 ? p.Tour250Heures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.Tour300Heures > 0 ? p.Tour300Heures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.FraisageHeures > 0 ? p.FraisageHeures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.EtauLimeurHeures > 0 ? p.EtauLimeurHeures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.AffutageHeures > 0 ? p.AffutageHeures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.SoudureHeures > 0 ? p.SoudureHeures.ToString() : "").FontSize(7);
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(p.AutresHeures > 0 ? p.AutresHeures.ToString() : "").FontSize(7);
                                var totalH = p.Tour200Heures + p.Tour250Heures + p.Tour300Heures + p.FraisageHeures + p.EtauLimeurHeures + p.AffutageHeures + p.SoudureHeures + p.AutresHeures;
                                t.Cell().Border(1).Padding(1).AlignCenter().Text(totalH > 0 ? totalH.ToString() : "").FontSize(7).Bold();
                            }
                        });
                    });

                    column.Item().PageBreak();

                    // Page 2: Preparation costs
                    column.Item().Table(table => {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("PREPARATION (COUT)").Bold();
                        table.Cell().Border(1).Padding(2).Text("COUT PIECES ET MATIERES CONSOMMABLES").Bold().FontSize(8);
                        
                        table.Cell().Border(1).Table(t => {
                            t.ColumnsDefinition(c => {
                                c.ConstantColumn(40); // BSM
                                c.RelativeColumn(3); // Matiere
                                c.RelativeColumn(1); // Section
                                c.RelativeColumn(1); // Longueur
                                c.RelativeColumn(1); // Prix
                                c.RelativeColumn(2); // Observation
                            });
                            t.Cell().Border(1).Text("N° BSM").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Désignation").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Section").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Longueur").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Prix (DA)").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Observation").FontSize(7).Bold();

                            foreach(var m in df.MatieresConsommees) {
                                t.Cell().Border(1).Text(m.N_BSM).FontSize(7);
                                t.Cell().Border(1).Text(m.DesignationMatiere).FontSize(7);
                                t.Cell().Border(1).Text(m.Section ?? "").FontSize(7);
                                t.Cell().Border(1).Text(m.Longueur.ToString("N2") ?? "").FontSize(7);
                                t.Cell().Border(1).Text(m.Prix.ToString("N2")).FontSize(7);
                                t.Cell().Border(1).Text(m.Observation ?? "").FontSize(7);
                            }
                        });

                        table.Cell().AlignRight().Padding(2).Text($"TOTAL PIECES: {df.TotalCoutPieces:N2} DA").Bold();

                        table.Cell().Border(1).Padding(2).Text("COUT MAIN D'OEUVRE").Bold().FontSize(8);
                        table.Cell().Border(1).Table(t => {
                            t.ColumnsDefinition(c => {
                                c.RelativeColumn(3); // Nom
                                c.RelativeColumn(1); // Heures
                                c.RelativeColumn(1); // Total
                            });
                            foreach(var i in df.Intervenants) {
                                t.Cell().Border(1).Text(i.NomsEtPrenoms).FontSize(7);
                                t.Cell().Border(1).Text(i.NombreHeures.ToString()).FontSize(7);
                                t.Cell().Border(1).Text(i.Total.ToString("N2")).FontSize(7);
                            }
                        });

                        table.Cell().Border(1).Row(r => {
                            r.RelativeItem(2).Padding(2).Column(c => {
                                c.Item().Text("Remarques :.................................................................................................................................").FontSize(8);
                                c.Item().Text("............................................................................................................................................").FontSize(8);
                            });
                            r.RelativeItem(1).BorderLeft(1).Padding(2).Column(c => {
                                c.Item().Text("Visa préparation").FontSize(8).Bold();
                                c.Item().Text("Date....................").FontSize(8);
                                c.Item().Text("Nom : .................").FontSize(8);
                                c.Item().Text("Visa :").FontSize(8);
                            });
                            r.RelativeItem(1).BorderLeft(1).Padding(2).Column(c => {
                                c.Item().Text($"COUT TOTAL DE L'OPERATION:").FontSize(8).Bold();
                                c.Item().AlignCenter().PaddingTop(5).Text($"{df.TotalCoutOperation:N2} DA").Bold().FontSize(10).Underline();
                            });
                        });
                        
                        table.Cell().Border(1).Row(r => {
                            r.RelativeItem(2).Padding(2).Text("");
                            r.RelativeItem(2).BorderLeft(1).Padding(2).Column(c => {
                                c.Item().Text("Visa comptabilité analytique").FontSize(8).Bold();
                                c.Item().Text("Date....................").FontSize(8);
                                c.Item().Text("Nom : .................").FontSize(8);
                                c.Item().Text("Visa :").FontSize(8);
                            });
                        });
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateConsignationPdfAsync(BonDeConsignation bon)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(c => ComposeFormHeader(c, "BON DE CONSIGNATION ET DECONSIGNATION", "04"));

                page.Content().PaddingTop(20).Column(column =>
                {
                    column.Item().Border(1).Padding(5).AlignCenter().Text("BON DE CONSIGNATION ET DECONSIGNATION").Bold().FontSize(12);
                    
                    column.Item().PaddingTop(20).Table(table => {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).Padding(5).Column(c => {
                            c.Item().AlignCenter().Text("CONSIGNATION").Bold().Underline();
                            c.Item().PaddingTop(10).Text(t => {
                                t.Span("L'équipement : ").Bold(); t.Span($"{bon.Equipement?.Designation}  ");
                                t.Span("Code : ").Bold(); t.Span($"{bon.Equipement?.Code}  ");
                                t.Span("Atelier : ").Bold(); t.Span($"{bon.Atelier}");
                            });
                            c.Item().PaddingTop(5).Text(t => {
                                t.Span("Est consigné le ").Bold(); t.Span($"{bon.DateHeureConsignation:dd/MM/yyyy}");
                                t.Span(" à ").Bold(); t.Span($"{bon.DateHeureConsignation:HH:mm}");
                            });
                            c.Item().PaddingTop(5).Text("Pour les raisons suivantes :").Bold();
                            c.Item().Text(bon.MotifConsignation ?? "");
                            
                            c.Item().PaddingTop(20).Row(r => {
                                r.RelativeItem().Column(vc => {
                                    vc.Item().Text("Maintenance").Bold();
                                    vc.Item().Text($"Mr : {bon.AgentConsignation?.FullName}");
                                    vc.Item().Text("Visa");
                                });
                                r.RelativeItem().Column(vc => {
                                    vc.Item().Text("Service Utilisateur").Bold();
                                    vc.Item().Text($"Mr : {bon.ServiceUtilisateurConsignation}");
                                    vc.Item().Text("Visa");
                                });
                            });
                        });

                        table.Cell().Border(1).Padding(5).Column(c => {
                            c.Item().AlignCenter().Text("DECONSIGNATION").Bold().Underline();
                            c.Item().PaddingTop(10).Text(t => {
                                t.Span("Est déconsigné le ").Bold(); t.Span($"{bon.DateHeureDeconsignation:dd/MM/yyyy}");
                                t.Span(" à ").Bold(); t.Span($"{bon.DateHeureDeconsignation:HH:mm}");
                            });
                            c.Item().PaddingTop(20).Row(r => {
                                r.RelativeItem().Column(vc => {
                                    vc.Item().Text("Maintenance").Bold();
                                    vc.Item().Text($"Mr : {bon.AgentDeconsignation?.FullName}");
                                    vc.Item().Text("Visa");
                                });
                                r.RelativeItem().Column(vc => {
                                    vc.Item().Text("Service Utilisateur").Bold();
                                    vc.Item().Text($"Mr : {bon.ServiceUtilisateurDeconsignation}");
                                    vc.Item().Text("Visa");
                                });
                            });
                        });
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateFicheEntretienPdfAsync(FicheEntretienPreventif fiche)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(c => ComposeFormHeader(c, "FICHE D'ENTRETIEN PREVENTIF PERIODIQUE", "03"));

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Table(table => {
                        table.ColumnsDefinition(cols => cols.RelativeColumn());
                        table.Cell().Border(1).Padding(2).AlignCenter().Text("FICHE D'ENTRETIEN PREVENTIF PERIODIQUE").Bold();
                        
                        table.Cell().Border(1).Row(row => {
                            row.RelativeItem().Padding(2).Column(c => {
                                c.Item().Text($"N° OT: {fiche.NumeroOT}");
                                c.Item().Text($"PARTIE: {fiche.Partie}");
                                c.Item().Text($"SECTION: {fiche.Section}");
                            });
                            row.RelativeItem().BorderLeft(1).Padding(2).Column(c => {
                                c.Item().Text($"CODE: {fiche.Equipement?.Code}");
                                c.Item().Text($"PERIODICITE: {fiche.Periodicite}");
                                c.Item().Text($"EQUIPEMENT: {fiche.Equipement?.Designation}");
                            });
                        });

                        table.Cell().Border(1).Table(t => {
                            t.ColumnsDefinition(c => {
                                c.ConstantColumn(20); // Pos
                                c.RelativeColumn(2); // Organes
                                c.RelativeColumn(3); // Opération
                                c.ConstantColumn(30); // Fait
                                c.ConstantColumn(40); // Temps Prévu
                                c.ConstantColumn(40); // Temps Réalisé
                                c.RelativeColumn(2); // Obs
                                c.RelativeColumn(2); // Suite à donner (8th col)
                            });
                            t.Cell().Border(1).AlignCenter().Text("Pos").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Organes").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Opération à effectuer").FontSize(7).Bold();
                            t.Cell().Border(1).AlignCenter().Text("Fait").FontSize(7).Bold();
                            t.Cell().Border(1).AlignCenter().Text("T. Prévu").FontSize(7).Bold();
                            t.Cell().Border(1).AlignCenter().Text("T. Réal").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Observation").FontSize(7).Bold();
                            t.Cell().Border(1).Text("Suite à donner").FontSize(7).Bold();

                            foreach(var item in fiche.Taches) {
                                t.Cell().Border(1).AlignCenter().Text(item.Position.ToString()).FontSize(7);
                                t.Cell().Border(1).Text(item.Organes).FontSize(7);
                                t.Cell().Border(1).Text(item.OperationAEffectuer).FontSize(7);
                                t.Cell().Border(1).AlignCenter().Text(item.EstFait ? "1" : "0").FontSize(7);
                                t.Cell().Border(1).AlignCenter().Text(item.TempsPrevuHeures.ToString()).FontSize(7);
                                t.Cell().Border(1).AlignCenter().Text(item.TempsRealiseHeures.ToString()).FontSize(7);
                                t.Cell().Border(1).Text(item.Observation ?? "").FontSize(7);
                                t.Cell().Border(1).Text(item.SuiteADonner ?? "").FontSize(7);
                            }
                        });

                        table.Cell().Border(1).Padding(5).AlignRight().Text($"Fait le : {fiche.DateFaitLe:dd/MM/yyyy}   Visa : {fiche.Intervenant?.FullName}").FontSize(8);
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateEquipementHistoryPdfAsync(Equipement equipement, List<DemandeTravail> interventions)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Template is explicitly landscape
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                
                // Add the new global header standard (we will implement this across all in Phase 4)
                page.Header().Element(c => ComposeFormHeader(c, "FICHE HISTORIQUE EQUIPEMENT", "02"));

                page.Content().PaddingTop(15).Column(column =>
                {
                    // Upper section info: Equipement, Code, Fait le
                    column.Item().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem(2).Text(t =>
                        {
                            t.Span("Equipement : ").Bold();
                            t.Span($"{equipement.Designation}");
                        });
                        row.RelativeItem(1).Text(t =>
                        {
                            t.Span("Code : ").Bold();
                            t.Span($"{equipement.Code}");
                        });
                        row.RelativeItem(1).AlignRight().Text(t =>
                        {
                            t.Span("Fait le : ").Bold();
                            t.Span($"{DateTime.Now:dd/MM/yyyy}");
                        });
                    });

                    column.Item().Table(table =>
                    {
                        // Match exactly the 11 columns of DOC 02
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(50);  // N° OT
                            cols.ConstantColumn(40);  // Temps marche
                            cols.RelativeColumn();    // Travaux/Causes
                            cols.ConstantColumn(40);  // Tps Interv
                            cols.ConstantColumn(90);  // PDR Designation
                            cols.ConstantColumn(60);  // PDR Reference
                            cols.ConstantColumn(60);  // PDR Code
                            cols.ConstantColumn(50);  // Cout PDR
                            cols.ConstantColumn(50);  // Cout MO
                            cols.ConstantColumn(60);  // Cout Oper
                            cols.ConstantColumn(60);  // Date Interv
                        });

                        Action<string> drawHeader = (text) => table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text(text).FontSize(8).Bold();

                        // Header Row 1 (Top headers spanning cells)
                        table.Cell().RowSpan(2).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text("N° OT").FontSize(8).Bold();
                        table.Cell().RowSpan(2).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text("Temps\nmarche").FontSize(8).Bold();
                        table.Cell().RowSpan(2).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text("Travaux effectués\net causes de la panne").FontSize(8).Bold();
                        table.Cell().RowSpan(2).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text("Tps\nInterv.").FontSize(8).Bold();
                        
                        table.Cell().ColumnSpan(3).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text("Consommation PDR et Consommable").FontSize(8).Bold();
                        
                        table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text("Coût").FontSize(8).Bold();
                        table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text("Coût").FontSize(8).Bold();
                        table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text("Coût").FontSize(8).Bold();
                        table.Cell().RowSpan(2).Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text("Date\nInterv.").FontSize(8).Bold();

                        // Header Row 2 (Sub-headers for PDR and Costs)
                        drawHeader("Désignation");
                        drawHeader("Référence");
                        drawHeader("Code");
                        drawHeader("PDR");
                        drawHeader("M.O");
                        drawHeader("Opér.");

                        Action<string> drawCell = (text) => table.Cell().Border(1).Padding(2).AlignCenter().AlignMiddle().Text(text).FontSize(7);
                        Action<string> drawLeftCell = (text) => table.Cell().Border(1).Padding(2).AlignLeft().AlignMiddle().Text(text).FontSize(7);

                        foreach (var dt in interventions.OrderByDescending(d => d.DateEmission))
                        {
                            // Each DT can span multiple rows if it has multiple PDRs. We join PDRs with line breaks or show 1 row if none.
                            var pdrs = dt.Consommables?.ToList() ?? new List<ConsommableUsage>();
                            string pdrsDesignation = pdrs.Any() ? string.Join("\n", pdrs.Select(p => p.ArticlePdr?.Designation)) : "-";
                            string pdrsReference = pdrs.Any() ? string.Join("\n", pdrs.Select(p => p.ArticlePdr?.ReferenceConstructeur ?? "-")) : "-";
                            string pdrsCode = pdrs.Any() ? string.Join("\n", pdrs.Select(p => p.ArticlePdr?.CodeArticle)) : "-";

                            // Total intervention time
                            double tpsInterv = dt.Intervenants?.Sum(i => i.HeuresTravaillees) ?? 0;

                            drawCell(dt.NumeroDT);
                            drawCell(dt.TempsDeMarcheHeures?.ToString() ?? "-");
                            
                            // Travaux et Causes
                            string travaux = $"Travaux: {dt.TravailExecute ?? "-"}\nPanne: {dt.TravailDemande ?? "-"}";
                            drawLeftCell(travaux);
                            
                            drawCell(tpsInterv > 0 ? tpsInterv.ToString() : "-");
                            
                            // PDRs
                            drawLeftCell(pdrsDesignation);
                            drawCell(pdrsReference);
                            drawCell(pdrsCode);

                            // Costs
                            drawCell(dt.TotalCoutPieces > 0 ? dt.TotalCoutPieces.ToString("N2") : "-");
                            drawCell(dt.TotalCoutMainOeuvre > 0 ? dt.TotalCoutMainOeuvre.ToString("N2") : "-");
                            drawCell(dt.TotalCoutOperation > 0 ? dt.TotalCoutOperation.ToString("N2") : "-");

                            // Date
                            drawCell(dt.DateExecutionFin.HasValue ? dt.DateExecutionFin.Value.ToString("dd/MM/yyyy") : dt.DateEmission.ToString("dd/MM/yyyy"));
                        }
                    });
                    
                    column.Item().PaddingTop(20).AlignRight().Text("Visa : ...................................").Bold();
                });

                page.Footer().Element(ComposeFooter);
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateStockStatePdfAsync(List<ArticlePdr> articles)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(c => ComposeHeader(c, "ÉTAT DE STOCK PDR", DateTime.Now.ToString("dd/MM/yyyy")));

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(TableHeader).Text("Code");
                        header.Cell().Element(TableHeader).Text("Désignation");
                        header.Cell().Element(TableHeader).Text("Stock");
                        header.Cell().Element(TableHeader).Text("Seuil");
                    });

                    foreach (var art in articles)
                    {
                        table.Cell().Element(TableCell).Text(art.CodeArticle);
                        table.Cell().Element(TableCell).Text(art.Designation);
                        table.Cell().Element(TableCell).Text(art.QuantiteEnStock.ToString());
                        table.Cell().Element(TableCell).Text(art.SeuilAlerte.ToString());
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GeneratePlanningPdfAsync(List<MaintenancePreventive> preventives)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(c => ComposeFormHeader(c, "PLANNING D'ENTRETIEN PREVENTIF PERIODIQUE", "07"));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    var grouped = preventives
                        .Where(p => p.IsActive)
                        .OrderBy(p => p.DateProchaineEcheance)
                        .GroupBy(p => new DateTime(p.DateProchaineEcheance.Year, p.DateProchaineEcheance.Month, 1));

                    foreach (var monthGroup in grouped)
                    {
                        column.Item().PaddingTop(10).PaddingBottom(5).Text($"MOIS: {monthGroup.Key.ToString("MMMM yyyy").ToUpper()}").Bold().FontSize(10);
                        
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40); // Jour
                                cols.ConstantColumn(80); // Code
                                cols.RelativeColumn();   // Operation
                                cols.ConstantColumn(100); // Section
                            });

                            Action<string> drawHeader = (text) => table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text(text).FontSize(8).Bold();

                            drawHeader("Date");
                            drawHeader("Code");
                            drawHeader("Opération");
                            drawHeader("Section");

                            Action<string> drawCell = (text) => table.Cell().Border(1).Padding(2).Text(text).FontSize(8);
                            Action<string> drawCenterCell = (text) => table.Cell().Border(1).Padding(2).AlignCenter().Text(text).FontSize(8);

                            foreach (var item in monthGroup.OrderBy(i => i.DateProchaineEcheance))
                            {
                                drawCenterCell(item.DateProchaineEcheance.ToString("dd/MM"));
                                drawCell(item.Equipement?.Code ?? "");
                                drawCell(item.Operation);
                                drawCell(item.Equipement?.Section ?? "");
                            }
                        });
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    #region New Document Stubs (Phase 3)

    public async Task<byte[]> GenerateQuestionnaireArretPdfAsync(QuestionnaireArretTechnique q)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 06
    }

    public async Task<byte[]> GeneratePlanningEntretienPreventifPdfAsync(List<Equipement> equipements, List<MaintenancePreventive> plannings, int annee)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 07
    }

    public async Task<byte[]> GenerateSuiviTempsMarchePdfAsync(Equipement eq, List<SuiviTempsMarcheHebdomadaire> suivi, int[] annees)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 08
    }

    public async Task<byte[]> GenerateHeuresMachinesPdfAsync(List<Equipement> equipements, List<SuiviTempsMarcheHebdomadaire> suivi, int annee)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 09
    }

    public async Task<byte[]> GeneratePlanningVisitePeriodiquePdfAsync(List<Equipement> equipements, List<MaintenancePreventive> plannings, int annee)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 10
    }

    public async Task<byte[]> GenerateFicheInspectionPdfAsync(FicheEntretienPreventif fiche)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 11
    }

    public async Task<byte[]> GenerateTravauxNonProgrammesPdfAsync(DemandeTravail dt)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 12
    }

    public async Task<byte[]> GenerateTravauxProgrammesPdfAsync(FicheEntretienPreventif fiche)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 13
    }

    public async Task<byte[]> GenerateRapportCirconstancielPdfAsync(RapportIncident rapport)
    {
        return Array.Empty<byte>(); // TODO: Implement DOC 14
    }

    #endregion

    #region Helpers

    private void ComposeHeader(IContainer container, string title, string reference)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("SAHEL GMAO").FontSize(16).Bold().FontColor("#1B5E20");
                column.Item().Text("Maintenance Industrielle").FontSize(10).Italic();
            });

            row.RelativeItem().Column(column =>
            {
                column.Item().AlignCenter().Text(title).FontSize(14).Bold();
                column.Item().AlignCenter().Text($"Réf: {reference}").FontSize(10);
            });
            
            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9);
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.PaddingTop(5).BorderTop(1).Row(row =>
        {
            row.RelativeItem().Text("Système de Gestion Sahel GMAO v2.0").FontSize(8);
            row.RelativeItem().AlignRight().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
        });
    }

    private IContainer BlockHeader(IContainer container) => container.Background(Colors.Grey.Lighten3).Padding(5).Border(0.5f).BorderColor(Colors.Grey.Lighten1).AlignLeft().DefaultTextStyle(x => x.Bold());
    private IContainer BlockContent(IContainer container) => container.Padding(5).Border(0.5f).BorderColor(Colors.Grey.Lighten1);
    private IContainer SectionHeader(IContainer container) => container.PaddingTop(10).PaddingBottom(5).AlignLeft().DefaultTextStyle(x => x.FontSize(11).Bold().FontColor("#1B5E20").Underline());
    private IContainer TableHeader(IContainer container) => container.Background("#1B5E20").Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.White));
    private IContainer TableCell(IContainer container) => container.Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

    #endregion
}
