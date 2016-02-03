using UnityEngine;
using System.Collections;

public static class Logger
{
    public enum LogType
    {
        UnityOnly = -1,
        Normal = 0,
        Warning = 1,
        Error = 2,
    }

    public delegate void LogHandler(string log, LogType logType);

    public static LogHandler OnLog;

    public static void Log(string message)
    {
        Debug.Log(message);
        OnLog(message, LogType.Normal);
    }

    public static void Log(string message, params object[] args)
    {
        Debug.LogFormat(message, args);
        OnLog(string.Format(message, args), LogType.Normal);
    }

    public static void LogErrorFormat(string message, params object[] args)
    {
        Debug.LogErrorFormat(message, args);
        OnLog(string.Format(message, args), LogType.Error);
    }
}
