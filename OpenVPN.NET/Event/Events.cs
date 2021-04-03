using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVPNNET.Event {
    /// <summary>
    /// 이벤트
    /// </summary>
    /// <typeparam name="Message">이벤트 메시지</typeparam>
    public class OpenVPNManagerEventEventArgs : EventArgs {
        internal OpenVPNManagerEventEventArgs(string @event, string message) {
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
    public class OpenVPNManagerEventEventArgs<Message> : EventArgs {
        internal OpenVPNManagerEventEventArgs(OpenVPNManagerEventTypes @event, Message message) {
            EventType = @event; EventMessage = message;
        }
        /// <summary>
        /// 이벤트 타입
        /// </summary>
        public OpenVPNManagerEventTypes EventType { get; }

        /// <summary>
        /// 이벤트 메시지
        /// </summary>
        public Message EventMessage { get; }
    }

    public class OpenVPNManagerReceiveEventArgs : EventArgs {
        internal OpenVPNManagerReceiveEventArgs(OpenVPNManagerReceiveInfo info) {
            ReceiveInfo = info;
        }

        public OpenVPNManagerReceiveInfo ReceiveInfo { get; }
    }
}
