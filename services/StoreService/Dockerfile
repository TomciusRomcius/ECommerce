FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM docker:28-dind AS test-base
RUN apk add dotnet9-sdk
ENV DOTNET_ROOT=$HOME/.dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /app

RUN dotnet tool install --global dotnet-ef --version 9.0.11
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["StoreService.Presentation/StoreService.Presentation.csproj", "StoreService.Presentation/StoreService.Presentation.csproj"]
COPY ["StoreService.Infrastructure/StoreService.Infrastructure.csproj", "StoreService.Infrastructure/StoreService.Infrastructure.csproj"]
COPY ["StoreService.Application/StoreService.Application.csproj", "StoreService.Application/StoreService.Application.csproj"]
COPY ["StoreService.Domain/StoreService.Domain.csproj", "StoreService.Domain/StoreService.Domain.csproj"]

# COPY ["StoreService.Presentation.Tests/StoreService.Presentation.Tests.csproj", "StoreService.Presentation.Tests/StoreService.Presentation.Tests.csproj"]
# COPY ["StoreService.Infrastructure.Tests/StoreService.Infrastructure.Tests.csproj", "StoreService.Infrastructure.Tests/StoreService.Infrastructure.Tests.csproj"]
# COPY ["StoreService.Domain.Tests/StoreService.Domain.Tests.csproj", "StoreService.Domain.Tests/StoreService.Domain.Tests.csproj"]

COPY ["StoreService.sln", "StoreService.sln"]

RUN dotnet restore
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION

FROM test-base AS test
WORKDIR /app
COPY --from=build /app .
COPY --from=build /root/.nuget/packages /root/.nuget/packages 
CMD [ "dotnet", "test" ]

FROM build as migrate
WORKDIR /app
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build", "--project", "StoreService.Application"]

FROM build AS publish
WORKDIR /app
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StoreService.Presentation" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreService.Presentation.dll"]
