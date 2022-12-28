using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

namespace MoneyPro.Services
{
    public class TagService
    {
        private List<Tag> list = new List<Tag>
        {
            new Tag("02 Ranger", DateTime.Now, 0),
            new Tag("12 FJR", DateTime.Now, 0),
            new Tag("Portugal", DateTime.Now, 0),
            new Tag("Canada", DateTime.Now, 0),
            new Tag("Tomi", DateTime.Now, 0),
            new Tag("Sharla", DateTime.Now, 0),
            new Tag("Steven", DateTime.Now, 0),
            new Tag("Matthew", DateTime.Now, 0),
            new Tag("Phillip", DateTime.Now, 0),
            new Tag("Jared", DateTime.Now, 0),
        };

        public Task<Tag[]> GetTagAsync()
        {
            return Task.FromResult(list.ToArray());
        }
    }
}
