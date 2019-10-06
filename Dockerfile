FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["Lupusec2Mqtt.csproj", "./"]
RUN dotnet restore "./Lupusec2Mqtt.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Lupusec2Mqtt.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lupusec2Mqtt.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lupusec2Mqtt.dll"]
