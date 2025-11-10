FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only OcrWorker project and its dependencies
COPY PaperlessREST.OcrWorker/PaperlessREST.OcrWorker.csproj PaperlessREST.OcrWorker/
COPY PaperlessREST.Application/PaperlessREST.Application.csproj PaperlessREST.Application/
RUN dotnet restore "PaperlessREST.OcrWorker/PaperlessREST.OcrWorker.csproj"

# copy the rest of the code
COPY PaperlessREST.OcrWorker/ PaperlessREST.OcrWorker/
COPY PaperlessREST.Application/ PaperlessREST.Application/

WORKDIR /src/PaperlessREST.OcrWorker
RUN dotnet publish "PaperlessREST.OcrWorker.csproj" -c Release -o /app/publish

ENTRYPOINT ["dotnet", "PaperlessREST.OcrWorker.dll"]
