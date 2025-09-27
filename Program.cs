using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Msmart
{
    // Transaction class
    public class Transaction
    {
        public string Type { get; set; }       // Income / Expense
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }

    class Entry
    {
        // Static list to hold transactions
        static List<Transaction> transactions = new List<Transaction>();
        static string filePath = "transactions.json";

        // Save transactions to JSON file
        public static void SaveTransactions(List<Transaction> transactions, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(transactions, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, jsonString);
        }

        // Load transactions from JSON file
        public static List<Transaction> LoadTransactions(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Transaction>();

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Transaction>>(jsonString) ?? new List<Transaction>();
        }

        // Entry point
        static void Main(string[] args)
        {
            Console.WriteLine("\n========= Welcome to M-Smart Budgeting App =========");
            Console.WriteLine("Helping you stay on top of your money.");
            Console.WriteLine("Never get caught without cash again!\n");

            // Load transactions at startup
            transactions = LoadTransactions(filePath);

            while (true)
            {
                Console.WriteLine("\nMain Menu");
                Console.WriteLine("1. Add a Transaction");
                Console.WriteLine("2. View Transactions");
                Console.WriteLine("3. Budget Alert and info");
                Console.WriteLine("4. Edit Transaction");
                Console.WriteLine("5. Exit");
                Console.Write("Your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransaction();
                        break;
                    case "2":
                        ViewTransactions();
                        break;
                    case "3":
                          BudgetAlert();
                        break;
                    case "4":
                          EditTransaction();
                        break;
                    case "5":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        // Add a transaction
        static void AddTransaction()
        {
            Console.WriteLine("\n=== Add a Transaction ===");
            Console.WriteLine("Paste your M-Pesa message below:");
            Console.Write("Paste here: ");
            string message = Console.ReadLine();

            string type = "Unknown";
            decimal amount = 0;
            string category = "Unknown";
            DateTime date = DateTime.Now;

            // Detect type
            if (message.Contains("Paid", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Sent", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Buy", StringComparison.OrdinalIgnoreCase))
            {
                type = "Expense";
            }
            else if (message.Contains("Received", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("Deposit", StringComparison.OrdinalIgnoreCase))
            {
                type = "Income";
            }

            // Regex for amount
            var amountMatch = Regex.Match(message, @"Ksh\s?([\d,]+\.\d{2})");
            if (amountMatch.Success)
            {
                amount = decimal.Parse(amountMatch.Groups[1].Value.Replace(",", ""));
            }

            // Regex for date
            var dateMatch = Regex.Match(message, @"on\s+(\d{1,2}/\d{1,2}/\d{2})");
            if (dateMatch.Success)
            {
                if (DateTime.TryParseExact(dateMatch.Groups[1].Value, "d/M/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    date = parsed;
                }
            }

            // Regex for category
            var categoryMatch = Regex.Match(message, @"(?:to|from)\s+([A-Za-z\s]+)\d?");
            if (categoryMatch.Success)
            {
                category = categoryMatch.Groups[1].Value.Trim();
            }

            // Create transaction object
            var transaction = new Transaction
            {
                Type = type,
                Amount = amount,
                Category = category,
                Date = date
            };

            // Add to list and save
            transactions.Add(transaction);
            SaveTransactions(transactions, filePath);

            Console.WriteLine("\n✅ Transaction added successfully!");
            Console.WriteLine($"Type: {transaction.Type}");
            Console.WriteLine($"Amount: {transaction.Amount}");
            Console.WriteLine($"Category: {transaction.Category}");
            Console.WriteLine($"Date: {transaction.Date.ToShortDateString()}");
        }

        // View all transactions
        static void ViewTransactions()
        {
            Console.WriteLine("\n=== All Transactions ===");
            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions found.");
                return;
            }

            foreach (var t in transactions)
            {
                Console.WriteLine($"{t.Date.ToShortDateString()} | {t.Type} | {t.Amount} | {t.Category}");
            }
        }

        //BudgetAlert
        static void BudgetAlert()
        {
            Console.WriteLine("Hello there !...Enter your interested budget amount");
            string input = Console.ReadLine();
            decimal budgetAmount;

            // Try to convert input to decimal
            if (!decimal.TryParse(input, out budgetAmount) || budgetAmount <= 0)
            {
                Console.WriteLine("❌ Not valid. Please enter a positive number.");
                return; // stop the method here
            }

            //Calculate total expenses
            decimal totalExpenses = 0;
            foreach (var t in transactions) 
            {
                if (t.Type == "Expense") 
                totalExpenses += t.Amount;
            }

            Console.WriteLine($"\n Your Budget: {budgetAmount}");
            Console.WriteLine($"Total expenses: {totalExpenses}");

            if(totalExpenses > budgetAmount)
            {
                Console.WriteLine("Alert! You have exceeded your budget");
                Console.WriteLine($"Overspent by {totalExpenses - budgetAmount}");
            }
            else
            {
                Console.WriteLine("You are within Your Budget");
                Console.WriteLine($"Remaining Budget: {budgetAmount - totalExpenses}");
            }
        } 

        static void EditTransaction()
        {
            Console.WriteLine("\nHello You can now edit the transactions made");

            if (transactions.Count == 0) 
            {
                Console.WriteLine("\nNo Transaction available to edit.");
                return;
            }

            Console.WriteLine("\n---Transacations List---");
            for (int i = 0; i < transactions.Count; i++) 
            {
                var t = transactions[i];
                Console.WriteLine($"{i + 1}. {t.Type} | {t.Category} | {t.Amount} | {t.Date}");
            }

            Console.WriteLine("\nEnter the number of the transaction you want to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > transactions.Count) 
            {
                Console.WriteLine("Invalid choice. Returning to menu!....");
                return;
            }

            var selected = transactions[choice - 1];

            Console.WriteLine($"\n--- Editing Transaction #{choice} ---");
            Console.WriteLine($"Current Type: {selected.Type}");
            Console.Write("Enter new Type (leave blank to keep): ");
            string updatedType = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(updatedType))
                selected.Type = updatedType;

            Console.WriteLine($"Current Category: {selected.Category}");
            Console.Write("Enter new Category (leave blank to keep): ");
            string updatedCategory = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(updatedCategory))
                selected.Category = updatedCategory;

            Console.WriteLine($"Current Amount: {selected.Amount}");
            Console.Write("Enter new Amount (leave blank to keep): ");
            string amountInput = Console.ReadLine();
            if (decimal.TryParse(amountInput, out var updatedAmount))
                selected.Amount = updatedAmount;

            Console.WriteLine($"Current Date: {selected.Date}");
            Console.Write("Enter new Date (leave blank to keep): ");
            string dateInput = Console.ReadLine();
            if (DateTime.TryParse(dateInput, out var updatedDate))
                selected.Date = updatedDate;

            Console.WriteLine("\n✅ Transaction updated successfully!");
        }

        static void DeleteTransaction()
        {
            if(transactions.Count == 0)
            {
                Console.WriteLine("\nNo Transactions available to delete.");
                return;
            }

            Console.WriteLine("\n-----Hey Hello you can now delete a transaction----------");

            //Print all reports
            for (int i = 0; i < transactions.Count; i++)
            {
                var t = transactions[i]; //Current transaction

                Console.WriteLine($"{i + 1}, {t.Type},{t.Amount},{t.Date},{t.Category}");

            }

            if(!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > transactions.Count)
            {
                Console.WriteLine("Invalid input...Returning to Menu");
                return;
            }

            var selected = transactions [choice - 1];

            Console.WriteLine($"\n--- Deleting Transaction #{choice} ---");
            Console.WriteLine($"{selected.Type}, {selected.Amount}, {selected.Date}, {selected.Category}");
            Console.Write("Are you sure you want to delete this? (y/n): ");
            string input = Console.ReadLine()?.ToLower();

           if(input == "y")
           {
             selected.Type = transactions[choice - 1].Type.Remove(0);
           }
           else if(input == "n")
           {
             Console.WriteLine("Returning to the menu");
           }
           else if (String.IsNullOrWhiteSpace(input))
           {
             Console.WriteLine("Invalid input");
           }
        }
    }
}
