using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class CustomDebug
{
    [Conditional("DEBUG")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [Conditional("DEBUG")]
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    [Conditional("DEBUG")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
        Debug.Assert(condition);
    }

    [Conditional("DEBUG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        Debug.DrawLine(start, end, color, duration);
    }

    [Conditional("DEBUG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        Debug.DrawRay(start, dir, color, duration);
    }

    [Conditional("DEBUG")]
    public static void Break()
    {
        Debug.Break();
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition, string message)
    {
        Debug.Assert(condition, message);
    }
}

