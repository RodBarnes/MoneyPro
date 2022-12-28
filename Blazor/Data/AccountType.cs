using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class AccountType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AccountType(string name)
        {
            Name = name;
        }
    }
}
