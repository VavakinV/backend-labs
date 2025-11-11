FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Consumer/Consumer.csproj Consumer/
COPY Common/Common.csproj Common/
COPY Models/Models.csproj Models/
COPY Messages/Messages.csproj Messages/
RUN dotnet restore Consumer/Consumer.csproj

COPY Consumer/ Consumer/
COPY Common/ Common/
COPY Models/ Models/
COPY Messages/ Messages/

RUN dotnet publish Consumer/Consumer.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Consumer.dll"]  