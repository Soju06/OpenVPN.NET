using System;

namespace OpenVPN.NET {
    /// <summary>
    /// 이벤트
    /// </summary>
    /// <typeparam name="Message">이벤트 메시지</typeparam>
    public class OpenVPNEventArgs : EventArgs {
        internal OpenVPNEventArgs(string @event, string message) {
            EventType = @event; EventMessage = message;
        }

        /// <summary>
        /// 이벤트 타입
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// 이벤트 메시지
        /// </summary>
        public string EventMessage { get; }
    }

    /// <summary>
    /// 이벤트
    /// </summary>
    /// <typeparam name="Message">이벤트 메시지</typeparam>
    public class OpenVPNEventArgs<Message> : EventArgs {
        internal OpenVPNEventArgs(EventTypes @event, Message message) {
            EventType = @event; EventMessage = message;
        }
        /// <summary>
        /// 이벤트 타입
        /// </summary>
        public EventTypes EventType { get; }

        /// <summary>
        /// 이벤트 메시지
        /// </summary>
        public Message EventMessage { get; }
    }

    public class OpenVPNReceiveEventArgs : EventArgs {
        internal OpenVPNReceiveEventArgs(OpenVPNReceiveInfo info) {
            ReceiveInfo = info;
        }

        public OpenVPNReceiveInfo ReceiveInfo { get; }
    }
}
