FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY ssl/mykey.key /https/
COPY ssl/mycert.crt /https/

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SecureNotes.API/SecureNotes.API.csproj", "SecureNotes.API/"]
RUN dotnet restore "SecureNotes.API/SecureNotes.API.csproj"
COPY . .
WORKDIR "/src/SecureNotes.API"
RUN dotnet build "SecureNotes.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SecureNotes.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecureNotes.API.dll"]
