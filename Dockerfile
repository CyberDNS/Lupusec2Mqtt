# Build on the native host arch, not the target arch: the SDK itself
# (restore/build/JIT) segfaults under QEMU emulation for arm/v7. This is
# safe because the app is published framework-dependent/portable (IL, not
# an arch-specific executable) — only the final runtime image below needs
# to match the target platform.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
