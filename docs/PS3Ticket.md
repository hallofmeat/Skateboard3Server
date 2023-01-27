# PS3 Ticket

Also known as a `X-I-5-Ticket`

These are based off the information in https://psdevwiki.com/ps3/X-I-5-Ticket and https://psdevwiki.com/ps3/PSN

Games request a ticket via the `sceNpManagerRequestTicket` or `sceNpManagerRequestTicket2` calls, then the ps3 requests a ticket from PSN.

## Ticket Format

This is based off the version 3.0 ticket but the version only affects the body format. Some of the values have changed for anonymity. This is an example of a PSN ticket for Skate 3:

```
31 00 00 00 00 00 00 F8 (header version: 3.0, length: 0xF8)
30 00 00 AC (section: body, length: 0xAC)
    00 08 00 14 (type: binary, length: 0x14)
        02 D6 5A 9A D8 1D 09 34 27 42 4B 9B 67 29 3E F2 97 55 BC 78 (serial_id)
    00 01 00 04 (type: uint32, length: 0x4)
        00 00 01 00 (issuer_id)
    00 07 00 08 (type: milliseconds, length: 0x8)
        00 00 01 74 DC 62 98 C4 (issued_date)
    00 07 00 08 (type: milliseconds, length: 0x8)
        00 00 01 74 DC 6B BF 78 (expire_date)
    00 02 00 08 (type: uint64, length: 0x8)
        00 00 00 00 00 01 E2 40 (user_id: 123456)
    00 04 00 20 (type: string, length: 0x20)
        74 65 73 74 75 73 65 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 (online_id: testuser)
    00 08 00 04 (type: binary, length: 0x4)
        75 73 00 01 (region: us, language: en)
    00 04 00 04 (type: string, length: 0x4) 
        62 36 00 00 (domain: b6)
    00 08 00 18 (type: binary, length 0x18)
        55 50 30 30 30 36 2D 42 4C 55 53 33 30 34 36 34 5F 30 30 00 00 00 00 00 (service_id)
    30 11 00 04 (section: date_of_birth)
        07 BD 02 1D (year: 1981, month: 02, day: 29)
    00 01 00 04 (type: uint32, length: 0x4)
        27 00 02 00 (age: 39, status: 0x2)
    30 10 00 00 (section: unknown)
        00 00 00 00
30 02 00 44 (section: footer, length: 0x44)
    00 08 00 04 (type: binary, length: 0x4)
        38 2D E5 8D (cipher_id)
    00 08 00 38 (type: binary, length: 0x38)
        30 36 02 19 00 CC 99 5F 2D 33 4A 4C 2B AD BB 07 1D 21 DF A1 05 BF 12 DC F1 58 07 E8 41 02 19 00 D3 A3 AA C3 4A 20 23 D9 04 2A 7B F2 E3 5A DE 2F E5 B5 41 BA 48 13 37 41 (signature)
```

## Format

### Sections
* `30 XX 00 YY`
* XX is section type, YY is section length
* `00` Body
* `02` Footer
* `10` Unknown
* `11` Date of Birth

### Values

* `00 XX 00 YY`
* XX is value type, YY is value length
* `00` empty
* `01` uint32
* `02` uint64
* `04` string
* `07` timestamp (milliseconds)
* `08` binary

## Ticket

### Header

* `31` Major Version: 3
* `00` Minor Version: 0
* `00 00 00 00 00` 
* `F8` Ticket Length: 0xF8

Valid header versions are (2.0, 2.1, 3.0, 4.0)

### Body

* serial_id
  * //TODO: figure out where this comes from
* issuer_id
  * `0` Prod/QA (QA)
  * `1-8` SP-INT (Developer)
  * `100` NP (Retail)
  * `3333` RPCN (RPCS3)
* issued_date
  * unix timestamp (in milliseconds) for issued time of ticket
* expire_date
  * unix timestamp (in milliseconds) for expire time of ticket
* user_id
  * Id for PSN
* online_id
  * Username for PSN
* region/language
  * `75 73 00` string value for region
  * `01` language value (`0x0` ja, `0x1` en, `0x2` fr, `0x3` es, `0x4` de, `0x5` it, `0x6` no, `0x7` pt, `0x8` ru, `0x9` ko, `0x10` zh)
* domain
  * //TODO: figure out what domain means
* service_id
  * string of game identifier (UP0006-BLUS30464_00)
* date_of_birth
  * DoB is its own section
  * `07 BD` Year: 1981
  * `02` Month: 02
  * `1D` Day: 29
* age/status
  * `27` Age: 39
  * `00`
  * `02` Status?
  * `00`
* unknown
  * //TODO: figure out what unknown section means

### Footer

* key_id
  * This is an id for the corresponding public key (and cipher_type?)
  
* signature
  * This is the signature for the ticket in ASN.1 format (most psn ticket are using a ECDSA cipher)

## Signature Verification

RPCN signature verification is the SHA224 of the data from the start of the body section (including section header) `30 00 00 <body_length>` to the end of the body section.

```
$ openssl dgst -sha224 -verify rpcn_ticket_public.pem -signature rpcn_ticket_signature.bin rpcn_ticket_body.bin
Verified OK
```

PSN signature verification is the SHA1 of the data from the very beginning of the ticket (including ticket header) `31 00 00 00 00 00 00 <ticket_length>` to the start of the signature data, after the `00 08 00 <signature_length>` information.

Note: There may be extra empty bytes after the end of the signature that may cause problems parsing the ASN.1 format signature.

## Obtaining Public Keys

Given an amount of signed tickets you can derive the ECDSA points using https://github.com/Slendy/PubKeyFinder

## Implementations

[https://github.com/LBPUnion/ProjectLighthouse/blob/4770beea393e6f23abe70ee718e8582b565f86a3/ProjectLighthouse/Tickets/NPTicket.cs](ProjectLighthouse)
[https://github.com/hallofmeat/Skateboard3Server/tree/d03b6f1467bc8b24150ae020f81af4280bc30ad6/src/Skateboard3Server.Blaze/Tickets](Skateboard3Server)