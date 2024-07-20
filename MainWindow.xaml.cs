using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDFTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string Lorem = """Dolor quia ratione qui molestias maxime.""";

    public MainWindow()
    {
        InitializeComponent();
    }

    private static void PrintXps(string filePath)
    {
        // Create the print dialog object and set options.
        PrintDialog printDialog = new();

        var isPrinted = printDialog.ShowDialog();

        if (isPrinted != true)
        {
            return;
        }

        // Open the selected document.
        XpsDocument xpsDocument = new(filePath, FileAccess.Read);

        // Get a fixed document sequence for the selected document.
        var fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

        // Create a paginator for all pages in the selected document.
        var docPaginator = fixedDocSeq.DocumentPaginator;

        // Print to a new file.
        printDialog.PrintDocument(docPaginator, "Printing stuff");
    }

    private void AssembleFiles()
    {
        Settings.License = LicenseType.Community;

        // code in your main method
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header()
                    .Text("Hello PDF!")
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text(Lorem);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        });

        var fileNameBase = "output.out";
        var targetBase = Path.Combine(AppContext.BaseDirectory, fileNameBase);
        var pdfTarget = Path.ChangeExtension(targetBase, "pdf");
        var xpsTarget = Path.ChangeExtension(targetBase, "xps");

        document.GeneratePdf(pdfTarget);
        document.GenerateXps(xpsTarget);

        PrintXps(xpsTarget);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        AssembleFiles();
    }
}
