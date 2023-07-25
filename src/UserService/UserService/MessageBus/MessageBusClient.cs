using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Converters;
using Shared.Events;
using System.Text;
using System.Text.Json;
using UserService.Common;

namespace UserService.MessageBus
{
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

                // User exchange and queue
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
                    );

                // Adding a delegate to ConnectionShutdown event to Log a message
                // indicating that the RabbitMQ connection has been shut down
                // when the RabbitMQ connection is lost & `ConnectionShutdown` event is triggered.
                _connection.ConnectionShutdown += 
                    (object sender, ShutdownEventArgs e) => {
                        _logger.LogInformation("RabbitMQ Connection Shutdown.");
                    };

                logger.LogInformation("Connected to MessageBus");
            }
            catch (Exception ex)
            {
                logger.LogError ($"Could not connect to the Message Bus: {ex.Message}");
            }
        }
        public void PublishUserEvent(BaseEvent @event)
        {
            var exchangeName = "user_exchange";
            var routingKey = "user." + @event.GetType().Name;

            PublishEvent(@event, routingKey, exchangeName);
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
                Converters = { new TypeConverter() },
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
}