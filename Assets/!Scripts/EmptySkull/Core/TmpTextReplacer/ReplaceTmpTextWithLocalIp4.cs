using System;
using System.Net;
using System.Net.Sockets;

namespace EmptySkull.Tools.Unity.UI
{
    public class ReplaceTmpTextWithLocalIp4 : ReplaceTmpText
    {
        protected override string ReplaceKey => "##IP##";
        protected override string NewText => LocalIp4();

        private static string LocalIp4()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}