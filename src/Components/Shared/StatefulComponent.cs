using Components.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Components.Shared;

public class StatefulComponent: ComponentBase
{
    [Inject]
    public required AppStateManager AppStateManager { get; init; }
    public AppState State => AppStateManager.State;

    protected override async Task OnInitializedAsync()
    {
        if (AppStateManager.IsInitialized)
            return;
        await AppStateManager.InitializeAsync().ConfigureAwait(false);
    }
}
