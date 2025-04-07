#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Smartplug.Api/Smartplug.Api.csproj", "Smartplug.Api/"]
COPY ["Smartplug.Application/Smartplug.Application.csproj", "Smartplug.Application/"]
COPY ["Smartplug.Persistence/Smartplug.Persistence.csproj", "Smartplug.Persistence/"]
COPY ["Smartplug.Domain/Smartplug.Domain.csproj", "Smartplug.Domain/"]
COPY ["Smartplug.Core/Smartplug.Core.csproj", "Smartplug.Core/"]
RUN dotnet restore "./Smartplug.Api/Smartplug.Api.csproj"
COPY . .
WORKDIR "/src/Smartplug.Api"
RUN dotnet build "./Smartplug.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Smartplug.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smartplug.Api.dll"]