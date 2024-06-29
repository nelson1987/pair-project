using MessagePack;

namespace Athena.Producer.Configurations;

[MessagePackObject]
public class EventType<T> where T : class
{
    [Key(0)]
    public DateTime Created { get; set; }

    [Key(1)]
    public T Event { get; set; }
}

public static class EventTypeBuilder
{
    public static EventType<T> Build<T>(T events) where T : class
    {
        return new EventType<T>()
        {
            Created = DateTime.Now,
            Event = events
        };
    }
}