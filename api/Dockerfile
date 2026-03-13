FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./api ./api
COPY ./Directory.Build.props ./
RUN dotnet restore ./api/KhanHomeFloralLine.Api/KhanHomeFloralLine.Api.csproj
RUN dotnet publish ./api/KhanHomeFloralLine.Api/KhanHomeFloralLine.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "KhanHomeFloralLine.Api.dll"]


