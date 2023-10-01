using IQ.Helpers.DataTableOperations.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI;

namespace IQ.Helpers.DatabaseOperations
{
    public class DatabaseExtensions
    {
        public static NpgsqlConnection? con;
        public static bool ConnectToDb(string ConnectionString, [Optional] Window m)
        {
            bool Connected;
            try
            {
                con = new NpgsqlConnection(
                            connectionString: ConnectionString);
                con.Open();
                Connected = true;
            }
            catch(Exception ex) 
            {
                string error = ex.Message;
                ShowCompletionAlertDialogAsync(error, m);
                Connected = false;
            }
            return Connected;
        }


        public static async void ShowCompletionAlertDialogAsync(string alert, Window m)
        {
            // Create a ContentDialog
            ContentDialog alertDialog = new ContentDialog
            {
                // Set the title, content, and close button text
                Title = "Alert",
                Content = alert,
                CloseButtonText = "OK"
            };

            // Set the foreground to hex color #020066
            alertDialog.Foreground = new SolidColorBrush(Color.FromArgb(255, 2, 0, 102));

            // Set the XamlRoot property to the same as an element in the app window
            // For example, if you have a StackPanel named MyPanel in your XAML
            alertDialog.XamlRoot = m.Content.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        public static string GetCurrentUserRole()
        {
            // SQL query to get current user role
            string query = "SELECT current_role;";

            using (NpgsqlCommand command = new NpgsqlCommand(query, con))
            {
                // Execute the query and retrieve the result
                string? userRole = command.ExecuteScalar()?.ToString();

                if (!string.IsNullOrEmpty(userRole))
                {

                    if (checkUserStatus(userRole))
                    {
                        string userIsAdmin = "Admin";
                        return userIsAdmin;
                    }
                    else
                    {
                        return "Branch";
                    }
                }
                else
                {
                    return "noRoleAttached";
                }
            }
        }

        static bool checkUserStatus(string userRole)
        {
            string queryUserStatus = "SELECT usesuper FROM pg_user WHERE usename = @username";
            using var queryUserStatusCommand = new NpgsqlCommand(queryUserStatus, con);
            queryUserStatusCommand.Parameters.AddWithValue("username", userRole);
            bool isSuperUser = (bool)queryUserStatusCommand.ExecuteScalar()!;
            return isSuperUser;
        }

        public static bool TriggerDbMassAction_Branch()
        {
            bool isCompleted;
            try
            {
                // Create Tables
                string createBranchInventoryTable = "CREATE TABLE IF NOT EXISTS BranchInventory (ModelID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, BrandID VARCHAR(255) NOT NULL, AddOns VARCHAR(255) NOT NULL, QuantityInStock INT NOT NULL, UnitPrice DECIMAL NOT NULL, TotalWorth DECIMAL NOT NULL);";
                using var createBranchInventoryTableCommand = new NpgsqlCommand(createBranchInventoryTable, con);
                createBranchInventoryTableCommand.ExecuteScalar();
                string createBranchInventoryTableIndex = "CREATE INDEX  IF NOT EXISTS InvModel ON BranchInventory(ModelID);";
                using var createBranchInventoryTableIndexCommand = new NpgsqlCommand(createBranchInventoryTableIndex, con);
                createBranchInventoryTableIndexCommand.ExecuteScalar();

                string createSalesTable = "CREATE TABLE IF NOT EXISTS BranchSales (InvoiceID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantitySold INT NOT NULL, SellingPrice DECIMAL NOT NULL, SoldTo VARCHAR(255) NOT NULL, CustomerContactInfo VARCHAR(255) NOT NULL, Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createSalesTableCommand = new NpgsqlCommand(createSalesTable, con);
                createSalesTableCommand.ExecuteScalar();
                string createSalesTableIndex = "CREATE INDEX IF NOT EXISTS SalesDate ON BranchSales(Date);";
                using var createSalesTableIndexCommand = new NpgsqlCommand(createSalesTableIndex, con);
                createSalesTableIndexCommand.ExecuteScalar();

                string createPurchasesTable = "CREATE TABLE IF NOT EXISTS BranchPurchases (InvoiceID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, AddOns VARCHAR(255) NOT NULL, QuantityBought INT NOT NULL, BuyingPrice DECIMAL NOT NULL, PurchasedFrom VARCHAR(255) NOT NULL, SupplierContactInfo VARCHAR(255) NOT NULL, Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP);";
                using var createPurchasesTableCommand = new NpgsqlCommand(createPurchasesTable, con);
                createPurchasesTableCommand.ExecuteScalar();
                string createPurchasesTableIndex = "CREATE INDEX IF NOT EXISTS PurchaseDate ON BranchPurchases(Date);";
                using var createPurchasesTableIndexCommand = new NpgsqlCommand(createPurchasesTableIndex, con);
                createPurchasesTableIndexCommand.ExecuteScalar();

                string createTransferInwardsTable = "CREATE TABLE IF NOT EXISTS BranchTransferInwards (TransferID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL,  AddOns VARCHAR(255) NOT NULL, QuantityTransferred INT NOT NULL, TransferredFrom VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, TransferredProductPrice DECIMAL NOT NULL, Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP);";
                using var createTransferInwardsTableCommand = new NpgsqlCommand(createTransferInwardsTable, con);
                createTransferInwardsTableCommand.ExecuteScalar();
                string createTransferInwardsTableIndex = "CREATE INDEX IF NOT EXISTS TInsDate ON BranchTransferInwards(Date);";
                using var createTransferInwardsTableIndexCommand = new NpgsqlCommand(createTransferInwardsTableIndex, con);
                createTransferInwardsTableIndexCommand.ExecuteScalar();

                string createTransferOutwardsTable = "CREATE TABLE IF NOT EXISTS BranchTransferOutwards  (TransferID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL,  AddOns VARCHAR(255) NOT NULL,  QuantityTransferred INT NOT NULL, TransferredTo VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, TransferredProductPrice DECIMAL NOT NULL, Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createTransferOutwardsTableCommand = new NpgsqlCommand(createTransferOutwardsTable, con);
                createTransferOutwardsTableCommand.ExecuteScalar();
                string createTransferOutwardsTableIndex = "CREATE INDEX IF NOT EXISTS TOutsDate ON BranchTransferOutwards(Date);";
                using var createTransferOutwardsTableIndexCommand = new NpgsqlCommand(createTransferOutwardsTableIndex, con);
                createTransferOutwardsTableIndexCommand.ExecuteScalar();

                string createReturnInwardsTable = "CREATE TABLE IF NOT EXISTS BranchReturnInwards (ReturnID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityReturned INT NOT NULL, ReturnedBy VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL,   Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createReturnInwardsTableCommand = new NpgsqlCommand(createReturnInwardsTable, con);
                createReturnInwardsTableCommand.ExecuteScalar();
                string createReturnInwardsTableIndex = "CREATE INDEX IF NOT EXISTS RInsDate ON BranchReturnInwards(Date);";
                using var createReturnInwardsTableIndexCommand = new NpgsqlCommand(createReturnInwardsTableIndex, con);
                createReturnInwardsTableIndexCommand.ExecuteScalar();

                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS BranchReturnOutwards (ReturnID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityReturned INT NOT NULL, ReturnedTo VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                createReturnOutwardsTableCommand.ExecuteScalar();
                string createReturnOutwardsTableIndex = "CREATE INDEX IF NOT EXISTS ROutsDate ON BranchReturnOutwards(Date);";
                using var createReturnOutwardsTableIndexCommand = new NpgsqlCommand(createReturnOutwardsTableIndex, con);
                createReturnOutwardsTableIndexCommand.ExecuteScalar();

                string createCommitHistoryTable = "CREATE TABLE IF NOT EXISTS BranchCommitHistory (CommitID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, CommitDescription VARCHAR(255) NOT NULL, CommitDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, ApprovalStatus VARCHAR(255) NOT NULL,   ApprovalDate TIMESTAMP);";
                using var createCommitHistoryTableCommand = new NpgsqlCommand(createCommitHistoryTable, con);
                createCommitHistoryTableCommand.ExecuteScalar();
                string createCommitHistoryTableIndex = "CREATE INDEX IF NOT EXISTS CommitDate ON BranchCommitHistory(CommitDate);";
                using var createCommitHistoryTableIndexCommand = new NpgsqlCommand(createCommitHistoryTableIndex, con);
                createCommitHistoryTableIndexCommand.ExecuteScalar();

                // Create  Triggers
                using var InventoryTriggerFunctionCommand = new NpgsqlCommand("CREATE OR REPLACE FUNCTION updateTotalWorth()   RETURNS TRIGGER AS $$   BEGIN    NEW.TotalWorth = NEW.UnitPrice * NEW.QuantityInStock;   RETURN NEW;   END;    $$ LANGUAGE plpgsql;", con);
                InventoryTriggerFunctionCommand.ExecuteNonQuery();

                // Create the trigger for INSERT operations
                using var InventoryInsertTriggerCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateTotalWorth_insert    BEFORE INSERT ON BranchInventory   FOR EACH ROW    EXECUTE FUNCTION update_total_worth();", con);
                InventoryInsertTriggerCommand.ExecuteNonQuery();

                // Create the trigger for UPDATE operations
                using var InventoryUpdateTriggerCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateTotalWorth_update    BEFORE UPDATE OF UnitPrice, QuantityInStock ON BranchInventory   FOR EACH ROW   EXECUTE FUNCTION update_total_worth();", con);
                InventoryUpdateTriggerCommand.ExecuteNonQuery();

                // Sales-Inventory AutoUpdate
                using var SalesCommand = new NpgsqlCommand("CREATE OR REPLACE FUNCTION updateInventory_Sales()   RETURNS TRIGGER AS $$   BEGIN   UPDATE BranchInventory   SET QuantityInStock = QuantityInStock - NEW.QuantitySold    WHERE ModelID = NEW.ModelID;    RETURN NEW;    END;   $$ LANGUAGE plpgsql;", con);
                SalesCommand.ExecuteNonQuery();

                // Sales - Inventory AutoUpdate Trigger
                using var triggerSalesCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateInventory_salesTrigger  AFTER INSERT ON BranchSales   FOR EACH ROW  EXECUTE FUNCTION updateInventory_Sales();", con);
                triggerSalesCommand.ExecuteNonQuery();


                // ReturnIn-Inventory AutoUpdate
                using var ReturnInCommand = new NpgsqlCommand("CREATE OR REPLACE FUNCTION updateInventory_ReturnIn()   RETURNS TRIGGER AS $$   BEGIN   UPDATE BranchInventory   SET QuantityInStock = QuantityInStock + NEW.QuantityReturned    WHERE ModelID = NEW.ModelID;    RETURN NEW;    END;   $$ LANGUAGE plpgsql;", con);
                ReturnInCommand.ExecuteNonQuery();

                // ReturnIn-Inventory AutoUpdate Trigger
                using var triggerReturnInCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateInventory_ReturnInTrigger  AFTER INSERT ON BranchReturnInwards   FOR EACH ROW  EXECUTE FUNCTION updateInventory_ReturnIn();", con);
                triggerReturnInCommand.ExecuteNonQuery();

                // ReturnOut-Inventory AutoUpdate
                using var ReturnOutCommand = new NpgsqlCommand("CREATE OR REPLACE  FUNCTION updateInventory_ReturnOut()   RETURNS TRIGGER AS $$   BEGIN   UPDATE BranchInventory   SET QuantityInStock = QuantityInStock - NEW.QuantityReturned    WHERE ModelID = NEW.ModelID;    RETURN NEW;    END;   $$ LANGUAGE plpgsql;", con);
                ReturnOutCommand.ExecuteNonQuery();

                // ReturnOut-Inventory AutoUpdate Trigger
                using var triggerReturnOutCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateInventory_ReturnOutTrigger  AFTER INSERT ON BranchReturnOutwards   FOR EACH ROW  EXECUTE FUNCTION updateInventory_ReturnOut();", con);
                triggerReturnOutCommand.ExecuteNonQuery();

                // TransferOut-Inventory AutoUpdate
                using var TransferOutCommand = new NpgsqlCommand("CREATE OR REPLACE FUNCTION updateInventory_TransferOut()   RETURNS TRIGGER AS $$   BEGIN   UPDATE BranchInventory   SET QuantityInStock = QuantityInStock - NEW.QuantityTransferred    WHERE ModelID = NEW.ModelID;    RETURN NEW;    END;   $$ LANGUAGE plpgsql;", con);
                TransferOutCommand.ExecuteNonQuery();

                // TransferOut-Inventory AutoUpdate Trigger
                using var triggerTransferOutCommand = new NpgsqlCommand("CREATE OR REPLACE TRIGGER updateInventory_TransferOutTrigger  AFTER INSERT ON BranchTransferOutwards   FOR EACH ROW  EXECUTE FUNCTION updateInventory_TransferOut();", con);
                triggerTransferOutCommand.ExecuteNonQuery();

                // Sales-Inventory AutoUpdate
                // using var SalesCommand = new NpgsqlCommand(@"CREATE OR REPLACE FUNCTION updateInventory()   RETURNS TRIGGER AS $$   BEGIN   UPDATE BranchInventory   SET QuantityInStock = QuantityInStock - NEW.QuantitySold    WHERE ModelID = NEW.ModelID;    RETURN NEW;    END;   $$ LANGUAGE plpgsql;", con);
                // SalesCommand.ExecuteNonQuery();

                // Sales - Inventory AutoUpdate Trigger
                // using var triggerSalesCommand = new NpgsqlCommand(@"CREATE TRIGGER updateInventory_trigger  AFTER INSERT ON BranchSale   FOR EACH ROW  EXECUTE FUNCTION updateInventory();", con);
                // triggerSalesCommand.ExecuteNonQuery();

                isCompleted = true;
            }
            catch (Exception ex)
            {
                isCompleted = false;
                Debug.WriteLine(ex);
            }

            return isCompleted;
        }

        public static bool TriggerDbMassAction_Admin()
        {
            bool isCompleted;
            try
            {
                /*
                string createTransferOutwardsTable = "CREATE TABLE IF NOT EXISTS TransferOutwards (\r\n    TransferID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityTransferred INT NOT NULL,\r\n    TransferredTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    TransferredOn TIMESTAMP NOT NULL\r\n);";
                using var createTransferOutwardsTableCommand = new NpgsqlCommand(createTransferOutwardsTable, con);
                string createReturnInwardsTable = "CREATE TABLE IF NOT EXISTS ReturnInwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedFrom VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnInwardsTableCommand = new NpgsqlCommand(createReturnInwardsTable, con);
                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS ReturnOutwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) FORIEGN KEY NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                string createWarehouseInventoryTable = "CREATE TABLE IF NOT EXISTS WarehouseInventory (\r\n    ModelID VARCHAR(255) UNIQUE NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    Description VARCHAR(255) NOT NULL,\r\n    QuantityInStock INT NOT NULL,\r\n    UnitPrice FLOAT NOT NULL,\r\n    TotalWorth FLOAT NOT NULL,\r\n);";
                using var createWarehouseInventoryTableCommand = new NpgsqlCommand(createWarehouseInventoryTable, con);
                */

                isCompleted = true;
            }
            catch
            {
                isCompleted = false;
            }

            return isCompleted;
        }

        public static bool TriggerDbMassAction_Warehouse()
        {
            bool isCompleted;
            try
            {

                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS ReturnOutwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) FORIEGN KEY NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                string createWarehouseInventoryTable = "CREATE TABLE IF NOT EXISTS WarehouseInventory (\r\n    ModelID VARCHAR(255) UNIQUE NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    Description VARCHAR(255) NOT NULL,\r\n    QuantityInStock INT NOT NULL,\r\n    UnitPrice FLOAT NOT NULL,\r\n    TotalWorth FLOAT NOT NULL,\r\n);";
                using var createWarehouseInventoryTableCommand = new NpgsqlCommand(createWarehouseInventoryTable, con);


                isCompleted = true;
            }
            catch
            {
                isCompleted = false;
            }

            return isCompleted;
        }


        public static async Task<List<string>> QuerySalesSuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT InvoiceID FROM BranchSales WHERE InvoiceID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }

