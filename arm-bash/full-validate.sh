#!/bin/bash
set -euo pipefail
IFS=$'\n\t'
# Above is a recommended header for all shell scripts.
# More: http://redsymbol.net/articles/unofficial-bash-strict-mode/

# Setup default values
declare environment="dev"

function usage {
     echo "Usage $0 [-h] [-e environment default=$environment]" 1>&2; exit 1;
}

while getopts ":he:" opt; do
  case ${opt} in
    h ) usage; exit;;
    e ) environment="$OPTARG"
      ;;
    \? ) echo "Unknown option: -$OPTARG" >&2; exit 1;;
    :  ) echo "Missing option argument for -$OPTARG" >&2; exit 1;;
    *  ) echo "Unimplemented option: -$OPTION" >&2; exit 1;;
  esac
done
shift $((OPTIND - 1))

./nsg-validate.sh -e "$environment"
./vnet-validate.sh -e "$environment"
