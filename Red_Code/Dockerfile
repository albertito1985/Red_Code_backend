# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Red_Code.csproj ./
RUN dotnet restore "Red_Code.csproj"

# Copy the remaining source and build
COPY . ./
RUN dotnet publish "Red_Code.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a non-root user for security best practices on Hetzner
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published output
COPY --from=build /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port 8080 (Hetzner will handle reverse proxy / port mapping)
EXPOSE 8080

# ASP.NET Core listens on the PORT environment variable or defaults to 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "Red_Code.dll"]
