using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

namespace NIK.CORE.DOMAIN.Implements.CommandQueryResponsibilitySegregation;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public ValueTask Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var handle = _serviceProvider.GetService<IBaseHandler<ICommand, UnitType>>();
        ArgumentNullException.ThrowIfNull(handle);
        var pipeLineBehavior = _serviceProvider.GetServices<IPipelineBehavior<ICommand, UnitType>>().Reverse();
        RequestHandlerDelegate<UnitType> next = () => handle.Handle(command, cancellationToken);
    }

    public ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}