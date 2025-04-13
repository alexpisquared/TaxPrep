using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Printing;
using MinFin7MdiLib.ViewModels;
using MinFin7MdiLib.Services;
using Db.FinDemo7.Models;
using System.Windows.Data;
using Microsoft.Win32;

namespace MinFin7MdiLib.Views
{
    /// <summary>
    /// Interaction logic for PrintableReport.xaml
    /// </summary>
    public partial class PrintableReport : Window
    {
        private readonly TaxReportViewModel _viewModel;
        private readonly TaxReportService _reportService;

        public PrintableReport(ILogger lgr, IBpr bpr, string owner, FinDemoContext dba)
        {
            InitializeComponent();
            _reportService = new TaxReportService();
            _viewModel = new TaxReportViewModel(_reportService);
            DataContext = _viewModel;

            // Set up commands
            _viewModel.PrintCommand = new RelayCommand(Print);
            _viewModel.ExportToPdfCommand = new RelayCommand(ExportToPdf);
            
            // Load data and generate report
            Loaded += async (s, e) => await LoadReportDataAsync(owner, dba);
        }

        private async Task LoadReportDataAsync(string owner, FinDemoContext dba)
        {
            try
            {
                _viewModel.StatusText = "Loading report data...";
                
                // Get the report data
                var data = await _reportService.GetTaxReportDataAsync(owner, dba, new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));
                
                if (data == null || !data.Any())
                {
                    MessageBox.Show("No data available for the report.", "Report Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    _viewModel.StatusText = "No data available";
                    return;
                }

                // Generate the report
                await Task.Run(() => GenerateReport(data));
                
                _viewModel.PrintDate = DateTime.Now;
                _viewModel.StatusText = $"Report generated at {_viewModel.PrintDate:g}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _viewModel.StatusText = "Error loading data";
            }
        }

