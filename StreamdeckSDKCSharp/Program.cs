using System;
using System.Threading.Tasks;
using Elgato.StreamdeckSDK;

namespace StreamdeckSDK
{
    class Program
    {
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("MyHandler caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }

        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;

            int port = 0;
            string pluginUUID = "";
            string registerEvent = "";
            string info;

            for (int argumentIndex = 0; argumentIndex < 4; argumentIndex++)
            {
                string parameter = args[ 2 * argumentIndex];
                string value = args[2 * argumentIndex + 1];

                if (parameter == "-port")
                {
                    port = int.Parse(value);
                }
                else if (parameter == "-pluginUUID")
                {
                    pluginUUID = value;
                }
                else if (parameter == "-registerEvent")
                {
                    registerEvent = value;
                }
                else if (parameter == "-info")
                {
                    info = value;
                }
            }
			ESDConnectionManager connection = new ESDConnectionManager(port, pluginUUID, registerEvent);
            connection.KeyDownForAction += (sender, msg) => connection.LogMessage(msg.Payload.Coordinate.ToString()).Wait(); 
            connection.Run().Wait();
			System.Console.WriteLine("DEBUG");
        }
    }
}
