namespace Skateboard3Server.Host.Blaze;

// openssl req -x509 -md5 -newkey rsa:1024 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost'
public class BlazeTlsOptions
{
    public BlazeTlsOptions(string privateKeyPem, string certificatePem)
    {
        PrivateKeyPem = privateKeyPem;
        CertificatePem = certificatePem;
    }
    public string PrivateKeyPem { get; }
    public string CertificatePem { get; }
}