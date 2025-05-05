# Use the official .NET 8 SDK image to build and publish the app
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

# Wait for it
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# change the ENTRYPOINT to use wait-for-it
ENTRYPOINT ["/wait-for-it.sh", "postgres:5433", "--", "dotnet", "Server.dll"]
