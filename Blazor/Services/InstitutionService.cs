using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class InstitutionService
    {
        private List<Institution> list = new List<Institution>
        {
            new Institution("US Bank", "503-555-1234"),
            new Institution("PenFed", "800-555-6789"),
            new Institution("Barclay", "877-555-2839"),
            new Institution("Personal Capital", "866-555-2738"),
            new Institution("UMB", "800-555-9098"),
            new Institution("USAA", "800-555-8734")
        };

        public Task<Institution[]> GetInstitutionAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
