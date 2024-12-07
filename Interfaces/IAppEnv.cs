namespace ClassLibrary.Interfaces;
public interface IAppEnv
{   
    string EnvName {get; set;}
    bool IsRunningInContainer {get; set;}
}