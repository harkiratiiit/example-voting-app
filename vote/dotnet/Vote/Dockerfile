FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /src
COPY ["Vote.csproj", "."]
RUN dotnet restore "Vote.csproj"
COPY . .
RUN dotnet build "Vote.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vote.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vote.dll"]