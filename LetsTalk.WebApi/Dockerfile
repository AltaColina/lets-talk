#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LetsTalk.WebApi/LetsTalk.WebApi.csproj", "LetsTalk.WebApi/"]
COPY ["LetsTalk.Shared/LetsTalk.Shared.csproj", "LetsTalk.Shared/"]
RUN dotnet restore "LetsTalk.WebApi/LetsTalk.WebApi.csproj"
COPY . .
WORKDIR "/src/LetsTalk.WebApi"
RUN dotnet build "LetsTalk.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsTalk.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetsTalk.WebApi.dll"]