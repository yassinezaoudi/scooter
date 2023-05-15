FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Scooter-Controller/Scooter-Controller.csproj", "Scooter-Controller/"]
RUN dotnet restore "Scooter-Controller/Scooter-Controller.csproj"
COPY . .
WORKDIR "/src/Scooter-Controller"
RUN dotnet build "Scooter-Controller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scooter-Controller.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scooter-Controller.dll"]
