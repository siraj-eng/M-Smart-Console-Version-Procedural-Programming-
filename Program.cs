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
        }
    }
}
