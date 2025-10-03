# Build stage - Use .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln .

# Copy project file for restore
COPY CrudServices/CrudServices.csproj CrudServices/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY CrudServices/ CrudServices/

# Build the project
WORKDIR /src/CrudServices
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - Use lightweight .NET 9 runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose port
EXPOSE 5002

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Development

# Entry point
ENTRYPOINT ["dotnet", "CrudServices.dll"]