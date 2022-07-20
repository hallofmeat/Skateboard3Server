# Skateboard3Server

Custom Server compatible with Skate 3
 
https://hallofmeat.net

## Done

* Authentication

## In-Progress

* Matchmaking
* Web/Social

## To-Do

* SkateParks
* SkateReel
* Teams

<br>
<br>
<br>

### Running on Linux
Linux doesn't allow accessing low port numbers by default with unprivileged users.\
You can allow access to these ports for the binary\
by running `sudo setcap cap_net_bind_service=+eip /path/to/binary`.\
Example: `sudo setcap cap_net_bind_service=+eip bin/Debug/net6.0/Skateboard3Server.Host`\
Then you should be able to run the binary without elevated privileges.

If don't want to allow access to low port numbers,\
you can alternatively change the port 80 to 8080 from `src/Skateboard3Server.Host/Program.cs`\
and then redirect traffic from port 80 to 8080 by running this command:\
`sudo iptables -t nat -A OUTPUT -o lo -p tcp --dport 80 -j REDIRECT --to-port 8080`
This is useful if you want to debug the program without elevated privileges.