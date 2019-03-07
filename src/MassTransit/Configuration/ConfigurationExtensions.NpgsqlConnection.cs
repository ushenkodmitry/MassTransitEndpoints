using System;
using Configuration.PipeConfigurators;
using GreenPipes;

namespace Configuration
{
    partial class ConfigurationExtensions
    {
        public static void UseNpgsqlConnection<TContext>(this IPipeConfigurator<TContext> configurator,
            string connectionString, bool enlistTransaction = false) 
            where TContext : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.AddPipeSpecification(new NpgsqlConnectionPipeSpecification<TContext>(connectionString, enlistTransaction));
        }
    }
}