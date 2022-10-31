namespace VMAdmin;

internal static class AuthEnvironment
{
	private const string TenantId = "AZURE_TENANT_ID";
	private const string ClientId = "AZURE_CLIENT_ID";
	private const string ClientSecret = "AZURE_CLIENT_SECRET";
	private const string ClientCertificate = "AZURE_CLIENT_CERTIFICATE_PATH";
	
	private static readonly Dictionary<string, string> OriginalEnvironmentVariables = InitAuthEnvironmentVariable();
	private static readonly Dictionary<string, string> ModifiedEnvironmentVariables = InitAuthEnvironmentVariable();

	static AuthEnvironment()
	{
		ReadAuthEnvironment();
	}

	private static void ReadAuthEnvironment()
	{
		foreach (string variable in OriginalEnvironmentVariables.Keys)
		{
			OriginalEnvironmentVariables[variable] = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process) ?? string.Empty;
		}
	}

	private static void WriteAuthEnvironment(Dictionary<string, string> environment)
	{
		foreach (string variable in environment.Keys)
		{
			Environment.SetEnvironmentVariable(variable, environment[variable], EnvironmentVariableTarget.Process);
		}
	}

	public static void WriteCustomAuthEnvironment()
	{
		WriteAuthEnvironment(ModifiedEnvironmentVariables);
	}
	
	public static void RestoreAuthEnvironment()
	{
		WriteAuthEnvironment(OriginalEnvironmentVariables);
	}
	
	private static Dictionary<string, string> InitAuthEnvironmentVariable()
	{
		return new Dictionary<string, string>
		{
			[TenantId] = "",
			[ClientId] = "",
			[ClientSecret] = "",
			[ClientCertificate] = ""
		};
	}

}