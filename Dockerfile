# Build
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build

WORKDIR /src

COPY . .

RUN dotnet restore MoopelFrontend.slnx

RUN dotnet publish MoopelFrontend/MoopelFrontend.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MoopelFrontend.dll"]