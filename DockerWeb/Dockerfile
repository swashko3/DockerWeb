#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5003
ENV ASPNETCORE_URLS=http://*:5003
HEALTHCHECK CMD curl --fail http://localhost:5003/health || exit

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY DockerWeb/DockerWeb.csproj DockerWeb/
RUN dotnet restore "DockerWeb/DockerWeb.csproj"
COPY . .
WORKDIR "/src/DockerWeb"
RUN dotnet build "DockerWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerWeb.dll"]
