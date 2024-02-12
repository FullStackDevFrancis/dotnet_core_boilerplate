# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR "./"
EXPOSE 80
EXPOSE 443

# Restore all packages and copy to new folder
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR "./"
COPY ["dotnet_core_boilerplate.csproj", "dotnet_core_boilerplate/"]
RUN dotnet restore "dotnet_core_boilerplate/dotnet_core_boilerplate.csproj"

# Copy all remaining files to dotnet_core_boilerplate folder
COPY . "dotnet_core_boilerplate/"
RUN ls
RUN dotnet build "dotnet_core_boilerplate/dotnet_core_boilerplate.csproj" -c Release -o /app/build

# Create executable files DLL 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnet_core_boilerplate/dotnet_core_boilerplate.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet_core_boilerplate.dll"]
