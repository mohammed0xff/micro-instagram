# How to use Kubernetes?
To use k8s we need to specify to it the desired state for our application to run at.

### an example on a "state"
Imaine we have two services :

1. Frontend Service:
   - The frontend service should be deployed with two replicas to ensure high availability.
   - It should use a specific container image tagged with a version number.
   - The service should expose port 80 and use a specific DNS name for external access.
   - It should have resource limits defined to ensure proper resource allocation.

2. Backend Service:
   - The backend service should also be deployed with two replicas.
   - It should use a different container image, possibly with a different version.
   - The service should expose port 8080 internally for communication with the frontend service.
   - It may require environment variables or configuration files to be mounted.

We can tell k8s exactly what state that we want our app in and it will do its best to maintain that state. 

## How to comunicate this to k8s?
A yaml file? you guessed it!
Yes, we are going to define our services, the desired state for each of them and networking to allow them to comunicate 
with each other and for us to be able to access them.

#### First off we need a deployment resource file
With a deployment to each service we can ensure that each service has its own dedicated pod
By having each service in its own pod, you can ensure that failures or issues in one service do not impact others. 
However this always depends on the requirement of the application and at certain senarios you might need to services to run at the same pod. 
But at the moment we want each service to have its own pod.

Here is an example of how to make a deployment resource for `user-service`
Specifing `kind` to be `Deployment`

```yaml
apiVersion: apps/v1

kind: Deployment
metadata:
  name: user-service
spec:
  replicas: 1
  selector:
    matchLabels:
      app: user-service
  template:
    metadata:
      labels:
        app: user-service
    spec:
      containers:
        - name: user-service
          image: user_service:local
          ports:
            - containerPort: 8000
            - containerPort: 8001
           env:
             - name: ASPNETCORE_ENVIRONMENT
               value: Production

```
I removed some details for example simplicity view [original](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_3.0_Kubernetes_Integration/k8s/user-service/user-service-deployment.yml)

And in the `spec` section we describes the desired state of the deployment.

- `replicas: 1`: 
Specifies that the desired number of replicas for the deployment is 1. ie. just one instance of the application.

- `selector`: 
Specifies the labels used to identify the pods controlled by this deployment. 
The Deployment will select and manage pods with this label, 
in a nother words any pod that will have this label is going to be managed by this resource deployment we are defining 

- `matchLabels`: 
Specifies the labels that the deployment uses to identify the pods. In this case, the label "app" with the value "user-service" is used.

- `template`: 
Defines the pod template used to create new pods controlled by this deployment.

Then we have another specs in our pod template
We specify that: 
* We are having one container.
* Giving it a name.
* The image it is baesd on `user_service:local` we already have it from our last version.
* Defining the ports that should be exposed by the container.

```
note that : the Depoloyment is designed to manage and control multiple replicas of only one "pod template". 
It ensures that the desired number of replicas are running and match the defined label selector. 
However, a Deployment does not manage multiple pods with different specifications.
```

Next step we are going to apply our file in k8s using this `kubectl` command

```shell
kubectl apply -f our-deployment-file.yml
```

### What have we made so far?

Can we access the application no?
Not yet!

Right after we apply the configuration file.
Kubernetes will start creating the necessary resources to fulfill the desired state defined in the file. 

This includes :
- Creating a Deployment resource named "user-service" in the cluster
- The Deployment will ensure that one replica of the application is running at all times. If there are no existing replicas, it will create a new one.
- The Deployment will use labels to identify and manage the pods associated with it. In this case, the label "app: user-service" will be applied to the pods.
- Kubernetes will create a Pod template based on the specified configuration.
- The Pod template will create a Pod with the specified container configuration.
- The container named "user-service" will be created using the specified container image, "user_service:local".

And now we have our container running on a pod at some node.
Very Cool!

But we still not cant access our continer why?

## Now we need a Service configuration, but what's a Service?

Pods in Kubernetes are designed to be ephemeral, meaning they can be created, destroyed, or moved around the cluster at any time. 
So, if we try to access a pod directly, we might end up connecting to a pod that no longer exists or has been replaced by a new one.

A Service acts as a load balancer and provides a single entry point to access multiple pods. 
It ensures that the traffic is evenly distributed among the available pods, even if they are created or scaled up or down. 
This way, even if a pod fails or is replaced, the Service will continue to route traffic to the healthy pods.

The Service uses the label selector defined in its configuration to identify the pods it should route traffic to. 
When you create a Service, you specify a set of labels, and the Service matches those labels with the labels assigned to the pods. 
Any pods with matching labels become part of the Service's endpoint pool.

Here's our service configuration for the `user-service`: 

```yaml
apiVersion: v1
kind: Service
metadata:
  name: user-service
spec:
  selector:
    app: user-service
  ports:
    - name: http
      protocol: TCP
      port: 8000
      targetPort: 8000

    - name: https
      protocol: TCP
      port: 8001
      targetPort: 8001
```

apply it with `kubectl apply -f our-user-service.yaml`


this will create a service named "user-service" with two ports defined: port 8000 and port 8001.
The Service will use a label `selector` to identify the pods that it should route traffic to. 

