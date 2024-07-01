namespace Athena.Tests.Producers;

using Athena.Domain;
using Athena.Producer.Configurations;
using Athena.Producer.Producers;
using Moq;
using RabbitMQ.Client;
using Xunit;

public class BackProducerTests
{
    [Fact]
    public void Send_ValidFuncionario_MessagePublishedSuccessfully()
    {
        // Arrange
        var mockOptions = new Mock<EventBusOptions>();
        mockOptions.Setup(x => x.ConnectionString).Returns("localhost");
        mockOptions.Setup(x => x.FuncionarioCreatedQueue.Name).Returns("funcionario_created_queue");

        var mockConnectionFactory = new Mock<ConnectionFactory>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();

        mockConnectionFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);
        mockConnection.Setup(x => x.CreateModel()).Returns(mockChannel.Object);

        var producer = new BackProducer(mockOptions.Object);
        var funcionario = new Funcionario { Id = Guid.NewGuid(), Nome = "John Doe" };

        // Act
        producer.Send(funcionario);

        // Assert
        mockChannel.Verify(x => x.QueueDeclare(
            It.Is<string>(q => q == "funcionario_created_queue"),
            It.Is<bool>(d => d == false),
            It.Is<bool>(e => e == false),
            It.Is<bool>(a => a == false),
            It.IsAny<IDictionary<string, object>>()), Times.Once);

        mockChannel.Verify(x => x.BasicPublish(
            It.Is<string>(e => e == ""),
            It.Is<string>(r => r == "funcionario_created_queue"),
            It.IsAny<IBasicProperties>(),
            It.IsAny<byte[]>()), Times.Once);

        Console.WriteLine("[Test Passed] Send_ValidFuncionario_MessagePublishedSuccessfully");
    }

    [Fact]
    public void Test_Send_Throws_When_Connection_Is_Null()
    {
        // Arrange
        var options = new EventBusOptions { ConnectionString = "localhost" };
        var producer = new BackProducer(options)
        {
            _connection = null
        };
        var funcionario = new Funcionario { Nome = "Teste" };

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => producer.Send(funcionario));
    }

    [Fact]
    public void Test_Send_Throws_When_Channel_Is_Null()
    {
        // Arrange
        var options = new EventBusOptions { ConnectionString = "localhost" };
        var producer = new BackProducer(options)
        {
            _connection = new Mock<IConnection>().Object,
            _channel = null
        };
        var funcionario = new Funcionario { Nome = "Teste" };

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => producer.Send(funcionario));
    }

    [Fact]
    public void Test_Send_Throws_When_QueueDeclare_Throws()
    {
        // Arrange
        var options = new EventBusOptions { ConnectionString = "localhost" };
        var mockChannel = new Mock<IModel>();
        mockChannel.Setup(c => c.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                   .Throws(new Exception("QueueDeclare failed"));
        var producer = new BackProducer(options)
        {
            _connection = new Mock<IConnection>().Object,
            _channel = mockChannel.Object
        };
        var funcionario = new Funcionario { Nome = "Teste" };

        // Act & Assert
        Assert.Throws<Exception>(() => producer.Send(funcionario));
    }

    [Fact]
    public void Test_Send_Throws_When_BasicPublish_Throws()
    {
        // Arrange
        var options = new EventBusOptions { ConnectionString = "localhost" };
        var mockChannel = new Mock<IModel>();
        mockChannel.Setup(c => c.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IBasicProperties>(), It.IsAny<byte[]>()))
                   .Throws(new Exception("BasicPublish failed"));
        var producer = new BackProducer(options)
        {
            _connection = new Mock<IConnection>().Object,
            _channel = mockChannel.Object
        };
        var funcionario = new Funcionario { Nome = "Teste" };

        // Act & Assert
        Assert.Throws<Exception>(() => producer.Send(funcionario));
    }

    [Fact]
    public void Test_Send_DoesNotThrow_When_Message_Is_Published_Successfully()
    {
        // Arrange
        var options = new EventBusOptions { ConnectionString = "localhost" };
        var mockChannel = new Mock<IModel>();
        var producer = new BackProducer(options)
        {
            _connection = new Mock<IConnection>().Object,
            _channel = mockChannel.Object
        };
        var funcionario = new Funcionario { Nome = "Teste" };

        // Act & Assert
        Assert.DoesNotThrow(() => producer.Send(funcionario));
    }
}