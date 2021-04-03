using OpenVPNNET;
using OpenVPNNET.Event;
using OpenVPNNET.Manager;
using OpenVPNNET.Manager.Stream;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVpnTest {
    internal class ConfigAndAccountExample {
        static OpenVPNManager Manager;
        static CancellationTokenSource CancellationToken;
        public static void Example() {
            CancellationToken = new();
            if (!OpenVPN.IsOpenVPNInstalled) {
                Console.WriteLine("OpenVPN is not installed!");
                Console.ReadLine();
                return;
            }

            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine("OpenVPN Manager - Config & Account Example");

            var Config = new OpenVPNManagerConfig();

            while (!CancellationToken.IsCancellationRequested) {
                try {
                    Program.Write("Config Path: ", ConsoleColor.Yellow);
                    Config.Config = File.ReadAllText(Console.ReadLine());
                    break;
                } catch (Exception ex) {
                    Program.WriteLine(ex.ToString(), ConsoleColor.Red);
                }
            }

            CancellationToken?.Token.ThrowIfCancellationRequested();

            Manager = new OpenVPNManager(Config);
            Manager.Stream.StateChangedEvent += Stream_StateChangedEvent;
            Manager.Stream.PasswordEvent += Stream_PasswordEvent;
            Manager.Stream.AnyEvent += Stream_AnyEvent;
            Console.WriteLine("OpenVPN opened.");
            Login();

            void Login() {
                try {
                    Task.Run(async () => {
                        Program.Write("Username: ", ConsoleColor.Cyan);
                        await Manager.Command.Username(Console.ReadLine(), 5, CancellationToken.Token);
                        Program.Write("Password: ", ConsoleColor.Cyan);
                        await Manager.Command.Password(Console.ReadLine(), 5, CancellationToken.Token);
                        while (!CancellationToken.IsCancellationRequested) await Task.Delay(100, CancellationToken.Token);
                    }, CancellationToken.Token).Wait();
                } catch {

                }
            }

            void Stream_PasswordEvent(OpenVPNManagerLoggerStream sender,
                OpenVPNManagerEventEventArgs<string> e) =>
                    Login();

            static void Stream_AnyEvent(OpenVPNManagerLoggerStream sender, OpenVPNManagerEventEventArgs e) {
                if(e.EventType != "STATE")
                    Program.WriteLine($"{e.EventType}>{e.EventMessage}", ConsoleColor.Gray);
            }

            static void Stream_StateChangedEvent(OpenVPNManagerLoggerStream sender, 
                OpenVPNManagerEventEventArgs<OpenVPNManagerStateMessage> e) {
                if(e?.EventMessage.State == StateMessageStateCode.CONNECTED) {
                    Program.WriteLine($"Connected. IP: {e.EventMessage.IPAddress}", ConsoleColor.Green);
                } else if (e.EventMessage is not null) {
                    Program.WriteLine($"Connecting.. {e.EventMessage.State}", ConsoleColor.Yellow);
                }
            }

            static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Exit();
            }

            static void Exit() {
                Console.WriteLine("Closeing..");
                CancellationToken?.Cancel();
                CancellationToken?.Dispose();
                Manager?.Close();
                Manager?.Dispose();
            }
        }
    }
}
