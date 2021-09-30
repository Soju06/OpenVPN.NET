using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenVPN.NET.Manager {
    public abstract class OpenVPNStreamBase : IDisposable {
        public TcpClient Client { get; }
        public uint BufferSize { get; protected set; } = 1024;

        protected NetworkStream Stream { get; }

        /// <summary>
        /// 연결 여부
        /// </summary>
        public bool Connected { get => Client?.Connected == true; }

        public OpenVPNStreamBase(TcpClient client) {
            if ((Stream = (Client = client).GetStream()) == null)
                throw new ArgumentNullException("stream");
            else if (!Client.Connected) throw new OpenVPNManagementDisconnectedException
                    ("this requires a TCP client connected");
            Stream.ReadTimeout = 10000;
            Thread.Sleep(100);
            SendGreeting();
        }

        private readonly object SendLock = new();

        /// <summary>
        /// Send
        /// </summary>
        protected virtual void Send(string cmd, bool locking = true) {
            lock (locking ? SendLock : new()) {
                if (Client == null || !Client.Connected)
                    throw new OpenVPNManagementDisconnectedException
                        (OpenVPNExceptionMessage.OpenVPNManagementDisconnected);
                var bf = Encoding.Default.GetBytes(cmd + "\r\n");
                Stream.Write(bf, 0, bf.Length);
                Stream.Flush();
            }
        }

        private void SendGreeting() {
            int i;
            var buffer = new byte[BufferSize];
            if(!Stream.DataAvailable)
                throw new OpenVPNManagementInitializationFailedException
                    (OpenVPNExceptionMessage.OpenVPNManagementInitializationFailed);
            while ((i = Stream.Read(buffer, 0, buffer.Length)) < 1) {
                if (i < BufferSize) break;
                throw new OpenVPNManagementInitializationFailedException
                    (OpenVPNExceptionMessage.OpenVPNManagementInitializationFailed);
            }
        }

        public bool Disposed { get; private set; } = false;
        public bool Disposing { get; private set; } = false;

        public virtual void Dispose() {
            if (Disposed) return;
            Disposing = true;
            try {
                Stream?.Close();
            } catch {

            } Disposing = false;
            Disposed = true;
        }
    }
}
