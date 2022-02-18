# Usage

```bash
# To create infrastructure (and deploy development environment)
./create_infrastructure.sh

# To destroy infrastructure
./destroy_infractructure.sh
```

```bash
# To validate all templates
./full-validate.sh [-e dev|prod default=dev]

# To deploy all templates
./full-deploy.sh [-e dev|prod default=dev]
```

```bash
# To validate particular template
./nsg-validate.sh [-e dev|prod default=dev] [-t template-filename default=nsg-template.json] [-p parameter-filename default=parameters-dev.json]

# To deploy particular template
./nsg-deploy.sh [-e dev|prod default=dev] [-t template-filename default=nsg-template.json] [-p parameter-filename default=parameters-dev.json]
```

# Files

- `env.sh` various constants declared as environment variables to use in scripts
- `deploy.sh` base deployment script. Other deployment scripts define default values and reference this script for deployment task
- `validate.sh` base validate script. Other validate scripts define default values and reference this script for validation task
- `*-template.json` ARM template files
- `parameters-*.json` ARM parameter files with parameters which are shared between all templates
- `*-template.parameters-*.json` ARM parameter files with parameters used only in this template
- `*.azcli` scratch file used with `Azure CLI tools` extension inside VS Code. This gives faster intellisense while exploring `az` CLI than bash completion.