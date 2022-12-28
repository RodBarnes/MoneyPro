using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class AccountService
    {
        private List<Account> list = new List<Account>
        {
            new Account("name", "123456"),
            new Account("name", "USBSDF"),
            new Account("name", "7928347"),
            new Account("name", "90-09234"),
            new Account("name", "234-23490234-3")
        };

        public Task<Account[]> GetAccountAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
