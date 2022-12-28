using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class CategoryService
    {
        private List<Category> list = new List<Category>
        {
            new Category("Donation", DateTime.Now, 0, true),
            new Category("Gas", DateTime.Now, 0, false),
            new Category("Interest", DateTime.Now, 0, true),
            new Category("Dine out", DateTime.Now, 0, false),
            new Category("Books", DateTime.Now, 0, false),
            new Category("Misc", DateTime.Now, 0, false),
            new Category("Other Income", DateTime.Now, 0, false),
            new Category("Groceries", DateTime.Now, 0, false),
            new Category("Medical", DateTime.Now, 0, true),
            new Category("Personal", DateTime.Now, 0, false)
        };

        public Task<Category[]> GetCategoryAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
