# Micro Instagram 

Welcome to this (hopefully) collaborative tutorial on microservices. Together, we're going to explore the `why` and the `how` of implementing a web application with microservices architecture.

## Why a tutorial? 

I beleive that every open source project should be tutorialized to some extent. and offer insight into the thought processes and decisions that went into creating the software. allowing ourselves to share our ideas, learn from each other's experiences, and even grow from our mistakes. 

Going beyond implementation, contemplate about the 'why' behind every decision and step. cause after all, technology isn't just about implementation. Implementation is actually a second step, one that follows careful planning and decision-making. It's about pushing the boundaries in a direction that satisfies a specific "need" or in the case of software, a "problem".

What we are trying to do here is creating a space for all of us to learn together. learn from each others ideas as well as mistakes, and grow as a community. This shared learning journey is the magic that keeps pushing technology forward.

So, I encourage you to dive in! Explore the tutorial, play around with the code, and don't be afraid to make mistakes. If you've got ideas on how to improve the project, have any qusiton or if you find something that could be done differently, we want to hear about it. Raise an issue, contibute to the project, shoot me an email, or even shout it out loud i might just hear you! Be welcomed to our shared space!

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## What are we going to make?

We're going to construct an instagram-like application to understand the principles of microservices and explore modern technologies. However, our focus isn't solely on recreating Instagram's specific functionalities. Instead, we're aiming to learn how to reason about software design.

Instagram provides a compelling, real-world example to dissect, understand, and plan. Throughout this process, we'll attempt to think like software designers, breaking down complex problems and planning their solutions.

We'll be implementing our application using technologies such as ASP.NET Core, RabbitMQ, and Docker. These tools will allow us to dive into the world of microservices and gain hands-on experience in building and deploying a modern, scalable application.

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## The Why

In this section, we're going to explore the "why" behind our decision to use a microservices architecture for our Instagram-like application, as opposed to a monolithic architecture.

Before we dive into the reasons behind our decision, let's define what we mean by "microservices".

A microservices architecture is a method of developing software systems that are made up of independently modular services. Each service runs a unique process and communicates through a well-defined, lightweight mechanism to serve a specific goal.

When crafting a software application, one of the critical decisions we make is the architectural style we adopt. It fundamentally influences how we organize our code, how our application scales, and how teams work together.

Choosing a microservices architecture offers us several key advantages:

1. Scalability : Microservices are independently deployable services. This means we can scale up the parts of our application that need it the most, rather than scaling the entire application.

2. Resilience : With a microservices architecture, if one service fails, the others can continue to function. This isolation reduces the risk of system-wide failures.

3. Technological Freedom : Microservices provide the flexibility to use different technologies and languages within the same application. We can choose the best technology for each service.

4. Faster Time to Market : Microservices can be developed, tested, and deployed independently, enabling faster and more frequent updates and improvements.

5. Single Responsibility : Each microservice has a specific role and fulfills a single business capability. This follows the single responsibility principle where a component of the software should have only one reason to change.

6 Loosely Coupled : Microservices are independent entities. Changes to one service should not require changes in another. This allows teams to work on different services without affecting the work of others.

7 Distributed Development  : Microservices can be developed using different programming languages and can use different data storage technologies.

8. Fault Isolation : A failure in one service does not directly affect others. This leads to better fault isolation and resilience.

9. Data Isolation : Each microservice can have its own database, which allows it to be decoupled from other services. This helps to maintain data consistency within the service.

10. Communication : Microservices communicate with each other through well-defined APIs and protocols, often HTTP/REST or asynchronous messaging when using an event-driven architecture.

While a monolithic architecture might seem simpler at first, as the application grows, the complexity can become a significant challenge. With microservices, we can better manage this complexity by breaking down the application into manageable, isolated components.

In the following sections, we'll delve deeper into these advantages and explore how we can leverage them to build our Instagram-like application.


-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## The How - Dissecting instagram and identifying key entities:

