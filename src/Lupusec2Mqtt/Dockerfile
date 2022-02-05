FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Lupusec2Mqtt/Lupusec2Mqtt.csproj", "Lupusec2Mqtt/"]
RUN dotnet restore "Lupusec2Mqtt/Lupusec2Mqtt.csproj"
COPY . .
WORKDIR "/src/Lupusec2Mqtt"
RUN dotnet build "Lupusec2Mqtt.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lupusec2Mqtt.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lupusec2Mqtt.dll"]