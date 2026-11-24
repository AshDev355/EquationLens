using System.Data;
using System.Globalization;
using Dapper;

namespace EquationLens.Data;

/// <summary>SQLite stores dates as text. This stores UTC ISO-8601 and always reads back UTC.</summary>
public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
        => parameter.Value = value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);

    public override DateTime Parse(object value)
    {
        if (value is DateTime dt) return dt.ToUniversalTime();
        return DateTime.Parse(value.ToString()!, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }
}
