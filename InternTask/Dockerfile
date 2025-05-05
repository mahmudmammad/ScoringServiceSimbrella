FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InternTask.csproj", "./"]
RUN dotnet restore "InternTask.csproj"
COPY . .
RUN dotnet build "InternTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InternTask.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InternTask.dll"]