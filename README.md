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