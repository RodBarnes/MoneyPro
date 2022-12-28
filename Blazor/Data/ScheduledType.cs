using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class ScheduledType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ScheduledType(string name)
        {
            Name = name;
        }
    }
}
