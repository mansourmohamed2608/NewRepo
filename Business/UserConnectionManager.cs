using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UserConnectionManager
    {
        private readonly Dictionary<Guid, string> _userConnectionMap = new Dictionary<Guid, string>();
        private readonly object _lock = new object();

        public void AddUserConnection(Guid userId, string connectionId)
        {
            lock (_lock)
            {
                _userConnectionMap[userId] = connectionId;
            }
        }

        public void RemoveUserConnection(Guid userId)
        {
            lock (_lock)
            {
                if (_userConnectionMap.ContainsKey(userId))
                {
                    _userConnectionMap.Remove(userId);
                }
            }
        }

        public string GetUserConnection(Guid userId)
        {
            lock (_lock)
            {
                if (_userConnectionMap.TryGetValue(userId, out string connectionId))
                {
                    return connectionId;
                }
                return null;
            }
        }
    }


}
