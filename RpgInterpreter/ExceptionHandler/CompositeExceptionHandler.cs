using System.Collections.Immutable;

namespace RpgInterpreter.ExceptionHandler;

public class CompositeExceptionHandler : IExceptionHandler
{
    private readonly IImmutableList<IExceptionHandler> _handlers;
    public CompositeExceptionHandler(IImmutableList<IExceptionHandler> handlers) => _handlers = handlers;

    public void RunAndHandle(Action action)
    {
        _handlers.Aggregate(action, (action1, handler) => () => handler.RunAndHandle(action1))();
    }
}