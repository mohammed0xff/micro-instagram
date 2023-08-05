In this verison we're going to containterize our application with docker 

## ![How tor run this version](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_2.0_Docker_Containerization/README-How-To-Run.md)

# Introduction 

In this version we are going to : 
* containerize our application with docker. 
* configure docker compose to define the services and with a single command, 
we can spin everything up or tear it all down (as they stated it in the docs!).


# Why Docker?

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


# How to Docker

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

Sometimes you might need write docker files yourself
However, its easier and faster to make VisualStudio generate them for you by: 
* right clicking on your project
* select Add 
* select Docker Support..

Lets take user service for example : 

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

```
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
```
first we are going to set the base image for our container. --> the Microsoft ASP.NET Core runtime version 7.0 using the `FROM` instruction. 

For example, `FROM ubuntu:20.04` specifies that your new container will be based on the Ubuntu 20.04 image.
And in this case our application is going to be built and run with `Microsoft ASP.NET Core runtime` 

When you build your Docker image using the Dockerfile, Docker will first pull the specified base image from a registry (such as Docker Hub) if it doesn't already exist locally. then, it will apply the subsequent instructions in the Dockerfile to customize and configure the container. 

The `AS base` part is an optional alias or stage name given to this particular base image. It allows you to refer to this stage later in the Dockerfile when performing multi-stage builds or separating different parts of the build process.


```
WORKDIR /app: 
```
setting the working directory << inside the container >> where subsequent commands will be executed.

```
EXPOSE 8000
```

Informs Docker that the container will listen on port `8000` and is ready to accept incoming network traffic on that port.

We can later map this port to another one << outside the container >> on the host.
for example to access our application we can go to `http://localhost:5000` if we mapped port 8000:5000 while running the container we can specify the port mapping argument with ` -p 8000:5000 ` ( will add more explaination on this point later )

```
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
```
Specifies another base image to create a build stage. It uses the Microsoft .NET Core SDK version 7.0, 
which includes all the tools necessary to build the application.

```
WORKDIR /src  
```
Sets the working directory to "/src" inside the container where the source code will be copied.

```
COPY ["./src/UserService/UserService.csproj", "./UserService/"]
````
Copies the "UserService.csproj" file from the local machine's "./src/UserService" directory to the container's "./UserService" directory.

```
COPY ["./src/Shared/Shared.csproj", "./Shared/"]
```
Copies the "Shared.csproj" file from the local machine's "./src/Shared" directory to the container's "./Shared" directory.
Remember we need to copy everything that our application needs to build and run successfully.
you might've guessed the next one, yes! packages.

```
RUN dotnet restore "./UserService/UserService.csproj"
```
This line restores the dependencies of the "UserService.csproj" project by running the "dotnet restore" command. 
This command fetches the required NuGet packages specified in the project file.

```
COPY . .
```
Copies the entire local directory to the current working directory inside the container. 
This includes the source code, project files, and any other necessary files.

```
WORKDIR "src/UserService"
```
Changes the working directory to "src/UserService" inside the container 
to execute further commands related to the `UserService` project.

```
RUN dotnet build "UserService.csproj" -c Release -o /app/build
```
Next we are going to builds the "UserService" project in Release configuration, placing the output files in the `/app/build` directory inside the container.

```
FROM build AS publish
```
Specifies a new stage named `publish` based on the previous `build` stage.

```
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false
```
Publishes the `UserService` project, generating the output files in the `/app/publish` directory inside the container. The `-p:UseAppHost=false` flag is used to disable generating an additional application host executable.

```
FROM base AS final
```
Specifies yet another stage named `final` based on the `base` stage.

```
WORKDIR /app
```
Changes the working directory to `/app` inside the container.

```
COPY --from=publish /app/publish .
```
Copies the output files from the `publish` stage (specifically, the `/app/publish` directory) to the current working directory inside the `final` stage.

```
ENTRYPOINT ["dotnet", "UserService.dll"]
```
Sets the default command to execute when starting a container from the resulting image. 
It instructs Docker to run the `UserService.dll` file using the `dotnet` command.

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## How to build and run our service with docker?

First we need to build an image. 
while you are in `./micro-instagram/Version_2.0_Docker_Containerization` directory
```shell
 docker build -t user_service  --file ./src/UserService/Dockerfile .
```
This command will build an image based on the instructions in your Dockerfile

**This is very important** , Docker might give you all sort of error you might have trouble with while trying to sleep at night!

**:: BEWARE ::**
of the arguments :
* `-t` for naming the image
* `--file` for specifing the path docker file
* `.` (just a period) this one argument with which we are specifing the build context for the Docker image <br/>
  remember this line from our dockerfile `COPY ["./src/UserService/UserService.csproj", "./UserService/"]` ? <br/>
  we are expecting this to be build from parent directory of `/src` <br/>
  This trick initially confused me. if you made any mistakes with it, docker will sure give you real hard time figuring what is wrong! <br/>
  why the src directory though? cause that's where everything we need. shared project. and we are going to be building and running both services with docker-compose<br/>
  in the next sections.

Next, we run the docker container. after the build process completes successfully, you can run a container based on the newly created image. 
Execute the following command:

```shell
docker run -p 8000:8000 user_service
```

To check if a Docker container is up and running, you can use the following command:

```shell
docker ps
```

Beware that you are running it solely! and it needs two dependencies : 1. rabbitMQ 2.SqlServer up n running. 
and them connection configured. 

## Is that what we need ?

Absolutely, if only we had just one service. 
Are we going to build and run our services one by one? is that reliable on a server?
What if some services depend on others?
That's why docker compose exists.

# Docker Compose 

## Why?
Docker Compose is used to define and run multi-container Docker applications. It will allow us to easily manage and orchestrate multiple Docker containers 
that work together to form an application.

## How?

![here](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_2.0_Docker_Containerization/README-docker-compose.md)


## Is that all of it ? 

Of course not, sure i cant explain everything about docker and even if i can, this would be redundant. cause the explainations already exists.
What we are to do here is to put those technologies in work and provide a space to play with them in action not in theory.

I encourage you to dive more in docker and its commands how does it work there is a lot to it, and if anything not getting your way search it up or raise and issue. always be positive nothing can go wrong. Every error or mistake you make becomes an opportunity to gain new insights, a new way to learn and a quesion that will set you up for your next growth.

Be tuned and have fun 💜.


### Furhter reading:
![what's a container?](https://www.docker.com/resources/what-container/) <br/>
![what is docker how docker works?](https://mindmajix.com/what-is-docker-how-docker-works) <br/>
(adding more soon)

Next version will be Kubernetes!
probably, not sure yet. 😃



