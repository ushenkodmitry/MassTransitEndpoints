using MassTransit;

namespace Contexts
{
    public interface AutoMapperContext
    {
        TDestination Map<TDestination>(object source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination target);

        TDestination Map<TSource1, TSource2, TDestination>(TSource1 source1,
            TSource2 source2, TDestination destination);

        TDestination Map<TSource1, TSource2, TSource3, TDestination>(TSource1 source1,
            TSource2 source2, TSource3 source3, TDestination destination);
    }
}