        private void GenerateReport(IEnumerable<VwTaxLiqReport> reportData)
        {
            Dispatcher.Invoke(() =>
            {
                // Clear existing data rows
                DataRows.Rows.Clear();
                
                // Group data by ExpenseGroup
                var groupedData = reportData.GroupBy(item => item.Group)
                    .OrderBy(g => g.Min(i => i.TlNumber));
                
                double grandTotal = 0;

                foreach (var group in groupedData)
                {
                    // Add group header
                    var groupRow = new TableRow { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEFF")) };
                    
                    var groupCell = new TableCell { ColumnSpan = 3 };
                    groupCell.Blocks.Add(new Paragraph(new Run(group.Key)) { FontWeight = FontWeights.Bold });
                    groupRow.Cells.Add(groupCell);
                    
                    DataRows.Rows.Add(groupRow);

                    // Add items for this group
                    foreach (var item in group.OrderBy(i => i.TlNumber))
                    {
                        var itemRow = new TableRow();
                        
                        // Category cell
                        var categoryCell = new TableCell();
                        var categoryParagraph = new Paragraph();
                        categoryParagraph.Inlines.Add(new Run($"{item.TlNumber}   {item.Category}") 
                        { 
                            FontSize = 9,
                            FontFamily = new FontFamily("Tahoma")
                        });
                        categoryCell.Blocks.Add(categoryParagraph);
                        itemRow.Cells.Add(categoryCell);
                        
                        // PartCalcShow cell
                        var partCalcCell = new TableCell();
                        var partCalcParagraph = new Paragraph { TextAlignment = TextAlignment.Right };
                        partCalcParagraph.Inlines.Add(new Run(item.PartCalcShow ?? string.Empty) 
                        { 
                            FontSize = 8, 
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444")),
                            FontWeight = FontWeights.Light,
                            FontFamily = new FontFamily("Tahoma")
                        });
                        partCalcCell.Blocks.Add(partCalcParagraph);
                        itemRow.Cells.Add(partCalcCell);
                        
                        // Amount cell
                        var amountCell = new TableCell();
                        var amountParagraph = new Paragraph { TextAlignment = TextAlignment.Right };
                        amountParagraph.Inlines.Add(new Run(item.TtlExp?.ToString("N2") ?? "0.00") 
                        { 
                            FontWeight = FontWeights.Medium,
                            FontFamily = new FontFamily("Tahoma")
                        });
                        amountCell.Blocks.Add(amountParagraph);
                        itemRow.Cells.Add(amountCell);
                        
                        DataRows.Rows.Add(itemRow);
                        
                        // Add to grand total
                        grandTotal += item.TtlExp ?? 0;
                    }
                }
                
                // Update the grand total
                _viewModel.GrandTotal = grandTotal;
                
                // Update the textblock directly if binding doesn't work
                if (GrandTotalText != null)
                {
                    GrandTotalText.Text = grandTotal.ToString("N2");
                }
            });
        }

        private void Print()
        {
            try
            {
                _viewModel.StatusText = "Preparing to print...";
                
                // Create PrintDialog and set options
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Clone the FlowDocument to avoid modifying the displayed document
                    FlowDocument documentToprint = CloneDocument(reportDocument);
                    
                    // Set up pagination for the document
                    documentToprint.PageHeight = printDialog.PrintableAreaHeight;
                    documentToprint.PageWidth = printDialog.PrintableAreaWidth;
                    documentToprint.PagePadding = new Thickness(50);
                    documentToprint.ColumnGap = 0;
                    documentToprint.ColumnWidth = printDialog.PrintableAreaWidth - 100;

                    // Create a paginator
                    IDocumentPaginatorSource paginatorSource = documentToprint;
                    
                    // Print document
                    printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Tax Report");
                    
                    _viewModel.StatusText = "Document sent to printer.";
                }
                else
                {
                    _viewModel.StatusText = "Print canceled.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing document: {ex.Message}", "Print Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _viewModel.StatusText = "Error printing document.";
            }
        }

        private void ExportToPdf()
        {
            try
            {
                _viewModel.StatusText = "Preparing PDF export...";
                
                // Create save file dialog
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = $"TaxReport_{DateTime.Now:yyyyMMdd}.pdf"
                };
                
                if (saveDialog.ShowDialog() == true)
                {
                    // Create XPS document writer
                    XpsDocument xpsDoc = new XpsDocument(Path.GetTempFileName(), FileAccess.ReadWrite);
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                    
                    // Clone document to avoid modifying display document
                    FlowDocument documentToExport = CloneDocument(reportDocument);
                    
                    // Write FlowDocument to XPS
                    writer.Write(((IDocumentPaginatorSource)documentToExport).DocumentPaginator);
                    
                    // Convert XPS to PDF (requires external library)
                    // This is a placeholder - in a real implementation you'd use a library 
                    // like PDFsharp-MigraDoc to convert XPS to PDF
                    
                    // For example:
                    // xpsDoc.ConvertToPdf(saveDialog.FileName);
                    
                    _viewModel.StatusText = $"PDF exported to {saveDialog.FileName}";
                    
                    // Since we're in a placeholder implementation:
                    MessageBox.Show("PDF export requires additional libraries. " +
                        "Please add a PDF conversion library like PdfSharp to complete this feature.", 
                        "PDF Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    xpsDoc.Close();
                }
                else
                {
                    _viewModel.StatusText = "PDF export canceled.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _viewModel.StatusText = "Error exporting to PDF.";
            }
        }

        private FlowDocument CloneDocument(FlowDocument source)
        {
            // This creates a copy of the document to avoid modifying the displayed version
            TextRange sourceRange = new TextRange(source.ContentStart, source.ContentEnd);
            MemoryStream stream = new MemoryStream();
            
            sourceRange.Save(stream, DataFormats.Xaml);
            stream.Position = 0;
            
            FlowDocument copy = new FlowDocument();
            TextRange copyRange = new TextRange(copy.ContentStart, copy.ContentEnd);
            copyRange.Load(stream, DataFormats.Xaml);
            
            // Copy document properties
            copy.PageHeight = source.PageHeight;
            copy.PageWidth = source.PageWidth;
            copy.PagePadding = source.PagePadding;
            copy.ColumnGap = source.ColumnGap;
            copy.ColumnWidth = source.ColumnWidth;
            
            return copy;
        }
    }
}
