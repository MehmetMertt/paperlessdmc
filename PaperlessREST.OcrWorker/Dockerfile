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

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app

# install ghostscript + tesseract + imagemagisck dependencies
RUN apt-get update && \
    apt-get install -y \
        ghostscript \
        tesseract-ocr \
        tesseract-ocr-deu \
        tesseract-ocr-eng \
        libleptonica-dev \
        libtesseract-dev \
        libmagickwand-dev && \
    rm -rf /var/lib/apt/lists/*

# create symlinks where .NET is searching
RUN mkdir -p /app/x64 && \
    ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so && \
    ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so && \
    ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so

# copy published output
COPY --from=build /app/publish .

# copy tessdata for custom language
COPY ./tessdata ./tessdata

ENV TESSDATA_PREFIX=/app
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PaperlessREST.OcrWorker.dll"]
