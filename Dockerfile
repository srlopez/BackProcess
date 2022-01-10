# Con procesos manuales de compilación o build
# FROM mcr.microsoft.com/dotnet/aspnet:5.0
# # dotnet publish -c Release
# COPY bin/Release/net5.0/publish/ App/
# WORKDIR /App
# ENV SEGUNDOS 3
# ENTRYPOINT ["dotnet", "BackProcess.dll"]


# Sin procesos manuales de compilación
# https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-5.0
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /App --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /App
COPY --from=build /App ./
ENV SEGUNDOS 3
ENTRYPOINT ["dotnet", "BackProcess.dll"]