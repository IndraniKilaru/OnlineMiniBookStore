FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookStore.NotificationService/BookStore.NotificationService.csproj", "BookStore.NotificationService/"]
COPY ["BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.NotificationService/BookStore.NotificationService.csproj"
COPY . .
WORKDIR "/src/BookStore.NotificationService"
RUN dotnet build "BookStore.NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.NotificationService.dll"]
