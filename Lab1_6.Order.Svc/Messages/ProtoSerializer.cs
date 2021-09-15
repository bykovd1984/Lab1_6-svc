using Confluent.Kafka;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_6.Order.Svc.Messages
{
    public class ProtoSerializer<T> : ISerializer<T> where T : Google.Protobuf.IMessage<T>
    {
        public IEnumerable<KeyValuePair<string, object>>
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
                => config;

        public void Dispose() { }

        public byte[] Serialize(T data, SerializationContext context)
        {
            return data.ToByteArray();
        }
    }
}
