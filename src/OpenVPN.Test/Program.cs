using System;
using System.Collections.Generic;

namespace OpenVpnTest {
    class Program {
        static void Main() {
            var examples = new Dictionary<string, ExampleInfo> {
                {
                    "1",
                    new ExampleInfo(ConfigAndAccountExample.Example,
                        "Use a configuration file and implement automatic login")
                }
            };
            while (true) {
                Console.WriteLine("OpenVPN.NET Examples");
                Console.WriteLine();
                foreach (var item in examples)
                    Console.WriteLine("{0} - {1}", item.Key, item.Value.Overview);
                Console.WriteLine();
                Console.Write("Please select an example: ");
                if(examples.TryGetValue(Console.ReadLine(), out var r))
                    r.Main.Invoke();
                Console.Clear();
            }
        }

        class ExampleInfo {
            public ExampleInfo(Action main, string overview) {
                Main = main; Overview = overview;
            }

            public Action Main { get; set; }
            public string Overview { get; set; }
        }

        private static readonly object Lock = new();

        public static void Write(string s, ConsoleColor? color = null) {
            lock (Lock) {
                if (color.HasValue)
                    Console.ForegroundColor = color.Value;
                Console.Write(s);
                Console.ResetColor();
            }
        }
        public static void WriteLine(string s, ConsoleColor? color = null) {
            lock (Lock) {
                if (color.HasValue)
                    Console.ForegroundColor = color.Value;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }
    }
}