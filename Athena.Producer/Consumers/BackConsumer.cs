using Athena.Domain;
using Athena.Producer.Configurations;
using Athena.Producer.Producers;
using MessagePack;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Athena.Producer.Consumers;

public class BackConsumer : BackgroundService
{
    private readonly EventBusOptions _options;
    private readonly BackProducer _producer;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public BackConsumer(EventBusOptions options, BackProducer producer)
    {
        _options = options;
        var factory = new ConnectionFactory()
        {
            HostName = _options.ConnectionString
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (int i = 0; i < 100; i++)
        {
            _producer.Send(new Funcionario()
            {
                Id = Guid.NewGuid(),
                Nome = $"Nome_{i}",
                Funcao = "Funcao",
                Admissao = DateTime.Now
            });
        }
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += Consumer_Received;
        _channel.BasicConsume(queue: _options.FuncionarioCreatedQueue.Name,
            autoAck: false,
        consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine(
                $"Worker ativo em: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private void Consumer_Received(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            EventType<Funcionario> mc2 = MessagePackSerializer.Deserialize<EventType<Funcionario>>(ea.Body,
                  MessagePack.Resolvers.ContractlessStandardResolver.Options);
            Console.WriteLine(
                $"[Nova mensagem | {DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n | Id | {mc2.Event.Id}" +
                $"\n | Nome | {mc2.Event.Nome}" +
                $"\n | Funcao | {mc2.Event.Funcao}");
        }
        catch (Exception ex)
        {
            _channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
            Console.WriteLine(ex);
            return;
        }
        try
        {
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            Console.WriteLine(ex);
        }
    }
}