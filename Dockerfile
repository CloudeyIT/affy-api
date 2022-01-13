FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Affy.Api/Affy.Api.csproj", "Affy.Api/"]
RUN dotnet restore "Affy.Api/Affy.Api.csproj"
COPY [".config", "."]
RUN dotnet tool restore
COPY . .
WORKDIR "/src/Affy.Api"
RUN dotnet build "Affy.Api.csproj" -c Release

FROM build AS publish
RUN dotnet publish "Affy.Api.csproj" -c Release -o /app/publish
RUN dotnet ef migrations bundle --no-build -o /app/publish/migrations --configuration Release

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Affy.Api.dll"]
