namespace VMAdmin;

public class AuthSettings
{
public bool UseSdkDefaultAuthenticationEnvironment { get; set; }
public string TenantId { get; set; } = null!;
public string ClientId { get; set; } = null!;
public string ClientSecret { get; set; } = null!;
public string ClientCertificatePath { get; set; } = null!;
}