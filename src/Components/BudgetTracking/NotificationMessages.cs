namespace Components.BudgetTracking;

internal static class NotificationMessages
{
    public const string TransactionCreationFailed = "Failed to create transaction entry.";
    public const string TransactionCreationSuccess = "Transaction entry created successfully.";
    public const string TransactionFetchFailed = "Failed to fetch transaction data.";
    public const string TransactionMetaDataFetchFailed = "Failed to fetch transaction meta-data.";
    public const string TransactionUpdationFailed = "Failed to update transaction entry.";
    public const string TransactionUpdationSuccess = "Transaction entry updated successfully.";
    public const string TransactionDeletionFailed = "Failed to delete transaction entry.";
    public const string TransactionDeletionSuccess = "Transaction entry deleted successfully.";
    public const string TransactionCreateInitializationFailed = "Failed to initiate transaction creation.";
    public const string TransactionUpdateInitializationFailed = "Failed to initiate transaction updation.";
    public const string TransactionDeleteInitializationFailed = "Failed to initiate transaction deletion.";
    public const string BudgetFetchFailed = "Failed to fetch budgets";
    public const string BudgetNotSelected = "Please select a budget for the transaction.";
    public const string AccountFetchFailed = "Failed to fetch accounts";
}
