FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SecureNotes.Blazor/SecureNotes.Blazor.csproj", "SecureNotes.Blazor/"]
RUN dotnet restore "SecureNotes.Blazor/SecureNotes.Blazor.csproj"
COPY . .
WORKDIR "/src/SecureNotes.Blazor"
RUN dotnet publish "SecureNotes.Blazor.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
COPY ssl/mycert.crt /etc/nginx/ssl/mycert.crt
COPY ssl/mykey.key /etc/nginx/ssl/mykey.key

WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY SecureNotes.Blazor/nginx.conf /etc/nginx/nginx.conf
