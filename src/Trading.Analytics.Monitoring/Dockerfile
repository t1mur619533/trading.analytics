#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Trading.Analytics.Monitoring/Trading.Analytics.Monitoring.csproj", "src/Trading.Analytics.Monitoring/"]
COPY ["src/Trading.Analytics.Domain/Trading.Analytics.Domain.csproj", "src/Trading.Analytics.Domain/"]
COPY ["src/Trading.Analytics/Shared/Trading.Analytics.Shared.csproj", "src/Trading.Analytics/Shared/"]
RUN dotnet restore "src/Trading.Analytics.Monitoring/Trading.Analytics.Monitoring.csproj"
COPY . .
WORKDIR "/src/src/Trading.Analytics.Monitoring"
RUN dotnet build "Trading.Analytics.Monitoring.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trading.Analytics.Monitoring.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trading.Analytics.Monitoring.dll"]