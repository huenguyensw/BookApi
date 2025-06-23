# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet build --configuration Release

# Publish the app
RUN dotnet publish -c Release -o out

# Use ASP.NET runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

WORKDIR /app

COPY --from=build /app/out .

# Expose port Render expects
EXPOSE 10000

# Set the environment URL (again, matches Program.cs)
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

# Run the application
ENTRYPOINT ["dotnet", "BookApi.dll"]
