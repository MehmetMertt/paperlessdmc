# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY PaperlessREST/PaperlessREST.API.csproj PaperlessREST/
COPY PaperlessRest.Application/PaperlessRest.Application.csproj PaperlessRest.Application/
COPY PaperlessREST.Domain/PaperlessREST.Domain.csproj PaperlessREST.Domain/
COPY PaperlessREST.Infrastructure/PaperlessREST.Infrastructure.csproj PaperlessREST.Infrastructure/

# Restore dependencies for API project
RUN dotnet restore "PaperlessREST/PaperlessREST.API.csproj"

# Copy the rest of the source code
COPY . .

# Publish the API project explicitly
WORKDIR "/src/PaperlessREST"
RUN dotnet publish "PaperlessREST.API.csproj" -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Run the API directly
ENTRYPOINT ["dotnet", "PaperlessREST.API.dll"]
