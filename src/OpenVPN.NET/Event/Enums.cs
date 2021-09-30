namespace OpenVPN.NET {    
    /// <summary>
    /// 이벤트 타입
    /// </summary>
    public enum EventTypes {
        /// <summary>
        /// 기록
        /// </summary>
        LOG,
        /// <summary>
        /// 비밀번호
        /// </summary>
        PASSWORD,
        /// <summary>
        /// 상태
        /// </summary>
        STATE
    }

    /// <summary>
    /// 로그 메시지 타입
    /// </summary>
    public enum LogMessageTypes {
        /// <summary>
        /// none
        /// </summary>
        None,
        /// <summary>
        /// info?
        /// </summary>
        I,
        /// <summary>
        /// direct?
        /// </summary>
        D,
        /// <summary>
        /// 경고?
        /// </summary>
        W
    }

    /// <summary>
    /// 메시지 상태 코드
    /// </summary>
    public enum MessageStateCode {
        None,
        /// <summary>
        /// Wait
        /// </summary>
        WAIT,
        /// <summary>
        /// 인증
        /// </summary>
        AUTH,
        /// <summary>
        /// 구성 가져오는중
        /// </summary>
        GET_CONFIG,
        /// <summary>
        /// 다시 연결중
        /// </summary>
        RECONNECTING,
        /// <summary>
        /// IP 할당
        /// </summary>
        ASSIGN_IP,
        /// <summary>
        /// 연결됨
        /// </summary>
        CONNECTED
    }
}
