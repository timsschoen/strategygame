using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class Resources : IResources
    {
        const int ResourceTypeCount = 5;

        static string[] ResourceNames = { "Steine", "Lehm", "Holz", "Stroh", "Getreide" };

        decimal[] Content;

        public Resources()
        {
            Content = new decimal[ResourceTypeCount];
        }

        public bool ContainsEnough(IResources toTestAgainst)
        {
            bool Result = false;

            return Result;
        }

        public decimal GetResourceCount(int Type)
        {
            if(ResourceTypeCount <= Type)
            {
                return 0;
            }

            return Content[Type];
        }

        public List<string> GetStringRepresentation()
        {
            for(int i = 0; i < ResourceTypeCount; i++)
            {

            }
        }
    }

    public interface IResources
    {
        bool ContainsEnough(IResources toTestAgainst);
        decimal GetResourceCount(int Type);
        List<string> GetStringRepresentation();
    }
}
