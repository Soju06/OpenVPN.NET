using Microsoft.Win32;
using System;
using System.IO;

namespace OpenVPN.NET {
    public static class OpenVPNEnvironment {
        private static RegistryKey GetOpenVPNUninstallRegistryKey() { 
            RegistryKey key = null;
            var is64bit = Environment.Is64BitOperatingSystem;
            for (int i = 0; i < 2; i++) {
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, i == 0 ? 
                    (is64bit ? RegistryView.Registry64 : RegistryView.Registry32) : RegistryView.Registry32)
                    .OpenSubKey(@"SOFTWARE\OpenVPN\", false);
                if (!is64bit || key != null) break;
            } return key;
        }

        /// <summary>
        /// OpenVPN 설치 여부
        /// </summary>
        public static bool IsOpenVPNInstalled {
            get => GetOpenVPNUninstallRegistryKey() != null;
        }

        /// <summary>
        /// OpenVPN 설치 경로
        /// </summary>
        public static string OpenVPNInstalledPath {
            get {
                var key = GetOpenVPNUninstallRegistryKey();
                if (key == null) return null;
                return key.GetValue(string.Empty, null)?.ToString();
            }
        }

        /// <summary>
        /// OpenVPN 실행 파일 경로
        /// </summary>
        public static string GetOpenVPNInstalledFilePath {
            get {
                var key = GetOpenVPNUninstallRegistryKey();
                if (key == null) return null;
                var path = key.GetValue("exe_path", null)?.ToString();
                if (File.Exists(path)) return path;
                return null;
            }
        }
    }
}
