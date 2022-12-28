using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Account
    {
        public int Id { get; set; }
        public Institution Institution { get; set; }
        public AccountType Type { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }

        public Account(string name, string reference)
        {
            Name = name;
            Reference = reference;
        }
    }
}
