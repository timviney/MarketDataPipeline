FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV LANG=en_GB.UTF-8 \
    LC_ALL=en_GB.UTF-8
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

COPY src ./src
RUN dotnet restore "./src/MarketReplay.Api/MarketReplay.Api.csproj"
RUN dotnet build "./src/MarketReplay.Api/MarketReplay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/MarketReplay.Api/MarketReplay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --from=publish /app/publish .
COPY data ./data
ENTRYPOINT ["dotnet", "MarketReplay.Api.dll"]