using System;
using System.IO;
using System.Text;
using NLog;
using Skateboard3Server.Common.Models;

namespace Skateboard3Server.Common.Decoders;

//https://psdevwiki.com/ps3/X-I-5-Ticket
//https://github.com/RipleyTom/rpcn/blob/master/src/server/client/ticket.rs
//https://www.psdevwiki.com/ps3/PSN
public interface IPs3TicketDecoder
{
    Ps3Ticket? DecodeTicket(byte[] ticketData);
}

public class Ps3TicketDecoder : IPs3TicketDecoder
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Ps3Ticket? DecodeTicket(byte[] ticketData)
    {
        try
        {
            var ticket = new Ps3Ticket();
            using (var reader = new BinaryReader(new MemoryStream(ticketData)))
            {
                //Ticket Header
                var majorVersion = (byte) (reader.ReadByte() >> 4);
                var minorVersion = reader.ReadByte();
                ticket.Header = new TicketHeader
                {
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion
                };

                reader.ReadBytes(4); //00 00 00 00

                var ticketLength = reader.ReadUInt16Be();
                if (ticketLength != (ticketData.Length - 8)) //subtract 8 because the header is 8 bytes long
                {
                    Logger.Debug($"Ticket length did not match data");
                    return null;
                }
                
                //TODO: check sections
                if (majorVersion == 2 && minorVersion == 0) //2.0
                {
                    ticket.Body = Read20Body(reader);
                }
                else if (majorVersion == 2 && minorVersion == 1) //2.1
                {
                    ticket.Body = Read21Body(reader); 
                }
                else if (majorVersion == 3 && minorVersion == 0) //3.0
                {
                    ticket.Body = Read30Body(reader);
                }
                else if (majorVersion == 4 && minorVersion == 0) //4.0
                {
                    ticket.Body = Read40Body(reader);
                }
                else
                {
                    Logger.Debug($"Unsupported ticket version {majorVersion}.{minorVersion}");
                    return null;
                }

                //TODO handle incomplete body reading
                ticket.Footer = ReadFooter(reader); //footer

            }

            return ticket;
        }
        catch (Exception e)
        {
            Logger.Error($"Invalid Ticket {e}");
            return null;
        }
    }

    private TicketBody Read20Body(BinaryReader reader)
    {
        var body = new TicketBody();
        var bodyHeader = ReadSectionHeader(reader);

        //serial_id
        var serialHeader = ReadValueHeader(reader);
        body.SerialId = reader.ReadBytes(serialHeader.Length);

        //issuer_id
        var issuerHeader = ReadValueHeader(reader);
        body.IssuerId = reader.ReadUInt32Be();

        //issued_date
        var issuedDateHeader = ReadValueHeader(reader);
        body.IssuedDate = reader.ReadUInt64Be();

        //expire_date
        var expireDateHeader = ReadValueHeader(reader);
        body.ExpireDate = reader.ReadUInt64Be();

        //user_id
        var userIdHeader = ReadValueHeader(reader);
        body.UserId = reader.ReadUInt64Be();

        //online_id
        var usernameHeader = ReadValueHeader(reader);
        //TODO: confirm utf8
        body.Username = Encoding.ASCII.GetString(reader.ReadBytes(usernameHeader.Length)).TrimEnd('\0');

        //region/lang
        var regionHeader = ReadValueHeader(reader);
        body.Region = Encoding.ASCII.GetString(reader.ReadBytes(3)).TrimEnd('\0');
        body.Language = reader.ReadByte();

        //domain
        var domainHeader = ReadValueHeader(reader);
        body.Domain = Encoding.ASCII.GetString(reader.ReadBytes(domainHeader.Length)).TrimEnd('\0');

        //service_id
        var serviceHeader = ReadValueHeader(reader);
        body.ServiceId = reader.ReadBytes(serviceHeader.Length);

        //age/status
        var statusHeader = ReadValueHeader(reader);
        body.Age = reader.ReadByte();
        reader.ReadByte(); //00
        body.Status = reader.ReadByte();
        reader.ReadByte(); //00

        //TODO ???
        //unknown
        reader.ReadBytes(8);

        return body;
    }

    private TicketBody Read21Body(BinaryReader reader)
    {
        var body = new TicketBody();
        var bodyHeader = ReadSectionHeader(reader);

        //serial_id
        var serialHeader = ReadValueHeader(reader);
        body.SerialId = reader.ReadBytes(serialHeader.Length);

        //issuer_id
        var issuerHeader = ReadValueHeader(reader);
        body.IssuerId = reader.ReadUInt32Be();

        //issued_date
        var issuedDateHeader = ReadValueHeader(reader);
        body.IssuedDate = reader.ReadUInt64Be();

        //expire_date
        var expireDateHeader = ReadValueHeader(reader);
        body.ExpireDate = reader.ReadUInt64Be();

        //user_id
        var userIdHeader = ReadValueHeader(reader);
        body.UserId = reader.ReadUInt64Be();

        //online_id
        var usernameHeader = ReadValueHeader(reader);
        //TODO: confirm utf8
        body.Username = Encoding.ASCII.GetString(reader.ReadBytes(usernameHeader.Length)).TrimEnd('\0');

        //region/lang
        var regionHeader = ReadValueHeader(reader);
        body.Region = Encoding.ASCII.GetString(reader.ReadBytes(3)).TrimEnd('\0');
        body.Language = reader.ReadByte();

        //domain
        var domainHeader = ReadValueHeader(reader);
        body.Domain = Encoding.ASCII.GetString(reader.ReadBytes(domainHeader.Length)).TrimEnd('\0');

        //service_id
        var serviceHeader = ReadValueHeader(reader);
        body.ServiceId = reader.ReadBytes(serviceHeader.Length);

        //age/status
        var statusHeader = ReadValueHeader(reader);
        body.Age = reader.ReadByte();
        reader.ReadByte(); //00
        body.Status = reader.ReadByte();
        reader.ReadByte(); //00

        //TODO: handle these correctly they are just for rpcn for now
        ReadValueHeader(reader); //status_duration
        ReadValueHeader(reader); //dob

        return body;
    }

    private TicketBody Read30Body(BinaryReader reader)
    {
        var body = new TicketBody();
        var bodyHeader = ReadSectionHeader(reader);

        //serial_id
        var serialHeader = ReadValueHeader(reader);
        body.SerialId = reader.ReadBytes(serialHeader.Length);

        //issuer_id
        var issuerHeader = ReadValueHeader(reader);
        body.IssuerId = reader.ReadUInt32Be();

        //issued_date
        var issuedDateHeader = ReadValueHeader(reader);
        body.IssuedDate = reader.ReadUInt64Be();

        //expire_date
        var expireDateHeader = ReadValueHeader(reader);
        body.ExpireDate = reader.ReadUInt64Be();

        //user_id
        var userIdHeader = ReadValueHeader(reader);
        body.UserId = reader.ReadUInt64Be();

        //online_id
        var usernameHeader = ReadValueHeader(reader);
        //TODO: confirm utf8
        body.Username = Encoding.ASCII.GetString(reader.ReadBytes(usernameHeader.Length)).TrimEnd('\0');

        //region/lang
        var regionHeader = ReadValueHeader(reader);
        body.Region = Encoding.ASCII.GetString(reader.ReadBytes(3)).TrimEnd('\0');
        body.Language = reader.ReadByte();

        //domain
        var domainHeader = ReadValueHeader(reader);
        body.Domain = Encoding.ASCII.GetString(reader.ReadBytes(domainHeader.Length)).TrimEnd('\0');

        //service_id
        var serviceHeader = ReadValueHeader(reader);
        body.ServiceId = reader.ReadBytes(serviceHeader.Length);

        //date_of_birth
        var dobHeader = ReadSectionHeader(reader);
        var dob = new DateOfBirth();
        dob.Year = reader.ReadUInt16Be();
        dob.Month = reader.ReadByte();
        dob.Day = reader.ReadByte();
        body.DateOfBirth = dob;

        //age/status
        var statusHeader = ReadValueHeader(reader);
        body.Age = reader.ReadByte();
        reader.ReadByte(); //00
        body.Status = reader.ReadByte();
        reader.ReadByte(); //00

        //unknown
        var unknownHeader = ReadSectionHeader(reader);
        ReadValueHeader(reader);

        return body;
    }

    private TicketBody Read40Body(BinaryReader reader)
    {
        var body = new TicketBody();
        var bodyHeader = ReadSectionHeader(reader);

        //serial_id
        var serialHeader = ReadValueHeader(reader);
        body.SerialId = reader.ReadBytes(serialHeader.Length);

        //issuer_id
        var issuerHeader = ReadValueHeader(reader);
        body.IssuerId = reader.ReadUInt32Be();

        //issued_date
        var issuedDateHeader = ReadValueHeader(reader);
        body.IssuedDate = reader.ReadUInt64Be();

        //expire_date
        var expireDateHeader = ReadValueHeader(reader);
        body.ExpireDate = reader.ReadUInt64Be();

        //user_id
        var userIdHeader = ReadValueHeader(reader);
        body.UserId = reader.ReadUInt64Be();

        //online_id
        var usernameHeader = ReadValueHeader(reader);
        //TODO: confirm utf8
        body.Username = Encoding.ASCII.GetString(reader.ReadBytes(usernameHeader.Length)).TrimEnd('\0');

        //region/lang
        var regionHeader = ReadValueHeader(reader);
        body.Region = Encoding.ASCII.GetString(reader.ReadBytes(3)).TrimEnd('\0');
        body.Language = reader.ReadByte();

        //domain
        var domainHeader = ReadValueHeader(reader);
        body.Domain = Encoding.ASCII.GetString(reader.ReadBytes(domainHeader.Length)).TrimEnd('\0');

        //service_id
        var serviceHeader = ReadValueHeader(reader);
        body.ServiceId = reader.ReadBytes(serviceHeader.Length);

        //date_of_birth
        var dobHeader = ReadSectionHeader(reader);
        var dob = new DateOfBirth();
        dob.Year = reader.ReadUInt16Be();
        dob.Month = reader.ReadByte();
        dob.Day = reader.ReadByte();
        body.DateOfBirth = dob;

        //age/status
        var statusHeader = ReadValueHeader(reader);
        body.Age = reader.ReadByte();
        reader.ReadByte(); //00
        body.Status = reader.ReadByte();
        reader.ReadByte(); //00

        //unknown
        var unknownHeader = ReadSectionHeader(reader);
        ReadValueHeader(reader);

        //unknown
        reader.ReadBytes(4);

        //unknown String (8)
        var unknownStringOne = ReadValueHeader(reader);
        body.UnknownStringOne = Encoding.ASCII.GetString(reader.ReadBytes(unknownStringOne.Length)).TrimEnd('\0');

        //unknown String (64)
        var unknownStringTwo = ReadValueHeader(reader);
        body.UnknownStringTwo = Encoding.ASCII.GetString(reader.ReadBytes(unknownStringTwo.Length)).TrimEnd('\0');

        return body;
    }

    private TicketFooter? ReadFooter(BinaryReader reader)
    {
        var footer = new TicketFooter();
        var section = reader.ReadByte(); //30
        var sectionType= reader.ReadByte(); // 02
        if (section != 0x30 || sectionType != 0x02)
        {
            Logger.Debug("Invalid Ticket Footer");
            return null;
        }
        var footerLength = reader.ReadUInt16Be();
        //cipher_id
        var cipherHeader = ReadValueHeader(reader);
        footer.CipherId = reader.ReadBytes(cipherHeader.Length);

        //signature
        var signatureHeader = ReadValueHeader(reader);
        footer.Signature = reader.ReadBytes(signatureHeader.Length);

        return footer;
    }

    private (byte SectionType, ushort Length) ReadSectionHeader(BinaryReader reader)
    {
        var section = reader.ReadByte(); //0x30
        //TODO: add section check
        var type = reader.ReadByte(); //0x0, 0x2, 0x10, 0x11
        var length = reader.ReadUInt16Be();
        return (type, length);
    }

    private (TicketDataType DataType, ushort Length) ReadValueHeader(BinaryReader reader)
    {
        var dataType = (TicketDataType) reader.ReadUInt16Be();
        var length = reader.ReadUInt16Be();
        return (dataType, length);
    }

    private enum TicketDataType : ushort
    {
        Empty = 0x0,
        Uint32 = 0x1,
        Uint64 = 0x2,
        String = 0x4,
        Timestamp = 0x7,
        Binary = 0x8
    }

}