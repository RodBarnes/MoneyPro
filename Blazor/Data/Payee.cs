using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Payee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateLastUsed { get; set;}
        public int ReferenceCount { get; set; }

        public Payee(string name, DateTime date, int count)
        {
            Name = name;
            DateLastUsed = date;
            ReferenceCount = count;
        }
    }
}