When considering how to design an application, I like to think about the entities involved. What are they? What actions can they perform? How do they interact with each other?

In the case of Instagram, the first entity we encounter is the user. Yes, you! While we're developers, we're also users of the applications we build. So, what can a user do? Here's a list of Instagram user actions that directly cause a change in the application:

* Create Account
* Follow / Unfollow
* Post Content
* Comment / Like
* Share Stories
* Send Direct Messages


By considering the actions of one entity, you start to see other entities and their relationships. For example, posting content introduces the 'Post' entity. Commenting or liking introduces 'Comments' and 'Likes' entities, and so on.

We can create an exhaustive list for each entity like this. 

- **User**: The central entity in the application. A user can perform actions like follow another user, create a post, post a story, like a story, send a message, and receive notifications.
- **Post**: An entity representing content shared by a user. A user can create a post, which then becomes part of their profile and appears in the feeds of their followers.
- **Follow**: A relationship entity that represents a user's interest in another user's content. When a user follows another, the follower's feed updates to include the followed user's posts.
- **Story**: This is a type of post that a user can create. It's temporary and disappears after 24 hours. Other users can view and like the story.
- **Profile**: This entity represents a user's identity on the platform, including their shared posts, bio, and profile picture. A user can edit their profile and it can be viewed by other users.
- **Notification**: This is a system-generated entity that alerts users to activity related to their account, such as a new follower, a like on teir post, or a new message.
- **Message**: An entity representing a private communication between users. A user can send a message to another user, and these messages are stored in a private thread.

And to make this text a bit visual i made this diagram : 

<p align="center">
  <img src="https://github.com/mohammed0xff/micro-instagram/blob/master/images/entities.png" width="700" />
</p>

![]()

Each of these entities has different actions associated with it, and these actions often involve interactions between multiple entities. For example, when a user (entity 1) likes a post (entity 2), it can generate a notification (entity 3) for the post's owner. This interconnectedness is part of what makes the application dynamic.

But you might wonder, "Are we creating a microservice for each entity?" Not exactly. While these entities guide us, they don't dictate a one-to-one mapping to microservices. However, considering these entities gives us a clear view of the internal workings of the application, highlighting areas of cohesion and separation.

It's a bit like designing a database, which also involves identifying entities and their relationships. But while database entities are static, application entities like those in Instagram are dynamic and perform actions. In a database, foreign keys might indicate dependencies, but applications have many more facets to consider.

Having said that, let's look at some initial microservices we could consider based on these entities:

1. User Service: Handles user-related operations such as account creation, authentication, profile management, data privacy settings, and managing relationships between users (following and unfollowing).
2. Profile microservice: This microservice would manage user profiles, including the ability to view and edit profiles, as well as user-generated content such as photos and videos.
3. Feed microservice: This microservice would handle the display of content on the user's feed, including the ability to like, comment, and share posts.
4. Search microservice: This microservice would handle search functionality, allowing users to search for other users, hashtags, and content.
5. Notification microservice: This microservice would handle notifications, such as likes, comments, and direct messages.
6. Messaging microservice: This microservice would handle direct messaging functionality between users.
7. Analytics microservice: This microservice would handle tracking and reporting on usage and engagement metrics, such as likes, comments, and views.
8. Payment microservice: This microservice would handle payment processing for features such as sponsored content and advertising.

Each of these entities could be a separate microservice with its own database and business logic. They would communicate with each other via APIs to perform the required operations.

Remember, this is just a starting point. As we get to know more about our application's requirements and our users' needs, we can refine this list. The beauty of a microservices architecture is its flexibility, which allows us to adapt our application as it evolves, ensuring we can continuously deliver a scalable, and efficient service.

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## The How - Asking quesions

The very first step in building a microservices architecture is to gain a perspective of the big picture of a fully working application. We have a set of services, each of which is a piece of software responsible for a set of tasks and has its data access. The question we need to ask ourselves is how will these separate services result in a seamless working application? Do they have to communicate?

