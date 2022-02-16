# Usage

```bash
# To validate nsg template use
./nsg-validate.sh

# To deploy nsg use
./nsg-deploy.sh
```

```bash
# To validate vnet template use
./vnet-validate.sh

# To deploy vnet use
./vnet-deploy.sh
```

# Files

- `env.sh` various constants declared as environment variables to use in scripts
- `deploy.sh` main deployment script. Other deployment scripts define default values and reference this script for deployment task
- `validate.sh` main validate script. Other validate scripts define default values and reference this script for validation task
- `*-template.json` ARM template files
- `*.azcli` scratch file used with `Azure CLI tools` extension inside VS Code. This gives faster intellisense while exploring `az` CLI than bash completion.