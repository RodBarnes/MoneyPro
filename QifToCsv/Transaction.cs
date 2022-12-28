using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QifToCsv
{
    class Category
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }

        public Category() { }
        public Category(string primary, string secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }
    }

    class Transaction
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public char Cleared { get; set; } = 'N';
        public string Memo { get; set; }
        public Category Category { get; set; }
        public Category Classification { get; set; }

        public Transaction() { }

        public virtual string ToCSV()
        {
            return $"{Date.ToShortDateString()},{Amount},\"{Memo}\",\"{Cleared}\"";
        }
    }

    class BankTransaction : Transaction
    {
        public string PayeeName { get; set; }
        public string Reference { get; set; }
        public List<Transaction> Subtransactions { get; set; } = new List<Transaction>();

        public BankTransaction() { }
        public BankTransaction(string[] lines, ref int linePtr)
        {
            // Loop through the lines, updating the transaction property for each line
            while (true)
            {
                var line = lines[linePtr++];
                var flag = line[0];
                if (flag == '^')
                    break;
                var value = line.Substring(1, line.Length - 1);
                ParseLine(flag, value);
            }
        }

        public override string ToCSV()
        {
            return $"{base.ToCSV()},{Reference},\"{PayeeName}\"";
        }

        private void ParseLine(char flag, string value)
        {
            if (flag == 'L')
            {
                var pos = value.IndexOf('/');
                if (pos > 0)
                {
                    Category = GetCategory(value.Substring(0, pos));
                    Classification = GetCategory(value.Substring(pos + 1, value.Length - pos - 1));
                }
                else
                {
                    Category = GetCategory(value);
                }
            }
            else if (flag == 'D')
            {
                Date = GetDate(value);
            }
            else if (flag == 'C')
            {
                Cleared = (value == "X" ? 'Y' : 'N');
            }
            else if (flag == 'P')
            {
                PayeeName = value;
            }
            else if (flag == 'T')
            {
                Amount = decimal.Parse(value);
            }
            else if (flag == 'M')
            {
                Memo = value;
            }
            else if (flag == 'N')
            {
                Reference = value;
            }
            else if (flag == 'S')
            {
                // Add a split object category
                var sub = new Transaction();
                var pos = value.IndexOf('/');
                if (pos > 0)
                {
                    sub.Category = GetCategory(value.Substring(0, pos));
                    sub.Classification = GetCategory(value.Substring(pos + 1, value.Length - pos - 1));
                }
                else
                {
                    sub.Category = GetCategory(value);
                }
                Subtransactions.Add(sub);
            }
            else if (flag == '$')
            {
                // Update latest split object with amount
                var sub = Subtransactions[Subtransactions.Count-1];
                sub.Amount = decimal.Parse(value);
            }
            else if (flag == 'E')
            {
                // Update lastest split object with memo
                var sub = Subtransactions[Subtransactions.Count - 1];
                sub.Memo = value;
            }
            else if (flag == 'A')
            {
                throw new NotImplementedException($"ParseValue: Field '{flag}' for '{value}' is not yet implemented");
            }
            else
            {
                throw new Exception($"ParseValue: Unrecognized flag value '{flag}' for '{value}'");
            }
        }

        private Category GetCategory(string value)
        {
            var primary = value;
            var secondary = "";

            if (value.Contains(":"))
            {
                var pos = value.IndexOf(":");
                primary = value.Substring(0, pos);
                secondary = value.Substring(pos + 1, value.Length - pos - 1);
            }

            return new Category(primary, secondary);
        }

        private DateTime GetDate(string value)
        {
            var d1pos = value.IndexOf('/');
            var d2pos = value.IndexOf("'");
            var mval = int.Parse(value.Substring(0, d1pos));
            var dval = int.Parse(value.Substring(d1pos + 1, d2pos - d1pos - 1));
            var yval = int.Parse(value.Substring(d2pos + 1, value.Length - d2pos - 1));

            return new DateTime(yval, mval, dval);
        }
    }

    class InvTransaction : Transaction
    {
        public string Action { get; set; }
        public string Security { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Commission { get; set; }
        public decimal TransferAmount { get; set; }

        public InvTransaction() { }
        public InvTransaction(List<string> lines)
        {

        }
    }
}
