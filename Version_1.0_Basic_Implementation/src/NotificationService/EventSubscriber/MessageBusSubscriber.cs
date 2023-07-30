using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.EventProcessing;
using Shared.Events;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NotificationService.Common;
using System.Reflection;

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
            _connection = factory.CreateConnection();

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                // rabbit mq configurations go here 
                _channel = _connection.CreateModel();

                // Adding a delegate to ConnectionShutdown event to Log a message
                // indicating that the RabbitMQ connection has been shut down
                // when the RabbitMQ connection is lost & `ConnectionShutdown` event is triggered.
                _connection.ConnectionShutdown +=
                    (object sender, ShutdownEventArgs e) => {
                        _logger.LogInformation("RabbitMQ Connection Shutdown");
                    };

                _logger.LogInformation("Listening on the Message Bus");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not connect to the Message Bus: {ex.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += HandleReceivedEvent!;

                // Consume from both the user-queue and follow-queue
                _channel.BasicConsume(queue: "user-queue", autoAck: true, consumer: consumer);
                _channel.BasicConsume(queue: "follow-queue", autoAck: true, consumer: consumer);

            }
            catch (Exception ex)
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
                var @event = (BaseEvent)DeserializeEvent(eventMessage, ea.RoutingKey);

                _eventProcessor.ProcessEvent(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on processing received message {ex.Message}");
            }
        }

        private object DeserializeEvent(string eventMessage, string routingKey)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentNullException(nameof(routingKey));
            }

            // Get the EventType from the routing key
            var splittedRoutingKey = routingKey.Split('.');
            var eventTypeName = splittedRoutingKey.Length > 1 ? splittedRoutingKey[1] : null;

            if (eventTypeName == null)
            {
                throw new ArgumentException($"Invalid routing key: {routingKey}");
            }

            // Convert the EventType to a Type
            var fullTypeName = $"Shared.Events.{eventTypeName}";
            var assemblyName = Assembly.GetAssembly(typeof(BaseEvent))!.GetName();

            Type? eventType = Type.GetType($"{fullTypeName}, {assemblyName}");

            if (eventType == null)
            {
                throw new ArgumentException($"Invalid event type: {eventTypeName}");
            }

            // Deserialize to the type
            var eventObj = JsonSerializer.Deserialize(eventMessage, eventType, options);

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
