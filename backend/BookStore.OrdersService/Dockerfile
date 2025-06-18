FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BookStore.OrdersService/BookStore.OrdersService.csproj", "BookStore.OrdersService/"]
COPY ["./BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.OrdersService/BookStore.OrdersService.csproj"
COPY ./BookStore.OrdersService/ ./BookStore.OrdersService/
COPY ./BookStore.Shared/ ./BookStore.Shared/
WORKDIR "/src/BookStore.OrdersService"
RUN dotnet build "BookStore.OrdersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.OrdersService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.OrdersService.dll"]
