using System;
using System.IO;
using DataClasses;

namespace QifToCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    throw new Exception("A QIF filename must be specified");
                }
                var fullname = args[0];
                //var fullname = @"D:\Source\BitBucket\MoneyPro\Database\Data\hsa.qif";
                //var fullname = @"D:\Source\BitBucket\MoneyPro\Database\Data\pfcu.qif";
                //var fullname = @"D:\Source\BitBucket\MoneyPro\Database\Data\usbank.qif";
                //var fullname = @"D:\Source\BitBucket\MoneyPro\Database\Data\savings.qif";
                //var fullname = @"D:\Source\BitBucket\MoneyPro\Database\Data\barclays.qif";

                var filename = Path.GetFileNameWithoutExtension(fullname);
                var fileext = Path.GetExtension(fullname);
                var filepath = Path.GetDirectoryName(fullname);
                //var rootname = Path.GetPathRoot(fullname);
                var acct = new Account(Path.GetFileNameWithoutExtension(fullname), null, "", "", 0, AccountStatus.Open);
                FileManager.ImportAccountTransactions(fullname, acct);

                // Output the data
                acct.ToCSV($"{filepath}\\{filename}.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
