using MirDB;
using Server.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Repository.File
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Session _session;
        private readonly DBCollection<AccountInfo> _collection;

        public AccountRepository(Session session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _collection = _session.GetCollection<AccountInfo>();
        }
    }
}
