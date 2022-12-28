using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class Main : List<Account>
    {
        public static List<Account> AccountsRead(AccountStatus status = AccountStatus.Open) => DatabaseManager.AccountsRead(status);
    }
}
