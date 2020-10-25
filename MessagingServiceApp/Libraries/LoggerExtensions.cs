using System;
using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static void LogError(this ILogger logger, Exception ex, string message = null, params object[] args)
    {
        logger.LogError(default(EventId), ex, message, args);
    }
    public static void LogInfo(this ILogger logger, string message = null, params object[] args)
    {
        logger.LogInformation(message, args);
    }
}