namespace ClassLibrary.BusinessObject;
public static class AppEnv
{
    public static string EnvName 
    {
        get => Environment.GetEnvironmentVariable("ENV_NAME") ?? "Development";
        set => throw new NotImplementedException("EnvName is read-only in this implementation.");
    }

    public static bool IsRunningInContainer 
    {
        get => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER").ToLower() == "true";
        set => throw new NotImplementedException("IsRunningInContainer is read-only in this implementation.");
    }
}