using System;
using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class SearchQuery
    {
        #region Properties

        public string Reference { get; set; }
        public string Account { get; set; } = null;
        public DateTime? FromDate { get; set; } = null;
        public DateTime? ToDate { get; set; } = null;
        public string Payee { get; set; } = null;
        public decimal? FromAmount { get; set; } = null;
        public decimal? ToAmount { get; set; } = null;
        public string Memo { get; set; } = null;
        public string Category { get; set; } = null;
        public string Subcategory { get; set; } = null;
        public string Class { get; set; } = null;
        public string Subclass { get; set; } = null;

        #endregion

        #region Methods

        public IList<SearchItem> Search() => DatabaseManager.Search(this);

        #endregion
    }
}
