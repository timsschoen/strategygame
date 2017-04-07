using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public class EffectDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JArray ArrayOfListsJSON = JArray.Load(reader);
            List<IBuildingEffect>[] ArrayOfLists = new List<IBuildingEffect>[ArrayOfListsJSON.Count];
            
            for(int i = 0; i < ArrayOfListsJSON.Count; i++)
            {
                ArrayOfLists[i] = new List<IBuildingEffect>();
                JArray effectList = (JArray)ArrayOfListsJSON[i];

                for(int j = 0; j < effectList.Count;j++)
                {
                    JObject effect = (JObject)effectList[j];

                    if (effect["Type"].ToString() == "Production")
                    {
                        ArrayOfLists[i].Add(effect.ToObject<ProductionEffect>());
                    }
                    else if (effect["Type"].ToString() == "KeyValue")
                    {
                        ArrayOfLists[i].Add(effect.ToObject<KeyValueEffect>());
                    }
                }
            }

            return ArrayOfLists;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //we dont need to write
        }
    }

    public class ResourceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IResources) || objectType == typeof(Resources));           
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            Resources parsedResources = new Resources();

            Dictionary<int, decimal> values = obj.ToObject<Dictionary<int, decimal>>();

            foreach(KeyValuePair<int,decimal> kvp in values)
            {
                parsedResources.SetResourceCount(kvp.Key, kvp.Value);
            }

            return parsedResources;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if((value is IResources) || !(value is Resources))
            {
                writer.WriteStartObject();
                                
                for(int i = 0; i < Resources.ResourceTypeCount; i++)
                {
                    decimal val = ((IResources)value).GetResourceCount(i);

                    if(val != 0)
                    {
                        writer.WritePropertyName(i.ToString());
                        writer.WriteValue(val);
                    }                         
                }
                
                writer.WriteEndObject();
            }
            else
            {
                JToken.FromObject(value).WriteTo(writer);
            }
        }
    }

}
