FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore-env
WORKDIR /src

COPY ["./src/MSLearnGPT.API/MSLearnGPT.API.csproj", "./MSLearnGPT.API/"]

RUN dotnet restore "./MSLearnGPT.API/MSLearnGPT.API.csproj"

# Build stage
FROM restore-env AS build-env
WORKDIR /src

COPY --from=restore-env /src .
COPY ["./Directory.Packages.props", "./src/MSLearnGPT.API/*", "./MSLearnGPT.API/"]

RUN dotnet build \
        # TODO: No-restore is not working properly now, needs to investigate
        # --no-restore \  
        --configuration Release \
        --runtime linux-x64 \
        "./MSLearnGPT.API/MSLearnGPT.API.csproj" \
    && dotnet publish \
        --no-restore \
        --configuration Release \
        --runtime linux-x64 \
        --output ./app \
        "./MSLearnGPT.API/MSLearnGPT.API.csproj"

## Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env
WORKDIR /app
USER app

COPY --from=build-env /src/app .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "MSLearnGPT.API.dll"]
