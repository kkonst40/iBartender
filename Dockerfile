FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . .

WORKDIR /app/src/iBartender.API
RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
EXPOSE 8080
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "iBartender.API.dll"]


#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#WORKDIR /src
#COPY ["src/iBartender.API/iBartender.API.csproj", "iBartender.API/"]
#COPY ["src/iBartender.Application/iBartender.Application.csproj", "iBartender.Application/"]
#COPY ["src/iBartender.Core/iBartender.Core.csproj", "iBartender.Core/"]
#COPY ["src/iBartender.Persistence/iBartender.Persistence.csproj", "iBartender.Persistence/"]
#RUN dotnet restore "iBartender.API/iBartender.API.csproj"
#COPY . ../
#WORKDIR /src/iBartender.API
#RUN dotnet build "iBartender.API.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish --no-restore -c Release -o /app/publish
#
#FROM mcr.microsoft.com/dotnet/aspnet:9.0
#EXPOSE 8080
#EXPOSE 8081
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "iBartender.API.dll"]