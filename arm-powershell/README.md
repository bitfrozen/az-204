# Usage

```powershell
# To create infrastructure
./create_infrastructure.ps1

# To destroy infrastructure
./remove_infrastructure.ps1
```

```powershell
# To deploy linked templates
./deploy.ps1 [-Environment dev|prod default=dev]
```

# Files

- `env.ps1` various constants declared as environment variables to use in scripts
- `deploy.ps1` deployment script
