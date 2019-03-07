using AutoMapper;
using System;

namespace Configuration
{
    public interface IAutoMapperConfigurator
    {
        bool ValidateConfiguration { get; set; }

        void ConfigureMapper(Action<IMapperConfigurationExpression> expression);
    }
}
