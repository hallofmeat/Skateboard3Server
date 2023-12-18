using System;
using System.Text;
using NLog;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;

namespace Skateboard3Server.BlazeProxy;

//https://stackoverflow.com/q/48995332
//https://stackoverflow.com/a/16815827
public class BlazeTlsClient : AbstractTlsClient
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public BlazeTlsClient(TlsCrypto crypto) : base(crypto)
    {
    }

    public override TlsAuthentication GetAuthentication() => new BlazeTlsAuthentication();
    protected override ProtocolVersion[] GetSupportedVersions() => ProtocolVersion.SSLv3.Only();
    protected override int[] GetSupportedCipherSuites() => new[] { CipherSuite.TLS_RSA_WITH_RC4_128_MD5, CipherSuite.TLS_RSA_WITH_RC4_128_SHA }; //Ciphers sent by ps3
    public override void NotifySecureRenegotiation(bool secureRenegotiation) { } //Prevent AbstractTlsPeer from throwing exception

    public override void NotifyAlertRaised(short alertLevel, short alertDescription, string message, Exception cause)
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
    public void NotifyServerCertificate(TlsServerCertificate serverCertificate) {}
    public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest) => null;
}