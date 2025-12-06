namespace Components.Shared.Models;

public record AppState(
    bool LeftSidebarExpanded,
    int Year,
    bool ShiftLateIncomeStatus = true, // TODO remove default value
    int ShiftLateIncomeStartingDay = 20); // TODO remove default value
