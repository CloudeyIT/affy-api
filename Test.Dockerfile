FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Affy.Test/Affy.Test.csproj", "Affy.Test/"]
RUN dotnet restore "Affy.Test/Affy.Test.csproj"
COPY . .
WORKDIR "/src/Affy.Test"
RUN dotnet build "Affy.Test.csproj" -c Debug

# Install PowerShell
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y powershell

FROM build AS final
ENV SNAPSHOOTER_STRICT_MODE="true"
WORKDIR "/src/Affy.Test"
ENTRYPOINT ["dotnet", "test", "--no-build"]
