using System.Diagnostics;

public class DiagnosticObserver : IObserver<DiagnosticListener>, IDisposable
{
    private readonly List<IDisposable> _subscriptions = new();
    private readonly ActivitySource _activitySource = new("Catalog.API");

    public void Start() => DiagnosticListener.AllListeners.Subscribe(this);

    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.AspNetCore" ||
            listener.Name == "MediatR")
        {
            var subscription = listener.Subscribe(new KeyValueObserver(_activitySource));
            _subscriptions.Add(subscription);
        }
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
        _subscriptions.Clear();
        _activitySource.Dispose();
    }

    private class KeyValueObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly ActivitySource _activitySource;

        public KeyValueObserver(ActivitySource activitySource)
        {
            _activitySource = activitySource;
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(KeyValuePair<string, object> kvp)
        {
            // Вивід діагностичної інформації в консоль
            Console.WriteLine($"[TRACE] {kvp.Key}: {kvp.Value}");
        }
    }
}