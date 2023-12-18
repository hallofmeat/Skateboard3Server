using System;
using System.IO;
using System.Text;
using NLog;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Org.BouncyCastle.X509;

namespace Skateboard3Server.Host.Blaze;

public class BlazeTlsServer : AbstractTlsServer
{
    private readonly BcTlsCrypto _crypto;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly X509Certificate _certificate;
    private readonly AsymmetricKeyParameter _privateKey;

    public BlazeTlsServer(BlazeTlsOptions options, BcTlsCrypto crypto) : base(crypto)
    {
        _crypto = crypto;

        var privatePemReader = new PemReader(new StringReader(options.PrivateKeyPem));
        _privateKey = (AsymmetricKeyParameter)privatePemReader.ReadObject();

        var certPemReader = new PemReader(new StringReader(options.CertificatePem));
        _certificate = (X509Certificate)certPemReader.ReadObject();
    }

    public override TlsCredentials GetCredentials() => new BcDefaultTlsCredentialedDecryptor(_crypto, new Certificate(new TlsCertificate[] { new BcTlsCertificate(_crypto, _certificate.CertificateStructure) }), _privateKey);

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
        Logger.Error($"Exception in BlazeTlsServer: {sb}");
    }
}