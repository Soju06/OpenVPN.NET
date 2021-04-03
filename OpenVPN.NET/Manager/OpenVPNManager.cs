﻿using OpenVPNNET.Manager.Command;
using OpenVPNNET.Manager.Stream;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OpenVPNNET.Manager
{
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
        public OpenVPNManagerLoggerStream Stream { get; }

        private IPEndPoint ManagementAddress { get; }
        private TcpClient ManagementCommandSocket { get; }
        private Process OpenVPNProcess { get; }
        private string ConfigPath { get; }

        public OpenVPNManager(OpenVPNManagerConfig config) {
            if (config == null) throw new ArgumentNullException("config");
            if(config.OpenVPNExePath == null)
                config.OpenVPNExePath = OpenVPN.GetOpenVPNInstalledFilePath;
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
            try {
                if (Stream.Connected) Stream.Close();
                Stream?.Dispose();
                OpenVPNProcess?.Kill();
                OpenVPNProcess?.Close();
                if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
            } catch {

            }
        }

        public bool Disposed { get; private set; } = false;
        public bool Disposing { get; private set; } = false;

        /// <summary>
        /// 종료 및 제거합니다.
        /// </summary>
        public void Dispose() {
            if (Disposed) return;
            Disposing = true;
            Close();
            Disposing = false;
            Disposed = true;
        }
    }
}
