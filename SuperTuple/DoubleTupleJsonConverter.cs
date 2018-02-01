using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperTuple
{
    public class DoubleTupleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var isDoubleValueTuple = objectType == typeof(ValueTuple<double, double>);
            var isDoubleTuple = objectType == typeof(Tuple<double, double>);
            var isDoubleSTuple = objectType == typeof(STuple<double, double>);
            return (isDoubleValueTuple || isDoubleTuple || isDoubleSTuple);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var deserialized = serializer.Deserialize<STuple<double, double>>(reader);
            (double, double) valueTuple = deserialized;
            return valueTuple;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (double, double) valueTuple = ((double, double))value;
            STuple<double, double> stuple = valueTuple;
            serializer.Serialize(writer, stuple, typeof(STuple<dynamic, dynamic>));
        }
    }
}
