using MoneyPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MoneyPro
{
    public static class ImportManager
    {
        public static void ImportAccountProperties(string filepath, out string acctType, out string name, out decimal balance)
        {
            var nameRex = new Regex("L\\[(.+)\\]");
            var balRex = new Regex("T(.+)");

            // Read the file into a list
            var lines = File.ReadLines($"{filepath}").ToList();

            // Get the account type
            var line = lines.FirstOrDefault(l => l[0] == '!');
            if (line == null)
                throw new Exception($"Import file '{filepath}' is not correct format.  First line does not have an account type.");

            // Set the account type
            acctType = line.Substring(6, line.Length - 6);

            // Get the Opening Balance entry,  if present
            line = lines.FirstOrDefault(l => l[0] == 'P');
            if (line != null)
            {
                balance = decimal.Parse(balRex.Match(lines.FirstOrDefault(l => l[0] == 'T')).Groups[1].Value);
                name = nameRex.Match(lines.FirstOrDefault(l => l[0] == 'L')).Groups[1].Value;
            }
            else
            {
                balance = 0;
                name = "";
            }
        }

        public static List<BankTransactionImport> ImportTransactions(string filepath, BackgroundWorker bw = null, DoWorkEventArgs bwe = null)
        {
            var list = new List<BankTransactionImport>();

            // For importing from a file
            // Based upon info from https://www.w3.org/2000/10/swap/pim/qif-doc/QIF-doc.htm

            var lines = File.ReadLines($"{filepath}").ToArray();
            var linePtr = 0;

            // Extract the data from the QIF into objects
            while (linePtr < lines.Length)
            {
                if (bwe != null && bwe.Cancel)
                {
                    break;
                }

                var line = lines[linePtr++];
                var flag = line[0];
                if (flag == '!')
                {
                    //var header = line.Substring(1, line.Length - 1);
                    //var typeName = header.Replace("Type:", "");
                    ParseBankAccount(list, lines, ref linePtr, bw, bwe);
                }
                else
                {
                    throw new Exception($"Unrecognized flag '{flag}' in file '{filepath}'.");
                }
            }

            return list;
        }

        private static void ParseBankAccount(List<BankTransactionImport> list, string[] lines, ref int linePtr, BackgroundWorker bw = null, DoWorkEventArgs bwe = null)
        {
            var readingOpeningBalance = false;

            try
            {
                // Loop through the lines, creating transactions for each record
                var trans = new BankTransactionImport();
                // Add base subtransaction
                trans.Subtransactions.Add(new Subtransaction());
                while (linePtr < lines.Length)
                {
                    if (bw != null && bw.CancellationPending)
                    {
                        bwe.Cancel = true;
                        break;
                    }

                    var line = lines[linePtr++];
                    var flag = line[0];
                    if (flag == '^')
                    {
                        if (trans.Subtransactions.Count == 1)
                        {
                            // There's only the base subtransaction so use it
                            trans.Subtransactions[0].TransactionId = trans.Model.TransactionId;
                        }
                        else
                        {
                            // There were actual subtransactions so remove the unneeded base
                            trans.Subtransactions.RemoveAt(0);
                        }
                        if (readingOpeningBalance)
                        {
                            // Skip the opening balance transaction
                            // This value is obtained when the file is selected for import
                            readingOpeningBalance = false;
                        }
                        else
                        {
                            list.Add(trans);
                        }
                        trans = new BankTransactionImport();
                        // Add base subtransaction
                        trans.Subtransactions.Add(new Subtransaction
                        {
                            TransactionId = trans.Model.TransactionId
                        });
                    }
                    else
                    {
                        var value = line.Substring(1, line.Length - 1);
                        if (flag == 'P' && value == "Opening Balance")
                        {
                            readingOpeningBalance = true;
                        }
                        if (!readingOpeningBalance)
                        {
                            ParseBankLine(trans, flag, value);
                        }
                    }
                    // For reporting progress
                    if (bw != null)
                    {
                        // Report progress
                        bw.ReportProgress(linePtr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception processing line {linePtr}: {ex.Message}", ex);
            }
        }

        private static void ParseBankLine(BankTransactionImport trans, char flag, string value)
        {
            switch (flag)
            {
                case 'L':
                    ParseCategory(trans.Subtransactions[trans.Subtransactions.Count - 1], value);
                    break;
                case 'T':
                    trans.Subtransactions[trans.Subtransactions.Count - 1].Amount = ParseDecimal(value);
                    break;
                case 'M':
                    trans.Model.Memo = value;
                    break;
                case 'D':
                    trans.Model.Date = ParseDate(value);
                    break;
                case 'C':
                    trans.Model.Status = (value == "X") ? TransactionStatus.C : TransactionStatus.N;
                    break;
                case 'P':
                    trans.Model.Payee = value;
                    break;
                case 'N':
                    trans.Model.Reference = value;
                    break;
                case 'S':
                    trans.Subtransactions.Add(new Subtransaction
                    {
                        TransactionId = trans.Model.TransactionId
                    });
                    ParseSplit(trans.Subtransactions[trans.Subtransactions.Count - 1], value);
                    break;
                case '$':
                    // Update latest split object with amount
                    trans.Subtransactions[trans.Subtransactions.Count - 1].Amount = decimal.Parse(value);
                    break;
                case 'E':
                    // Update lastest split object with memo
                    trans.Subtransactions[trans.Subtransactions.Count - 1].Memo = value;
                    break;
                default:
                    throw new NotImplementedException($"Transaction.ParseLine('{flag}',\"{ value }\", is not recognized.");
                    // A
            }
        }

        private static void ParseInvLine(InvTransactionImport trans, char flag, string value)
        {
            switch (flag)
            {
                case 'D':
                    trans.Model.Date = ParseDate(value);
                    break;
                case 'C':
                    trans.Model.Status = (value == "X") ? TransactionStatus.C : TransactionStatus.N;
                    break;
                case 'M':
                    trans.Model.Memo = value;
                    break;
                case 'P':
                    trans.Model.Description = value;
                    break;
                case 'N':
                    trans.Model.Action = value;
                    break;
                case 'Y':
                    trans.Model.Security = value;
                    break;
                case 'I':
                    trans.Model.Price = ParseDecimal(value);
                    break;
                case 'Q':
                    trans.Model.Quantity = ParseDecimal(value);
                    break;
                case 'O':
                    trans.Model.Commission = ParseDecimal(value);
                    break;
                case '$':
                    trans.Model.TransferAmount = ParseDecimal(value);
                    break;
                default:
                    throw new NotImplementedException($"Transaction.ParseLine('{flag}',\"{ value }\", is not recognized.");
            }
        }

        private static void ParseCategory(Subtransaction sub, string value)
        {
            var pos1 = value.IndexOf('/');
            if (pos1 > 0)
            {
                // Assume it has a category and a classification
                SplitCategory(value.Substring(0, pos1), out string primary, out string secondary);
                sub.Category = primary;
                sub.Subcategory = secondary;
                SplitCategory(value.Substring(pos1 + 1, value.Length - pos1 - 1), out string primary2, out string secondary2);
                sub.Class = primary2;
                sub.Subclass = secondary2;
            }
            else if (pos1 == 0)
            {
                // This occurs in some records where it is a transfer from an investment account
                pos1 += 1;
                var pos2 = value.IndexOf('|');
                if (pos2 > 0)
                {
                    SplitCategory(value.Substring(pos1, pos2 - 1), out string primary2, out string secondary2);
                    sub.Class = primary2;
                    sub.Subclass = secondary2;
                    SplitCategory(value.Substring(pos2 + 1, value.Length - pos2 - 1), out string primary, out string secondary);
                    sub.Category = primary;
                    sub.Subcategory = secondary;
                }
                else
                {
                    // Empty category so give it a default
                    sub.Category = "";
                    sub.Subcategory = "";
                    SplitCategory(value.Substring(pos1, value.Length - 1), out string primary, out string secondary);
                    sub.Class = primary;
                    sub.Subclass = secondary;
                }
            }
            else
            {
                // Assume it has only a category and no classification
                SplitCategory(value, out string primary, out string secondary);
                sub.Category = primary;
                sub.Subcategory = secondary;
            }
        }

        private static void ParseSplit(Subtransaction sub, string value)
        {
            // Add a split object category
            var pos = value.IndexOf('/');
            if (pos > 0)
            {
                // Assume it has a category and a classification
                SplitCategory(value.Substring(0, pos), out string primary, out string secondary);
                sub.Category = primary;
                sub.Subcategory = secondary;
                SplitCategory(value.Substring(pos + 1, value.Length - pos - 1), out string primary2, out string secondary2);
                sub.Class = primary2;
                sub.Subclass = secondary2;
            }
            else
            {
                // Assume it has only a category and no classification
                SplitCategory(value, out string primary, out string secondary);
                sub.Category = primary;
                sub.Subcategory = secondary;
            }
        }

        private static void SplitCategory(string value, out string primaryOut, out string secondaryOut)
        {
            // This assumes a standard MS Money value with primary and secondary
            // delimited by a colon
            var primary = value;
            var secondary = "";

            if (value.Contains(":"))
            {
                var pos = value.IndexOf(":");
                primary = value.Substring(0, pos);
                secondary = value.Substring(pos + 1, value.Length - pos - 1);
            }

            primaryOut = primary;
            secondaryOut = secondary;
        }

        private static DateTime ParseDate(string value)
        {
            // NOTE: Some records have a / instead of ' as the delimiter before date
            // This was found when importing a cash account with a date from 1996
            var d1pos = value.IndexOf('/');
            var d2pos = (value.Contains("'")) ? value.IndexOf("'") : value.IndexOf('/', d1pos + 1);
            var mval = int.Parse(value.Substring(0, d1pos));
            var dval = int.Parse(value.Substring(d1pos + 1, d2pos - d1pos - 1));
            var yval = int.Parse(value.Substring(d2pos + 1, value.Length - d2pos - 1));

            return new DateTime(yval, mval, dval);
        }

        private static decimal ParseDecimal(string value) => string.IsNullOrEmpty(value) ? 0 : decimal.Parse(value);
    }
}
