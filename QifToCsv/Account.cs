using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QifToCsv
{
    class Account
    {
        public string Type { get; set; }
        public List<Transaction> Transactions { get; set; }

        private int linePtr = 1;

        public Account() { }

        public Account(string accountType, string[] lines)
        {
            Transactions = new List<Transaction>();
            try
            {
                switch (accountType)
                {
                    case "Bank":
                        ParseBankAccount(lines, ref linePtr);
                        break;
                    case "Cash":
                        ParseBankAccount(lines, ref linePtr);
                        break;
                    case "CCard":
                        ParseBankAccount(lines, ref linePtr);
                        break;
                    case "Oth A":
                        ParseBankAccount(lines, ref linePtr);
                        break;
                    case "Oth L":
                        ParseBankAccount(lines, ref linePtr);
                        break;
                    case "Invst":
                        ParseInvAccount(lines, ref linePtr);
                        break;
                    case "Cat":
                        throw new NotImplementedException("Category List is not implemented");
                    /*  Category List
                        *  Field	Indicator Explanation
                        N	Category name:subcategory name
                        D	Description
                        T	Tax related if included, not tax related if omitted
                        I	Income category
                        E	Expense category (if category type is unspecified, quicken assumes expense type)
                        B	Budget amount (only in a Budget Amounts QIF file)
                        R	Tax schedule information
                        ^	End of entry
                        */
                    case "Class":
                        throw new NotImplementedException("Class List is not implemented");
                    /*  Class List
                        *  Field	Indicator Explanation
                        N	Class name
                        D	Description
                        ^	End of entry
                        */
                    case "Memorized":
                        throw new NotImplementedException("Memorized Transaction is not implemented");
                    /*  Memorized Transaction
                        */
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{linePtr}: {ex.Message}");
            }
        }

        public void WriteBankTransactionsCSV(string pathname)
        {
            try
            {
                using (var writer = new StreamWriter(pathname))
                {
                    foreach (BankTransaction trans in Transactions)
                    {
                        writer.WriteLine(trans.ToCSV());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ParseBankAccount(string[] lines, ref int linePtr)
        {
            // Loop through the lines, creating transactions for each record
            while (true)
            {
                if (linePtr >= lines.Length)
                    break;
                Transactions.Add(new BankTransaction(lines, ref linePtr));
            }
        }

        private void ParseInvAccount(string[] lines, ref int linePtr)
        {
            throw new NotImplementedException("ParseInvAccount: Investment Accounts are not yet implemented");
        }
    }
}
