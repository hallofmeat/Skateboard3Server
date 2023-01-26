using System;
using System.IO;
using NLog;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Skateboard3Server.Common.Models;

namespace Skateboard3Server.Common.Tickets;

public interface IPs3TicketValidator
{
    bool ValidateTicket(Ps3Ticket ticket);
    bool ValidateSignature(Ps3Ticket ticket);
}

public class Ps3TicketValidator : IPs3TicketValidator
{
    //Based on https://github.com/LBPUnion/ProjectLighthouse/blob/4770beea393e6f23abe70ee718e8582b565f86a3/ProjectLighthouse/Tickets/NPTicket.cs
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static ECDomainParameters FromX9EcParams(X9ECParameters param) =>
        new(param.Curve, param.G, param.N, param.H, param.GetSeed());

    private static readonly ECDomainParameters Secp224K1 = FromX9EcParams(ECNamedCurveTable.GetByName("secp224k1"));
    private static readonly ECDomainParameters Secp192R1 = FromX9EcParams(ECNamedCurveTable.GetByName("secp192r1"));

    private static readonly ECPoint RpcnPublic = Secp224K1.Curve.CreatePoint(
        new BigInteger("b07bc0f0addb97657e9f389039e8d2b9c97dc2a31d3042e7d0479b93", 16),
        new BigInteger("d81c42b0abdf6c42191a31e31f93342f8f033bd529c2c57fdb5a0a7d", 16));

    private static readonly ECPoint PsnPublic = Secp192R1.Curve.CreatePoint(
        new BigInteger("a93f2d73da8fe51c59872fad192b832f8b9dabde8587233", 16),
        new BigInteger("93131936a54a0ea51117f74518e56aae95f6baff4b29f999", 16));

    public bool ValidateTicket(Ps3Ticket ticket)
    {
        //TODO validate ServiceId (is it for skate3?)
        //TODO validate IssuedDate
        //TODO validate ExpireDate
        //TODO validate issuer
        //TODO add flag for validation
        return ValidateSignature(ticket);
    }


    public bool ValidateSignature(Ps3Ticket ticket)
    {
        try
        {
            if (ticket.Footer == null)
            {
                Logger.Debug("Ticket footer null, can't validate ticket");
                return false;
            }

            var signature = ParseSignature(ticket.Footer.Signature);
            var cipherHex = Convert.ToHexString(ticket.Footer.CipherId);

            byte[]? verifyData;
            SignatureParams sigParams;
            switch (cipherHex)
            {
                case "5250434E": //rpcn
                    verifyData = ticket.RawBody;
                    sigParams = new SignatureParams("SHA-224", RpcnPublic, Secp224K1);
                    break;
                case "382DE58D": //psn
                    verifyData = ticket.RawTicket;
                    sigParams = new SignatureParams("SHA-1", PsnPublic, Secp192R1);
                    break;
                default:
                    Logger.Warn($"Unknown Cipher ID {cipherHex}");
                    return false;
            }

            if (verifyData == null)
            {
                Logger.Warn("Ticket data is missing, malformed ticket?");
                return false;
            }

            var pubKey = new ECPublicKeyParameters(sigParams.PublicKey, sigParams.CurveParams);
            var signer = SignerUtilities.GetSigner($"{sigParams.HashAlgo}withECDSA");
            signer.Init(false, pubKey);
            signer.BlockUpdate(verifyData, 0, verifyData.Length);

            return signer.VerifySignature(signature);
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Failed to validate ticket signature!");
            return false;
        }
    }

    private byte[] ParseSignature(byte[] signature)
    {
        //Handles ignoring empty bytes at the end of the signature
        var input = new MemoryStream(signature, writable: false);
        var result = new Asn1InputStream(input, signature.Length).ReadObject();
        return result.GetEncoded();
    }

    private class SignatureParams
    {
        public string HashAlgo { get; set; }
        public ECPoint PublicKey { get; set; }
        public ECDomainParameters CurveParams { get; set; }

        public SignatureParams(string hashAlgo, ECPoint pubKey, ECDomainParameters curve)
        {
            HashAlgo = hashAlgo;
            PublicKey = pubKey;
            CurveParams = curve;
        }
    }


}