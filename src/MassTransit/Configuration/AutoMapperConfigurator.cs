using AutoMapper;
using System;

namespace Configuration
{
    sealed class AutoMapperConfigurator : IAutoMapperConfigurator
    {
        public MapperConfiguration MapperConfiguration { get; private set; }

        public bool ValidateConfiguration { get; set; }

        public void ConfigureMapper(Action<IMapperConfigurationExpression> expression)
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(expression);

            MapperConfiguration = mapperConfiguration;
        }
    }
}
