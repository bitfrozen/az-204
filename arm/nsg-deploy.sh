#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup default values
declare environment="DEV"
declare template_filename="nsg-template.json"

function usage {
     echo "Usage $0 [-h] [-e environment default=$environment] [-f template-filename default=$template_filename]" 1>&2; exit 1;
}

while getopts ":he:f:" opt; do
  case ${opt} in
    h ) usage; exit;;
    e ) environment="$OPTARG"
      ;;
    f ) template_filename="$OPTARG"
      ;;
    \? ) echo "Unknown option: -$OPTARG" >&2; exit 1;;
    :  ) echo "Missing option argument for -$OPTARG" >&2; exit 1;;
    *  ) echo "Unimplemented option: -$OPTION" >&2; exit 1;;
  esac
done
shift $((OPTIND - 1))

./deploy.sh -e "$environment" -f "$template_filename"
