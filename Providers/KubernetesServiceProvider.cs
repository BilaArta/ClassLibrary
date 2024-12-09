using System.IO;
using ClassLibrary.Interfaces;
public class KubernetesSecretsProvider : ISecretsProvider
{
    private readonly string _secretsPath;

    public KubernetesSecretsProvider(string secretsPath = "/var/run/secrets/")
    {
        _secretsPath = secretsPath;
    }

    public Task<string> GetSecretAsync(string secretName)
    {
        var secretFilePath = Path.Combine(_secretsPath, secretName);

        if (!File.Exists(secretFilePath))
            throw new FileNotFoundException($"Secret file not found: {secretFilePath}");

        return Task.FromResult(File.ReadAllText(secretFilePath));
    }
}
