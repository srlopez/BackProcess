# BackProcess

Plantilla ejemplo de un proceso `dotnet` consola que:  
- Ejecuta una tarea (en un hilo) periódicamente
    // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?view=net-6.0

- Obtiene de una variable de entorno SEGUNDOS en intervalo que tarda en volver a ejecutar la tarea (Por defecto 5 segundos)

```bash
export SEGUNDOS=1 # set SEGUNDOS=1
dotnet run 
Aplicación thread ID: 1
Tarea 1 thread ID: 4
Tarea 2 thread ID: 4
Tarea 3 thread ID: 6
Tarea 5 thread ID: 6
```

Contenerizamos la [imagen](https://docs.microsoft.com/es-es/dotnet/core/docker/build-container?tabs=windows) con el siguiente `Dockerfile` :

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:5.0
# dotnet publish -c Release
COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENV SEGUNDOS 3
ENTRYPOINT ["dotnet", "BackProcess.dll"]
```

Los pasos serían:
```bash
dotnet publish -c Release
docker build -t backprocess .
```
Pero debemos realizar el proceso manual de la compilación y publicación de la aplicación que nos deja los ejecutables en `bin/Release/net5.0/publish/`, y no me gusta mucho.

Este paso lo podemos mejorar publicando la aplicación en el mismo Dockerfile con el SDK, y al finalizar creando la imagen con el [RUNTIME](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-5.0):

```Dockerfile
# Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /App --no-restore

# Final
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /App
COPY --from=build /App ./
ENV SEGUNDOS 3
ENTRYPOINT ["dotnet", "BackProcess.dll"]
```

Ejecutaríamos el mismo `docker build -t backprocess .`  
Este proceso nos deja una imagen `<none>` que la podemos eliminar con 
`docker rmi $(docker images --filter "dangling=true" -q --no-trunc)`
Para finalizar ponemos la aplicación en una composición que no realiza todos los pasos de creación de imagen, lanzar el servicio y dejarlo lista por si lo necesita otro servicio.

```yml
version: '3.1'
services:
  backprocess:
    build:
        context: .
        dockerfile: Dockerfile
    restart: always
    environment:
      SEGUNDOS: 3
```

y lo lanzamos con `docker-compose up`