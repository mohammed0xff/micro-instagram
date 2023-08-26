# [How to run](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_3.0_Kubernetes_Integration/README-How-To-Run.md)

# Intorduction
 
At this point we can deploy our application, we already configured our services with docker compose. 
we can simply drop `docker-compose up` in the cli and all of our services should be up and running.
That's all we need right? Yes our application is running what else? 
At production time, there are other kinds of concerns that we may encounter!

In this version we are going to integrate kubernetes into our app and learn more about the lifetime of containers 
at prodcution time. And sure the app is not ready yet. We can't just have an app that allows users to follow each others without any content shared between them.
But that the beauty of microservices, we can deploy our applicatoin while it's still in development and publishing our services one by adding more featrues and functionality to our app.

## Why do we need Kubernetes?

If we run all the services on a single server, it can become difficult to manage and scale the application. 
We may also encounter issues such as service dependencies, service failure, and ensuring high availability in critical systems. 
To minimize these problems, we need a solution like Kubernetes.

Kubernetes allows us to separate the configuration and run each service on its own server. 
It provides features like automatic scaling, load balancing, self-healing, and service discovery. 
With Kubernetes, we can ensure that our application runs smoothly, even in complex and critical systems.

## What is Kubernetes?

Kubernetes is an open-source platform for orchestrating containers (typically Docker containers). 
It manages the lifecycle and networking of containers that are scheduled to run, 
so you don't have to worry if your app crashes or becomes unresponsive, 
Kubernetes will automatically tear it down and start a new container.

Kubernetes can provide : 

1. Scalability: Kubernetes enables easy scaling of applications by allowing you to add or remove containers based on demand. It automatically manages the distribution of workload across multiple containers, ensuring efficient resource utilization.

2. Fault tolerance: Kubernetes provides mechanisms like self-healing and replication to ensure high availability of applications. If a container fails, Kubernetes automatically replaces it with a new one, reducing downtime.

3. Load balancing: Kubernetes distributes incoming network traffic across multiple containers, ensuring that no single container is overwhelmed with requests. This helps in optimizing application performance and improves reliability.

4. Resource optimization: Kubernetes allows you to define resource requirements and limits for containers, ensuring efficient utilization of computing resources. It also provides tools for monitoring and managing resource usage, helping you optimize costs.

5. Deployment flexibility: Kubernetes supports various deployment strategies, such as rolling updates and canary deployments. This allows you to seamlessly deploy new versions of your application without disrupting the user experience.

6. Automation and management: Kubernetes provides a rich set of APIs and command-line tools that enable automation of various tasks, such as deployment, scaling, and monitoring. It also offers a web-based dashboard for visual management of clusters.

7. Secret and Configuration Management: Configuration and secrets information can be securely stored and updated without rebuilding images. Stack configuration does not require the unveiling of secrets, which limits the risk of data compromise. 


## How to use it
In order to fully utilize Kubernetes, we need to learn about its architecture and how to set things up. 
Once we have a good understanding of these concepts, everything else will become clear.

To help you get started, I will provide a walkthrough of the Kubernetes architecture and try to give you 
a high-level overview of the technology based on my knowledge. 
I will also include links to additional resources that are highly useful. 
I hope this information will be helpful in your journey with Kubernetes.

## The Architecture

