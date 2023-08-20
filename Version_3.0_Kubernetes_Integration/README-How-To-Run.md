# 🔥 How to run

- Make sure you have docker engine running with kubernetes enabled 
to enable kubernates on docker desktop :
1. 
2. 
3. 

- Make sure you have the two local docker images from the previous version `user_service` and `notification_service`. 

If not :<br/>
you can generate them with docker compose run `docker-compose up` this will create images 
as well as container we dont need docker containers ourselves rn as containers are going to be 
managed by k8s from now on. This command allows you to spin up both images with one command instead 
of doing each separately with docker build command.


## 1. Generate SSL Certificate 

```shell
kubectl create secret generic aspnet-https --from-file="$HOME/.aspnet/https/cert.pfx" --from-literal=cert-password=cert-password
```

## 2. Apply deployment foreach of our services 

Each service is placed in a separate folder. To apply all the files in the folder, use the following command:

```shell
kubectl apply -f ./
```

Apply them in the following order:
1. sql-server 
2. rmq
3. user-service
3. notification-service


## 3. View Created Pods 

```shell
kubectl get pods 
```


## 🧪 Testing 

Access user service at: 
[https://localhost:38001/swagger/index.html](https://localhost:38001/swagger/index.html) <br/>

Access notification service at 
[https://localhost:37001/swagger/index.html](https://localhost:37001/swagger/index.html)






