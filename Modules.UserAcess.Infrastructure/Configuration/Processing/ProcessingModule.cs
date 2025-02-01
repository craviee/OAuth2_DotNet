// using BuildingBlocks.Application.Events;
// using BuildingBlocks.Infrastructure;
// using BuildingBlocks.Infrastructure.DomainEventsDispatching;
// using MediatR;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using Scrutor;
//
// namespace Modules.UserAcess.Infrastructure.Configuration.Processing;
//
// internal class ProcessingModule
// {
//     public static void Register(ServiceCollection services)
//     {
//         services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
//         services.AddScoped<IDomainNotificationsMapper, DomainNotificationsMapper>();
//         services.AddScoped<IDomainEventsAccessor, DomainEventsAccessor>();
//         services.AddScoped<IUnitOfWork, UnitOfWork>();
//         services.AddScoped<ICommandsScheduler, CommandsScheduler>();
//         
//         services.AddTransient(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
//         services.AddTransient(typeof(ICommandHandler<,>), typeof(UnitOfWorkCommandHandlerWithResultDecorator<,>));
//         services.AddTransient(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
//         services.AddTransient(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerWithResultDecorator<,>));
//         services.AddTransient(typeof(IRequestHandler<>), typeof(LoggingCommandHandlerDecorator<>));
//         services.AddTransient(typeof(IRequestHandler<,>), typeof(LoggingCommandHandlerWithResultDecorator<,>));
//         services.AddTransient(typeof(INotificationHandler<>), typeof(DomainEventsDispatcherNotificationHandlerDecorator<>));
//
//         services.Scan(scan => scan
//             .FromAssemblies(Assemblies.Application)
//             .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventNotification<>)))
//             .AsImplementedInterfaces()
//             .WithTransientLifetime());
//     }
// }