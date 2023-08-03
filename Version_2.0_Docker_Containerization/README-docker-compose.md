# What is docker compose ?

Docker Compose is a tool that allows you to define and run multi-container Docker applications. 
It uses a YAML file to specify the services, networks, and volumes required for your application, 
and then automatically creates and configures all the necessary containers. With Docker Compose, 
you can easily manage and scale your application as a single unit. 

It simplifies the process of running complex applications that consist of multiple interconnected containers 
by providing a declarative and structured way to define the infrastructure and dependencies, 
and, with a single command, you create and start all the services from your configuration.

Just by dropping 

```cmd
    docker-compose up
```

in the command line where the `docker-compose.yml` file is located
docker compose will spin everything and your whole application will be up and running in seconds.

And from the docker docs : 

Compose works in all environments: production, staging, development, testing, as well as CI workflows. 
It also has commands for managing the whole lifecycle of your application:

    * Start, stop, and rebuild services
    * View the status of running services
    * Stream the log output of running services
    * Run a one-off command on a service

The key features of Compose that make it effective are:

    * Have multiple isolated environments on a single host
    * Preserves volume data when containers are created
    * Only recreate containers that have changed
    * Supports variables and moving a composition between environments

Note : Docker Compose exists by default if you do have docker installed on your machine 
you dont have to install it.


# How to use it in our application 

First off we are going to make a `docker-compose.yml` file in which we are going to tell docker compose : 
* What services we want to run. 
* Which depends on which.
* Network and port mapping of each of them. 
* Other specification like environment variables and stuff.

So, what services we have? <br/>
Currently we have just two `UserService` and `NotificationService`
Do they depend on anything ? <br/>
Of course, before they can be run we need and SqlServer and rabbitMQ server running 

So, this is the docker file we created, im going to add comments in-code for a better explaination. 
If you want to have a clear look at it without comments ![here](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_2.0_Docker_Containerization/docker-compose.yml)

```docker-compose
version: "3.9" # the version of the docker compose we've writen this file for.


# here we are defining a network called "app-network" with the bridge driver.
# What's a bridge driver? The bridge driver is the default network driver in Docker Compose. 
# It creates an isolated network for the containers and gives each container its own IP address within that network.

# The containers will be connected to the network and will be able to communicate with other containers 
# in the same network using their container names as the hostname like : `https://rabbitmq:5672`

networks:
  app-network:
    driver: bridge

# and here we start listing our servies 
services:

  # RabbitMQ
  rabbitmq:
    image: rabbitmq:3.9-management-alpine # rabbimq image on Docker Hub, which is going to be pulled 
                                          # and used to create our rabbitmq container.
    ports: # mapping default rabbit mq ports 5672, 15672 from inside to outside docker.
      - "5672:5672"
      - "15672:15672"
    environment: # setting up environment variables 
      RABBITMQ_DEFAULT_USER: "guest"
      RABBITMQ_DEFAULT_PASS: "guest"
    networks: # here we are specifing the network that is going to be used by rabbitmq
      - app-network

  # SQL Server
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2017-latest 
    container_name: sqlserver
    hostname: mssql
    cap_add: [ 'SYS_PTRACE' ]
    restart: always
    ports:
      - 1433:1433
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=hP75#q3q
    networks:
      - app-network

  # Services  

  user_service:
    image: user_service:local # our image name that is going to be built if doesnt exist already.
    container_name: user_service    
    build: # very important : we need to specify the where the docker file is and the build context
           # which we talked about in the dockerfile example
           # the build context is just like: docker asks itself 
           # `where am i right now?` when it starts building your images using dockerfile.
           # remember this line from Dockerfile 
           # COPY ["./src/UserService/UserService.csproj", "./UserService/"]
           # that line is expected to be run from the parent folder of `src`
           # so we specify our build context as `.` (a period) the current directory (parent folder of `src`).
           # if you found this confusing please let me know!
        context: .
        dockerfile: ./src/UserService/Dockerfile
    ports:
      - "8000:8000"
    depends_on: # here we are specifing the dependencies which docker comppose are going to use to configure 
                # the order in which services are going to start.
      - rabbitmq
      - sqlserver
    networks:
      - app-network

  notification_service:
    image: notification_service:local
    container_name: notification_service
    build: 
        context: .
        dockerfile: ./src/NotificationService/Dockerfile
    ports:
      - "7000:7000"
    depends_on:
      - rabbitmq
      - sqlserver
      - user_service
    networks:
      - app-network
```


### And we might have another file called `docker-compose.override.yml` 

The purpose of that file is to override or extend the configurations specified in the original 
docker-compose.yml file. This allows you to make additional changes to the services defined in 
the original file without modifying the original file itself. It is useful when you want 
to customize certain aspects of the configurations for different environments, such as development, 
staging, or production.

and when running docker-compose commands, include the -f flag to specify the files you want to use. 
For example, to start the containers using the base configuration with the development override:

```shell
    docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

Here, we are currently setting some environment variables for development. 
However, it is highly likely that we will have additional files and configurations 
specifically for production later on.

```docker-compose
version: '3.9'

services:

  notification_service:
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - ASPNETCORE_URLS=http://+:7000

  user_service:
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - ASPNETCORE_URLS=http://+:8000
```

## Notes on the environment 

We are setting the environment to `Docker` so we need to edit our `Program.cs`
file or our application wont use swagger <br/> 
we added `|| app.Environment.IsEnvironment("Docker")` to the if condition.

```cs
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger()
        .UseSwaggerUI(options =>
        {
            // ...
        });
    app.UseSwaggerUI();
}

```

One more thing : <br/>
our application `appsettings` in this case both of our services are going to be configured from 
* `appsettings.json` normally
* and `appsettings.Docker.json`
* NOT `appsettings.Development.json` unless you set the environemt to `Development`

And this is super cool! We can have a configuration file for each of our environments.