using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class AccountTypeService
    {
        private List<AccountType> list = new List<AccountType>
        {
            new AccountType("Checking"),
            new AccountType("Savings"),
            new AccountType("Investment"),
            new AccountType("Credit Card"),
            new AccountType("Money Market"),
            new AccountType("Loan"),
            new AccountType("Asset")
        };

        public Task<AccountType[]> GetAccountTypeAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
