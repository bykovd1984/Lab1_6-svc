FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY ./  ./

RUN dotnet publish "IdentityServerAspNetIdentity.csproj" --output /app/ --configuration Release

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "IdentityServerAspNetIdentity.dll"]