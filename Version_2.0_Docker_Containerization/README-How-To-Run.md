# How to run this version 

Make sure you have docker installed and docker engine running.
If you dont have docker installed on your machine search it up, its very easy to install.

### To run the application : 
* head to root directory `Version_2.0_Docker_Containerization`
* run command `docker-compose up -d` <br/>
	`-d` for detached (so we can run commands later in the same shell window).

### To stop the application 
* run command `docker-compose down` <br/>


you can access 
* User service on ![http://localhost:8000/swagger/index.html](http://localhost:8000/swagger/index.html) <br/>
* Notification service on ![http://localhost:7000/swagger/index.html](http://localhost:7000/swagger/index.html) <br/>

### http not https?
Yes, when using docker we have to setup https ouserlves and that is what we are going to do in the next version

