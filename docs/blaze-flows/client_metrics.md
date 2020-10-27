# Util, Client Metrics

This flow is used in combination with the QoS servers returned as part of the PreAuthResponse (Util, 0x7). This reports the ping/speed test/firewall results gained from the QoS servers.

```mermaid
sequenceDiagram
    Note over Client, Blaze Server: <start of login flow goes here>
    Blaze Server-->>Client: TCP: PreAuthResponse (Util, 0x7)
    Client->>QoS Server: HTTP: GET /qos/qos?vers=1&qtyp=1&prpt=10000
    QoS Server-->>Client: HTTP: Response
    loop UDP based speed test/ping test
        Client->>QoS Server: UDP Echo Request
    	QoS Server-->>Client: UDP Echo Response
    end
    Client->>QoS Server: HTTP: GET /qos/qos?vers=1&qtyp=2&prpt=10000
    QoS Server-->>Client: HTTP: Response
    loop UDP based speed test/ping test
        Client->>QoS Server: UDP Echo Request
    	QoS Server-->>Client: UDP Echo Response
    end
    Client->>QoS Server: HTTP: GET /qos/firewall?vers=1&nint=2
    QoS Server-->>Client: HTTP: /qos/firewall?vers=1&nint=2 Response
    Client->>QoS Server: HTTP: GET /qos/firetype?vers=1&rqid=214&rqsc=630&inip=<int ip>&inpt=10000
    QoS Server-->>Client: HTTP: Response
    Client->>Blaze Server: TCP: ClientMetricsRequest (Util, 0x16)
    Blaze Server-->>Client: TCP: ClientMetricsResponse (Util, 0x16)
    
```

