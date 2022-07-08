FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY ./BuildNumberGenerator/bin/Release/net6.0/* /BuildNumberGenerator/
WORKDIR ./
ENTRYPOINT ./BuildNumberGenerator
