using Elgato.StreamdeckSDK;
using Elgato.StreamdeckSDK.Types.Common;

namespace Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            ESDAppArguments arguments = ESDAppArguments.Parse(args);

            ESDConnectionManager connection = new ESDConnectionManager(arguments.Port, arguments.PluginUUID, arguments.RegisterEvent);
            connection.Run().Wait();

        }
    }
}
