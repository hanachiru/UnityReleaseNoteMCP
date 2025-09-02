using System;

namespace UnityReleaseNoteMCP.Domain;

public class ToolExecutionException : Exception
{
    public ToolExecutionException()
    {
    }

    public ToolExecutionException(string message)
        : base(message)
    {
    }

    public ToolExecutionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
