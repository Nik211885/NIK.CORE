using NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;

namespace NIK.CORE.DOMAIN.Pipelines.PipesMediator;
public class ModelStateValidationPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public ValueTask<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
