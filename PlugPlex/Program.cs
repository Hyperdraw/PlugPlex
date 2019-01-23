using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using System.IO;

namespace PlugPlex
{
    class Program
    {
        public static WebSocketServer server;
        private static List<Plug> plugs = new List<Plug>();
        private static List<string> plugNames = new List<string>();

        static void Main(string[] args)
        {
            LoadPlugs();
            
            server = new WebSocketServer(23524);

            foreach(Plug plug in plugs)
            {
                MethodInfo genericMi = typeof(WebSocketServer).GetMethod("AddWebSocketService", new Type[] { typeof(string) });
                MethodInfo mi = genericMi.MakeGenericMethod(new Type[] { plug.GetType() });
                mi.Invoke(server, new string[] { "/" + plug.name });
            }

            server.Start();

            Console.WriteLine();

            Console.ReadKey(true);
        }

        private static void LoadPlugs()
        {
            string plugDir = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)), "Plugs");

            if (!Directory.Exists(plugDir))
            {
                Directory.CreateDirectory(plugDir);
            }

            DirectoryInfo dir = new DirectoryInfo(plugDir);

            foreach(FileInfo file in dir.GetFiles("*"))
            {
                if (file.Extension == ".dll")
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));
                    }
                    catch (BadImageFormatException)
                    {
                        continue;
                    }

                    foreach (Type type in assembly.ExportedTypes)
                    {
                        if (type.IsSubclassOf(typeof(Plug)))
                        {
                            Plug plug = (Plug)Activator.CreateInstance(type);

                            if (!plugNames.Contains(plug.name))
                            {
                                plugs.Add(plug);
                                Console.WriteLine("Loaded plug " + type.Name);
                                plugNames.Add(plug.name);
                            }
                            else
                            {
                                Console.WriteLine("Error: ambiguous plug name - " + plug.name);
                            }
                        }
                    }
                }
            }
        }
    }
}
