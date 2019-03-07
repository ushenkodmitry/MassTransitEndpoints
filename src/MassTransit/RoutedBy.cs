namespace MassTransit
{
    public interface RoutedBy<out T>
    {
        T RoutingKey { get; }
    }
}
