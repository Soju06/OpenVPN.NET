using System;

namespace OpenVPNNET.Manager {
    public static class OpenVPNExceptionMessage {
        public const string OpenVPNNotInstalled = "OpenVPN is not installed";
        public const string OpenVPNManagementInitializationFailed = "Failed to initialize the OpenVPN Management";
        public const string OpenVPNManagementDisconnected = "Disconnected from OpenVPN Management socket\nHowever, OpenVPN may be running";
    }

    /// <summary>
    /// OpenVPN이 설치 되지 않았습니다.
    /// </summary>
    [Serializable]
    public class OpenVPNNotInstalledException : Exception {
        public OpenVPNNotInstalledException() { }
        public OpenVPNNotInstalledException(string message) : base(message) { }
        public OpenVPNNotInstalledException(string message, Exception inner) : base(message, inner) { }
        protected OpenVPNNotInstalledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// OpenVPN에서 Management를 초기화 실패했습니다.
    /// </summary>
    [Serializable]
    public class OpenVPNManagementInitializationFailedException : Exception {
        public OpenVPNManagementInitializationFailedException() { }
        public OpenVPNManagementInitializationFailedException(string message) : base(message) { }
        public OpenVPNManagementInitializationFailedException(string message, Exception inner) : base(message, inner) { }
        protected OpenVPNManagementInitializationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// OpenVPN에서 명령을 처리하는데 오류가 발생했습니다.
    /// </summary>
    [Serializable]
    public class OpenVPNManagementCommandErrorException : Exception {
        public OpenVPNManagementCommandErrorException() { }
        public OpenVPNManagementCommandErrorException(string message) : base(message) { }
        public OpenVPNManagementCommandErrorException(string message, Exception inner) : base(message, inner) { }
        protected OpenVPNManagementCommandErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// OpenVPN Management 소켓에 연결이 끊어졌습니다
    /// </summary>
    [Serializable]
    public class OpenVPNManagementDisconnectedException : Exception
    {
        public OpenVPNManagementDisconnectedException() { }
        public OpenVPNManagementDisconnectedException(string message) : base(message) { }
        public OpenVPNManagementDisconnectedException(string message, Exception inner) : base(message, inner) { }
        protected OpenVPNManagementDisconnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
