using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class PayeeService
    {
        private List<Payee> list = new List<Payee>
        {
            new Payee("Randy", DateTime.Now, 0),
            new Payee("PenFed", DateTime.Now, 0),
            new Payee("Barclay", DateTime.Now, 0),
            new Payee("Fred Meyer", DateTime.Now, 0),
            new Payee("Costco", DateTime.Now, 0),
            new Payee("Winco", DateTime.Now, 0),
            new Payee("Subway", DateTime.Now, 0),
            new Payee("Chevron", DateTime.Now, 0),
            new Payee("Home Depot", DateTime.Now, 0)
        };

        public Task<Payee[]> GetPayeeAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
