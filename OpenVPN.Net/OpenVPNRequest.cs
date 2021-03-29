using System.Collections.Generic;

namespace OpenVPNNet {
    public class OpenVPNRequest : Dictionary<string, string> {
        public static OpenVPNRequest Parse(string s) {
            var request = new OpenVPNRequest();
            var p = s.Split(',');
            for (int i = 0; i < p.Length; i++) {
                var q = s.IndexOf('=');
                if (q > 0) request.Add(s.Substring(0, q),
                    s.Substring(q + 1));
            } return request;
        }
    }
}
