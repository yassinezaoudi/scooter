FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FreeScooters/FreeScooters.csproj", "FreeScooters/"]
RUN dotnet restore "FreeScooters/FreeScooters.csproj"
COPY . .
WORKDIR "/src/FreeScooters"
RUN dotnet build "FreeScooters.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FreeScooters.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreeScooters.dll"]
