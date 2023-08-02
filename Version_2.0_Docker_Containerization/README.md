In this verison we're going to containterize our application with docker 

# Why ?

Imagine you have a computer with different applications installed, and each application requires different dependencies and configurations. 
Now, let's say you need to move these applications to a different computer or share them with your team. You might come across compatibility issues 
or spend a lot of time setting up the environment on every machine.

This is where Docker comes to the rescue! 
Docker is a tool that allows you to package your applications and their dependencies into a standardized, self-contained unit called a "container." 
These containers are lightweight and can run on any machine that has Docker installed, regardless of the operating system or underlying infrastructure.

Think of Docker containers as little virtual machines, but without the overhead of running a full operating system. 
They provide isolation, ensuring that your application runs in an environment that remains consistent across different machines. 
This consistency eliminates the need for modifying configurations or resolving dependency conflicts every time you move your application.

Docker uses a system called "containerization" to achieve this. It combines the application code, runtime environment, system libraries, and dependencies, 
all bundled into a single container. Containers are then isolated from each other, so even if one container misbehaves or crashes, 
it won't affect others running on the same machine.

Additionally, Docker simplifies the process of distributing and sharing applications. You can push your containers to a central repository called Docker Hub, 
allowing others to easily download and run your application on their machines, without worrying about compatibility issues.

In a nutshell, Docker helps in creating portable, reliable, and scalable environments for running applications. 
It streamlines the development and deployment process by ensuring consistent behavior across different systems.


# How 

First off we need to make a docker file for each of our services.

### Why do we need a `Dockerfile` ?

The purpose of a Dockerfile is to define the instructions needed to build a Docker image. It provides a way to automate and standardize the process of 
creating a container image, making it easier to reproduce and rebuild the same image across different environments. Dockerfiles specify the base image to use, 
the files and directories to add to the image, the commands to run during the build process, and any configurations or settings required. 
Dockerfiles are typically used in combination with Docker build commands to create Docker images that can be used to run containers.

### What are docker images and containers ?

Docker images are the building blocks of Docker containers. They are lightweight, standalone, and executable packages that contain everything needed to run 
a piece of software, including the code, runtime environment, libraries, and dependencies. Docker images are based on a base image and can be customized and 
layered with additional configurations and files to meet specific application requirements.

On the other hand, Docker containers are instances of Docker images. They encapsulate the application and its dependencies into an isolated environment, 
providing a consistent and reproducible runtime. Containers provide the ability to package applications and their dependencies together in a lightweight, 
portable, and isolated manner, ensuring that they can run consistently across different environments and systems. Containers can be easily created, started, 
stopped, moved, and removed, making them highly flexible and scalable.

### Making a `Dockerfile`

Sometimes you might need make docker yourself
However its easier and faster to make VisualStudio generate them for you by: right clicking on your project -> Add -> Docker Support..

Lets take userservice for example : 

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base 
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["./src/UserService/UserService.csproj", "./UserService/"]
COPY ["./src/Shared/Shared.csproj", "./Shared/"]
RUN dotnet restore "./UserService/UserService.csproj"
COPY . .
WORKDIR "src/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]
```

Gibberish right ?

Let's explain this, remember we are just setting instruction. so, what do we need to do?
yes, we want to build our project on a container.
but where is the container ? 
The container files exist within the Docker host's file system.
so, we need to move what we need from our project in src folder in our host machine to Docker container
and excute commands needed to build our project. simple as that.

### Let's expain the process


### How to build and run our service with docker?


### Is that what we need ?

Absolutely, if only we had just one service. 
Are we going to build and run our services one by one? is that reliable on a server?
What if some services depend on others?
That's why docker compose exists.

# Docker Compose 



## Why?
Docker Compose is used to define and run multi-container Docker applications. It will allow us to easily manage and orchestrate multiple Docker containers 
that work together to form an application.


## How?







