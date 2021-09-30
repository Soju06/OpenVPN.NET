using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace OpenVPNNET {
    public static class OpenVPNUtility {
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

        public static Process CreateOpenVPNProcess(string openVPNPath, string configPath,
            string serviceEventName, IPEndPoint managementAddress, bool hideWindiow, bool useUAC) {
            var process = new Process {
                EnableRaisingEvents = true
            };
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = hideWindiow;
            process.StartInfo.WindowStyle = hideWindiow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            if(useUAC) process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = $"--config \"{configPath}\" --service \"{serviceEventName}\" 0 --management {managementAddress.Address} {managementAddress.Port}" +
                " --management-query-passwords --management-signal --management-forget-disconnect --auth-retry interact";
            process.StartInfo.FileName = openVPNPath;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(openVPNPath);
            process.Start();
            return process;
        }

        public static string Escape(string s) {
            if (s is null) return "";
            var r = "";
            for (int i = 0; i < s.Length; i++) {
                r += (s[i]) switch {
                    //'#' => "\\#",
                    //';' => "\\;",
                    '\\' => "\\\\",
                    _ => s[i],
                };
            }
            return r;
        }

        public static string EscapeStateCode(string state, sbyte mode = 1) {
            string e = "";
            char c;
            bool ec = false;
            var ops = new uint[2];
            var cs = "_-";
            for (int i = 0; i < state.Length; i++) {
                c = state[i];
                if(cs.Contains(c))
                    ops[c == cs[0] ? 0 : 1] += 1;
                if ((i == 0 || ec) && 'a' <= c && 'z' >= c) {
                    c = (char)(c - ('a' - 'A')); 
                    if (i != 0 && ec) ec = false;
                } else if (cs.Contains(c))
                    ec = true;
                switch (mode) {
                    case 1:
                        if (c >= 'A' && c <= 'Z' && i > 0 
                            && (!cs.Contains(state[i - 1])) 
                            && (!(state[i - 1] >= 'A' && state[i - 1] <= 'Z')))
                                 e += ops[0] >= ops[1] ? cs[0] : cs[1];
                        break;
                } e += c;
            } return e;
        }
    }
}
