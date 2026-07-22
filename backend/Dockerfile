FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/SportsGurukul.Domain/SportsGurukul.Domain.csproj src/SportsGurukul.Domain/
COPY src/SportsGurukul.Application/SportsGurukul.Application.csproj src/SportsGurukul.Application/
COPY src/SportsGurukul.Infrastructure/SportsGurukul.Infrastructure.csproj src/SportsGurukul.Infrastructure/
COPY src/SportsGurukul.Api/SportsGurukul.Api.csproj src/SportsGurukul.Api/
RUN dotnet restore src/SportsGurukul.Api/SportsGurukul.Api.csproj

COPY src/ src/
RUN dotnet publish src/SportsGurukul.Api/SportsGurukul.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SportsGurukul.Api.dll"]