        public static async Task<ObservableCollection<BranchSale>> QuerySalesResultsFromDatabase(string userQuery)
        {
            ObservableCollection<BranchSale> searchResults = new ObservableCollection<BranchSale>();

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM BranchSales WHERE InvoiceID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map the database results to your result type
                            BranchSale result = new BranchSale
                            {
                                // Map properties from reader columns
                                InvoiceId = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantitySold = reader.GetInt32(3),
                                SellingPrice = reader.GetDecimal(4),
                                SoldTo = reader.GetString(5),
                                CustomerContactInfo = reader.GetString(6),
                            };

                            searchResults.Add(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResults;
        }

        public static async Task<ObservableCollection<BranchInventory>> QueryInventoryResultsFromDatabase(string userQuery)
        {
            ObservableCollection<BranchInventory> searchResults = new ObservableCollection<BranchInventory>();

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM BranchInventory WHERE ModelID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map the database results to your result type
                            BranchInventory result = new BranchInventory
                            {
                                // Map properties from reader columns
                                ModelID = reader.GetString(0),
                                BrandID = reader.GetString(1),
                                AddOns = reader.GetString(2),
                                QuantityInStock = reader.GetInt32(3),
                                UnitPrice = reader.GetDecimal(4),
                                TotalWorth = reader.GetDecimal(5),
                            };

                            searchResults.Add(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResults;
        }

        public static async Task<List<string>> QueryInventorySuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT ModelID FROM BranchInventory WHERE ModelID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }

        public static async Task<List<string>> QueryBrandIDSuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT BrandID FROM BranchInventory WHERE BrandID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }

