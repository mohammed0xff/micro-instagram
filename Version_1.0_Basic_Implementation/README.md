# Intorduction

This is the basic implementation of our applciation as we planned it in the main README.md file 
where we have just two services comunicating with each other using RabbitMQ as the message broker.
### first service  : `UserService` 
This service is responsible for :
* Authenticatoin - might move authentication to a seperate service later (upcomming versoins), 
if you think it's better choice to leave it as is for now, please let me know. -
* User creation and follow relationship management.

### Second service : `NotificationService` 
This service is responsible for:  
* User's notifications management, its responsible for storing notifications as well as 
-in the futrue- sending notifications to users in realtime.

# [How to run](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_1.0_Basic_Implementation/README.How-To-Run.md) 
