using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class EntityCollection : Dictionary<int, IEntity>
    {
        private int _NextID = 0;
        public int NextID
        {
            get
            {
                _NextID++;
                return _NextID;
            }
        }
    }
}
