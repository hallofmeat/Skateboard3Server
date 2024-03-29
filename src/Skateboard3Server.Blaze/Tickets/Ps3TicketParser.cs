﻿using System;
using System.IO;
using System.Text;
using NLog;
using Skateboard3Server.Common;

namespace Skateboard3Server.Blaze.Tickets;

//https://psdevwiki.com/ps3/X-I-5-Ticket
//https://github.com/RipleyTom/rpcn/blob/master/src/server/client/ticket.rs
//https://www.psdevwiki.com/ps3/PSN
public interface IPs3TicketParser
{
    Ps3Ticket? ParseTicket(byte[] ticketData);
}

public class Ps3TicketParser : IPs3TicketParser
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Ps3Ticket? ParseTicket(byte[] ticketData)
    {
        try
        {
            var ticket = new Ps3Ticket();
            using var reader = new BinaryReader(new MemoryStream(ticketData));

            //Ticket Header
            var majorVersion = (byte) (reader.ReadByte() >> 4);
            var minorVersion = reader.ReadByte();
            reader.ReadBytes(4); //00 00 00 00
            var ticketLength = reader.ReadUInt16Be();

            ticket.Header = new TicketHeader
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
            };

            if (ticketLength != (ticketData.Length - 8)) //subtract 8 because the header is 8 bytes long
            {
                Logger.Debug($"Ticket length did not match data");
                return null;
            }

            var bodyOffset = (int)reader.BaseStream.Position;
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
                Logger.Warn($"Unsupported ticket version {majorVersion}.{minorVersion}");
                return null;
            }

            //TODO: what if there is data in-between the body and the footer (right now we are reading it as part of body reading)

            //From body header to end of body
            ticket.RawBody = ticketData.AsSpan().Slice(bodyOffset, ticket.Body.Length+4).ToArray(); //4 bytes for body header

            var footerOffset = (int)reader.BaseStream.Position;
            ticket.Footer = ReadFooter(reader); //footer
            if (ticket.Footer != null)
            {
                //From start of ticket to start of signature data
                var signatureOffset = footerOffset + 0x10; //30 02 00 44 (ticket_footer) 00 08 00 04 (key_id_header) 38 2D E5 8D (key_id) 00 08 00 38 (signature_header)
                ticket.RawTicket = ticketData[..signatureOffset];
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
        body.Length = bodyHeader.Length;

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
        //TODO: confirm encoding
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
        body.Length = bodyHeader.Length;

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
        body.Length = bodyHeader.Length;

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
        //TODO: confirm encoding
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
        body.Length = bodyHeader.Length;

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
        //TODO: confirm encoding
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
        reader.ReadBytes(unknownStringOne.Length);

        //unknown String (64)
        var unknownStringTwo = ReadValueHeader(reader);
        reader.ReadBytes(unknownStringTwo.Length);

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
        //key_id
        var keyIdHeader = ReadValueHeader(reader);
        footer.KeyId = reader.ReadBytes(keyIdHeader.Length);

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