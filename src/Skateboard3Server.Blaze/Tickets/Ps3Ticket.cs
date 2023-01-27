#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Tickets;

public class Ps3Ticket
{
    public TicketHeader Header { get; set; }

    public TicketBody Body { get; set; }
        
    public TicketFooter? Footer { get; set; }

    //Raw sections used for signature verification
    public byte[] RawBody { get; set; } //From start of the body header to end of the body (used for RPCN signature verification)
    public byte[]? RawTicket { get; set; } //From the start of the ticket to the start of the signature data (used for PSN signature verification)
}

public class TicketHeader
{
    public byte MajorVersion { get; set; }
    public byte MinorVersion { get; set; }
}

public class TicketBody
{
    public ushort Length { get; set; }

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
}

public class TicketFooter
{
    public byte[] KeyId { get; set; } 

    public byte[] Signature { get; set; } //EC public key
}

public class DateOfBirth
{
    public ushort Year { get; set; }
    public byte Month { get; set; }
    public byte Day { get; set; }
}