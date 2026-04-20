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
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                page.Header().Element(c => ComposeHeader(c, "DEMANDE DE TRAVAIL", dt.NumeroDT));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(10);

                    // Section 1: Équipement
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                        });

                        table.Cell().Element(BlockHeader).Text("Équipement");
                        table.Cell().Element(BlockContent).Text(dt.Equipement.Code);
                        table.Cell().Element(BlockHeader).Text("Désignation");
                        table.Cell().Element(BlockContent).Text(dt.Equipement.Designation);
                    });

                    // Section 2: Détails Demande
                    column.Item().Column(c =>
                    {
                        c.Item().Element(SectionHeader).Text("Détails de la Demande");
                        c.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(3);
                            });

                            table.Cell().Element(BlockHeader).Text("Date d'émission");
                            table.Cell().Element(BlockContent).Text(dt.DateEmission.ToString("dd/MM/yyyy HH:mm"));
                            
                            table.Cell().Element(BlockHeader).Text("Statut");
                            table.Cell().Element(BlockContent).Text(dt.Statut.ToString());

                            table.Cell().Element(BlockHeader).Text("Travail Demandé");
                            table.Cell().Element(BlockContent).Text(dt.TravailDemande);
                        });
                    });

                    // Section 3: Travail Exécuté (if available)
                    if (!string.IsNullOrEmpty(dt.TravailExecute))
                    {
                        column.Item().Column(c =>
                        {
                            c.Item().Element(SectionHeader).Text("Travail Exécuté");
                            c.Item().Element(BlockContent).Text(dt.TravailExecute);
                        });
                    }
                    
                    // Section 4: Signature slots
                    column.Item().PaddingTop(30).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                        });

                        table.Cell().AlignCenter().Text("Émetteur").Bold();
                        table.Cell().AlignCenter().Text("Maintenance").Bold();
                        table.Cell().AlignCenter().Text("Ordonnanceur").Bold();
                        
                        table.Cell().PaddingTop(40).AlignCenter().Text("....................");
                        table.Cell().PaddingTop(40).AlignCenter().Text("....................");
                        table.Cell().PaddingTop(40).AlignCenter().Text("....................");
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
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
