namespace Athena.Producer.Configurations;

public class EventBusOptions
{
    public string ConnectionString { get; set; }
    public FuncionarioCreatedQueue FuncionarioCreatedQueue { get; set; }
}

public class FuncionarioCreatedQueue
{
    public string Name { get; set; }
}