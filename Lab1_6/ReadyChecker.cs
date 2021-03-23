using System.Threading.Tasks;

namespace Lab1_6
{
    public class ReadyChecker
    {
        public static bool IsReady { get; private set; } = false;

        public static void Start()
        {
            Task.Run(async () =>
            {
                await Task.Delay(5000);

                IsReady = true;
            });
        }
    }
}
