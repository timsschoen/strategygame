using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{

    [JsonConverter(typeof(ResourceConverter))]
    public class Resources : IResources
    {
        public const int ResourceTypeCount = 5;

        static string[] ResourceNames = { "Steine", "Lehm", "Holz", "Stroh", "Getreide" };

        public decimal[] Content;

        public Resources()
        {
            Content = new decimal[ResourceTypeCount];
        }

        public void Add(IResources resources)
        {
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                Content[i] += resources.GetResourceCount(i);
            }
        }

        public bool ContainsMoreThan(IResources toTestAgainst)
        {
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                if(Content[i] < toTestAgainst.GetResourceCount(i))
                    return false;                
            }
            return true;
        }

        public decimal GetResourceCount(int Type)
        {
            if(ResourceTypeCount <= Type)
            {
                return 0;
            }

            return Content[Type];
        }

        public List<Tuple<int, string>> GetStringRepresentation()
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();

            for(int i = 0; i < ResourceTypeCount; i++)
            {
                if (Content[i] > 0)
                    result.Add(new Tuple<int,string>(i, ResourceNames[i] + ": " + Content[i].ToString()));
            }

            return result;
        }

        public string GetStringRepresentation(int resourceType)
        {
            if (ResourceTypeCount <= resourceType)
            {
                return "";
            }

            return ResourceNames[resourceType] + ": " + Content[resourceType].ToString();
        }

        public void SetResourceCount(int type, decimal value)
        {
            if (type >= ResourceTypeCount || type < 0)
            {
                return;
            }

            Content[type] = value;
        }

        public void Subtract(IResources resources)
        {
            for(int i = 0; i < ResourceTypeCount; i++)
            {
                Content[i] -= resources.GetResourceCount(i);
            }
        }
    }

    [JsonConverter(typeof(ResourceConverter))]
    public interface IResources
    {
        /// <summary>
        /// tests if these resources contain more of each resource type stored in toTestAgainst
        /// </summary>
        /// <param name="toTestAgainst">resources to test against</param>
        /// <returns></returns>
        bool ContainsMoreThan(IResources toTestAgainst);

        /// <summary>
        /// get count of resources of the given type in this instance
        /// </summary>
        /// <param name="type">resourcetype</param>
        /// <returns></returns>
        decimal GetResourceCount(int type);
        
        /// <summary>
        /// sets the new count for a resource type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        void SetResourceCount(int type, decimal value);

        /// <summary>
        /// Adds all resources contained in resources to this instance
        /// </summary>
        /// <param name="resources">the resources to add</param>
        void Add(IResources resources);

        /// <summary>
        /// Subtracts all resources contained in resources from this instance
        /// </summary>
        /// <param name="resources">the resources to add</param>
        void Subtract(IResources resources);

        /// <summary>
        /// Get string represtations of the resources stored
        /// </summary>
        /// <returns>list of tuples with resourcetype-id and the string represtation</returns>
        List<Tuple<int,string>> GetStringRepresentation();

        /// <summary>
        /// Get string represtations of a specific resources stored
        /// </summary>
        /// <param name="type">resourcetype</param>
        /// <returns>string represtation</returns>
        string GetStringRepresentation(int resourceType);
    }
}
