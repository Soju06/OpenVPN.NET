using System;
using System.Collections.Generic;

namespace OpenVPNNET {
    internal class OpenVPNRequest : Dictionary<string, string> {
        public static OpenVPNRequest Parse(string s) {
            var request = new OpenVPNRequest();
            var p = s.Split(',');
            for (int i = 0; i < p.Length; i++) {
                var q = s.IndexOf('=');
                var key = s.Substring(0, q);
                if (q > 0 && !string.IsNullOrWhiteSpace(key)) request.Add(key,
                    s.Substring(q + 1));
            } return request;
        }
    }
}
