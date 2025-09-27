using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                Console.WriteLine("4. Exit");
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

            foreach( var t in transactions)
            {
                Console.WriteLine($"\n---Current Transaction---");
                Console.WriteLine($"{t.Type}");
                Console.WriteLine($"{t.Category}");
                Console.WriteLine($"{t.Amount}");
                Console.WriteLine($"{t.Date}");

                //update logic
                 string updatedType = Console.ReadLine();
                 string updatedCategory = Console.ReadLine();
                 decimal updatedAmount = Convert.ToDecimal( Console.ReadLine());
                 var updatedDate = Convert.ToDateTime(Console.ReadLine());

                //Assigning back updated values
                if (String.IsNullOrWhiteSpace(updatedType))
                {
                    Console.WriteLine("Invalid type!");
                    return;
                }
                else
                {
                    t.Type = updatedType;
                }

                if (decimal.TryParse(Console.ReadLine(), out  updatedAmount))
                t.Amount = updatedAmount; 

                if(DateTime.TryParse(Console.ReadLine(), out updatedDate))
                    t.Date = updatedDate;

                if (String.IsNullOrWhiteSpace(updatedCategory))
                {
                    Console.WriteLine("Invalid type!");
                    return;
                }
                else
                {
                    t.Category = updatedCategory;
                }
     
                //update message
                Console.WriteLine("Transaction Updated successfully!!!");
            }

        }
    }
}