In this case, the pods must have the label "app: user-service" to be included in the Service's endpoint pool.
The ports section specifies the ports that the Service should listen on and forward traffic to. 
For example, port 8000 is defined with the name "http" and targetPort 8000. Similarly, port 8001 is defined with the name "https" and targetPort 8001.
By creating this Service configuration, we are instructing Kubernetes to create a stable endpoint for our application. 
Other resources within the cluster or external users can now access our application using the Service's name and the specified ports, 
such as "user-service" and port 8000 for HTTP traffic or port 8001 for HTTPS traffic.

The Service acts as a bridge, providing a stable and abstracted endpoint for our application. 
It enables external or internal users to access our application without worrying about the underlying pod dynamics. 
It simplifies networking and makes it easier to manage and scale our application.

Service configuration is a crucial component in making our application accessible and resilient within the Kubernetes cluster regardless of pod changes.

#### Wait a minuite, did you say "Within the Kubernetes cluster"? We still cant access our service through "localhost:8001" ??
You are right. we are one step from having that.

## We need to make our service accessable from the outside of the cluster
how to do that?
We have three options - there are more but we are going to examine three rn -

### 1. Ingress: 
Ingress is an API object that manages external access to the services within a cluster. 
It acts as a reverse proxy and provides features like SSL termination, path-based routing, and more. 
We would need to set up an Ingress controller (e.g. Nginx Ingress Controller) and configure an Ingress resource to route traffic from our local machine to the Service.
Additionally, we would need to set up DNS or add an entry to our local machine's hosts file to map the desired domain name to the Ingress IP.

### 2. NodePort: 
A NodePort type Service exposes the Service on a static port on each node in the cluster. 
we would need to create a NodePort Service and specify the desired port number. 
The Service will then forward traffic from that port to the Service's targetPort. 
We can access the Service using the Node's IP address and the specified NodePort from our local machine.

### 3. Port forwarding 
Port forwarding allows you to access a specific port on a pod or service running within the cluster from your local machine. 
It is commonly used for debugging, accessing a database, or interacting with an application running in the cluster.

To use port forwarding, you can use the `kubectl port-forward` command. Here's the syntax:
```shell
kubectl port-forward service/user-service 8001:80
```


We are using the second option at this point, we are going to setup ingress in the next versions when we add our api gateway 
But a NodePort now is very suitable. Let's keep things simple.

### How to make a node port to forward traffic from outside the cluster that port to the Service's targetPort?

Here is the configuration we used in this version:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: user-service-nodeport
spec:
  selector:
    app: user-service
  type: NodePort
  ports:
    - name: http
      port: 80
      targetPort: 8000
      nodePort: 38000
    - name: https
      port: 443
      targetPort: 8001
      nodePort: 38001
```

You know how to apply it.

let's explain some fields : 

- `port`: This is the port number that the service will listen on. In this example, 
the service will listen on port 80 for HTTP traffic and port 443 for HTTPS traffic.

- `targetPort`: This is the port number on the pods that the service will forward traffic to. In this example, 
HTTP traffic will be forwarded to port 8000 on the pods, and HTTPS traffic will be forwarded to port 8001.

- `nodePort`. specifies the port number on each node in the cluster that traffic will be forwarded to. 
In this example, for HTTP traffic, incoming requests on port 38000 of any node will be forwarded to the service, 
and for HTTPS traffic, requests on port `38001` will be forwarded.

Here is a diagram to help you visualize what's going on:

<p align="center">
  <img src="https://github.com/mohammed0xff/micro-instagram/blob/master/images/nodeport-mapping.png" />
</p>

And now we can access our service through `https://localhost:38001/swagger/index.html`
lets celeprate 🎉🎉

## Conclusion

In this file we expalined how to :   
- Create a deployment
- Set up a service configuration for it
- and finnaly map this service to to a port outside the node 

Note that our service is dependant on an sql server and rabbitmq server
I also created a deployment configurations for them as well as notification service 
To apply them and run the application you can head to [how to run guide](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_3.0_Kubernetes_Integration/README-How-To-Run.md)
The configurations for other services is quite similar, hope nothing will be confusing to you.

### One more thing
The appsettings configuration
How to setup our service to comunicate to each other ? inside the same pod?, same node? outside the cluster?
We are going to learn about that soon.
but now let's just make our application run

I added a `appsettings.Production.json` file :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver-service,1433;Integrated Security=true;Initial Catalog=UserService;"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq-service",
    "UserName": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```
As we set the environment to be prodution in deployment file :
```yaml
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: Production
```

The application now is going to select the `appsettings.Production.json` to configure the appsettings.


Remember In docker compose we just used the name of the service that we provided and used the same network with `driver: bridge` across containers.
Notice that now we are using `sqlserver-service` defined in sql-service [configuration file](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_3.0_Kubernetes_Integration/k8s/sql-server/sqlserver-service.yml)
As we are reaching the sqlserver ""service"" from inside the node.

Next version we are going to set up an api gatway that will act as an entry point for API calls and represent client requests to target services!

![](https://i.pinimg.com/originals/74/d3/f3/74d3f3e8f2a52fc9a05dee27d81e6702.jpg)



