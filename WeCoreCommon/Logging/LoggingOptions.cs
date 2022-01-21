namespace WeCoreCommon.Logging;
public sealed class LoggingOptions
{
    public const string LogginSectionName = "welogging";

    public bool UseElapsedTime { get; set; }

    public string LogLevel { get; set; }

    public char Separator { get; set; }

    public int SeparatorLength { get; set; } = 50;
}

