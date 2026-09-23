# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ClubCordobaWallet.Domain/*.csproj src/ClubCordobaWallet.Domain/
COPY src/ClubCordobaWallet.Application/*.csproj src/ClubCordobaWallet.Application/
COPY src/ClubCordobaWallet.Infrastructure/*.csproj src/ClubCordobaWallet.Infrastructure/
COPY src/ClubCordobaWallet.Api/*.csproj src/ClubCordobaWallet.Api/
RUN dotnet restore src/ClubCordobaWallet.Api/ClubCordobaWallet.Api.csproj

COPY src/ src/
RUN dotnet publish src/ClubCordobaWallet.Api/ClubCordobaWallet.Api.csproj -c Release -o /app --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
# Npgsql negocia GSS encryption contra Postgres y necesita esta lib nativa;
# la imagen base de .NET no la trae, sin ella la conexión falla al arrancar
# (DllNotFoundException: libgssapi_krb5.so.2) antes de poder migrar la DB.
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ClubCordobaWallet.Api.dll"]
