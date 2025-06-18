FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BookStore.BooksService/BookStore.BooksService.csproj", "BookStore.BooksService/"]
COPY ["./BookStore.Shared/BookStore.Shared.csproj", "BookStore.Shared/"]
RUN dotnet restore "BookStore.BooksService/BookStore.BooksService.csproj"
COPY ./BookStore.BooksService/ ./BookStore.BooksService/
COPY ./BookStore.Shared/ ./BookStore.Shared/
WORKDIR "/src/BookStore.BooksService"
RUN dotnet build "BookStore.BooksService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.BooksService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.BooksService.dll"]
