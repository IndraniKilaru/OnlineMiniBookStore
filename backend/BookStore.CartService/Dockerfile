FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BookStore.CartService/BookStore.CartService.csproj", "BookStore.CartService/"]
COPY ["./BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.CartService/BookStore.CartService.csproj"
COPY ./BookStore.CartService/ ./BookStore.CartService/
COPY ./BookStore.Shared/ ./BookStore.Shared/
WORKDIR "/src/BookStore.CartService"
RUN dotnet build "BookStore.CartService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.CartService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.CartService.dll"]
