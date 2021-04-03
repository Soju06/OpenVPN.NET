using OpenVPNNET.Manager;
using System;
using System.Net;

namespace OpenVPNNET.Event {
    public class OpenVPNManagerLogMessage {
        internal OpenVPNManagerLogMessage(string s) {
            var idx = 0;
            for (int i = 0; i < 3; i++) {
                var ix = i != 2 ? s.IndexOf(',', idx) : -1;
                string l;
                if (ix < 0) l = s.Substring(idx);
                else l = s.Substring(idx, ix - idx);
                idx = ix + 1;
                switch (i) {
                    case 0: { // time
                            if (long.TryParse(l, out var r))
                                Time = new DateTime(1970, 1, 1).AddSeconds(r);
                            break;
                        }
                    case 1: { // type
                            if (string.IsNullOrWhiteSpace(l))
                                Type = LogMessageTypes.None;
                            else if (Enum.TryParse(l, out LogMessageTypes r))
                                Type = r;
                            break;
                        }
                    case 2: {
                            Message = l;
                            break;
                        }
                } if (idx >= s.Length) break;
            }
        }

        /// <summary>
        /// 시간 GMT
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 로그 타입
        /// </summary>
        public LogMessageTypes Type { get; set; }
        /// <summary>
        /// 메시지
        /// </summary>
        public string Message { get; set; }
    }

    public class OpenVPNManagerReceiveInfo {
        internal OpenVPNManagerReceiveInfo(bool isSuccess,
            string message, OpenVPNManagementCommandErrorException exception, bool isUserCommand) {
            IsSuccess = isSuccess; Message = message; ErrorException = exception; IsUserCommand = isUserCommand;
        }
        public bool IsSuccess { get; }
        public string Message { get; }
        public string ErrorMessage { get => ErrorException?.Message; }
        public OpenVPNManagementCommandErrorException ErrorException { get; }

        internal bool IsUserCommand { get; }

        /// <summary>
        /// 오류가 기록되어있는 경후 예외를 발생합니다.
        /// </summary>
        public void ThrowErrorException() {
            if (!IsSuccess && ErrorException != null)
                throw ErrorException;
        }
    }

    public class OpenVPNManagerStateMessage {
        internal OpenVPNManagerStateMessage(string s) {
            var idx = 0;
            RemainingParameters = new string[2];
            for (int i = 0; i < 8; i++) {
                var ix = i != 7 ? s.IndexOf(',', idx) : -1;
                string l;
                if (ix < 0) l = s.Substring(idx);
                else l = s.Substring(idx, ix - idx);
                idx = ix + 1;
                switch (i) {
                    case 0: { // time
                            if (long.TryParse(l, out var r))
                                Time = new DateTime(1970, 1, 1).AddSeconds(r);
                            break;
                        }
                    case 1: { // state
                            if (string.IsNullOrWhiteSpace(l))
                                State = StateMessageStateCode.None;
                            else if (Enum.TryParse(l, out StateMessageStateCode r))
                                State = r;
                            break;
                        }
                    case 2: { // state message
                            StateMessage = l;
                            break;
                        }
                    case 3: { // dns
                            if (IPAddress.TryParse(l, out var r))
                                DNS = r;
                            break;
                        }
                    case 4: { // ip
                            if (IPAddress.TryParse(l, out var r))
                                IPAddress = r;
                            break;
                        }
                    case 5: { // port
                            if (ushort.TryParse(l, out var r))
                                Port = r;
                            break;
                        }
                    case 6:
                    case 7: {
                            RemainingParameters[i - 6] = l;
                            break;
                        }
                } if (idx >= s.Length) break;
            }
        }

        /// <summary>
        /// 시간 GMT
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 로그 타입
        /// </summary>
        public StateMessageStateCode State { get; set; }
        /// <summary>
        /// 상태 메시지
        /// </summary>
        public string StateMessage { get; set; }
        /// <summary>
        /// DNS
        /// </summary>
        public IPAddress DNS { get; set; }
        /// <summary>
        /// IP 주소
        /// </summary>
        public IPAddress IPAddress { get; set; }
        /// <summary>
        /// 포트
        /// </summary>
        public ushort? Port { get; set; }

        public string[] RemainingParameters { get; set; }
    }
}