The answer is yes. Each service should be able to listen and process every action that relates to its functionality and produce or announce actions that are out of its hand for other services to handle.

- There are two types of comunications :
Synchronous or Asynchronous communication methods are the two main types of communication methods, and each has its benefits and drawbacks. 
* Synchronous communication involves making HTTP requests to other services
* Asynchronous communication involves using message queues or publish-subscribe mechanisms to send messages between services.

Remember we always want to ask the question first before jumping on solutions. 
keeping an image of what's going on in the back of your mind and filling gaps in it one step at a time.

Great job on your tutorial! Here's a suggestion to finalize the text:

When building a microservices architecture, it's important to consider how and what the services will communicate. Let's consider our instagram-like application, 
for example : When someone follows you on instagram, the `User Service` creates a follow between you and the follower. The `Notification Service` then sends you a notification about the new follow.

To enable this communication, the `User Service` could send a synchronous HTTP request to the Notification Service or use asynchronous communication using a message queue. If the action of follow only concerns the `Notification Service`, a synchronous HTTP request would be a fine choice. However, if the follow concerns not just the `Notification Service` but more than one service, like the Feed Service, it's better to use a publish-subscribe mechanism with a message bus like RabbitMQ or Apache Kafka.

Also using synchronous communication may not always be the best solution. For example, when the User Service creates a follow between two users, it doesn't need to wait for the Notification Service to send the notification. Therefore, using synchronous communication in this case would add unnecessary latency. Instead, using asynchronous communication with a message queue allows the `User Service` to continue with its job, while the Notification Service processes the event in the background and sends the notification when it's ready. This approach improves the performance and scalability of the application.
We will explore other use cases of synchronous communications later in this tutorial.

On the other hand, message buses are a beautiful solution because they allow you to publish an event whenever something happens. Whoever needs to handle that event can subscribe to it and respond accordingly. This approach provides a scalable solution and helps to decouple services, making it easier to maintain and modify the application over time.

in the next seciton we are going to explore RabbitMQ.

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## Message Bus - RabbitMQ

Every software or technology that exists is intended to solve a specific problem. As developers, our objective is to learn how to utilize these technologies in order to create practical solutions for the real world. It is crucial for us to constantly remind ourselves of the question: "Why does this technology exist?" shifting our minds back to the actual problem that the technology is designed to address, and starting from there.

So, what is the problem here? yes, we have two services that want to comunicate together asynchronusly.
consider this example. Me (service one, producer) wanna tell you (service two, consumer) something (and im not very urgent) but you might be busy. So, i write it down and leave it on your desk.
So, you can read the message whenever you finish your current task.

And this is basically it that's what rabbit mq is, in a nut shell 
but what if we have one than more service that want to comunicate ?

If there are more than two services that need to communicate, that's we they created "Queues" as a centralized and standardized way of exchanging messages. Each service can publish messages to one or more queues, and each consumer can subscribe to the queues it needs to receive messages from. This enables decoupling of producers and consumers, allowing messages to be sent and processed asynchronously.

#### What are queues ?

Queues in RabbitMQ are the basic building blocks for messaging in the RabbitMQ message broker system.
A queue is an ordered list of messages. When a sender (publisher) send a message, it gets delivered to a specific queue. Multible receivers (consumers) can then subscribe to the queue and receive messages from it. 
Queues enable decoupling of producers and consumers, allowing messages to be sent and processed asynchronously, improving scalability and fault in distributed systems.

So, the queue acts as a middle point between services. It serves as a communication channel between services, even if they are not aware of each other. When a service wants to communicate something, it refers to the specific queue (or pattern, as we will explain later) it should send the message to. Similarly, if a service is waiting for events to occur, it knows which queues it should receive or consume messages from.

Now, let's explore RabbitMQ and see a real-life scenario. We will play the role of both ends - the publisher and the consumer - and observe the perspective from each side.

#### As a publisher 


#### As a consumer

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --


