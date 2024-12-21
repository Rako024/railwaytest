# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["ExpoSite/ExpoSite.csproj", "ExpoSite/"]
COPY ["Expo.Business/Expo.Business.csproj", "Expo.Business/"]
COPY ["Expo.Data/Expo.Data.csproj", "Expo.Data/"]
COPY ["Expo.Core/Expo.Core.csproj", "Expo.Core/"]

RUN dotnet restore "ExpoSite/ExpoSite.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ExpoSite"
RUN dotnet build "ExpoSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpoSite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpoSite.dll"]
