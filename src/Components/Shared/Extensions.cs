namespace Components.Shared;

internal static class Extensions
{
    extension(decimal price)
    {
        public string ToCurrency() => price.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-IN"));
    }
}
