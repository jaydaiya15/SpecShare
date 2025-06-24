# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Glitch uses port 3000 for HTTP
ENV ASPNETCORE_URLS=http://+:3000

COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "SpecShare.dll"]
