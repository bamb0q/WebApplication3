FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["WebApplication3.csproj", "./"]
RUN dotnet restore "WebApplication3.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "WebApplication3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication3.dll"]
