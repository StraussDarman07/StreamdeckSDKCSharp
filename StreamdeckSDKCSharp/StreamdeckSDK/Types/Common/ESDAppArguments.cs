using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elgato.StreamdeckSDK.Types.Common
{
    public class ESDAppArguments
    {
        private const string PORT = "-port";
        private const string PLUGIN_UUID = "-pluginUUID";
        private const string REGISTER_EVENT = "-registerEvent";
        private const string INFO = "-info";

        public int Port { get; private set; }
        public string PluginUUID { get; private set; }
        public string RegisterEvent{ get; private set; }
        public string Info { get; private set; }

        private ESDAppArguments(){}

        public static ESDAppArguments Parse(string[] args)
        {
            ESDAppArguments arguments = new ESDAppArguments();
            for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
            {
                string parameter = args[argumentIndex];
                int nextElement = argumentIndex + 1;

                if (nextElement < args.Length)
                {
                    string value = args[nextElement];
                    argumentIndex++;
                    if (parameter == "-port")
                    {
                        arguments.Port = int.Parse(value);
                    }
                    else if (parameter == "-pluginUUID")
                    {
                        arguments.PluginUUID = value;
                    }
                    else if (parameter == "-registerEvent")
                    {
                        arguments.RegisterEvent = value;
                    }
                    else if (parameter == "-info")
                    {
                        arguments.Info = value;
                    }
                }
            }

            return arguments;
        }
    }
}
