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
COPY ["PaymentService.Presentation/PaymentService.Presentation.csproj", "PaymentService.Presentation/PaymentService.Presentation.csproj"]
COPY ["PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "PaymentService.Infrastructure/PaymentService.Infrastructure.csproj"]
COPY ["PaymentService.Application/PaymentService.Application.csproj", "PaymentService.Application/PaymentService.Application.csproj"]
COPY ["PaymentService.Domain/PaymentService.Domain.csproj", "PaymentService.Domain/PaymentService.Domain.csproj"]
COPY ["PaymentService.sln", "PaymentService.sln"]

RUN dotnet restore "./PaymentService.Presentation/PaymentService.Presentation.csproj"
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o ../app-build/build

FROM build as migrate
WORKDIR /app
# TODO: add back --no-build flag after solving deps.json does not exist error.
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "PaymentService.Application"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService.Presentation" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Presentation.dll"]
