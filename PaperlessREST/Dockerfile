# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (mit korrekter Großschreibung)
COPY PaperlessREST/PaperlessREST.API.csproj PaperlessREST/
COPY PaperlessREST.Application/PaperlessREST.Application.csproj PaperlessREST.Application/
COPY PaperlessREST.Domain/PaperlessREST.Domain.csproj PaperlessREST.Domain/
COPY PaperlessREST.Infrastructure/PaperlessREST.Infrastructure.csproj PaperlessREST.Infrastructure/
COPY PaperlessREST.DataAccess/PaperlessREST.DataAccess.csproj PaperlessREST.DataAccess/
COPY PaperlessREST.Business/PaperlessREST.Business.csproj PaperlessREST.Business/

# Restore dependencies
RUN dotnet restore "PaperlessREST/PaperlessREST.API.csproj"

# Copy the rest of the source code
COPY . .

# Publish the API project
WORKDIR "/src/PaperlessREST"
RUN dotnet publish "PaperlessREST.API.csproj" -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaperlessREST.API.dll"]