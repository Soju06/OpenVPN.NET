using OpenVPNNET.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVPNNET.Manager.Stream {
    public class OpenVPNManagerLoggerStream : OpenVPNManagerStreamBase {
        /// <summary>
        /// 작업 결과를 수신합니다.
        /// </summary>
        public event OpenVPNManagerLoggerReceiveEventHandler ReceivedEvent;
        /// <summary>
        /// 모든 이벤트를 수신합니다.
        /// </summary>
        public event OpenVPNManagerLoggerAnyEventEventHandler AnyEvent;
        /// <summary>
        /// Log 타입의 이벤트를 수신합니다.
        /// </summary>
        public event OpenVPNManagerLoggerEventEventHandler<OpenVPNManagerLogMessage> LogEvent;
        /// <summary>
        /// Password 타입의 이벤트를 수신합니다.
        /// </summary>
        public event OpenVPNManagerLoggerEventEventHandler<string> PasswordEvent;
        /// <summary>
        /// State 타입의 이벤트를 수신합니다.
        /// </summary>
        public event OpenVPNManagerLoggerEventEventHandler<OpenVPNManagerStateMessage> StateChangedEvent;

        private readonly byte[] Buffer;
        private readonly CancellationTokenSource CancellationToken;

        public OpenVPNManagerLoggerStream(TcpClient client) : base(client) {
            base.Send("log all on");
            base.Send("echo all on");
            base.Send("state all on");
            Buffer = new byte[BufferSize];
            CancellationToken = new();
            Task.Run(async () => await ReceiveLoop(CancellationToken.Token));
        }

        private async Task ReceiveLoop(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                if (Stream == null || !Stream.DataAvailable) {
                    await Task.Delay(100, token); continue;
                }

                string s = "";
                while (true) {
                    int i = await Stream.ReadAsync(Buffer, 0, Buffer.Length, token);
                    s += Encoding.Default.GetString(Buffer, 0, i);
                    if (i < Buffer.Length) break;
                } ReceivedEventInvoke(new(ReceiveDecode(s)));
            }
        }

        private OpenVPNManagerReceiveInfo ReceiveDecode(string s) {
            var ss = s.Split('\n');
            string r = "", em = null;
            bool nw, succ = true, iuc = false;
            for (int i = 0; i < ss.Length; i++) {
                nw = false;
                var l = ss[i];
                if (l.Length > 1) {
                    if (l[0] == '>') { // event
                        nw = true;
                        EventDecode(l.Substring(1));
                    } else if (l.Trim().IndexOf("END") == 0) iuc = nw = true;
                } if (!nw && !string.IsNullOrWhiteSpace(l)) r += l + '\n';
            }
            if (r.Contains("SUCCESS: ")) {
                r = r.Replace("SUCCESS: ", "");
                iuc = succ = true;
            } else if (r.Contains("ERROR: ")) { 
                r = r.Replace("ERROR: ", "");
                iuc = true;
                succ = false; em = r;
            }
            return new(succ, em == null ? r : null, em == null ? null : new OpenVPNManagementCommandErrorException(em), iuc);
        }

        private void EventDecode(string s) {
            var si = s.IndexOf(':');
            if (si < 0) return;
            string type = s.Substring(0, si), 
                message = s.Substring(si + 1);
            if (string.IsNullOrWhiteSpace(type)) return;
            AnyEventInvoke(type, message);
            EventDecode(type, message);
        }

        private void EventDecode(string type, string message) {
            if (!Enum.TryParse(type, out OpenVPNManagerEventTypes r)) return;
            switch (r) {
                case OpenVPNManagerEventTypes.LOG:
                    LogEventInvoke(message);
                    break;
                case OpenVPNManagerEventTypes.PASSWORD:
                    PasswordEventInvoke(message);
                    break;
                case OpenVPNManagerEventTypes.STATE:
                    StateEventInvoke(message);
                    break;
            }
        }

        protected List<CommandInfo> CommandQueue { get; } = new();

        private readonly object SendLock = new();

        /// <summary>
        /// 명령을 보냅니다
        /// </summary>
        /// <param name="cmd">명령어</param>
        /// <param name="timeout">시간 제한</param>
        /// <returns></returns>
        public async Task<OpenVPNManagerReceiveInfo> Send(string cmd, long timeout = 20, CancellationToken? token = null) {
            CancellationToken.Token.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(cmd)) throw new ArgumentException("command is empty");
            if (!Connected) throw new OpenVPNManagementDisconnectedException(OpenVPNExceptionMessage.OpenVPNManagementDisconnected);
            lock (SendLock) { }
            var ci = new CommandInfo(cmd, timeout);
            CommandQueue.Add(ci);
            base.Send(cmd);
            while (!ci.IsTimeout && 
                !CancellationToken.IsCancellationRequested &&
                (!token.HasValue || !token.Value.IsCancellationRequested)) {
                if (ci.Completed && ci.ReceiveInfo != null) {
                    var info = ci.ReceiveInfo;
                    CommandQueue.Remove(ci);
                    return info;
                } 
                if(token.HasValue)
                    await Task.Delay(100, token.Value);
                else await Task.Delay(100);
            }
            CommandQueue.Remove(ci); 
            return null;
        }

        /// <summary>
        /// 매니저에 연결되어있는 경우 OpenVPN을 종료합니다
        /// </summary>
        public void Close() {
            try {
                if (!Connected) return;
                Task.Run(async () => await Send("exit", 5));
            } catch {

            }
        }

        private void CommandQueueProcess(OpenVPNManagerReceiveInfo info) {
            if (CommandQueue.Count <= 0 || info == null || !info.IsUserCommand) return;
            for (int i = 0; i < CommandQueue.Count; i++)
                try {
                    var c = CommandQueue[i];
                    if (c.IsTimeout)
                        CommandQueue.Remove(c);
                    else if (!c.Completed) {
                        c.ReceiveInfo = info;
                        c.Completed = true;
                        break;
                    }
                }
                catch {

                }
        }

        private void ReceivedEventInvoke(OpenVPNManagerReceiveEventArgs e) {
            CommandQueueProcess(e?.ReceiveInfo);
            ReceivedEvent?.Invoke(this, e);
        }
        private void AnyEventInvoke(string type, string message) =>
            AnyEvent?.Invoke(this, new(type, message));
        private void PasswordEventInvoke(string message) =>
            PasswordEvent?.Invoke(this, new(OpenVPNManagerEventTypes.PASSWORD, message));
        private void LogEventInvoke(string message) =>
            LogEvent?.Invoke(this, new(OpenVPNManagerEventTypes.LOG, new(message)));
        private void StateEventInvoke(string message) =>
            StateChangedEvent?.Invoke(this, new(OpenVPNManagerEventTypes.STATE, new(message)));

        public override void Dispose() {
            try {
                Close();
                CancellationToken.Cancel();
                base.Dispose();
            } catch {

            }
        }

        protected class CommandInfo {
            internal CommandInfo(string command, long timeout) {
                Command = command; Timeout = DateTime.UtcNow.AddSeconds(timeout);
            }

            public string Command;
            public DateTime Timeout;
            public bool Completed;
            public OpenVPNManagerReceiveInfo ReceiveInfo;

            public bool IsTimeout {
                get => Timeout < DateTime.UtcNow;
            }
        }
    }
    
    public delegate void OpenVPNManagerLoggerStreamAsyncCallbackHandler(OpenVPNManagerLoggerStream sender, OpenVPNManagerReceiveEventArgs e);

    public delegate void OpenVPNManagerLoggerReceiveEventHandler(OpenVPNManagerLoggerStream sender, OpenVPNManagerReceiveEventArgs e);
    public delegate void OpenVPNManagerLoggerEventEventHandler<Message>(OpenVPNManagerLoggerStream sender, OpenVPNManagerEventEventArgs<Message> e);
    public delegate void OpenVPNManagerLoggerAnyEventEventHandler(OpenVPNManagerLoggerStream sender, OpenVPNManagerEventEventArgs e);
}
