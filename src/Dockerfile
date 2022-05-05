FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build
WORKDIR /app

COPY *.sln ./
COPY UnityPerformancePortal/*.csproj ./UnityPerformancePortal/
COPY UnityPerformancePortal.Front/*.csproj ./UnityPerformancePortal.Front/
COPY UnityPerformancePortal.Server/*.csproj ./UnityPerformancePortal.Server/
COPY UnityPerformancePortal.Driver.BigQuery/*.csproj ./UnityPerformancePortal.Driver.BigQuery/
COPY UnityPerformancePortal.Driver.LiteDB/*.csproj ./UnityPerformancePortal.Driver.LiteDB/
COPY UnityPerformancePortal.Local/*.csproj ./UnityPerformancePortal.Local/
RUN dotnet restore

COPY . ./
WORKDIR /app

# Build
RUN dotnet publish UnityPerformancePortal.Server -c Release -o out/server
RUN dotnet publish UnityPerformancePortal.Front -c Release -o out/front

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14 AS runtime
WORKDIR /app
COPY --from=build /app/out/server ./
COPY --from=build /app/out/front/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "UnityPerformancePortal.Server.dll"]