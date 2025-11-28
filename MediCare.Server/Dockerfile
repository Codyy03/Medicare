FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5187
EXPOSE 7014

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MediCare.Server.csproj"
RUN dotnet build "MediCare.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediCare.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediCare.Server.dll"]
