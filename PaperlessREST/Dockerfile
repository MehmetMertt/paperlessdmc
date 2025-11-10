# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY PaperlessREST/PaperlessREST.API.csproj PaperlessREST/
COPY PaperlessREST.Application/PaperlessREST.Application.csproj PaperlessREST.Application/
COPY PaperlessREST.Domain/PaperlessREST.Domain.csproj PaperlessREST.Domain/
COPY PaperlessREST.Business/PaperlessREST.Business.csproj PaperlessREST.Business/
COPY PaperlessREST.DataAccess/PaperlessREST.DataAccess.csproj PaperlessREST.DataAccess/
COPY PaperlessREST.Infrastructure/PaperlessREST.Infrastructure.csproj PaperlessREST.Infrastructure/

# Restore dependencies
RUN dotnet restore "PaperlessREST/PaperlessREST.API.csproj"

# copy rest of the code but not the OCR-Worker (otherwise mutiple publish output files with the same relative path error)
COPY PaperlessREST/ PaperlessREST/
COPY PaperlessREST.Application/ PaperlessREST.Application/
COPY PaperlessREST.Domain/ PaperlessREST.Domain/
COPY PaperlessREST.Business/ PaperlessREST.Business/
COPY PaperlessREST.DataAccess/ PaperlessREST.DataAccess/
COPY PaperlessREST.Infrastructure/ PaperlessREST.Infrastructure/

WORKDIR /src/PaperlessREST
RUN dotnet publish "PaperlessREST.API.csproj" -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaperlessREST.API.dll"]