        public static async Task<string> QueryBrandNameFromDatabase(string userQuery)
        {
            string? searchResult = "";

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT BrandID FROM BranchInventory WHERE ModelID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            searchResult = reader.GetString(0);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResult;
        }

        public static async Task<List<string>> QueryPurchasesSuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT InvoiceID FROM BranchPurchases WHERE InvoiceID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }

        internal static async Task<ObservableCollection<BranchPurchase>> QueryPurchasesResultsFromDatabase(string userQuery)
        {
            ObservableCollection<BranchPurchase> searchResults = new ObservableCollection<BranchPurchase>();

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM BranchPurchases WHERE InvoiceID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map the database results to your result type
                            BranchPurchase result = new BranchPurchase
                            {
                                // Map properties from reader columns
                                InvoiceID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityBought = reader.GetInt32(4),
                                BuyingPrice = reader.GetDecimal(5),
                                PurchasedFrom = reader.GetString(6),
                                SupplierContactInfo = reader.GetString(7),
                            };

                            searchResults.Add(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResults;
        }

        internal static async Task<ObservableCollection<BranchTIn>> QueryTInsResultsFromDatabase(string userQuery)
        {
            ObservableCollection<BranchTIn> searchResults = new ObservableCollection<BranchTIn>();

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM BranchTransferInwards WHERE TransferID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map the database results to your result type
                            BranchTIn result = new BranchTIn
                            {
                                // Map properties from reader columns
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredFrom = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            searchResults.Add(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResults;
        }

        internal static async Task<List<string>> QueryTInsSuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT TransferID FROM BranchTransferInwards WHERE TransferID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }

        internal static async Task<ObservableCollection<BranchTOut>> QueryTOutsResultsFromDatabase(string userQuery)
        {
            ObservableCollection<BranchTOut> searchResults = new ObservableCollection<BranchTOut>();

            try
            {

                // Perform a database query to fetch search results
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM BranchTransferOutwards WHERE TransferID = @userQuery";
                    command.Parameters.AddWithValue("userQuery", userQuery);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map the database results to your result type
                            BranchTOut result = new BranchTOut
                            {
                                // Map properties from reader columns
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredTo = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            searchResults.Add(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return searchResults;
        }

        internal static async Task<List<string>> QueryTOutsSuggestionsFromDatabase(string userInput)
        {
            List<string> suggestions = new List<string>();

            try
            {

                // Perform a database query to fetch suggestions
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT TransferID FROM BranchTransferOutwards WHERE TransferID LIKE @userInput";
                    command.Parameters.AddWithValue("userInput", "%" + userInput + "%");

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }

            return suggestions;
        }
    }
}
