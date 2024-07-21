# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Order/Ordering.API/Ordering.API/Ordering.API.csproj", "Services/Order/Ordering.API/Ordering.API/"]
COPY ["Services/Order/Ordering.Application/Ordering.Application.csproj", "Services/Order/Ordering.Application/"]
COPY ["Services/Order/Ordering.Core/Ordering.Core.csproj", "Services/Order/Ordering.Core/"]
COPY ["Services/Order/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Services/Order/Ordering.Infrastructure/"]
#COPY ["Infrastructure/EventBus.Messages/EventBus.Messages.csproj", "Infrastructure/EventBus.Messages/"]
COPY ["Infrastructure/Common.Logging/Common.Logging.csproj", "Infrastructure/Common.Logging/"]

RUN dotnet restore "./Services/Order/Ordering.API/Ordering.API/Ordering.API.csproj"
COPY . .
WORKDIR "/src/Services/Order/Ordering.API/Ordering.API"
RUN dotnet build "./Ordering.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ordering.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]