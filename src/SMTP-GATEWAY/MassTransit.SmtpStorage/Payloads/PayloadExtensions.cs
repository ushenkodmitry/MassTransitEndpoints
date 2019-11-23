using GreenPipes;

namespace MassTransit.Payloads
{
    static class PayloadExtensions
    {
        public static CreatedId<TModel, T> AddCreatedId<TModel, T>(this PipeContext context)
            where T : struct => context.GetOrAddPayload(() => new CreatedId<TModel, T>());

        public static bool TryGetCreatedId<TModel, T>(this PipeContext context, out CreatedId<TModel, T> createdId)
            where T : struct => context.TryGetPayload(out createdId);
    }
}
