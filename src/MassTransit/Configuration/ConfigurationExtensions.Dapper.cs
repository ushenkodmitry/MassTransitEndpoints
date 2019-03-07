using System;
using Configuration.PipeConfigurators;
using GreenPipes;

namespace Configuration
{
    partial class ConfigurationExtensions
    {
        public static void UseDapper<TContext>(this IPipeConfigurator<TContext> configurator) 
            where TContext : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.AddPipeSpecification(new DapperPipeSpecification<TContext>());
        }
    }
}