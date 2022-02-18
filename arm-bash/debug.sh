#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup script environment
. env.sh

# Declare variables
declare environment=
declare template_filename=

function usage {
  echo "Usage $0 [-h] -e <environment> -t <template-filename>" 1>&2
  exit 1
}

# Gather arguments
while getopts ":he:t:p:" opt; do
  case ${opt} in
  h)
    usage
    exit
    ;;
  e)
    environment="$OPTARG"
    ;;
  t)
    template_filename="$OPTARG"
    ;;
  \?)
    echo "Unknown option: -$OPTARG" >&2
    exit 1
    ;;
  :)
    echo "Missing option argument for -$OPTARG" >&2
    exit 1
    ;;
  *)
    echo "Unimplemented option: -$OPTION" >&2
    exit 1
    ;;
  esac
done
shift $((OPTIND - 1))

# Check that all arguments have been specified
if [[ -z "$environment" || -z "$template_filename" ]]; then
  usage
fi

# Degenerate deployment name
timestamp=$(date --utc +%Y%m%d%H%M%SZ)
deployment_name=${template_filename%-*}-$timestamp

# Set recource group name depending on environment argument
# ${var^^} modifies value to UPPERCASE
# ${!var} indirect reference
resource_group_var="RG_${environment^^}"
resource_group="${!resource_group_var}"

# Perform deployment
echo "Deploying $template_filename to resource group $resource_group"
echo "Starting deployment..."
(
  set -x
  az deployment group create --name "$deployment_name" --resource-group "$resource_group" --template-file "$template_filename" --verbose
)

if [ $? == 0 ]; then
  echo "Template has been successfully deployed"
fi
