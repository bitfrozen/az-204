#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup script environment
. env.sh

echo ""
echo "Starting creating infrastructure..."
(
  set -x
  # Create resource groups and dashboard
  az group create -n $RG_DEV -l $LOCATION --tags project=iac env=dev description="resources for dev environment" --output table && \
  az group create -n $RG_PROD -l $LOCATION --tags project=iac env=prod description="resources for production environment" --output table && \
  az group create -n $RG_DASH -l $LOCATION --tags project=iac description="container for shared dashboards" --output table && \
  ./dashboard-deploy.sh
)

if [ $? == 0 ]; then
  echo ""
  echo "Infrastructure has been successfully created"
fi

echo ""
echo "Starting template deployment ..."
(
  set -x
  ./full-validate.sh -e "dev" && \
  ./full-deploy.sh -e "dev"
)

if [ $? == 0 ]; then
  echo "Templates has been successfully deployed"
fi
