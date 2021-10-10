FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./  ./

RUN dotnet publish "Lab1_6.DeliverySvc/Lab1_6.DeliverySvc.csproj" --output /app/ --configuration Release

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Lab1_6.DeliverySvc.dll"]