FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && \
    chown -R $APP_UID:$APP_UID /home/app/.aspnet/DataProtection-Keys

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApps/Shopping.Web/Shopping.Web.csproj", "WebApps/Shopping.Web/"]
RUN dotnet restore "./WebApps/Shopping.Web/Shopping.Web.csproj"
COPY . .
WORKDIR "/src/WebApps/Shopping.Web"
RUN dotnet build "./Shopping.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Shopping.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.Web.dll"]