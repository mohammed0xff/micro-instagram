# 🔥 How to run 

We should be able to run this application same as the previous version.

### To start the application : 
* head to root directory `Version_2.1_Docker_Containerization`
* run command `docker-compose up -d` <br/>

### To stop the application :
* run command `docker-compose down` <br/>

## ⚠️ BUT FIRST ⚠️

We need to do a very important step, which is setting up our https certificate <br/>
Or your services will give this error or a similar one about certificate passowrd and shut down :
`Unhandled exception. System.IO.DirectoryNotFoundException: Could not find a part of the path '/app/"/https/cert.pfx"` <br/>

1. Make sure you have docker installed and running.

3. Create and install an https certificate:

```
dotnet dev-certs https -ep "$HOME/.aspnet/https/cert.pfx" -p cert-password --trust 
```
This will Generate a self-signed certificate to enable HTTPS use in development. more on this see [docs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs) <br/>

3. Now evrything is fine 😃 and we can 
run command `docker-compose up -d` to start the application. <br/> 

# 🧪 Testing (now with HTTPS!)
* User service on ![https://localhost:8000/swagger/index.html](https://localhost:8000/swagger/index.html) <br/>
* Notification service on ![https://localhost:7000/swagger/index.html](https://localhost:7000/swagger/index.html) <br/>


