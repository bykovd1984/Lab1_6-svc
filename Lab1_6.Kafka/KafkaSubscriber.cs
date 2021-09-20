using System.Threading.Tasks;

namespace Lab1_6.Kafka
{
    public abstract class KafkaSubscriber<T>
    {
        public abstract Task ProcessMessage(T message);
    }
}
