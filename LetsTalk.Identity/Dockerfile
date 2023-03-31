#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

#WORKDIR /
#RUN mkdir -p /https-root
#COPY ["Certificates/root-cert.cer", "/https-root/root-cert.cer"]
#COPY ["LetsTalk.Identity/docker-entrypoint.sh", "/docker-entrypoint.sh"]
#RUN chmod +x docker-entrypoint.sh
#RUN ./docker-entrypoint.sh

WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LetsTalk.Identity/LetsTalk.Identity.csproj", "LetsTalk.Identity/"]
RUN dotnet restore "LetsTalk.Identity/LetsTalk.Identity.csproj"
COPY . .
WORKDIR "/src/LetsTalk.Identity"
RUN dotnet build "LetsTalk.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsTalk.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetsTalk.Identity.dll"]