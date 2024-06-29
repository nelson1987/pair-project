using Athena.Producer.Configurations;
using Athena.Producer.Consumers;
using Athena.Producer.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<EventBusOptions>(
                new EventBusOptions()
                {
                    ConnectionString = "localhost",
                    FuncionarioCreatedQueue = new FuncionarioCreatedQueue()
                    {
                        Name = "teste_1"
                    }
                });
            services.AddSingleton<BackProducer>();
            services.AddHostedService<BackConsumer>();
        });