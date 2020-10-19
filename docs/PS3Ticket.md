# PS3 Ticket

Also known as a `X-I-5-Ticket`

These are based off the information in https://psdevwiki.com/ps3/X-I-5-Ticket and https://psdevwiki.com/ps3/PSN

Games request a ticket via the `sceNpManagerRequestTicket` or `sceNpManagerRequestTicket2` calls, then the ps3 requests a ticket from the PSN.

//TODO: how PSN gets tickets

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
        00 00 00 64 75 A1 62 0C (user_id: 100)
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
`30 XX 00 YY`
XX is section type, YY is section size
`00` Body
`02` Footer
`10` Unknown
`11` Date of Birth

### Values

`00 XX 00 YY`
XX is value type, YY is value length
`00` empty
`01` uint32
`02` uint64
`04` string
`07` timestamp (milliseconds)
`08` binary

### Header

`31` Major Version: 3
`00` Minor Version: 0
`00 00 00 00 00` 
`F8` Ticket Length: 0xF8

Valid header versions are (2.0, 2.1, 3.0, 4.0)

### Body

* serial_id
  //TODO: figure out where this comes from
* issuer_id
  `0` Prod/QA (QA)
  `1-8` SP-INT (Developer)
  `100` NP (Retail)
* issued_date
  unix timestamp for issued time of ticket
* expire_date
  unix timestamp for expire time of ticket
* user_id
  Id for PSN
* online_id
  Username for PSN
* region/language
  `75 73 00` string value for region
  `01` language value (`0x0` ja, `0x1` en, `0x2` fr, `0x3` es, `0x4` de, `0x5` it, `0x6` no, `0x7` pt, `0x8` ru, `0x9` ko, `0x10` zh)
* domain
  //TODO: figure out what domain means
* service_id
  string of game identifier (UP0006-BLUS30464_00)
* date_of_birth
  DoB is its own section
  `07 BD` Year: 1981
  `02` Month: 02
  `1D` Day: 29
* age/status
  `27` Age: 39
  `00`
  `02` Status?
  `00`
* unknown
  //TODO: figure out what unknown section means

### Footer

* cipher_id
  This is an Id for looking up the corresponding cipher/public key on the game server side (types of ciphers are HMAC, RSA, EC)

* signature
  This is used for input for the cipher, this ticket was using a RSA cipher. The the body section of the ticket is SHA1 hashed and fed into RSA_verify with this signature and the public key on the game server. The signature in this ticket is a RSAPublicKey in the DER format. 

  ```
  $ openssl rsa -text -RSAPublicKey_in -in ticket_sig.der -inform der
  RSA Public-Key: (192 bit)
  Modulus:
      00:cc:99:5f:2d:33:4a:4c:2b:ad:bb:07:1d:21:df:
      a1:05:bf:12:dc:f1:58:07:e8:41
  Exponent:
      00:d3:a3:aa:c3:4a:20:23:d9:04:2a:7b:f2:e3:5a:
      de:2f:e5:b5:41:ba:48:13:37:41
  writing RSA key
  -----BEGIN PUBLIC KEY-----
  MEowDQYJKoZIhvcNAQEBBQADOQAwNgIZAMyZXy0zSkwrrbsHHSHfoQW/EtzxWAfo
  QQIZANOjqsNKICPZBCp78uNa3i/ltUG6SBM3QQ==
  -----END PUBLIC KEY-----
  ```

  

