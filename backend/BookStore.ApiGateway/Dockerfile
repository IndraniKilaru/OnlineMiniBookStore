FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookStore.ApiGateway/BookStore.ApiGateway.csproj", "BookStore.ApiGateway/"]
COPY ["BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.ApiGateway/BookStore.ApiGateway.csproj"
COPY . .
WORKDIR "/src/BookStore.ApiGateway"
RUN dotnet build "BookStore.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.ApiGateway.dll"]
