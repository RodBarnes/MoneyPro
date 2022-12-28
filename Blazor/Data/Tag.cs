using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateLastUsed { get; set; }
        public int ReferenceCount { get; set; }

        public Tag(string name, DateTime date, int count)
        {
            Name = name;
            DateLastUsed = date;
            ReferenceCount = count;
        }
    }
}
