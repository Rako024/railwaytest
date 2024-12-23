# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and project files
COPY ["ExpoSite.sln", "./"]
COPY ["Expo.Business/Expo.Business.csproj", "Expo.Business/"]
COPY ["Expo.Data/Expo.Data.csproj", "Expo.Data/"]
COPY ["Expo.Core/Expo.Core.csproj", "Expo.Core/"]
COPY ["ExpoSite/ExpoSite.csproj", "ExpoSite/"]

# Restore dependencies
RUN dotnet restore "ExpoSite.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/ExpoSite"
RUN dotnet build "ExpoSite.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ExpoSite.csproj" -c Release -o /app/publish

# Runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpoSite.dll"]
