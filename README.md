# Micro Instagram 

Welcome to this (hopefully) collaborative tutorial on microservices. Together, we're going to explore the `why` and the `how` of implementing a web application with microservices architecture.

## Table of contents
1. [Why a tutorial?](#Why-a-tutorial)
2. [What are we going to make?](#what-are-we-going-to-make)
3. [Our plan.](#our-plan)
4. [The nature of software](#the-nature-of-software)
5. [The Why.](#the-why)
6. [The How - Dissecting instagram and identifying key entities.](#the-how---dissecting-instagram-and-identifying-key-entities)
7. [Comunication between services.](#asking-questions---comunication-between-services)
<details>
<summary>8. Message Bus </summary>

* [RabbitMQ](#message-bus---rabbitmq)
* [User Service As a publisher](#user-service-as-a-publisher)
* [Notification Service As a consumer](#notification-service-as-a-consumer)
* [Recapping and asking more questions](#recapping-and-asking-more-questions)

</details>

9. [What's next ?](#whats-next-)

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
# Our Plan

## ✅ Done 

- **Version 1.0:** Implemented `UserService` and `NotificationService` services, enabling communication via RabbitMQ message broker.
- **Version 2.0:** Containerized services using Docker and configured docker-compose for easier deployment.
- **Version 2.1:** Added HTTPS configuration to enhance the security of our containers.

## 🛠️ In Progress

- **Version 3.0:** Currently working on integrating Kubernetes (k8s) into our project.

## 🌟 To Do

- **API Gateway:** Implement an API gateway for improved service management and routing.
- **React Web Client:** Develop a React web client to enhance application testing and user experience.


-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 
##  The nature of software

Now, we have no clue about how instagram's team planned and developed their platform throughout the years. and actually, we don't need to, because even if we did, we 
probably wouldn't understand much of it!

What we can do, however, is contemplate instagram as a completed application. We can break it down into smaller pieces and examine each piece. When thinking about instagram or any software for that matter there are countless ways to plan and design it. Sometimes, we might start coding, only to realize realize that your plan is completely broken. But that's the nature of logic and thought. unlike strict realiy , our minds are broad enough to accommodate even the most imperfect ideas, which is where imagination kicks in, taking things to the next level.

The reality of computations and programming , on the other hand, is a bit like assembling Lego pieces. We contemplate over how to piece together chunks of code into a working software that won't collapse if we tweak one element.

We could trace back to the beginning of it all, to the emergence of computational reality from simple electric currents representing numbers in wires. But let's keep it simple, we aim to push one step at a time. and always ask ourselves one single question "Is it working?" Yes? Then Cheers, that's already more than we asked for!

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

## The Why

In this section, we're going to explore the "why" behind our decision to use a microservices architecture for our Instagram-like application, as opposed to a monolithic architecture.

Before we dive into the reasons behind our decision, let's define what we mean by "microservices".

A `microservices architecture` is a method of developing software systems that are made up of independently modular services. Each service runs a unique process and communicates through a well-defined, lightweight mechanism to serve a specific goal.

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

## Asking Questions - Comunication between services

The very first step in building a microservices architecture is to gain a perspective of the big picture of a fully working application. We have a set of services, each of which is a piece of software responsible for a set of tasks and has its data access. The question we need to ask ourselves is how will these separate services result in a seamless working application? Do they have to communicate?

The answer is yes. Each service should be able to listen and process every action that relates to its functionality and produce or announce actions that are out of its hand for other services to handle.

- There are two types of comunications :
Synchronous or Asynchronous communication methods are the two main types of communication methods, and each has its benefits and drawbacks. 
* Synchronous communication involves making HTTP requests to other services
* Asynchronous communication involves using message queues or publish-subscribe mechanisms to send messages between services.

Remember we always want to ask the question first before jumping on solutions. keeping an image of what's going on in the back of your mind and filling gaps in it one step at a time.

When building a microservices architecture, it's important to consider how and what the services will communicate. Let's consider our instagram-like application, 
for example : When someone follows you on instagram, the `User Service` creates a follow between you and the follower. The `Notification Service` then sends you a notification about the new follow.

To enable this communication, the `User Service` could send a synchronous HTTP request to the `Notification Service` or use asynchronous communication using a message queue. If the action of follow only concerns the `Notification Service`, a synchronous HTTP request would be a fine choice. However, if the follow concerns not just the `Notification Service` but more than one service, like the `Feed Service`, it's better to use a publish-subscribe mechanism with a message bus like RabbitMQ or Apache Kafka.

Also using synchronous communication may not always be the best solution. For example, when the User Service creates a follow between two users, it doesn't need to wait for the `Notification Service` to send the notification. Therefore, using synchronous communication in this case would add unnecessary latency. Instead, using asynchronous communication with a message queue allows the `User Service` to continue with its job, while the `Notification Service` processes the event in the background and sends the notification when it's ready. This approach improves the performance and scalability of the application.
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

### What are queues ?

Queues in RabbitMQ are the basic building blocks for messaging in the RabbitMQ message broker system.
A queue is an ordered list of messages. When a sender (publisher) send a message, it gets delivered to a specific queue. Multible receivers (consumers) can then subscribe to the queue and receive messages from it. 
Queues enable decoupling of producers and consumers, allowing messages to be sent and processed asynchronously, improving scalability and fault in distributed systems.

So, the queue acts as a middle point between services. It serves as a communication channel between services, even if they are not aware of each other. When a service wants to communicate something, it refers to the specific queue (or pattern, as we will explain later) it should send the message to. Similarly, if a service is waiting for events to occur, it knows which queues it should receive or consume messages from.

Now, let's explore RabbitMQ and see a real-life scenario. We will play the role of both ends - the publisher and the consumer - and observe the perspective from each side.

But before doing so, lets take a look at the internal workings of RabbitMQ by asking some questions.

#### Will each publisher announce the list of queues that its wants the message to be mapped to ?

No, RabbitMQ features a very cool concept which is called "an Exchange" Exchanges : are the entities responsible for receiving messages from publisher and routing them to one or more queues based on specific rules. RabbitMQ supports four types of exchanges: `direct`, `fanout`, `topic`, and `headers`. we are going to explore each one in a sec.

#### How to use exchanges and how does they know about the queues that exist ?

First, you need to create an exchange and specify its type. RabbitMQ supports four types of exchanges: direct, fanout, topic, and headers. Each exchange type has different routing rules and is suitable for different use cases.

Once you have created an exchange, you can bind it to one or more queues. This is done by creating a binding between the exchange and the queue. Bindings specify the routing rules for messages to be delivered from the exchange to the queue. When a message is sent to the exchange, RabbitMQ uses the routing rules specified by the bindings to determine which queues the message should be delivered to.

Exchanges do not know about the queues that exist in RabbitMQ. Instead, bindings are used to connect the exchange to specific queues. When a binding is created, it specifies the name of the queue that the exchange should route messages to. The queue must already exist in RabbitMQ for the binding to be created.

#### a Binding ?

In RabbitMQ, a binding is a link between an exchange and a queue. Bindings define the routing rules for messages to be delivered from the exchange to the queue. When a message is published to an exchange, the exchange uses the bindings to determine which queues the message should be routed to.

Bindings are created using a routing key, which is a string that specifies the routing criteria for a message. The routing key is used by the exchange to decide which queues to deliver the message to. The routing key can be any string, and its format depends on the exchange type.

There are four types of exchanges in RabbitMQ:

1. Direct exchange: Messages are delivered to queues based on a matching routing key. The routing key specified in the binding must exactly match the routing key used by the producer.

2. Fanout exchange: Messages are delivered to all queues that are bound to the exchange. The routing key specified in the binding is ignored.

3. Topic exchange: Messages are delivered to queues based on a matching pattern. The routing key specified in the binding can contain wildcards to match multiple routing keys.

4. Headers exchange: Messages are delivered to queues based on a set of header values. The routing key specified in the binding is ignored.

To use bindings in RabbitMQ, you need to create an exchange, create one or more queues, and then create a binding between the exchange and the queue. When creating a binding, you need to specify the routing key and any other options that are specific to the exchange type.

#### a Routing key ?

Routing keys are used by exchanges to determine how to route messages to queues. Routing keys are strings that are included with each message that is published to an exchange. The routing key is used by the exchange to determine which queues the message should be delivered to.

The format of the routing key depends on the type of exchange that is being used. For direct and topic exchanges, the routing key is a string that is matched against the routing criteria specified in the exchange bindings. 

- For fanout exchanges, the routing key is ignored.
- For direct exchanges, the routing key must match exactly with the routing key specified in the binding for the message to be delivered to the queue. For example, if the binding specifies a routing key of "my_routing_key", then a message published with the routing key "my_routing_key" will be delivered to the queue, but a message published with the routing key "another_routing_key" will not.

- For topic exchanges, the routing key is a string that can contain one or more words separated by dots, such as "my.routing.key". the words in the routing key are used to match against the routing patterns specified in the exchange bindings using special wildcard characters. the two wildcard characters that are supported are:

(star) * : Matches exactly one word in the routing key. <br/>
(hash) # : Matches zero or more words in the routing key. <br/>

For example, if an exchange binding specifies a routing pattern of "my.#.key", then a message published with the routing key "my.routing.key" or "my.very.long.routing.key" will be delivered to the queue, but a message published with the routing key "another.routing.key" will not.

Routing keys allow messages to be selectively delivered to specific queues based on the routing criteria specified in the exchange bindings. This provides a flexible and powerful mechanism for building complex messaging systems.

Enough theory, let's take an exmple.

Remember last time when you followed another user on instagram? 

What did happen behind the scenes?

-> You clicked a button </br>
-> The web client sends a POST request to the server on the endpoint /users/follow/{your-friend}. </br>
-> The `User Service` receives the request and creates a follow relationship between you and the other user. </br>
-> The `User Service` publishes an event to RabbitMQ indicating that a new follow relationship has been created. </br>
-> The `Notification Service` consumes the event from RabbitMQ and creates a new notification indicating that you have followed the other user </br>
-> The `Notification Service` sends the notification to your friend telling them that now they are followed by you. </br>

that may not be exactly the case on real instagram, but it is with ours.

Now, let's think about the queues we need 
if we just created a follow, what do we need to publish? 
of cousre its an event that associated to entity `follow` and holds the needed attributes that discribe the action that happened
so, to comunicate this we need to stadarlize things up so each service is aware of what type of event it is about to recive or publish
there are three ways that im aware of at this time and im going to talk about breifley and we can explore these approaches in more detail later on.

If you have any suggestions for any of them or have a fourth one, please let us know by sharing your thoughts.
 
1. First way is to include a type field in the event payload :

Having our base event class like this. Holding attributes like `Id`, `CreationDate` and a virtual field `EventType` to indicate event type

```cs
    public abstract class BaseEvent
    {
        public BaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    
        public Guid Id { get; }
        public DateTime CreationDate { get; }
        public virtual Type EventType => typeof(BaseEvent);
    }
```

Why do we need a base event? 
because there are basic attributes that every event must contain. By having a base event, we can avoid duplicating these basic attributes in all the event classes we have. And we need all of our events to inherit from `BaseEvent` so all of them must override the field `EventType`.

And our `FollowCreatedEvent` might look like this. Overriding `EventType` to its type `typeof(FollowBaseEvent)`
```cs
    public class FollowCreatedEvent : BaseEvent
    {
        public override Type EventType => typeof( FollowCreatedEvent );

        public Guid FollowerId { get; set; }
        public Guid FollowedId { get; set; }
        public string FollowerUsername { get; set; }
    }
```

Now listeners can recieve and desericalize the event knowing its type from the field `EventType`

2. Second way is Use different exchange or topic for each event type, and that's the one i choosed and we are going to implement in the very next section.

3. A third way is to use a message schema or contract that defines the structure and format of the event, including any required fields or metadata. When consuming events, services can validate the message against the schema or contract to ensure that it's of the expected type. let's take a brief look.

Let's say that you have a `User Service` that publishes events related to new follow relationship. You want to ensure that other services consuming these events know exactly what to expect in terms of the event payload.

To accomplish this, you could define a message schema or contract for the `FollowCreated` event that includes the expected structure and format of the event. 
Here's an example of what this schema might look like in JSON format:

```json
    {
      "type": "object",
      "properties": {
        "id": {
          "type": "string"
        },
        "creationDate": {
          "type": "string",
          "format": "date-time"
        },
        "eventType": {
          "type": "string"
        },
        "followerId": {
          "type": "string"
        },
        "followedId": {
          "type": "string"
        },
        "followerUsername": {
          "type": "string"
        }
      },
      "required": [
        "id",
        "creationDate",
        "eventType",
        "followerId",
        "followedId",
        "followerUsername"
      ]
    }
```

When the `User Service` publishes a `FollowCreated` event, it can include the event payload as a JSON object that adheres to this schema. 
Other services consuming this event can then validate the message against the schema to ensure that it's of the expected type.


#### `User Service` As a publisher 

first off we need to start a connection to RabbitMQ 
We do this by calling `factory.CreateConnection()` and assigns the resulting connection to the connection field of the class. then create a new model (channel) from the connection.

Once the connection is established, the code creates a new channel on the connection. Channels are used to send and receive messages to and from the RabbitMQ server.
and of course before starting to throw messages at rabbitMQ we want to declare exhcanges that we are going to use and bind them to the queues that we are going to publish messages to.
here we declare `follow_exchange`. we know that an exchange is an entity that receives messages from producers and routes them to queues with matching routing keys. The `follow_exchange` exchange is of type ExchangeType.Topic, which means it can route messages based on a pattern match on the routing key.

After the exchange is declared, we are going to declare a new queue named `follow-queue`. and we also know that a queue is a buffer that holds messages that are waiting to be processed by a consumer. In this case, the follow-queue will hold messages related to new follow relationships.

Finally, we are going to bind the `follow-queue` to the `follow_exchange` exchange with the routing key `follow.*`. Binding is the process of connecting a queue to an exchange so that messages can be routed to the queue. In this case, the `follow-queue` will receive messages that have a routing key that starts with `follow.`.

For exampe : routing key wil be like `follow.FollowCreated` , `follow.FollowDeleted` etc. all routed to the queue : `follow-queue`.
and the second part of the routing key, the one that follows the dot `FollowCreated`, is for determining the type of the published event, This allows consumers to easily identify event types, which is the second method we discussed in the previous section. which i think is pretty cool.

```cs
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessageBusClient> _logger;

        public MessageBusClient(
            IOptions<RabbitMQSettings> rabbitMQSettings, 
            ILogger<MessageBusClient> logger
            )
        {
            _logger = logger;
            // create a new connection factory with the settings from the appsettings.json file
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                UserName = rabbitMQSettings.Value.UserName,
                Password = rabbitMQSettings.Value.Password,
                Port = rabbitMQSettings.Value.Port
            };

            try
            {
              // create a new connection using the connection factory
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // declare the Follow exchange
                _channel.ExchangeDeclare(
                    exchange: "follow_exchange",
                    type: ExchangeType.Topic
                    );

                // declare the Follow queue
                _channel.QueueDeclare(
                    queue: "follow-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                    );

                // bind the Follow queue to the Follow exchange with the routing key "follow.*"
                _channel.QueueBind(
                    queue: "follow-queue",
                    exchange: "follow_exchange",
                    routingKey: "follow.*"
                    );

                logger.LogInformation("Connected to MessageBus");
            }
            catch (Exception ex)
            {
                logger.LogError ($"Could not connect to the Message Bus: {ex.Message}");
            }
        }

        public void PublishFollowEvent(BaseEvent @event)
        {
            var exchangeName = "follow_exchange";
            var routingKey = "follow." + @event.GetType().Name;

            PublishEvent(@event, routingKey, exchangeName);
        }

        private void PublishEvent(BaseEvent @event, string routingKey, string exchangeName)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };

            var typedEvent = Convert.ChangeType(@event, @event.GetType());
            var message = JsonSerializer.Serialize(typedEvent, options);

            if (_connection.IsOpen)
            {
                _logger.LogInformation("RabbitMQ connection is open, sending message.");
                SendMessage(message, exchangeName, routingKey);
            }
            else
            {
                _logger.LogInformation("RabbitMQ connection is closed.");
            }
        }

        private void SendMessage(string message, string exchangeName, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: exchangeName,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);

            _logger.LogInformation($"Message sent successfully, Message: {message}");
        }
    }

```


#### `Notification Service` As a consumer 

As a consumer that must be always be listening for any incomming messages we can let our `MessageBusSubscriber` inherit from `BackgroundService`

from the definition :  ` The BackgroundService class implements the IHostedService interface and provides a simple framework for creating a background task that can be started and stopped by the application's IHost implementation. It handles the lifecycle management of the background task, including starting and stopping it when the application is started or stopped.`

```cs
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly ILogger<MessageBusSubscriber> _logger;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;

        public MessageBusSubscriber(
            IOptions<RabbitMQSettings> rabbitMQSettings,
            IEventProcessor eventProcessor,
            ILogger<MessageBusSubscriber> logger)
        {
            _logger = logger;
            _eventProcessor = eventProcessor;

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                UserName = rabbitMQSettings.Value.UserName,
                Password = rabbitMQSettings.Value.Password,
                Port = rabbitMQSettings.Value.Port
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("Listening on the Message Bus");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not connect to the Message Bus: {ex.Message}");
            }
        }

        // This method is called when the IHostedService starts.
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += HandleReceivedEvent!;

                // Consume from follow-queue
                _channel.BasicConsume(queue: "follow-queue", autoAck: true, consumer: consumer);

            }catch(Exception ex)
            {
                _logger.LogError($"Error on starting MessageBusSubscriber service {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private void HandleReceivedEvent(object ModuleHandle, BasicDeliverEventArgs ea)
        {
            try
            {
                _logger.LogInformation("Event message was received");

                var body = ea.Body;
                var eventMessage = Encoding.UTF8.GetString(body.ToArray());

                // DeserializeEvent function that we are going to implement
                // should return an event deserialized to its type after determining its type
                // with the help of routing key as we talked earlier.
                var @event = (BaseEvent)DeserializeEvent(eventMessage, ea.RoutingKey);

                _eventProcessor.ProcessEvent(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on processing received message {ex.Message}");
            }
        }
    }

```

- As we did with our publisher we are going to create a connection. then we are going to tell RabbitMQ what queues we are interested in.

```cs
      _channel.BasicConsume(queue: "follow-queue", autoAck: true, consumer: consumer);
```   
- And we can add it to the service collection as a `HostedService` that start running as soon as the service does.

```cs
      builder.Services.AddHostedService<MessageBusSubscriber>();
```

Now we have two services one publishing events and the other is listening and conuming those events! <br/>
let's celebrate!

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --

### RabbigMQ | Recapping and asking more quesions 

#### Who is supposed to declare Queues?

while the publisher is typically responsible for declaring the exchange and queue, it is also possible for the subscriber to declare the queue and the exchange if the subscriber needs to "ensure" that the queue and exchange exist before it starts consuming messages.

this can be useful in scenarios where the subscriber is deployed on a different machine than the publisher or in a distributed system where the publisher and subscriber may be started in any order.

#### Is the consumer aware of all the details? ( the consumer point of view )

from the consumer's perspective in RabbitMQ, the primary concern is the queue from which they receive messages.

In RabbitMQ, messages are not published directly to queues. Instead, the producer sends messages to an exchange. The exchange is then responsible for routing the message to one or more queues based on certain rules. These rules can be configured using bindings and routing keys.

However, a consumer does not need to be aware of these details. all the consumer needs to know is the name of the queue from which it should consume messages. The consumer subscribes to a queue and then processes messages from that queue. It does not need to be aware of which  exchange the messages came from or what routing rules were used to get the message to the queue.

#### If you have any other question please drop it right here or raise an issue about it.
-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --

### What's next ?

So, what's next we said that we are not particularyly intersted in implementing instagram functionalities, we can continue adding more services to this implementation along with the two we currentlly have (User Service and Notification Service), but we are more intersted in using and learning about the why and how of every microservices technology.

Lets list them :
* Kubernetes - a container orchestration and management platform.
* Docker - a containerization technology.
* Docker-compose - Tool for defining and running multi-container Docker applications.
* gRPC - high performance Remote Procedure Call (RPC) framework that can run in any environment.
* API Gateway - Middleware that acts as a gateway between clients and backend services, providing authentication, rate limiting, and other functionalities.
* HAProxy - Load balancer and proxy server for TCP/HTTP-based applications.

Consider this image as a reference.

<p align="center">
  <img src="https://github.com/mohammed0xff/micro-instagram/blob/master/images/typical-microservice-high-level-view.png" />
</p>


We can continue on improving this code adding those technologies. however, this might be overwhelming for a biginner to look at as a whole.
So, will gradually create other versions of the application, with each version being one step further from the previous one. We will proceed one step at a time, and each new version will include just one additional feature compared to the previous one. and i will figure out a way so they can be easily navigated between.

Also this will allow anyone who might decide to tweek this applicatoin in any way or push it towards any side adding some features, doing anything differntly and explaining their point of view. and this would be VERY exciting!

Too much to learn? Dont worry, While always keeping the big picture in your mind, and always asking the `Why` we will never be confused. or allow yourself to be confused. remember, confusion a fundmental part of the learning process! 

-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --


