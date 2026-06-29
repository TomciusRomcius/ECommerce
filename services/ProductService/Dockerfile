FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /app

RUN dotnet tool install --global dotnet-ef --version 9.0.11
ENV PATH="$PATH:/root/.dotnet/tools"
COPY ["ProductService.Presentation/ProductService.Presentation.csproj", "ProductService.Presentation/ProductService.Presentation.csproj"]
COPY ["ProductService.Infrastructure/ProductService.Infrastructure.csproj", "ProductService.Infrastructure/ProductService.Infrastructure.csproj"]
COPY ["ProductService.Application/ProductService.Application.csproj", "ProductService.Application/ProductService.Application.csproj"]
COPY ["ProductService.Domain/ProductService.Domain.csproj", "ProductService.Domain/ProductService.Domain.csproj"]
COPY ["ProductService.sln", "ProductService.sln"]

RUN dotnet restore "./ProductService.Presentation/ProductService.Presentation.csproj"
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o ../app-build/build

FROM build as migrate
WORKDIR /app
# TODO: add back --no-build flag after solving deps.json does not exist error.
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "ProductService.Application"]


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductService.Presentation" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Presentation.dll"]
