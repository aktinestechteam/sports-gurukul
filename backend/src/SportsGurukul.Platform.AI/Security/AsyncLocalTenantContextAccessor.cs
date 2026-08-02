using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Security;

public class AsyncLocalTenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantContext?> _current = new();

    public TenantContext? Current => _current.Value;

    public IDisposable Push(TenantContext context)
    {
        var previous = _current.Value;
        _current.Value = context;
        return new Scope(() => _current.Value = previous);
    }

    private sealed class Scope(Action onDispose) : IDisposable
    {
        private Action? _onDispose = onDispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _onDispose, null)?.Invoke();
        }
    }
}
