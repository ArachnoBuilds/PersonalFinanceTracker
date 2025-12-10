using Application.Schema.BudgetPlanning.Models;

namespace Components.Shared.Models;

public record AppState(
    int Year,
    bool LeftSidebarExpanded = true,
    Month Month = Month.Jan,
    bool ShiftLateIncomeStatus = true, // TODO remove default value
    int ShiftLateIncomeStartingDay = 20); // TODO remove default value
