using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperTuple
{
    public class IntTupleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var isIntValueTuple = objectType == typeof(ValueTuple<int, int>);
            var isIntTuple = objectType == typeof(Tuple<int, int>);
            var isIntSTuple = objectType == typeof(STuple<int, int>);
            return (isIntValueTuple || isIntTuple || isIntSTuple);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var deserialized = serializer.Deserialize<STuple<int, int>>(reader);
            (int, int) valueTuple = deserialized;
            return valueTuple;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (int, int) valueTuple = ((int, int))value;
            STuple<int, int> stuple = valueTuple;
            serializer.Serialize(writer, stuple, typeof(STuple<dynamic, dynamic>));
        }
    }
}
