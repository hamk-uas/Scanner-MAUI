using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_MAUI.Helpers
{
    public class Keys
    {
        private static Keys _instance;
        private static readonly object _lock = new object();
        private JObject _apiKey;

        private const string Namespace = "Scanner_MAUI";
        private const string FileName = "secrets.json";

        private Keys()
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Keys)).Assembly;
                var stream = assembly.GetManifestResourceStream($"{Namespace}.{FileName}");
                using (var reader = new StreamReader(stream))
                {
                    var file = reader.ReadToEnd();
                    _apiKey = JObject.Parse(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to loading file secrets.json {ex.Message}");
            }
        }

        //Singleton double-check multithreading
        public static Keys Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Keys();
                        }
                    }
                }
                return _instance;
            }
        }

        //indexator
        public string this[string SecretKey]
        {
            get
            {
                try
                {
                    var path = SecretKey.Split(':');

                    JToken node = _apiKey[path[0]];
                    for (int index = 1; index < path.Length; index++)
                    {
                        node = node[path[index]];
                    }
                    return node.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to get secrets by key {ex.Message}");
                    return "";
                }
            }
        }
    }
}
