FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
COPY ./BuildNumberGenerator/bin/Release/net6.0/* /buildnumbergenerator/
WORKDIR ./buildnumbergenerator
ENTRYPOINT ./BuildNumberGenerator
