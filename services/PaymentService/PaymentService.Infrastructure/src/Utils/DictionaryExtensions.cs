using System.Data;

namespace PaymentService.Infrastructure.src.Utils;

public static class DbDictionaryExtensions
{
    /// <summary>
    ///     Converts column name to lower case and throws error if result is not of type T
    /// </summary>
    public static T GetColumn<T>(this Dictionary<string, object> dict, string columnName)
    {
        string col = columnName.ToLower();

        object? result;

        dict.TryGetValue(col, out result);

        if (result is DBNull) return default;

        if (result is not T)
            throw new DataException($@"Column '{col}' does not satisfy expected type.
                                        Expected type: {typeof(T)} Received type: {result?.GetType()}");

        return (T)result;
    }
}