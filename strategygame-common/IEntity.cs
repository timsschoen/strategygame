using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public interface IEntity
    {
        string Name { get; set; }
        int Owner { get; set; }
    }

    public class EntityMessage : BaseMessage
    {
        public EntityMessage(int ClientID, int EntityID, IEntity Entity)
        {
            this.ClientID = ClientID;
            this.Entity = Entity;
            this.EntityID = EntityID;
        }
        
        public int EntityID;
        public IEntity Entity;
    }

    public class BaseEntity : IEntity
    {
        public BaseEntity(string Name, int Owner)
        {
            this.Name = Name;
            this.Owner = Owner;
        }

        public string Name { get; set; }
        public int Owner { get; set; }
    }
}
