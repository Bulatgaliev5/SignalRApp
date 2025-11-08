# Используем официальный образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код и собираем
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Создаем runtime-образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Открываем порт 80
EXPOSE 80

# Команда запуска
ENTRYPOINT ["dotnet", "SignalrNETMAUIClient.dll"]