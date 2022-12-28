using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class BudgetService
    {
        private List<Budget> list = new List<Budget>
        {
            new Budget("Food", 500.00M, 12, DateTime.Now, 0),
            new Budget("Electricity", 200.00M, 12, DateTime.Now, 0),
            new Budget("Garbage", 10.00M, 4, DateTime.Now, 0),
            new Budget("Internet", 75.00M, 12, DateTime.Now, 0),
            new Budget("Phone", 120.00M, 12, DateTime.Now, 0),
            new Budget("House", 345.90M, 1, DateTime.Now, 0),
            new Budget("Vehicle", 1000.00M, 1, DateTime.Now, 0),
            new Budget("Computer", 50.00M, 6, DateTime.Now, 0),
            new Budget("Appliance", 500.00M, 12, DateTime.Now, 0),
            new Budget("Property", 750.00M, 12, DateTime.Now, 0)
        };

        public Task<Budget[]> GetBudgetAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
