using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace OpenVPNNet {
    internal static class OpenVPNUtility {
        public static IPEndPoint GetManagementIPEndPoint() {
            const ushort MinUserPort = 1024;
            const ushort MaxUserPort = 49151;
            var usedPortList = new List<ushort>();
            var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            for (int i = 0; i < connections.Length; i++)
                usedPortList.Add((ushort)connections[i].LocalEndPoint.Port);
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            for (int i = 0; i < listeners.Length; i++)
                usedPortList.Add((ushort)listeners[i].Port);
            var avaports = new List<ushort>();
            for (ushort i = MinUserPort; i < MaxUserPort - MinUserPort; i++)
                if (!usedPortList.Contains(i))
                    avaports.Add(i);
                else usedPortList.Remove(i);
            if (avaports.Count <= 0) return null;
            return new(IPAddress.Loopback, avaports[new Random().Next(0, avaports.Count)]);
        }

        public static string ConfigSaveTemp(string config) {
            var temp = Path.GetTempPath();
            string path;
            while (true) if (!File.Exists(path = Path.Combine(temp, Guid.NewGuid().ToString()))) break;
            File.WriteAllText(path, config);
            return path;
        }

        public static Process CreateOpenVPNProcess(string openVPNPath, string configPath, string serviceEventName, IPEndPoint managementAddress) {
            var process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = $"--config \"{configPath}\" --service \"{serviceEventName}\" 0 --management {managementAddress.Address} {managementAddress.Port}" +
                " --management-query-passwords --management-signal --management-forget-disconnect --auth-retry interact";
            process.StartInfo.FileName = openVPNPath;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(openVPNPath);
            process.Start();
            return process;
        }

        public static string Escape(string s) {
            var r = "";
            for (int i = 0; i < s.Length; i++) {
                r += (s[i]) switch {
                    '#' => "\\#",
                    ';' => "\\;",
                    '\\' => "\\\\",
                    _ => s,
                };
            }
            return r;
        }
    }
}
