# Login, Existing Account

This is the first flow that occurs against the application specific blaze server, for Skate 3 this is hosted at eadpgs-blapp001 port: 10744. This flow happens after you have already logged into EA. That first login associates your PS3 userId to your EA account. This means you can remove your save and not be prompted to re login to EA.

```mermaid
sequenceDiagram
    Client->>Server: PreAuthRequest (Util, 0x7)
    Server-->>Client: PreAuthResponse (Util, 0x7)
    Client->>Server: LoginRequest (Authentication, 0xC8)
    Server-->>Client: LoginResponse (Authentication, 0xC8)
    Note over Client, Server: Continues if PS3 ticket is linked to an existing EA account
    Server-->Client: UserAddedNotification (UserSession, 0x2)
    Server-->Client: UserExtendedDataNotification (UserSession, 0x1)
    Client->>Server: PostAuthRequest (Util, 0x8)
    Server-->>Client: PostAuthResponse (Util, 0x8)
    Client->>Server: SessionDataRequest (Authentication, 0xE6)
    Server-->>Client: SessionDataResponse (Authentication, 0xE6)
    Client->>Server: HardwareFlagsRequest (UserSession, 0x8)
    Server-->>Client: HardwareFlagsResponse (UserSession, 0x8)
    Client->>Server: FriendsListRequest (Social, 0x6)
    Server-->>Client: FriendsListResponse (Social, 0x6)
    Client->>Server: NetworkInfoRequest (UserSession, 0x14)
    Server-->>Client: NetworkInfoResponse (UserSession, 0x14)
    Client->>Server: SkateStatsRequest (Stats, 0x2)
    Server-->>Client: SkateStatsResponse (Stats, 0x2)
    loop for each DLC
    	Client->>Server: DlcRequest (Authentication, 0x20)
    	Server-->>Client: DlcResponse (Authentication, 0x20)
    end
    %%TODO Unknown0B, 0xA8C
    %%TODO Unknown0B, 0x640
```