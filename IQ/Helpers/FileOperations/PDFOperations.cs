using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql;
using Windows.Storage.Pickers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.UI.Xaml.Documents;
using Aspose.Pdf;
using IQ.Helpers.DataTableOperations.ViewModels;
using Document = iTextSharp.text.Document;
using Rectangle = iTextSharp.text.Rectangle;
using IQ.Helpers.DataTableOperations.Classes;
using System.Collections.ObjectModel;
using IQ.Views.AdminViews.Pages.Sales;
using IQ.Views;
using IQ.Views.AdminViews.Pages.Purchases;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml;
using PageSize = iTextSharp.text.PageSize;

namespace IQ.Helpers.FileOperations
{
    public class PDFOperations
    {
        public static async Task<string> GetSaveFilePathAsync(string fileName, Window m)
        {
            // Create a FileSavePicker
            FileSavePicker savePicker = new FileSavePicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);


            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("PDF files", new List<string> { ".pdf" });
            savePicker.DefaultFileExtension = ".pdf";
            savePicker.SuggestedFileName = $"{fileName}"; // Default file name

            // Show the FileSavePicker dialog to the user
            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                return file.Path; // Return the selected file path
            }
            else
            {
                return null!; // User canceled the operation
            }
        }

        public static async Task CreateSalesPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Sales", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new SalesHeaderFooter();

            // Open the document for writing
            doc.Open();

            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchSale> salesForMonth = GetSalesForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(7); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Invoice ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("Quantity Sold");
            table.AddCell("Selling Price");
            table.AddCell("Sold To");
            table.AddCell("Customer Contact Info");

            // Add table data from the retrieved data
            foreach (var sale in salesForMonth)
            {
                table.AddCell(sale.InvoiceId);
                table.AddCell(sale.ModelID);
                table.AddCell(sale.BrandID);
                table.AddCell(sale.QuantitySold.ToString());
                table.AddCell(sale.SellingPrice.ToString());
                table.AddCell(sale.SoldTo);
                table.AddCell(sale.CustomerContactInfo);
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchSale> GetSalesForMonth()
        {
            ObservableCollection<BranchSale> salesForMonth = new ObservableCollection<BranchSale>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".Sales
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanySalesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sale = new BranchSale
                            {
                                InvoiceId = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantitySold = reader.GetInt32(3),
                                SellingPrice = reader.GetDecimal(4),
                                SoldTo = reader.GetString(5),
                                CustomerContactInfo = reader.GetString(6),
                            };

                            salesForMonth.Add(sale);
                        }
                    }
                }
            }

            return salesForMonth;
        }

        public class SalesHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Monthly Sales"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }

        public static async 
        Task
CreatePurchasesPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Purchases", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new PurchasesHeaderFooter();

            // Open the document for writing
            doc.Open();
            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchPurchase> purchasesForMonth = GetPurchasesForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(8); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Invoice ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("AddOns");
            table.AddCell("Quantity Sold");
            table.AddCell("Selling Price");
            table.AddCell("Sold To");
            table.AddCell("Customer Contact Info");

            // Add table data from the retrieved data
            foreach (var purchase in purchasesForMonth)
            {
                table.AddCell(purchase.InvoiceID);
                table.AddCell(purchase.ModelID);
                table.AddCell(purchase.BrandID);
                table.AddCell(purchase.AddOns);
                table.AddCell(purchase.QuantityBought.ToString());
                table.AddCell(purchase.BuyingPrice.ToString());
                table.AddCell(purchase.PurchasedFrom);
                table.AddCell(purchase.SupplierContactInfo);
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchPurchase> GetPurchasesForMonth()
        {
            ObservableCollection<BranchPurchase> purchasesForMonth = new ObservableCollection<BranchPurchase>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".Purchases
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new BranchPurchase
                            {
                                InvoiceID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityBought = reader.GetInt32(4),
                                BuyingPrice = reader.GetDecimal(5),
                                PurchasedFrom = reader.GetString(6),
                                SupplierContactInfo = reader.GetString(7),
                            };

                            purchasesForMonth.Add(purchase);
                        }
                    }
                }
            }

            return purchasesForMonth;
        }

        public class PurchasesHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Monthly Purchases"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }

        public static async Task CreateRInsPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Return Inwards", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new RInsHeaderFooter();

            // Open the document for writing
            doc.Open();
            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchRIn> rinsForMonth = GetRInsForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(7); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Return ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("Quantity Returned");
            table.AddCell("Returned By");
            table.AddCell("Reason For Return");
            table.AddCell("Signed By");

            // Add table data from the retrieved data
            foreach (var rin in rinsForMonth)
            {
                table.AddCell(rin.ReturnID);
                table.AddCell(rin.ModelID);
                table.AddCell(rin.BrandID);
                table.AddCell(rin.QuantityReturned.ToString());
                table.AddCell(rin.ReturnedBy);
                table.AddCell(rin.ReasonForReturn);
                table.AddCell(rin.SignedBy);
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchRIn> GetRInsForMonth()
        {
            ObservableCollection<BranchRIn> rinsForMonth = new ObservableCollection<BranchRIn>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".ReturnInwards
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rin = new BranchRIn
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedBy = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            rinsForMonth.Add(rin);
                        }
                    }
                }
            }

            return rinsForMonth;
        }

        public class RInsHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Monthly Return Inwards"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }

        public static async 
        Task
CreateROutsPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Return Outwards", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new ROutsHeaderFooter();

            // Open the document for writing
            doc.Open();
            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchROut> routsForMonth = GetROutsForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(7); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Return ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("Quantity Returned");
            table.AddCell("Returned To");
            table.AddCell("Reason For Return");
            table.AddCell("Signed By");

            // Add table data from the retrieved data
            foreach (var rout in routsForMonth)
            {
                table.AddCell(rout.ReturnID);
                table.AddCell(rout.ModelID);
                table.AddCell(rout.BrandID);
                table.AddCell(rout.QuantityReturned.ToString());
                table.AddCell(rout.ReturnedTo);
                table.AddCell(rout.ReasonForReturn);
                table.AddCell(rout.SignedBy);
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchROut> GetROutsForMonth()
        {
            ObservableCollection<BranchROut> routsForMonth = new ObservableCollection<BranchROut>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".ReturnOutwards
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanySalesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rout = new BranchROut
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedTo = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            routsForMonth.Add(rout);
                        }
                    }
                }
            }

            return routsForMonth;
        }

        public class ROutsHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Monthly Return Outwards"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }

        public static async 
        Task
CreateTInsPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Transfer Inwards", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new TInsHeaderFooter();

            // Open the document for writing
            doc.Open();
            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchTIn> tinsForMonth = GetTInsForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(8); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Transfer ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("AddOns");
            table.AddCell("Quantity Transferred");
            table.AddCell("Transferred To");
            table.AddCell("Signed By");
            table.AddCell("Transferred Product Price");

            // Add table data from the retrieved data
            foreach (var sale in tinsForMonth)
            {
                table.AddCell(sale.TransferID);
                table.AddCell(sale.ModelID);
                table.AddCell(sale.BrandID);
                table.AddCell(sale.AddOns);
                table.AddCell(sale.QuantityTransferred.ToString());
                table.AddCell(sale.TransferredFrom);
                table.AddCell(sale.SignedBy);
                table.AddCell(sale.TransferredProductPrice.ToString());
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchTIn> GetTInsForMonth()
        {
            ObservableCollection<BranchTIn> tinsForMonth = new ObservableCollection<BranchTIn>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".TransferInwards
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tin = new BranchTIn
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredFrom = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            tinsForMonth.Add(tin);
                        }
                    }
                }
            }

            return tinsForMonth;
        }

        public class TInsHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Transfer Inwards"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }

        public static async 
        Task
CreateTOutsPdfForMonth(Window m)
        {
            string outputPath = await GetSaveFilePathAsync($"{App.UserName} Monthly Transfer Outwards", m);
            // Create a new document
            Document doc = new Document((PageSize.LETTER.Rotate()));
            doc.SetMargins(10, 10, 30, 10);

            // Create a PdfWriter instance to write to the output file
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

            // Set up a custom page event to add a header to each page
            writer.PageEvent = new TOutHeaderFooter();

            // Open the document for writing
            doc.Open();
            // Calculate the available width for the table (including left and right margins)
            float availableWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;

            // Retrieve data for the specified month
            ObservableCollection<BranchTOut> toutsForMonth = GetTOutsForMonth();

            // Create a PdfPTable to display the data
            PdfPTable table = new PdfPTable(8); // Assuming you have 7 columns

            // Set the total width of the table to the available width
            table.TotalWidth = availableWidth;

            // Calculate the column width as a fraction of the available width
            float[] columnWidths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Adjust the weights as needed
            table.SetTotalWidth(columnWidths);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell("Transfer ID");
            table.AddCell("Model ID");
            table.AddCell("Brand ID");
            table.AddCell("AddOns");
            table.AddCell("Quantity Transferred");
            table.AddCell("Transferred To");
            table.AddCell("Signed By");
            table.AddCell("Transferred Product Price");

            // Add table data from the retrieved data
            foreach (var sale in toutsForMonth)
            {
                table.AddCell(sale.TransferID);
                table.AddCell(sale.ModelID);
                table.AddCell(sale.BrandID);
                table.AddCell(sale.AddOns);
                table.AddCell(sale.QuantityTransferred.ToString());
                table.AddCell(sale.TransferredTo);
                table.AddCell(sale.SignedBy);
                table.AddCell(sale.TransferredProductPrice.ToString());
            }

            // Add the table to the document
            doc.Add(table);

            // Close the PDF document
            doc.Close();
        }

        public static ObservableCollection<BranchTOut> GetTOutsForMonth()
        {
            ObservableCollection<BranchTOut> salesForMonth = new ObservableCollection<BranchTOut>();

            // Retrieve data from your database for the specified month
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * FROM ""{App.UserName}"".TransferOutwards
                      WHERE EXTRACT(MONTH FROM Date) = EXTRACT(MONTH FROM CURRENT_DATE)
                        AND EXTRACT(YEAR FROM Date) = EXTRACT(YEAR FROM CURRENT_DATE);", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var TOut = new BranchTOut
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredTo = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            salesForMonth.Add(TOut);
                        }
                    }
                }
            }

            return salesForMonth;
        }

        public class TOutHeaderFooter : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // Create a PdfPTable to hold the header
                PdfPTable header = new PdfPTable(1);
                header.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                header.DefaultCell.BorderColor = new BaseColor(255, 255, 255);

                // Add header content (customize as needed)
                PdfPCell cell = new PdfPCell(new Phrase($"{App.UserName} Monthly Transfer Outwards"));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 10f;
                header.AddCell(cell);

                // Add the header to the page
                header.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + header.TotalHeight, writer.DirectContent);
            }
        }
    }
}
