using Athena.Domain;
using Athena.Producer.Configurations;
using MessagePack;
using RabbitMQ.Client;

namespace Athena.Producer.Producers;

public class BackProducer : IDisposable
{
    private readonly EventBusOptions _options;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public BackProducer(EventBusOptions options)
    {
        _options = options;
        var factory = new ConnectionFactory()
        {
            HostName = _options.ConnectionString
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

    public void Send(Funcionario funcionario)
    {
        try
        {
            _channel.QueueDeclare(queue: _options.FuncionarioCreatedQueue.Name,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var evento = EventTypeBuilder.Build(funcionario);
            byte[] bytes = MessagePackSerializer.Serialize(evento,
              MessagePack.Resolvers.ContractlessStandardResolver.Options);
            //string message = System.Text.Json.JsonSerializer.Serialize(funcionario);
            //byte[] bytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "",
                                 routingKey: _options.FuncionarioCreatedQueue.Name,
                                 basicProperties: null,
                                 body: bytes);
            Console.WriteLine(
                $"[Mensagem enviada] {funcionario.Nome}");

            Console.WriteLine("Concluido o envio de mensagens");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção: {ex.GetType().FullName} | " +
                         $"Mensagem: {ex.Message}");
        }
    }
}