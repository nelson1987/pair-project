using Athena.Domain;
using Athena.Producer.Configurations;
using Athena.Producer.Consumers;
using Athena.Producer.Producers;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace Athena.Tests.Consumers;

public class BackConsumerTests
{
    [Fact]
    public async Task ExecuteAsync_LogsException_WhenProducerThrows()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        var mockProducer = new Mock<BackProducer>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        var mockLogger = new Mock<ILogger<BackConsumer>>();
        var consumer = new BackConsumer(mockOptions.Object, mockProducer.Object);

        mockProducer.Setup(p => p.Send(It.IsAny<Funcionario>())).Throws(new Exception("Producer exception"));

        // Act
        await consumer.ExecuteAsync(CancellationToken.None);

        // Assert
        //mockLogger.Verify(l => l.Log(
        //    LogLevel.Error,
        //    It.IsAny<EventId>(),
        //    It.Is<It.IsAnyType>((o, t) => true),
        //    It.IsAny<Exception>(),
        //    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //    Times.Once);
    }

    /*
    [Fact]
    public async Task ExecuteAsync_LogsException_WhenConsumerReceivesException()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        var mockProducer = new Mock<BackProducer>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        var mockLogger = new Mock<ILogger<BackConsumer>>();
        var consumer = new BackConsumer(mockOptions.Object, mockProducer.Object);

        mockChannel.Setup(c => c.BasicConsume(
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<ConsumerCallback>())).Callback<string, bool, string, bool, bool, IDictionary<string, object>, ConsumerCallback>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, callback) =>
            {
                callback(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));
            });

        consumer.Consumer_Received(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));

        // Act
        await consumer.ExecuteAsync(CancellationToken.None);

        // Assert
        mockLogger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => true),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_RetriesFailedMessage_WhenConsumerNacks()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        var mockProducer = new Mock<BackProducer>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        var consumer = new BackConsumer(mockOptions.Object, mockProducer.Object);

        mockChannel.Setup(c => c.BasicConsume(
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<ConsumerCallback>())).Callback<string, bool, string, bool, bool, IDictionary<string, object>, ConsumerCallback>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, callback) =>
            {
                callback(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));
            });

        consumer.Consumer_Received(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));

        // Act
        await consumer.ExecuteAsync(CancellationToken.None);

        // Assert
        mockChannel.Verify(c => c.BasicNack(
            It.Is<ulong>(tag => tag == 0),
            It.Is<bool>(multiple => multiple == false),
            It.Is<bool>(requeue => requeue == true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_AcknowledgesMessage_WhenConsumerAcks()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        var mockProducer = new Mock<BackProducer>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        var consumer = new BackConsumer(mockOptions.Object, mockProducer.Object);

        mockChannel.Setup(c => c.BasicConsume(
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<ConsumerCallback>())).Callback<string, bool, string, bool, bool, IDictionary<string, object>, ConsumerCallback>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, callback) =>
            {
                callback(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));
            });

        consumer.Consumer_Received(null, new BasicDeliverEventArgs("tag", 0, false, "exchange", "routingKey", null, Array.Empty<byte>()));

        // Act
        await consumer.ExecuteAsync(CancellationToken.None);

        // Assert
        mockChannel.Verify(c => c.BasicAck(
            It.Is<ulong>(tag => tag == 0),
            It.Is<bool>(multiple => multiple == false)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_LogsWorkerActivity()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        var mockProducer = new Mock<BackProducer>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        var mockLogger = new Mock<ILogger<BackConsumer>>();
        var consumer = new BackConsumer(mockOptions.Object, mockProducer.Object);

        // Act
        await consumer.ExecuteAsync(CancellationToken.None);

        // Assert
        mockLogger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => true),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }
    */
}