FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview

WORKDIR /app

COPY publish/ .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MoopelFrontend.dll"]