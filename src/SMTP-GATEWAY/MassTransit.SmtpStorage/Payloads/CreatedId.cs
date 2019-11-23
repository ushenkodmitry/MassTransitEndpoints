namespace MassTransit.Payloads
{
    public sealed class CreatedId<TModel, T>
        where T : struct
    {
        public T Id { get; set; }
    }
}
