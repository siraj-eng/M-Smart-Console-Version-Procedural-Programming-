using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Msmart
{
    class Entry
    {
        //Transaction class
        public class Transaction 
        {
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public DateTime Date { get; set; }
            public string Category { get; set; }

        }
        public static void SaveTransactions(List<Transaction> transactions, string filePath) 
        { 
            string JsonString = JsonSerializer.Serialize(transactions, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            File.WriteAllText(filePath, JsonString);    
        }

        public static List<Transaction> LoadTransactions(string filePath) 
        {
            if (!File.Exists(filePath))
            {
               return new List<Transaction>(); //Return empty list if file doesn't exist
            }

            string jsonString = File.ReadAllText(filePath); 
            return JsonSerializer.Deserialize<List<Transaction>>(jsonString) ?? new List<Transaction>();
        }
        static void Main(string[] args)
        {
            Console.WriteLine("\n========= Welcome to M-Smart Budgeting App =========");
            Console.WriteLine("\n Helping you stay on top of your money.");
            Console.WriteLine(" Never get caught without cash again!\n");

            Console.WriteLine("Main Menu");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine(" 1. Add a Transaction");
            Console.WriteLine(" 2. View Balance");
            Console.WriteLine(" 3. View Reports");
            Console.WriteLine(" 4. Enter Budget");
            Console.Write("\nYour choice: ");

            AddTransaction();
        }

        static void AddTransaction()
        {
            // Welcoming message
            Console.WriteLine("\n=== Add a Transaction ===");
            Console.WriteLine("Welcome to M-Smart Transactions!");
            Console.WriteLine("Paste your M-Pesa message below,");
            Console.WriteLine("and we’ll process it for you.\n");

            Console.Write("Paste here: ");
            string message = Console.ReadLine();

            string type = "Unknown";
            decimal amount = 0;
            string category = "Unknown";
            string date = "Unknown";
            string Balance = "Unknown";

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

            // ✅ Regex for amount (looks for "Ksh123.45")
            var amountMatch = Regex.Match(message, @"Ksh\s?([\d,]+\.\d{2})");
            if (amountMatch.Success)
            {
                amount = decimal.Parse(amountMatch.Groups[1].Value.Replace(",", ""));
            }

            // ✅ Regex for date (looks for "on 10/6/25")
            var dateMatch = Regex.Match(message, @"on\s+(\d{1,2}/\d{1,2}/\d{2})");
            if (dateMatch.Success)
            {
                date = dateMatch.Groups[1].Value;
            }

            // ✅ Regex for category (after "to" or "from" until numbers)
            var categoryMatch = Regex.Match(message, @"(?:to|from)\s+([A-Za-z\s]+)\d?");
            if (categoryMatch.Success)
            {
                category = categoryMatch.Groups[1].Value.Trim();
            }

            // ✅ Regex for Balance (after "New M-Pesa Balance" until numbers)
            var balanceMatch = Regex.Match(message, @"balance\s+is\s+Ksh([\d,]+\.\d{2})", RegexOptions.IgnoreCase);
            if (balanceMatch.Success)
            {
                var raw = balanceMatch.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(raw, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed))
                    Balance = parsed.ToString();

            }

            SaveTransactions();

            // Output Transaction
            Console.WriteLine("\n✅ Parsed Transaction:");
            Console.WriteLine($"Type: {type}");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"Category: {category}");
            Console.WriteLine($"Date: {date}");

            
        }

    }
}
