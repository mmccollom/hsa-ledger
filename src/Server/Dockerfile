# Use the official .NET 8 SDK image to build and publish the app3
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything and restore
COPY . .

# Build the Server project
RUN dotnet restore "src/Server/Server.csproj"
RUN dotnet publish "src/Server/Server.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Dotnet command to run server
ENTRYPOINT ["dotnet", "Server.dll"]
