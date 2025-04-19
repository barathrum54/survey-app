FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY . .

RUN dotnet restore
RUN dotnet build --no-restore

EXPOSE 5000
ENTRYPOINT ["dotnet", "src/SurveyApp.API/bin/Debug/net6.0/SurveyApp.API.dll"]
