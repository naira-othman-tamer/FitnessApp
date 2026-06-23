using Autofac;
using FCE.Infrastructure;
using FluentValidation;
using System.Reflection;
using Module = Autofac.Module;

namespace FCE.Configs
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GeneralRepository<>))
                   .As(typeof(GeneralRepository<>))
                   .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                  .As<UnitOfWork>()
                  .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .AsClosedTypesOf(typeof(IValidator<>))
                   .AsImplementedInterfaces();

        }
    }
}
