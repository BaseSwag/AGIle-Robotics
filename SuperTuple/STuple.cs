using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperTuple
{
    public class STuple<T1, T2>
    {
        private (T1, T2) tuple;
        public T1 Item1 { get => tuple.Item1; set => tuple.Item1 = value; }
        public T2 Item2 { get => tuple.Item2; set => tuple.Item2 = value; }

        public STuple(T1 o1, T2 o2) => tuple = (o1, o2);
        public STuple((T1, T2) t) => tuple = t;
        public STuple(Tuple<T1, T2> t) => tuple = t.ToValueTuple();

        public static implicit operator ValueTuple<T1, T2>(STuple<T1, T2> t) => t.tuple;
        public static implicit operator Tuple<T1, T2>(STuple<T1, T2> t) => t.tuple.ToTuple();

        public override string ToString()
        {
            return $"({Item1.ToString()}, {Item2.ToString()})";
        }
    }
}
