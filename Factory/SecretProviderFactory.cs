using Microsoft.Extensions.Configuration;
using ClassLibrary.Interfaces;

public static class SecretsProviderFactory
{
    public static ISecretsProvider Create(string providerType, IConfiguration configuration)
    {
        return providerType switch
        {
            // "AWS" => new AwsSecretsProvider(new AmazonSecretsManagerClient()),
            // "Azure" => new AzureKeyVaultSecretsProvider(configuration["KeyVaultUrl"]),
            "Kubernetes" => new KubernetesSecretsProvider(configuration["KubernetesSecretsPath"]),
            _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
        };
    }
}
