using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenVPNNet
{
    public class OpenVPNConfig {
        public OpenVPNConfig() { }
        public OpenVPNConfig(string config, params IPAddress[] dns) {
            Config = config;
            if (dns != null && dns.Length > 0) DNS.AddRange(dns);
        }

        private string Config { get; set; }

        /// <summary>
        /// DNS
        /// </summary>
        public List<IPAddress> DNS { get; } = new List<IPAddress>();

        public IPEndPoint ManagementAddress { get; set; }

        public string GetConfig() {
            var sb = new StringBuilder();
            sb.AppendLine(ConfigEncoding(Config));
            if(DNS != null)
                foreach (var item in DNS) 
                    sb.AppendLine($"dhcp-option DNS {item}");
            return sb.ToString();
        }
        
        private string ConfigEncoding(string config) {
            var commands = config.Split('\n').ToList();

            const string management = "management";
            const string managementFormat = "management {0} {1}";
            var managementIndex = -1;

            for (int i = 0; i < commands.Count; i++) {
                if (commands[i].StartsWith(management))
                    //if (managementIndex == -1) managementIndex = i;
                    //else 
                    commands[i] = string.Empty;
            }

            var managementCommand = string.Format(managementFormat,
                    ManagementAddress.Address.ToString(), ManagementAddress.Port);
            if (managementIndex == -1)
                commands.Add(managementCommand);
            else commands[managementIndex] = managementCommand;

            return string.Join(Environment.NewLine, commands);
        }
    }
}
