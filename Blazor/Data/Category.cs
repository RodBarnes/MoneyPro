using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsTaxRelated { get; set; }
        public DateTime DateLastUsed { get; set; }
        public int ReferenceCount { get; set; }

        public Category(string name, DateTime date, int count, bool isTaxable)
        {
            Name = name;
            DateLastUsed = date;
            ReferenceCount = count;
            IsTaxRelated = isTaxable;
        }
    }
}
