FROM mcr.microsoft.com/dotnet/aspnet-alpine:6.0
COPY ./BuildNumberGenerator/bin/Release/net6.0/* /buildnumbergenerator/
WORKDIR ./buildnumbergenerator
ENTRYPOINT ./BuildNumberGenerator
