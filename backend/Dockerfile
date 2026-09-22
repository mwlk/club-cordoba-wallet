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
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ClubCordobaWallet.Api.dll"]
