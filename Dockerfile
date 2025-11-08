# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj из подкаталога
COPY SignalRApp/*.csproj ./SignalRApp/
WORKDIR /app/SignalRApp
RUN dotnet restore

# Копируем остальной код
COPY SignalRApp/. ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Открываем порт 80
EXPOSE 80

ENTRYPOINT ["dotnet", "SignalRApp.dll"]
