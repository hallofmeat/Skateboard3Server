using System;
using System.Text;
using NLog;
using Org.BouncyCastle.Crypto.Tls;

namespace Skate3Server.BlazeProxy
{
    //https://stackoverflow.com/q/48995332
    //https://stackoverflow.com/a/16815827
    public class BlazeTlsClient : DefaultTlsClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override TlsAuthentication GetAuthentication() => new BlazeTlsAuthentication();
        public override int[] GetCipherSuites() => new[] { CipherSuite.TLS_RSA_WITH_RC4_128_SHA, CipherSuite.TLS_RSA_WITH_RC4_128_MD5 }; //Ciphers sent by ps3
        public override ProtocolVersion ClientVersion => ProtocolVersion.SSLv3; //Force SSLv3
        public override ProtocolVersion MinimumVersion => ProtocolVersion.SSLv3; //Force SSLv3

        public override void NotifySecureRenegotiation(bool secureRenegotiation) { } //Prevent AbstractTlsPeer from throwing exception

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            base.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
            var sb = new StringBuilder();
            sb.AppendLine($"AlertLevel: {alertLevel}");
            sb.AppendLine($"AlertDescription: {alertDescription}");
            sb.AppendLine($"Message: {message}");
            sb.AppendLine($"Exception: {cause}");
            Logger.Error($"Exception in BlazeTlsClient: {sb}");
        }
    }

    public class BlazeTlsAuthentication : TlsAuthentication
    {
        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest) => null;
        public void NotifyServerCertificate(Certificate serverCertificate) { }
    }
}