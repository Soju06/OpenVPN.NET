using Microsoft.Win32;
using OpenVpnManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVpnTest {
    class Program {
        private static OpenVPNManager Manager;

        static void Main(string[] args) {
            if (!OpenVPN.IsOpenVPNInstalled) {
                Console.WriteLine("OpenVPN이 설치되어있지 않습니다!");
                Console.ReadLine();
                Environment.Exit(0);
            }
            Application.ApplicationExit += Application_ApplicationExit;
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine("OpenVPN");
            Manager = new OpenVPNManager(new OpenVPNConfig(File.ReadAllText(@"D:\Users\soju_\Downloads\jp585.nordvpn.com.udp1194.ovpn")));
            Console.WriteLine("OpenVPN opened.");
            Console.WriteLine("state: {0}", Manager.State(true));
            //Console.WriteLine("login..");
            //Console.WriteLine(Manager.Username(""));
            //Console.WriteLine(Manager.Password(""));
            Console.WriteLine("pid: {0}", Manager.Pid());
            Console.WriteLine(Manager.Help());
            Console.WriteLine(Manager.Log());
            while (true) {
                Console.Write("tx>");
                try {
                    var cmd = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(cmd)) continue;
                    Console.WriteLine("rx>\n" + Manager.SendCommand(cmd));
                } catch (OpenVPNManagementCommandErrorException ex) {
                    Console.WriteLine("rx ERROR>\n{0}", ex.Message);
                }
            }
            //Application.Run();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            Exit();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e) => Exit();

        private static void Exit() {
            Console.WriteLine("종료중..");
            Manager?.Dispose();
            Environment.Exit(0);
        }
    }
}