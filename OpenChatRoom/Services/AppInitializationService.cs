public class AppInitializationService
{
    private bool _initialized = false;

    public bool IsInitialized => _initialized;

    public event Func<Task>? OnInitialized;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;

        if (OnInitialized != null)
            await OnInitialized.Invoke();
    }
}
