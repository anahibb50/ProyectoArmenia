namespace LigaArmenia.Domain.Rules;

public static class ReglasJugador
{
    public const int EdadMayor = 50;
    public const int MinimoDocumento = 5;
    public const int MaximoDocumento = 35;
    public const int MaximoNombres = 180;

    public static DateOnly Hoy()
    {
        var utc = DateTime.UtcNow;
        if (TimeZoneInfo.TryFindSystemTimeZoneById("America/Guayaquil", out var zona)
            || TimeZoneInfo.TryFindSystemTimeZoneById("SA Pacific Standard Time", out zona))
        {
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc, zona));
        }

        return DateOnly.FromDateTime(utc.AddHours(-5));
    }

    public static int? Edad(DateOnly? nacimiento, DateOnly hoy)
    {
        if (nacimiento is null || nacimiento.Value > hoy)
            return null;

        var edad = hoy.Year - nacimiento.Value.Year;
        if (nacimiento.Value > hoy.AddYears(-edad))
            edad--;

        return edad;
    }

    public static bool EsMayorDe50(DateOnly? nacimiento, DateOnly hoy)
    {
        return Edad(nacimiento, hoy) is int edad && edad >= EdadMayor;
    }

    public static string? NormalizarDocumento(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return null;

        var limpio = new string(documento.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
        if (limpio.Length is < MinimoDocumento or > MaximoDocumento)
            return null;

        if (limpio.Any(c => !char.IsLetterOrDigit(c) && c != '-'))
            return null;

        return limpio;
    }

    public static string? NormalizarNombres(string? nombres)
    {
        if (string.IsNullOrWhiteSpace(nombres))
            return null;

        var partes = nombres.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var limpio = string.Join(' ', partes);
        if (limpio.Length is 0 or > MaximoNombres)
            return null;

        return limpio;
    }
}
