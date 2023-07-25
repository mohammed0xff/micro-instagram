using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.EventProcessing;
using Shared.Events;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NotificationService.Common;
using Shared.Events.FollowEvents.Follow;

namespace NotificationService.EventSubscriber
{
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
                _logger.LogInformation("Connected to Message Bus");

                // Adding a delegate to ConnectionShutdown event to Log a message
                // indicating that the RabbitMQ connection has been shut down
                // when the RabbitMQ connection is lost & `ConnectionShutdown` event is triggered.
                _connection.ConnectionShutdown += 
                    (object sender, ShutdownEventArgs e) => {
                        _logger.LogInformation("RabbitMQ Connection Shutdown");
                    };
                
                InitializeRabbitMQ();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private void InitializeRabbitMQ()
        {

            _channel = _connection.CreateModel();

/*            // User exchange and queue
            _channel.ExchangeDeclare(
                exchange: "user_exchange", 
                type: ExchangeType.Fanout
                );
            
            _channel.QueueDeclare(
                queue: "user-queue", 
                durable: true, 
                exclusive: false, 
                autoDelete: false
                );
            
            _channel.QueueBind(
                queue: "user-queue", 
                exchange: "user_exchange", 
                routingKey: "user.*"
                );


            // Follow exchange and queue
            _channel.ExchangeDeclare(
                exchange: "follow_exchange", 
                type: ExchangeType.Fanout
                );
            
            _channel.QueueDeclare(
                queue: "follow-queue", 
                durable: true, 
                exclusive: false, 
                autoDelete: false
                );
            
            _channel.QueueBind(
                queue: "follow-queue", 
                exchange: "follow_exchange", 
                routingKey: "follow.*"
                );*/

            _logger.LogInformation("Listening on the Message Bus");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (ModuleHandle, ea) =>
                {
                    try
                    {
                        _logger.LogInformation("Event message was received");

                        var body = ea.Body;
                        var eventMessage = Encoding.UTF8.GetString(body.ToArray());
                        var @event = (BaseEvent)DeserializeEvent(eventMessage, ea.RoutingKey);

                        _eventProcessor.ProcessEvent(@event);
                    
                    }catch (Exception ex)
                    {
                        _logger.LogError($"Error on processing recieved message {ex.Message}");
                    }
                };

                // Consume from both the user-queue and follow-queue
                _channel.BasicConsume(queue: "user-queue", autoAck: true, consumer: consumer);
                _channel.BasicConsume(queue: "follow-queue", autoAck: true, consumer: consumer);

            }catch(Exception ex)
            {
                _logger.LogError($"Error on starting MessageBusSubscriber service {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private object DeserializeEvent(string eventMessage, string routingKey)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            // Get the EventType from the routing key
            var splittedRoutingKey = routingKey.Split('.');
            var eventTypeName = splittedRoutingKey.Length > 1 ? splittedRoutingKey[1] : null;

            if (eventTypeName == null)
            {
                throw new ArgumentException($"Invalid routing key: {routingKey}");
            }

            // Convert the EventType to a Type
            var fullTypeName = $"Shared.Events.{eventTypeName}";

            // Get the assembly that contains the type
            Type? eventType = Type.GetType(fullTypeName); // returning null !
            
            if (eventType == null)
            {
                // throw new ArgumentException($"Invalid event type: {eventType}");
            }

            // Deserialize to the type
            // var eventObj = JsonSerializer.Deserialize(eventMessage, eventType, options);
            var eventObj = JsonSerializer.Deserialize(eventMessage, typeof(FollowCreatedEvent), options);

            return eventObj;
        }
        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

    }
}
