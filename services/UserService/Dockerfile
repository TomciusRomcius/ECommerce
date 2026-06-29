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

COPY ["UserService.Presentation/UserService.Presentation.csproj", "UserService.Presentation/UserService.Presentation.csproj"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/UserService.Infrastructure.csproj"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/UserService.Application.csproj"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/UserService.Domain.csproj"]

# COPY ["UserService.Presentation.Tests/UserService.Presentation.Tests.csproj", "UserService.Presentation.Tests/UserService.Presentation.Tests.csproj"]
# COPY ["UserService.Infrastructure.Tests/UserService.Infrastructure.Tests.csproj", "UserService.Infrastructure.Tests/UserService.Infrastructure.Tests.csproj"]
# COPY ["UserService.Domain.Tests/UserService.Domain.Tests.csproj", "UserService.Domain.Tests/UserService.Domain.Tests.csproj"]

COPY ["UserService.sln", "UserService.sln"]

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
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build", "--project", "UserService.Application"]

FROM build AS publish
WORKDIR /app
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.Presentation" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Presentation.dll"]
