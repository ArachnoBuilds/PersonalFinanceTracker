using Components.Shared.Models;

namespace Components.Shared;

public class AppStateManager(ICacheService cacheSvc)
{
    private const string AppStateCacheKey = "app-state";
    public AppState State
    {
        get => field ?? throw new NullReferenceException("Application state isn't initialized");
        private set => field = value;
    }
    public bool IsInitialized { get; private set; } = false;
    public event Action<int>? OnYearChanged;
    public event Func<int, Task>? OnYearChangedAsync;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        var state = await cacheSvc.GetAsync<AppState>(AppStateCacheKey);
        if (state == null)
        {
            state = new(true, DateTime.UtcNow.Year);
            await cacheSvc.SetAsync(AppStateCacheKey, state);
        }
        State = state;
        IsInitialized = true;
    }

    public async Task SetYearAsync(int year)
    {
        // update
        State = State with { Year = year };

        // persist
        // TODO check if set happend correctly, take required actions if not
        _ = await cacheSvc.SetAsync(AppStateCacheKey, State);

        // invoke
        OnYearChanged?.Invoke(year);
        OnYearChangedAsync?.Invoke(year);
    }

    public async Task SetLeftSidebarExpandedAsync(bool expanded)
    {
        // update
        State = State with { LeftSidebarExpanded = expanded };

        // persist
        // TODO check if set happend correctly, take required actions if not
        _ = await cacheSvc.SetAsync(AppStateCacheKey, State);
    }
}
