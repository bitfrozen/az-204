#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup script environment
. env.sh

echo ""
echo "Starting to destroy infrastructure..."
(
  set -x
  # Cleanup created groups
  az group list --query "[? contains(name,'iac')]".name -o tsv | xargs -ortl az group delete -y --no-wait -n
)

if [ $? == 0 ]; then
  echo ""
  echo "Infrastructure successfully destroyed"
fi
