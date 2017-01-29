using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    //http://stackoverflow.com/a/12202914/3063213
    public class ConcreteJsonConverter<TConcrete> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //assume we can convert to anything for now
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {   
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }

    public class ConcreteListJsonConverter<TInterface, TConcrete> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //assume we can convert to anything for now
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //http://stackoverflow.com/questions/8925400/cast-listt-to-listinterface
            List<TConcrete> ConcreteList = serializer.Deserialize<List<TConcrete>>(reader);

            if (ConcreteList == null)
                return null;

            return ConcreteList.Cast<TInterface>().ToList();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }
    
}
