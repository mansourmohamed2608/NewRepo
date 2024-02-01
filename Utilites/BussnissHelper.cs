using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class BussnissHelper
    {
        public static (int take, int skip) GetTakeSkipValues(int pageNumber ,int pageSize)
        {
            return (pageSize, (pageNumber - 1) * pageSize);
        }
    }
}
