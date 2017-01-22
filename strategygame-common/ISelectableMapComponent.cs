using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public interface ISelectableMapComponent : IMapComponent
    {
        bool isSelected { get; set; }
    }
}
