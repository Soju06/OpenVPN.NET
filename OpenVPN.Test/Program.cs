using Microsoft.Win32;
using OpenVPNNET;
using OpenVPNNET.Event;
using OpenVPNNET.Manager;
using OpenVPNNET.Manager.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVpnTest {
    class Program {
        static void Main(string[] args) {
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

        private static readonly object Lock = new object();

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
        
        //private static OpenVPNManager Manager;

            //if (!OpenVPN.IsOpenVPNInstalled) {
            //    WriteLine("OpenVPN이 설치되어있지 않습니다!");
            //    Console.ReadLine();
            //    Environment.Exit(0);
            //}

            //Application.ApplicationExit += Application_ApplicationExit;
            //Console.CancelKeyPress += Console_CancelKeyPress;
            //WriteLine("OpenVPN");
            //var config = new OpenVPNManagerConfig(File.ReadAllText(@"D:\Users\soju_\Downloads\jp526.nordvpn.com.udp.ovpn")) {
            //    NoWindowDaemon = false
            //};
            //Manager = new OpenVPNManager(config);
            //WriteLine("OpenVPN opened.");
            //Manager.Stream.ReceivedEvent += Logger_ReceivedEvent;
            //Manager.Stream.LogEvent += Logger_LogEvent;
            //Manager.Stream.StateChangedEvent += Logger_StateEvent;
            //Manager.Stream.AnyEvent += Logger_AnyEvent;
            //Manager.Stream.PasswordEvent += Logger_PasswordEvent;
            //Task.Run(async () => {
            //    await Manager.Command.Username("on;\\d#d");
            //    //await Manager.Stream.Send("password Auth ond");

            //    while (true) {
            //        Write("tx>");
            //        var v = Console.ReadLine();
            //        if (string.IsNullOrWhiteSpace(v)) continue;
            //        if(v == "pid")
            //            WriteLine("pid: " + await Manager.Command.GetPid(), ConsoleColor.DarkYellow);
            //        else if(v == "state") {
            //            var r = await Manager.Command.GetState();
            //            WriteLine($"state: {r.State} message: {r.StateMessage} DNS: {r.DNS} IP: {r.IPAddress} port: {r.Port}", ConsoleColor.DarkYellow);
            //        }   
            //        else {
            //            var r = await Manager.Stream.Send(v);
            //            WriteLine("rx>\n" + (r.IsSuccess ? r.Message : r.ErrorMessage),
            //                r.IsSuccess ? ConsoleColor.Yellow : ConsoleColor.Red);
            //        }
            //    }
            //}).Wait();
            //Application.Run();

        //private static void Logger_StateEvent(OpenVPNManagerLoggerStream sender, OpenVPNManagerEventEventArgs<OpenVPNManagerStateMessage> e) {
        //    WriteLine(e.EventMessage.Time.ToString(), ConsoleColor.Magenta);
        //    WriteLine($"state: {e.EventMessage.State}", ConsoleColor.Magenta);
        //    WriteLine($"message: {e.EventMessage.StateMessage}", ConsoleColor.Magenta);
        //    WriteLine($"dns: {e.EventMessage.DNS}", ConsoleColor.Magenta);
        //    WriteLine($"ip: {e.EventMessage.IPAddress}:{e.EventMessage.Port}", ConsoleColor.Magenta);
        //}

        //private static void Logger_ReceivedEvent(OpenVPNManagerLoggerStream sender, OpenVPNManagerReceiveEventArgs e)
        //{
        //    //Console.WriteLine(e.ReceiveInfo.IsSuccess);
        //    //if (e.ReceiveInfo.IsSuccess)
        //    //    WriteLine(e.ReceiveInfo.Message, ConsoleColor.Yellow);
        //    //else
        //    //    WriteLine(e.ReceiveInfo.ErrorMessage, ConsoleColor.Red);
        //}

        //private static void Logger_PasswordEvent(OpenVPNManagerLoggerStream logger, OpenVPNManagerEventEventArgs<string> @event)
        //{
        //    WriteLine($"{@event.EventType} {@event.EventMessage}", ConsoleColor.Blue);
        //}

        //private static void Logger_AnyEvent(OpenVPNManagerLoggerStream logger, OpenVPNManagerEventEventArgs @event)
        //{
        //    WriteLine($"{@event.EventType} {@event.EventMessage}", ConsoleColor.Cyan);
        //}

        //private static void Logger_LogEvent(OpenVPNManagerLoggerStream logger, OpenVPNManagerEventEventArgs<OpenVPNManagerLogMessage> @event) {
        //    WriteLine($"type {@event.EventType} {@event.EventMessage.Type} {@event.EventMessage.Time} {@event.EventMessage.Message}", ConsoleColor.Green);
        //    Console.WriteLine();
        //}


        //private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
        //    e.Cancel = true;
        //    Exit();
        //}

        //private static void Application_ApplicationExit(object sender, EventArgs e) => Exit();

        //private static void Exit() {
        //    Console.WriteLine("종료중..");
        //    Manager.Close();
        //    Manager?.Dispose();
        //    Environment.Exit(0);
        //}
    }
}