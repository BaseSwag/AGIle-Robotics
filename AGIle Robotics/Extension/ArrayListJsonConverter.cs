using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AGIle_Robotics.Extension
{
    class ArrayListJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool isArray = typeof(T[]) == objectType;
            bool isList = typeof(List<T>) == objectType;
            bool isIList = typeof(IList<T>) == objectType;
            return (isArray || isList || isIList);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var deserialized = serializer.Deserialize<List<T>>(reader);
            return deserialized.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = ((T[])value).ToList();
            serializer.Serialize(writer, list, typeof(List<T>));
        }
    }
}
