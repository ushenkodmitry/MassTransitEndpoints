namespace MassTransit.Payloads
{
    sealed class Identity<TModel, T>
    {
        public T Id { get; }

        public Identity(T id = default) => Id = id;
    }
}
