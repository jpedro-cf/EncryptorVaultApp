FROM mcr.microsoft.com/dotnet/sdk:8.0  AS build

WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 5190
WORKDIR /app
COPY --from=build /app/publish .

COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENV ASPNETCORE_URLS=http://+:5190

CMD ["dotnet", "EncryptionApp.dll"]