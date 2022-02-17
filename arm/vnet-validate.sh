#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup default values
declare environment="dev"
declare template_filename="vnet-template.json"
declare parameter_filename=

function parameter_filename() {
  if [[ -z "$parameter_filename" ]]; then
    echo "parameters-$environment.json"
  else
    echo $parameter_filename
  fi
}

function usage {
     echo "Usage $0 [-h] [-e environment default=$environment] [-t template-filename default=$template_filename] [-p parameter-filename default=$(parameter_filename)]" 1>&2; exit 1;
}

while getopts ":he:t:p:" opt; do
  case ${opt} in
    h ) usage; exit;;
    e ) environment="$OPTARG"
      ;;
    t ) template_filename="$OPTARG"
      ;;
    p ) parameter_filename="$OPTARG"
      ;;
    \? ) echo "Unknown option: -$OPTARG" >&2; exit 1;;
    :  ) echo "Missing option argument for -$OPTARG" >&2; exit 1;;
    *  ) echo "Unimplemented option: -$OPTION" >&2; exit 1;;
  esac
done
shift $((OPTIND - 1))

./validate.sh -e "$environment" -t "$template_filename" -p "$(parameter_filename)"
