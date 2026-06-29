FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "ProductService/ProductService.Infrastructure/"]
RUN dotnet restore "ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj"
COPY . .
WORKDIR "/src/ProductService/ProductService.Infrastructure"
RUN dotnet build "./ProductService.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductService.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Infrastructure.dll"]
