namespace ClassLibrary.Interfaces;
public interface ISecretsProvider
{
    Task<string> GetSecretAsync(string secretName);
}
