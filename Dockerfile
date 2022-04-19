ARG BuildVersion

FROM mcr.microsoft.com/dotnet/runtime:6.0-focal AS base
WORKDIR /app


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" nomoretrollsuser && chown -R nomoretrollsuser /app
USER nomoretrollsuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
ARG BuildVersion
WORKDIR /src

COPY ["src/nomoretrolls/nomoretrolls.csproj", "src/nomoretrolls/"]
RUN dotnet restore "src/nomoretrolls/nomoretrolls.csproj"
COPY . .
WORKDIR "/src/src/nomoretrolls"
RUN dotnet tool restore
RUN dotnet paket restore
RUN dotnet build "nomoretrolls.csproj" -c Release -o /app/build /p:AssemblyInformationalVersion=${BuildVersion} /p:AssemblyFileVersion=${BuildVersion}

FROM build AS publish
ARG BuildVersion
RUN dotnet publish "nomoretrolls.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:AssemblyInformationalVersion=${BuildVersion} /p:AssemblyFileVersion=${BuildVersion} /p:Version=${BuildVersion}

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nomoretrolls.dll", "start"]