using System;

namespace Msmart
{
    class Entry
    {
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

            var parts = message.Split(",").Select(message => message.Trim()).ToArray();

            string type = "Unknown";
            decimal amount = 0;
            string category = "Unknown";
            string date = "Unknown";

            //Detect type
            if (message.Contains("Paid", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Sent", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Buy", StringComparison.OrdinalIgnoreCase))
            {
                type = "Expense";
            }
            else if (message.Contains("Recieved", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("Deposit", StringComparison.OrdinalIgnoreCase)) 
            {
                type = "Income";
            }

            //Detect amount
            foreach (var part in parts) 
            {
                if (decimal.TryParse(part, out amount))
                    break;
                
            }
        }  
    }
}
