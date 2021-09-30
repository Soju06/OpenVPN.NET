using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OpenVPN.NET.Manager {
    /// <summary>
    /// OpenVPN Manager
    /// </summary>
    public class OpenVPNManager : IDisposable {
        /// <summary>
        /// OpenVPN 실행 파일
        /// </summary>
        public string OpenVPNExePath { get; }
        /// <summary>
        /// 이벤트 이름
        /// </summary>
        public string ServiceEventName { get; } = "OpenVPNManagerEvent";
        /// <summary>
        /// 명령어
        /// </summary>
        public OpenVPNManagerCommand Command { get; }
        /// <summary>
        /// 스트림
        /// </summary>
        public OpenVPNMessageStream Stream { get; }

        IPEndPoint ManagementAddress { get; }
        TcpClient ManagementCommandSocket { get; }
        Process OpenVPNProcess { get; }
        string ConfigPath { get; }

        public OpenVPNManager(OpenVPNConfig config) {
            if (config == null) throw new ArgumentNullException("config");
            if(config.OpenVPNExePath == null)
                config.OpenVPNExePath = OpenVPNEnvironment.GetOpenVPNInstalledFilePath;
            if (!File.Exists(config.OpenVPNExePath)) throw new OpenVPNNotInstalledException
                    (OpenVPNExceptionMessage.OpenVPNNotInstalled);
            if (!string.IsNullOrWhiteSpace(config.ServiceEventName)) 
                ServiceEventName = config.ServiceEventName;
            OpenVPNExePath = config.OpenVPNExePath;
            config.ManagementAddress = ManagementAddress = OpenVPNUtility.GetManagementIPEndPoint();
            OpenVPNProcess = OpenVPNUtility.CreateOpenVPNProcess(
                OpenVPNExePath, ConfigPath = OpenVPNUtility.ConfigSaveTemp(config.GetConfig()),
                ServiceEventName, ManagementAddress, config.NoWindowDaemon, config.UseUAC);
            ManagementCommandSocket = new();
            ManagementCommandSocket.Connect(ManagementAddress);
            Stream = new(ManagementCommandSocket);
            Command = new(Stream);
        }

        /// <summary>
        /// OpenVPN을 종료합니다.
        /// </summary>
        public void Close() {
            if (Disposed) return;
            Disposing = true;
            try {
                if (Stream.Connected) Stream.Close();
                Stream?.Dispose();
                OpenVPNProcess?.Kill();
                OpenVPNProcess?.Close();
                if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
            } catch {

            }
            Disposing = false;
            Disposed = true;
        }

        public bool Disposed { get; private set; } = false;
        public bool Disposing { get; private set; } = false;

        /// <summary>
        /// 종료 및 제거합니다.
        /// </summary>
        public void Dispose() => Close();
    }
}
