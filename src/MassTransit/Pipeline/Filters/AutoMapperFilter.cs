using AutoMapper;
using Contexts;
using GreenPipes;
using System.Threading.Tasks;

namespace Pipeline.Filters
{
    public sealed class AutoMapperFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IMapper _mapper;

        public AutoMapperFilter(IMapper mapper) => _mapper = mapper;

        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            AutoMapperContext autoMapperContext = new ConsumeAutoMapperContext(_mapper);

            context.GetOrAddPayload(() => autoMapperContext);

            return next.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(AutoMapperFilter<TContext>));
        }

        sealed class ConsumeAutoMapperContext : AutoMapperContext
        {
            readonly IMapper _mapper;

            public ConsumeAutoMapperContext(IMapper mapper) => _mapper = mapper;

            public TDestination Map<TDestination>(object source) => _mapper.Map<TDestination>(source);

            public TDestination Map<TSource, TDestination>(TSource source, TDestination target) => _mapper.Map(source, target);

            public TDestination Map<TSource1, TSource2, TDestination>(TSource1 source1, TSource2 source2,
                TDestination destination)
            {
                destination = Map(source1, destination);
                destination = Map(source2, destination);

                return destination;
            }

            public TDestination Map<TSource1, TSource2, TSource3, TDestination>(TSource1 source1, TSource2 source2,
                TSource3 source3, TDestination destination)
            {
                destination = Map(source1, source2, destination);
                destination = Map(source3, destination);

                return destination;
            }
        }
    }
}
