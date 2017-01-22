using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class Resources : IResources
    {
        public Dictionary<int, decimal> Content;

        public bool ContainsEnough(IResources toTestAgainst)
        {
            throw new NotImplementedException();
        }
    }

    public interface IResources
    {
        bool ContainsEnough(IResources toTestAgainst);
    }
}
