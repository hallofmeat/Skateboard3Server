# Redirector, Server Info

This is the first request ever sent to the blaze server. This request returns details for the application specific blaze server, this is hosted on gosredirector port: 42100

```mermaid
sequenceDiagram
    Client->>Server: ServerInfoRequest (Redirector, 0x1)
    Server-->>Client: ServerInfoResponse (Redirector, 0x1)
```