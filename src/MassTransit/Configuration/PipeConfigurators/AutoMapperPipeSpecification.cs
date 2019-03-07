using AutoMapper;
using GreenPipes;
using GreenPipes.Validation;
using Pipeline.Filters;
using System;
using System.Collections.Generic;

namespace Configuration.PipeConfigurators
{
    public sealed class AutoMapperPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly AutoMapperConfigurator _autoMapperConfigurator;

        public AutoMapperPipeSpecification(Action<IAutoMapperConfigurator> configureAutoMapper)
        {
            AutoMapperConfigurator autoMapperConfigurator = new AutoMapperConfigurator();
            configureAutoMapper(autoMapperConfigurator);
            _autoMapperConfigurator = autoMapperConfigurator;
        }

        void IPipeSpecification<TContext>.Apply(IPipeBuilder<TContext> builder)
        {
            var mapper = _autoMapperConfigurator.MapperConfiguration.CreateMapper();

            AutoMapperFilter<TContext> filter = new AutoMapperFilter<TContext>(mapper);

            builder.AddFilter(filter);
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (!_autoMapperConfigurator.ValidateConfiguration) yield break;

            bool isValid = true;

            try { _autoMapperConfigurator.MapperConfiguration.AssertConfigurationIsValid(); }
            catch (AutoMapperConfigurationException) { isValid = false; }

            if (!isValid)
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Warning, "AutoMapper", "Configuration invalid");
        }
    }
}
