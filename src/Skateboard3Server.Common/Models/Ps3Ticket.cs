#pragma warning disable CS8618

namespace Skateboard3Server.Common.Models;

public class Ps3Ticket
{
    public TicketHeader Header { get; set; }

    public TicketBody Body { get; set; }
        
    public TicketFooter? Footer { get; set; }
}

public class TicketHeader
{
    public byte MajorVersion { get; set; }
    public byte MinorVersion { get; set; }
}

public class TicketBody
{
    public byte[] SerialId { get; set; } //(ticket_id for rpcn, serial_id for PSN)

    public uint IssuerId { get; set; } //issuer_id

    public ulong IssuedDate { get; set; } //issued_date (milliseconds)

    public ulong ExpireDate { get; set; } //expire_date (milliseconds)

    public ulong UserId { get; set; } //user_id

    public string Username { get; set; } //online_id

    public string Region { get; set; } //region

    public byte Language { get; set; } //language

    public string Domain { get; set; } //domain

    public byte[] ServiceId { get; set; } //service_id

    public DateOfBirth DateOfBirth { get; set; } //date_of_birth

    public byte Age { get; set; } //age

    public byte Status { get; set; } //status

    public string UnknownStringOne { get; set; } //unknown_string_one

    public string UnknownStringTwo { get; set; } //unknown_string_two
}

public class TicketFooter
{
    public byte[] CipherId { get; set; } 

    public byte[] Signature { get; set; } //RSA public key, HMAC key, EC public key

}

public class DateOfBirth
{
    public ushort Year { get; set; }
    public byte Month { get; set; }
    public byte Day { get; set; }
}