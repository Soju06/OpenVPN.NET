using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVPNNet {
    /// <summary>
    /// OpenVPN Manager
    /// </summary>
    public class OpenVPNManager : IDisposable
    {
        /// <summary>
        /// OpenVPN 실행 파일
        /// </summary>
        public string OpenVPNPath { get; }

        /// <summary>
        /// 이벤트 이름
        /// </summary>
        public string ServiceEventName { get; } = "OpenVPNManagerEvent";

        public event OpenVPNManagerReadLineEventHandler ReadLineEvent;

        private IPEndPoint ManagementAddress { get; }
        private TcpClient ManagementSocket { get; }
        private NetworkStream ManagementStream { get; }
        private Process OpenVPNProcess { get; }
        private string ConfigPath { get; }

        private const int BufferSize = 1024;

        public OpenVPNManager(OpenVPNConfig config,
            string openvpnPath = null, string serviceEventName = null) {
            if (config == null) throw new ArgumentNullException("config");
            if(openvpnPath == null)
                openvpnPath = OpenVPN.GetOpenVPNInstalledFilePath;
            if (string.IsNullOrWhiteSpace(openvpnPath)) throw new OpenVPNNotInstalledException
                    (OpenVPNExceptionMessage.OpenVPNNotInstalled);
            if (serviceEventName != null) ServiceEventName = serviceEventName;
            OpenVPNPath = openvpnPath;
            config.ManagementAddress = ManagementAddress = OpenVPNUtility.GetManagementIPEndPoint();
            OpenVPNProcess = OpenVPNUtility.CreateOpenVPNProcess(
                OpenVPNPath, ConfigPath = OpenVPNUtility.ConfigSaveTemp(config.GetConfig()), ServiceEventName, ManagementAddress);
            ManagementSocket = new();
            ManagementSocket.Connect(ManagementAddress);
            ManagementStream = ManagementSocket.GetStream();
            ManagementStream.ReadTimeout = 10000;
            var buffer = new byte[BufferSize];
            if (ManagementStream.Read(buffer, 0, buffer.Length) < 1)
                throw new OpenVPNManagementInitializationFailedException
                    (OpenVPNExceptionMessage.OpenVPNManagementInitializationFailed);
        }

        /// <summary>
        /// 인증 실패 재시도 모드
        /// </summary>
        public string SetAuthFailureRetryMode(OpenVPNManagerAuthRetryMode mode) =>
            SendCommand($"auth-retry {mode.ToString().ToLower()}");

        /// <summary>
        /// 입출력 바이트 표시, sec 초마다 업데이트 (0 = off)
        /// </summary>
        public string InOutByteCountUpdateEverySecond(uint sec) =>
            SendCommand($"bytecount {sec}");

        /// <summary>
        /// 로그와 비슷하지만 에코 버퍼의 메시지만 표시합니다
        /// </summary>
        public string Echo() =>
            SendCommand("echo all");
        
        /// <summary>
        /// 로그와 비슷하지만 에코 버퍼의 메시지만 표시합니다
        /// </summary>
        public string Echo(bool on) =>
            SendCommand($"echo {(on ? "on" : "off")}");
        
        /// <summary>
        /// 로그와 비슷하지만 에코 버퍼의 메시지만 표시합니다
        /// </summary>
        public string Echo(uint n) =>
            SendCommand($"echo {n}");

        /// <summary>
        /// 지금까지 입력한 비밀번호를 잊어버립니다
        /// </summary>
        public string ForgetPasswords() =>
            SendCommand($"forget-passwords");

        /// <summary>
        /// 명령 정보를 출력합니다
        /// </summary>
        public string Help() =>
            SendCommand("help");

        /// <summary>
        /// 보류 플래그를 On/Off 상태로 설정 / 표시합니다
        /// </summary>
        public string Hold(OpenVPNManagerHoldMode mode) =>
            SendCommand($"hold {mode.ToString().ToLower()}");

        /// <summary>
        /// 전역 서버 부하 통계를 표시합니다
        /// </summary>
        public string LoadStats() =>
            SendCommand("load-stats");

        /// <summary>
        /// 실시간 로그 표시 On/Off
        /// </summary>
        public string Log(bool on) =>
            SendCommand($"log {(on ? "on" : "off")}");

        /// <summary>
        /// 전체 기록에 대해 마지막 N 줄을 표시합니다
        /// </summary>
        public string Log(uint n) =>
            SendCommand($"log {n}");

        /// <summary>
        /// 전체 로그를 표시합니다
        /// </summary>
        public string Log() =>
            SendCommand("log all");

        /// <summary>
        /// level를 표시합니다
        /// </summary>
        public string Mute() =>
            SendCommand("mute");

        /// <summary>
        /// 로그 음소거 수준을 level으로 설정합니다
        /// </summary>
        public string Mute(byte level) =>
            SendCommand($"mute{level}");

        /// <summary>
        /// (Windows 전용) 네트워크 정보 및 라우팅 테이블을 표시합니다
        /// </summary>
        public string Net() =>
            SendCommand("net");

        public string Username(string username) =>
            SendCommand($"username Auth {username}");

        public string Password(string password) =>
            SendCommand($"password Auth {password}");
        
        public string State(bool all = false) =>
            SendCommand($"state{(all ? " all" : "")}");

        public string SetState(bool on) =>
            SendCommand($"state {(on ? "on" : "off")}");

        /// <summary>
        /// pid, 실패시 음수 반환
        /// </summary>
        public int Pid() {
            if (OpenVPNRequest.Parse(SendCommand("pid"))
                .TryGetValue("pid", out var p) && int.TryParse(p, out var pid))
                return pid;
            return -1;
        }

        /// <summary>
        /// 시그널을 보냅니다
        /// </summary>
        public string SendSignal(OpenVPNManagerSignal signal) =>
            SendCommand($"signal SIG{signal.ToString().ToUpper()}");

        public readonly object SendCommandLock = new();

        /// <summary>
        /// 명령을 보냅니다
        /// </summary>
        public string SendCommand(string cmd, bool locking = true) {
            lock (locking ? SendCommandLock : new object()) {
                if (ManagementSocket == null || !ManagementSocket.Connected)
                    throw new OpenVPNManagementDisconnectedException
                        (OpenVPNExceptionMessage.OpenVPNManagementDisconnectedFailed);
                var bf = Encoding.Default.GetBytes(cmd + "\r\n");
                ManagementStream.Write(bf, 0, bf.Length);
                ManagementStream.Flush();
                var buffer = new byte[BufferSize];
                int size;
                var sb = new StringBuilder();
                string line;
                Thread.Sleep(150);
                while ((size = ManagementStream.Read(buffer, 0, buffer.Length)) > 0) {
                    Thread.Sleep(100);
                    line = Encoding.Default.GetString(buffer, 0, size);
                    if(size < buffer.Length) {
                        if (line.Contains("ERROR: "))
                            throw new OpenVPNManagementCommandErrorException
                                (line.Replace("ERROR: ", "").Replace("\r\n", ""));
                        string msg;
                        if (line.Contains("\r\nEND"))
                            msg = line.Substring(0, line.IndexOf("\r\nEND"));
                        else if (line.Contains("SUCCESS: "))
                            msg = line.Replace("SUCCESS: ", "").Replace("\r\n", "");
                        else continue;
                        sb.Append(msg);
                        break;
                    } else sb.AppendLine(line);
                }
                var message = sb.ToString();
                ReadLineEvent?.Invoke(this, message);
                return message;
            }
        }

        /// <summary>
        /// OpenVPN을 종료합니다.
        /// </summary>
        public void Stop() {
            try {
                if (ManagementSocket != null &&
                    ManagementSocket.Connected)
                    SendCommand("exit", false);

                ManagementStream?.Close();
                ManagementSocket?.Close();
                OpenVPNProcess?.Kill();
                OpenVPNProcess?.Close();
                if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
            } catch {

            }
        }

        public void Dispose() {
            Stop();
        }
    }
    
    /// <summary>
    /// 시그널
    /// </summary>
    public enum OpenVPNManagerSignal {
        Hup, Term, Usr1, Usr2, Int
    }

    /// <summary>
    /// 인증 재시도 모드
    /// </summary>
    public enum OpenVPNManagerAuthRetryMode {
        /// <summary>
        /// 없음
        /// </summary>
        None,
        /// <summary>
        /// 상호 작용
        /// </summary>
        Interact,
        /// <summary>
        /// 상호 작용 없음
        /// </summary>
        Nointeract
    }

    public enum OpenVPNManagerHoldMode {
        On, Off, Release
    }

    public delegate void OpenVPNManagerReadLineEventHandler(OpenVPNManager sender, string line);
}
