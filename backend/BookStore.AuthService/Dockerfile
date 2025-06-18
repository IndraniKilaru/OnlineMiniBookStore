FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BookStore.AuthService/BookStore.AuthService.csproj", "BookStore.AuthService/"]
COPY ["./BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.AuthService/BookStore.AuthService.csproj"
COPY ./BookStore.AuthService/ ./BookStore.AuthService/
COPY ./BookStore.Shared/ ./BookStore.Shared/
WORKDIR "/src/BookStore.AuthService"
RUN dotnet build "BookStore.AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.AuthService.dll"]
