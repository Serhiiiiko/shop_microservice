FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


RUN mkdir -p /app/data && \
    mkdir -p /home/app/.aspnet/DataProtection-Keys && \
    chmod 755 /app/data && \
    chmod 755 /home/app/.aspnet/DataProtection-Keys && \
    chown -R $APP_UID:$APP_UID /app && \
    chown -R $APP_UID:$APP_UID /home/app

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Identity/Identity.API/Identity.API.csproj", "Services/Identity/Identity.API/"]
RUN dotnet restore "./Services/Identity/Identity.API/Identity.API.csproj"
COPY . .
WORKDIR "/src/Services/Identity/Identity.API"
RUN dotnet build "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


RUN mkdir -p /app/data && chmod 755 /app/data

ENTRYPOINT ["dotnet", "Identity.API.dll"]