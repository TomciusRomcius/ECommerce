FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProductService/ProductService.Application/ProductService.Application.csproj", "ProductService/ProductService.Application/"]
RUN dotnet restore "ProductService/ProductService.Application/ProductService.Application.csproj" --no-cache
COPY . .
WORKDIR "/src/ProductService/ProductService.Application"
RUN dotnet build "./ProductService.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductService.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Application.dll"]
