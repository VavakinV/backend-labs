FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

COPY WebApi/WebApi.csproj WebApi/
COPY Common/Common.csproj Common/
COPY Models/Models.csproj Models/
COPY Migrations/Migrations.csproj Migrations/
COPY Messages/Messages.csproj Messages/
RUN dotnet restore "WebApi/WebApi.csproj"

COPY . .

RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]