using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenVPNNET.Manager
{
    public class OpenVPNManagerConfig {
        public OpenVPNManagerConfig() { }
        public OpenVPNManagerConfig(string config, params IPAddress[] dns) {
            Config = config;
            if (dns != null && dns.Length > 0) DNS.AddRange(dns);
        }

        /// <summary>
        /// openvpn.exe 경로
        /// </summary>
        public string OpenVPNExePath { get; set; } = OpenVPN.GetOpenVPNInstalledFilePath;

        /// <summary>
        /// 서비스 이벤트 이름
        /// </summary>
        public string ServiceEventName { get; set; } = "OpenVPNManagerEvent";

        /// <summary>
        /// 구성
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// 데몬 콘솔을 숨깁니다
        /// </summary>
        public bool NoWindowDaemon { get; set; } = true;

        /// <summary>
        /// UAC를 사용합니다 만약 (NoWindowDaemon is true) 경우 UAC 승인 화면이 생략될 수 있습니다.
        /// </summary>
        public bool UseUAC { get; set; } = true;

        /// <summary>
        /// DNS
        /// </summary>
        public List<IPAddress> DNS { get; } = new List<IPAddress>();

        public IPEndPoint ManagementAddress { get; set; }

        public string GetConfig() {
            var sb = new StringBuilder();
            sb.AppendLine(ConfigEncoding(Config ?? ""));
            if(DNS != null)
                foreach (var item in DNS) 
                    sb.AppendLine($"dhcp-option DNS {item}");
            return sb.ToString();
        }
        
        private string ConfigEncoding(string config) {
            if (config is null) throw new ArgumentNullException("config");
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
