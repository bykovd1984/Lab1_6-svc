using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_6.Order.Svc.Messages
{
    public class ProtoDeserializer<T> : IDeserializer<T> where T : Google.Protobuf.IMessage<T>
    {
        Func<byte[], bool, SerializationContext, T> _func;

        public ProtoDeserializer(Func<byte[], bool, SerializationContext, T> func)
        {
            _func = func;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return _func(data.ToArray(), isNull, context);    
        }
    }
}
