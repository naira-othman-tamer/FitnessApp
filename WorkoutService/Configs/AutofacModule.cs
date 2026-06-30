using Autofac;
using WorkoutService.Features.Common.Pipeline;
using WorkoutService.Infrastructure;
using FluentValidation;
using MediatR;
using System.Reflection;
using Module = Autofac.Module;

namespace WorkoutService.Configs
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

            #region MediatR Pipeline
            builder.RegisterGeneric(typeof(ValidationBehavior<,>))
                   .As(typeof(IPipelineBehavior<,>))
                   .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(TransactionBehavior<,>))
                   .As(typeof(IPipelineBehavior<,>))
                   .InstancePerLifetimeScope();
            #endregion

        }
    }
}
