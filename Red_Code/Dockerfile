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

# Copy published output
COPY --from=build /app/publish .

# ASP.NET Core will listen on the PORT environment variable (used by platforms like Railway)
# You can still document a default port for local runs
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "Red_Code.dll"]