![](https://www.opsramp.com/wp-content/uploads/2022/07/Kubernetes-Architecture-1536x972.png)

### What's all that!
Let's ask ourselves one simple question to get our hands on it and from that we can trace things UP one by one.

#### Where will our services run?
Our services is going to run in the "pod"

### What's a pod?

To run our application in Kubernetes, we package it into a container (typically a Docker container) 
and ask Kubernetes to run it. A pod is the smallest and most basic unit of deployment It contains one or more containers.  
It represents a single instance of a running process or a group of tightly coupled processes running together on a worker node.
When a pod is deployed or killed, all of the containers inside it are started or killed together.
and each pod can contain one or more containers that share the same network namespace and storage volumes.

Cool fact : the term "pod" is derived from the word used to describe a group of whales. Therefore, you can imagine a pod as a collection of your Docker containers, similar to how a group of whales is referred to as a pod. In Kubernetes, pods are scheduled as a single unit and consist of multiple containers. 


### Up one level ( Worker Nodes ) 

A node is the fundamental unit of computing resources. 
It can be a physical or virtual machine within a cluster that runs containerized applications. 
Nodes are responsible for running and managing the containers.

Each node in a Kubernetes cluster has the necessary components to communicate with the cluster and execute tasks. These components include:
1. kubelet: The kubelet is an agent that runs on each node and communicates with the Kubernetes control plane. It manages the containers and ensures they are running and healthy. The kubelet takes instructions from the control plane and starts, stops, or restarts containers as needed.
2. Container runtime: The container runtime is responsible for running the containers. Kubernetes supports multiple container runtimes, such as Docker, containerd, and CRI-O. The container runtime pulls container images, creates and manages containers, and provides isolation between containers on the node.
3. kube-proxy: The kube-proxy is a network proxy that runs on each node. It enables network communication between services and pods. The kube-proxy maintains network rules and routes traffic to the appropriate containers based on the service configuration.

Nodes in a Kubernetes cluster work together to form a distributed system that can schedule and manage containerized applications. They provide the necessary computing resources, such as CPU, memory, and storage, to run the containers. The control plane, including the master node, 
manages and orchestrates the nodes to ensure the desired state of the cluster.
Kubernetes (K8s) enables the management of a cluster of nodes where containers can be deployed and executed across multiple servers. Each worker node, which can be physical machines, virtual machines, or cloud instances, runs a container runtime like Docker. 
Kubernetes uses a distributed system architecture to distribute containers based on resource requirements. 
The control plane, including the master node, manages the cluster, schedules containers, monitors node health, and handles automatic rescheduling. By leveraging Kubernetes, you can easily scale applications, distribute workloads, and achieve high availability. 
Kubernetes abstracts the underlying infrastructure, allowing for focused application management and scaling.

### Up one level ( Services )

Service is an abstraction layer that acts as a stable network endpoint for accessing a set of 
pods in a Kubernetes cluster. It provides a way to expose and access applications running within the cluster.

When you create a service in Kubernetes, it assigns a unique IP address and DNS name to that service. 
This IP address and DNS name remain stable even if the underlying pods that the service is targeting are 
scaled up, down, or replaced.

Services can be categorized into two main types: 

1. ClusterIP: This is the default type of service in Kubernetes. 
It exposes the service on an internal IP address within the cluster. 
It is only accessible from within the cluster and not from outside.

2. NodePort: This type of service exposes the service on a static port on each cluster node's IP address. 
It allows external traffic to access the service by connecting to any node's IP address on the specified port.

Services can also be configured with load balancers, external IPs, 
and other advanced features depending on the cloud provider or infrastructure being used.

In summary, services provide a way to expose applications within a cluster and make them accessible 
to other services or external users. 
They offer a stable network endpoint that can be used to access the underlying pods running the application.

### Up one level ( Cluster ) 

Think of a Kubernetes cluster as a group of computers, either physical or virtual, 
that work together to run our applications. It's like a team of machines collaborating 
to make sure everything runs smoothly.

In this team, you have a leader called the Master Node. 
This leader manages and coordinates all the activities within the cluster. 
It keeps track of what needs to be done, makes decisions, and assigns tasks to the worker nodes.

The worker nodes are the rest of the team members. 
They are the ones responsible for actually running our applications. 
Each worker node can run multiple containers, which are like small, isolated units that hold our 
application and its dependencies.

To keep everything connected, Kubernetes has a networking system. 
It assigns a unique IP address to each container and ensures they can communicate with each other, 
no matter which worker node they are on. 
This way, our applications can easily talk to each other, share data, and work together seamlessly.

Storage is also an important part of the cluster. 
Kubernetes provides various storage options, so our applications can have access to durable storage for things like databases or file systems.

In addition to all that, Kubernetes allows us to create namespaces. 
These are like virtual subdivisions within the cluster. 
They help keep things organized by providing separate environments for different teams or projects. 
Each namespace has its own set of resources, so you don't have to worry about conflicts or mixing things up.

### Up one level ( Namespaces )

Namespaces provide a way to logically partition a cluster into multiple virtual clusters, 
allowing different teams or projects to have their own isolated environments within the same physical or virtual infrastructure.

Each namespace contains its own set of resources, including pods, services, and persistent volumes. 
This separation helps avoid naming conflicts and provides better resource management and access control.

By default, Kubernetes creates a "default" namespace, 
but you can create additional namespaces according to your organizational needs. 
Namespaces can be used to group related resources, apply resource quotas, and set access control policies.

Namespaces also enable better visibility and easier management of resources within a cluster. 
For example, you can list and manage resources within a specific namespace without being 
overwhelmed by resources from other namespaces.


# How to set and configure our services to run on k8s?

In the comming sections we are going away from theory and diagrams. 
we will learn how to set up our app on k8s and what exactly we need to tell k8s to do.
What state we need our application to be at that k8s will keen on maintaining, etc.

I encourage you to revisit the previous sections after you finish the whole file and checkout 
the provided links at the end, hope this will be helpful too.

# [🌟Implementation](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_3.0_Kubernetes_Integration/README-2.md)

-- -- -- 
## Further Readings (stuff i found useful)

- [Kubernetes Concepts and Architecture](https://platform9.com/blog/kubernetes-enterprise-chapter-2-kubernetes-architecture-concepts/)
- [Understanding Kubernetes Architecture with Diagrams](https://phoenixnap.com/kb/understanding-kubernetes-architecture-diagrams)
- [Andrew Lock's Introduction to Kubernetes](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-1-an-introduction-to-kubernetes)
- [Bruno Terkaly's ASP.NET Applications (Core) in Kubernetes](https://bterkaly.medium.com/running-asp-net-applications-in-kubernetes-a-detailed-step-by-step-approach-96c98f273d1a)








