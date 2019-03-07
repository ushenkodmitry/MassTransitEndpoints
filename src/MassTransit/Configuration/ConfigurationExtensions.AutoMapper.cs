using Configuration.PipeConfigurators;
using GreenPipes;
using System;

namespace Configuration
{
    static partial class ConfigurationExtensions
    {
        public static void UseAutoMapper<TContext>(this IPipeConfigurator<TContext> configurator, Action<IAutoMapperConfigurator> configureAutoMapper)
            where TContext : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.AddPipeSpecification(new AutoMapperPipeSpecification<TContext>(configureAutoMapper));
        }
    }
}