FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app 

# Restore
COPY *.sln .
COPY nuget.config .
COPY src/Skateboard3Server.Host/*.csproj ./src/Skateboard3Server.Host/
COPY src/Skateboard3Server.Blaze/*.csproj ./src/Skateboard3Server.Blaze/
COPY src/Skateboard3Server.Web/*.csproj ./src/Skateboard3Server.Web/
COPY src/Skateboard3Server.Common/*.csproj ./src/Skateboard3Server.Common/
COPY src/Skateboard3Server.Data/*.csproj ./src/Skateboard3Server.Data/ 
COPY tools/Skateboard3Server.BlazeProxy/*.csproj ./tools/Skateboard3Server.BlazeProxy/ 
COPY tests/Skateboard3Server.Blaze.Tests/*.csproj ./tests/Skateboard3Server.Blaze.Tests/ 
COPY tests/Skateboard3Server.Common.Tests/*.csproj ./tests/Skateboard3Server.Common.Tests/ 
RUN dotnet restore 

# Build/Publish
COPY src/Skateboard3Server.Host/. ./src/Skateboard3Server.Host/
COPY src/Skateboard3Server.Blaze/. ./src/Skateboard3Server.Blaze/
COPY src/Skateboard3Server.Web/. ./src/Skateboard3Server.Web/
COPY src/Skateboard3Server.Common/. ./src/Skateboard3Server.Common/
COPY src/Skateboard3Server.Data/. ./src/Skateboard3Server.Data/ 
COPY tools/Skateboard3Server.BlazeProxy/. ./tools/Skateboard3Server.BlazeProxy/ 
COPY tests/Skateboard3Server.Blaze.Tests/. ./tests/Skateboard3Server.Blaze.Tests/ 
COPY tests/Skateboard3Server.Common.Tests/. ./tests/Skateboard3Server.Common.Tests/ 
WORKDIR /app/src/Skateboard3Server.Host
#TODO: --no-restore
RUN dotnet publish -c Release -o /app/publish

#TODO: unit tests

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS runtime
WORKDIR /app 

# gosredirector
EXPOSE 42100
# blaze-app
EXPOSE 10744
# web (normally would be 80 but we cant use 80 in rootless containers)
EXPOSE 8080
# reset base image aspnetcore_http_ports to remove warning
ENV ASPNETCORE_HTTP_PORTS ""
# override appsettings to force 8080
ENV KESTREL__ENDPOINTS__WEB__URL http://*:8080

COPY --from=build /app/publish ./

USER $APP_UID
ENTRYPOINT ["dotnet", "Skateboard3Server.Host.dll"]