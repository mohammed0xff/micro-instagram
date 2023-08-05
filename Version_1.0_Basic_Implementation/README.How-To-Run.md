# How to run this version

1. open `.sln` file 
2. set up solution property startup project to run Munltiple projects
3. have a rabbitMQ server running
To get a RabbitMQ server up and running without Docker, you can follow these steps:
- Download and install the latest version of RabbitMQ Server from the official website: [here](https://www.rabbitmq.com/download.html)
- After installation, open the RabbitMQ Command Prompt from the start menu. This will provide access to the RabbitMQ command-line tools.
- Run the following command to start the RabbitMQ server:
```
rabbitmq-server start
```
- Once the server is running, you can access the RabbitMQ management console by opening a web browser and navigating to the following `http://localhost:15672`
- You will need to log in with the default username and password, which are "guest" and "guest" respectively. 
- After logging in, you can create queues, exchanges and bindings, and configure other settings using the management console.

Note that RabbitMQ requires Erlang to operate, so make sure you have installed Erlang on your system before installing RabbitMQ. 
You can download the latest version of Erlang from the official website: [here](https://www.erlang.org/downloads)

OR 

if you have docker in root folder run the command `docker-compose up -d` and you will have a RabbitMQ up and running.

4. start projects by pressing `f5` or clicking start in Visual Studio.

got any problem with that? please raise any issues you have and we will do our best to help you.

# How to test this version

to access user service : https://localhost:7078/swagger/index.html
to access notification service : https://localhost:7077/swagger/index.html

1. login with username `user1` and password `string`
2. add the token provided in the response body to swagger authorizations
3. try get `/api/Users` to get all users in the application
4. choose unfollowed one and follow for example user4

5. now user4 is followed by you and must have recieved a notification
6. to verify that user4 has received a notification. login with username : `user4` and password `string`
7. go to https://localhost:7077/swagger/index.html
8. add the token to to swagger authorizations - in notification service -
9. send a GET request to `/api/Notifictions/recent`
10. a new notification added to the indicating that user1 now is following user4 (the one we just logged in as).



