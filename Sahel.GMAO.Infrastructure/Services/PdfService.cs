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
                        int rowsToDraw = Math.Max(7, intervenants.Count + 1);
                        
                        for(int r = 0; r < rowsToDraw; r++)
                        {
                            var inv = r < intervenants.Count ? intervenants[r] : null;

                            table.Cell().Border(1).Padding(1).Text(inv?.Intervenant?.FullName ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(1).Text(inv?.Qualification ?? "").FontSize(8);
                            
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
                        int piecesRows = Math.Max(8, consommables.Count + 1);

                        for(int r=0; r<piecesRows; r++)
                        {
                            var c = r < consommables.Count ? consommables[r] : null;
                            table.Cell().Border(1).Padding(2).Text("").FontSize(8); // N° BSM
                            table.Cell().Border(1).Padding(2).Text(c?.ArticlePdr?.Designation ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? c.Quantite.ToString() : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? c.PrixUnitaireApplique.ToString("N2") : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(c != null ? ((decimal)c.Quantite * c.PrixUnitaireApplique).ToString("N2") : "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text("").FontSize(8);
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
                        int moRows = Math.Max(6, intervenantsMO.Count + 1);

                        for(int r=0; r<moRows; r++)
                        {
                            var inv = r < intervenantsMO.Count ? intervenantsMO[r] : null;
                            table.Cell().Border(1).Padding(2).Text(inv?.Intervenant?.FullName ?? "").FontSize(8);
                            table.Cell().Border(1).Padding(2).Text(inv?.Qualification ?? "").FontSize(8);
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

    private void ComposeFormHeader(IContainer container)
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
            row.RelativeItem().PaddingTop(10).AlignCenter().Text("DEMANDE DE TRAVAIL").Bold().FontSize(12);

            // Right Box: Versioning
            row.ConstantItem(100).BorderLeft(1).BorderColor("#553C7B").Padding(5).Column(col => 
            {
                col.Item().AlignCenter().Text("IMP MAIN 01").Bold().FontSize(9);
                col.Item().AlignCenter().Text("Version: 01").Bold().FontColor("#D81B60").FontSize(9);
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
                page.Header().Element(c => ComposeHeader(c, "DEMANDE DE FABRICATION", df.NumeroFabrication));
                
                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(3); });
                        table.Cell().Element(BlockHeader).Text("Pièce");
                        table.Cell().Element(BlockContent).Text(df.DesignationPiece);
                        table.Cell().Element(BlockHeader).Text("Quantité");
                        table.Cell().Element(BlockContent).Text(df.Quantite.ToString());
                        table.Cell().Element(BlockHeader).Text("Date Souhaitée");
                        table.Cell().Element(BlockContent).Text(df.DateSouhaitee.ToString("dd/MM/yyyy"));
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
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.Header().Element(c => ComposeHeader(c, "FICHE HISTORIQUE D'ÉQUIPEMENT", equipement.Code));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().Text($"{equipement.Designation} ({equipement.Section})").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(TableHeader).Text("N° DT");
                            header.Cell().Element(TableHeader).Text("Date");
                            header.Cell().Element(TableHeader).Text("Intervention");
                            header.Cell().Element(TableHeader).Text("Statut");
                            header.Cell().Element(TableHeader).Text("Coût Total");
                        });

                        foreach (var dt in interventions)
                        {
                            table.Cell().Element(TableCell).Text(dt.NumeroDT);
                            table.Cell().Element(TableCell).Text(dt.DateEmission.ToString("dd/MM/yyyy"));
                            table.Cell().Element(TableCell).Text(dt.TravailDemande);
                            table.Cell().Element(TableCell).Text(dt.Statut.ToString());
                            table.Cell().Element(TableCell).Text($"{dt.TotalCoutOperation:N2} DZD");
                        }
                    });
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
