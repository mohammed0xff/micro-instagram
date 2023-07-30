using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.Common;
using NotificationService.EventProcessing;
using NotificationService.EventSubscriber;


namespace MyNamespace
{
    /// <summary>
    /// just wanted to quickly test DeserializeEvent method, had to make it public  
    /// and this class has a lot to it that needed to be initialized. 
    /// angry methods ?? COMMENT ALL.
    /// </summary>
    public class MessageBusSubscriberTest
    {
        private readonly MessageBusSubscriber _subscriber;
        private readonly Mock<IEventProcessor> _eventProcessorMock;
        private readonly Mock<ILogger<MessageBusSubscriber>> _loggerMock;

        public MessageBusSubscriberTest()
        {

            _eventProcessorMock = new Mock<IEventProcessor>();
            _loggerMock = new Mock<ILogger<MessageBusSubscriber>>();
            var rabbitMQSettings = new Mock<IOptions<RabbitMQSettings>>();
            rabbitMQSettings.Setup(x => x.Value).Returns(new RabbitMQSettings());

            _subscriber = new MessageBusSubscriber(rabbitMQSettings.Object, _eventProcessorMock.Object, _loggerMock.Object);
        }

        /*        [Fact]
                public void DeserializeEvent_ReturnsCorrectEventType()
                {
                    // Arrange
                    var eventMessage = "{\"FollowerId\":\"c2359443-1f3b-41b0-b4fd-3617899a3bc2\",\"FollowedId\":\"f5a1c48c-10a7-4718-8186-8207c96e13d8\",\"FollowerUsername\":\"User1\",\"Id\":\"fccac826-7145-4af3-b076-f4b8ea32634c\",\"CreationDate\":\"2023-07-30T01:27:11.5786454+03:00\"}";
                    var routingKey = "follow.FollowCreatedEvent";
                    var expectedEventType = typeof(FollowCreatedEvent);

                    // Act
                    var result = _subscriber.DeserializeEvent(eventMessage, routingKey);

                    // Assert
                    Assert.IsType(expectedEventType, result);
                }

                [Fact]
                public void DeserializeEvent_ThrowsArgumentException_WhenRoutingKeyIsNull()
                {
                    // Arrange
                    var eventMessage = "{\"name\":\"John\",\"age\":30}";
                    string routingKey = null;

                    // Act & Assert
                    var ex = Assert.Throws<ArgumentNullException>(
                        () => _subscriber.DeserializeEvent(eventMessage, routingKey)
                        );
                }

                [Fact]
                public void DeserializeEvent_ThrowsArgumentException_WhenRoutingKeyIsInvalid()
                {
                    // Arrange
                    var eventMessage = "{\"name\":\"John\",\"age\":30}";
                    string routingKey = "invalid-routingkey";

                    // Act & Assert
                    var ex = Assert.Throws<ArgumentException>(
                        () => _subscriber.DeserializeEvent(eventMessage, routingKey)
                        );
                    Assert.Equal(ex.Message, $"Invalid routing key: {routingKey}");
                }

                [Fact]
                public void DeserializeEvent_ThrowsArgumentException_WhenInvalidEventType()
                {
                    // Arrange
                    var eventMessage = "{\"name\":\"John\",\"age\":30}";
                    string routingKey = "follow.FollowTweekedEvent";

                    // Act & Assert
                    var ex = Assert.Throws<ArgumentException>(
                        () => _subscriber.DeserializeEvent(eventMessage, routingKey)
                        );
                    Assert.Equal(ex.Message, $"Invalid event type: FollowTweekedEvent");
                }
        */
    }
}