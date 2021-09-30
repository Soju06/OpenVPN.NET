namespace OpenVPN.NET.Manager {
    /// <summary>
    /// 시그널
    /// </summary>
    public enum ManagerSignal {
        Hup, Term, Usr1, Usr2, Int
    }

    /// <summary>
    /// 인증 재시도 모드
    /// </summary>
    public enum ManagerAuthRetryMode {
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

    public enum ManagerHoldMode {
        On, Off, Release
    }

    public enum ManagerState {
        /// <summary>
        /// OpenVPN의 초기 상태
        /// </summary>
        Connecting,
        /// <summary>
        /// (클라이언트 전용) 서버에서 초기 응답을 기다리는 중입니다.
        /// </summary>
        Wait,
        /// <summary>
        /// (클라이언트 전용) 서버로 인증합니다.
        /// </summary>
        Auth,
        /// <summary>
        /// (클라이언트 전용) 서버에서 구성 옵션 다운로드.
        /// </summary>
        GetConfig,
        /// <summary>
        /// 가상 네트워크 인터페이스에 IP 주소 할당.
        /// </summary>
        AssignIP,
        /// <summary>
        /// 시스템에 라우터 추가.
        /// </summary>
        AddRoutes,
        /// <summary>
        /// 연결됨.
        /// </summary>
        Connected,
        /// <summary>
        /// 재연결 중.
        /// </summary>
        Reconnecting,
        /// <summary>
        /// 정상 종료중
        /// </summary>
        Exittng,
        /// <summary>
        /// (클라이언트 전용) DNS 조회
        /// </summary>
        Resolve,
        /// <summary>
        /// (클라이언트 전용) TCP 서버에 연결
        /// </summary>
        TCP_Connect
    }
}
