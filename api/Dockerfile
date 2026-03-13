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
EXPOSE 8080
CMD ["sh", "-c", "dotnet KhanHomeFloralLine.Api.dll --urls http://+:${PORT:-8080}"]


