using System;
using System.IO;
using System.Text;
using NLog;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;

namespace Skateboard3Server.Host.Blaze
{
    public class BlazeTlsServer : DefaultTlsServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly X509Certificate _certificate;
        private readonly AsymmetricKeyParameter _privateKey;

        public BlazeTlsServer(BlazeTlsOptions options)
        {
            var privatePemReader = new PemReader(new StringReader(options.PrivateKeyPem));
            _privateKey = (AsymmetricKeyParameter)privatePemReader.ReadObject();

            var certPemReader = new PemReader(new StringReader(options.CertificatePem));
            _certificate = (X509Certificate)certPemReader.ReadObject();
        }

        protected override TlsEncryptionCredentials GetRsaEncryptionCredentials()
        {
            return new DefaultTlsEncryptionCredentials(mContext, new Certificate(new[] {_certificate.CertificateStructure}), _privateKey);
        }

        protected override int[] GetCipherSuites() => new[] { CipherSuite.TLS_RSA_WITH_RC4_128_MD5, CipherSuite.TLS_RSA_WITH_RC4_128_SHA }; //Ciphers sent by ps3
        protected override ProtocolVersion MaximumVersion => ProtocolVersion.SSLv3; //Force SSLv3
        protected override ProtocolVersion MinimumVersion => ProtocolVersion.SSLv3; //Force SSLv3

        public override void NotifySecureRenegotiation(bool secureRenegotiation) { } //Prevent AbstractTlsPeer from throwing exception

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
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
}

