FROM mcr.microsoft.com/dotnet/aspnet:5.0
# dotnet publish -c Release
COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENV SEGUNDOS 3
ENTRYPOINT ["dotnet", "BackProcess.dll"]