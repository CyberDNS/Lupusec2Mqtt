FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG VERSION=0.0.0
WORKDIR /src
COPY src/Lupusec2Mqtt/Lupusec2Mqtt.csproj Lupusec2Mqtt/
RUN dotnet restore "Lupusec2Mqtt/Lupusec2Mqtt.csproj"
COPY src/Lupusec2Mqtt/ Lupusec2Mqtt/
RUN dotnet publish "Lupusec2Mqtt/Lupusec2Mqtt.csproj" -c Release -o /app/publish -p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lupusec2Mqtt.dll"]
