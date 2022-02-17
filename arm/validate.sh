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
declare parameter_filename=
declare parameter_argument=

function usage {
  echo "Usage $0 [-h] -e <environment> -t <template-filename> [-p <parameter-filename>]" 1>&2
  exit 1
}

function check_parameters {
  if [[ -z "$parameter_filename" ]]; then
    return
  fi

  parameter_argument=(--parameters "${parameter_filename}")

  template_param_filename=$(basename -s .json "$template_filename")."$parameter_filename"
  if [[ ! -f "$template_param_filename" ]]; then
    return
  fi

  parameter_argument=(${parameter_argument[@]} --parameters "${template_param_filename}")
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
  p)
    parameter_filename="$OPTARG"
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

# Set recource group name depending on environment argument
# ${var^^} modifies value to UPPERCASE
# ${!var} indirect reference
resource_group_var="RG_${environment^^}"
resource_group="${!resource_group_var}"

# If parameter file specified, add parameter arguments to deployment
# Adding both global parameter file (specified by user) and template specific
check_parameters

# Perform validation
echo ""
echo "Validating $template_filename with resource group $resource_group"
echo "Starting validation..."
(
  set -x
  az deployment group validate --resource-group "${resource_group}" --template-file "${template_filename}" ${parameter_argument[@]} --output table
)

if [ $? == 0 ]; then
  echo "Template has been successfully validated"
fi
