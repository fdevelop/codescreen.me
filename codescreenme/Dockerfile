#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

# Further on dotnet project
WORKDIR /src
COPY ["codescreenme.csproj", ""]
RUN dotnet restore "./codescreenme.csproj"

# Restore NPM dependencies
COPY ["ClientApp/package.json", "ClientApp/"]
COPY ["ClientApp/package-lock.json", "ClientApp/"]
WORKDIR /src/ClientApp

# Install Nodejs
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl --silent --location https://deb.nodesource.com/setup_12.x | bash - && apt-get install -yq nodejs build-essential

RUN npm install

WORKDIR /src
COPY . .
WORKDIR /src
RUN dotnet build "codescreenme.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "codescreenme.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "codescreenme.dll"]