using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public int PerYear { get; set; }
        public DateTime DateLastUsed { get; set; }
        public int ReferenceCount { get; set; }

        public Budget(string name, decimal limit, int perYear, DateTime date, int count)
        {
            Name = name;
            Limit = limit;
            PerYear = perYear;
            DateLastUsed = date;
            ReferenceCount = count;
        }
    }
}
