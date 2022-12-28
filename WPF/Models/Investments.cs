using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoneyPro.Models
{
    public class Investments
    {
        private List<Investment> list { get; set; } = new List<Investment>();

        public Investments() { }

        public Investments(string filepath)
        {
            /*
             * Files are downloaded from https://www.eoddata.com
             * Login, click on Symbol Lists, select the exchange from the drop-down,
             * and click DOWNLOAD SYMBOL LIST in the upper right.
             * 
             * The files are tab delimited showing Symbol, Description.
             * 
             * American Stock Exchange (AMEX.txt)
             * New York Stock Exchange (NYSE.txt)
             * NASDAQ (NASDAQ.txt)
             * Mutual Funds (USMF.txt)
             * Global Indices (INDEX.txt)
             */
            string[] lines;
            list.Clear();
            var sources = new string[] { "AMEX", "NYSE", "NASDAQ", "USMF", "INDEX" };
            foreach (var source in sources)
            {
                lines = File.ReadLines($"{filepath}\\{source}.TXT").ToArray();
                foreach (var line in lines)
                {
                    var info = line.Split('\t');
                    var ticker = info[0];
                    if (ticker == "Symbol")
                        continue;
                    var company = info[1];
                    var inv = new Investment
                    {
                        Ticker = ticker,
                        Company = company,
                        Source = source
                    };
                    list.Add(inv);
                }
            }
        }

        #region Properties

        public void ToDB() => DatabaseManager.InvestmentsInsert(list);
        public void Add(Investment inv) => list.Add(inv);
        public Investment GetInvestmentWithTicker(string ticker) => list.Find(i => i.Ticker == ticker);
        public List<Investment> GetInvestmentsCompanyContainsText(string text) => list.Where(i => i.Company.Contains(text)).ToList();

        public bool Exists(string ticker)
        {
            var inv = list.Find(i => i.Ticker == ticker);

            return (inv == null) ? false : true;
        }

        public string GetTickerForCompany(string company)
        {
            var inv = list.Find(i => i.Company == company);

            return (inv == null) ? null : inv.Ticker;
        }

        #endregion
    }
}
