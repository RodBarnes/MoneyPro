using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Institution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public Institution(string name, string phone)
        {
            Name = name;
            Phone = phone;
        }
    }
}
