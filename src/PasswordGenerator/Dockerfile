FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PasswordGenerator/PasswordGenerator.csproj", "PasswordGenerator/"]
RUN dotnet restore "PasswordGenerator/PasswordGenerator.csproj"
COPY . .
WORKDIR "/src/PasswordGenerator"
RUN dotnet build "PasswordGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordGenerator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordGenerator.dll"]
