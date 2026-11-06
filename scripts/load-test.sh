#!/usr/bin/env bash
set -euo pipefail

URL="${1:-https://st10538419.azurewebsites.net}"
DURATION="${DURATION:-720}"
CONCURRENCY="${CONCURRENCY:-20}"
URL="${URL%/}"

echo "Target ${URL}"
echo "Duration ${DURATION} seconds"
echo "Concurrency ${CONCURRENCY} workers, two requests each wave (/health and /Account/Login)"

start=$(date +%s)
end=$((start + DURATION))
dir=$(mktemp -d)
wave=0

while [ "$(date +%s)" -lt "$end" ]; do
  wave=$((wave + 1))
  for i in $(seq 1 "$CONCURRENCY"); do
    curl -sS -o /dev/null -w "%{http_code}\n" --max-time 20 "${URL}/health" >> "${dir}/codes" &
    curl -sS -o /dev/null -w "%{http_code}\n" --max-time 20 "${URL}/Account/Login" >> "${dir}/codes" &
  done
  wait
done

total=0
ok=0
fail=0
if [ -f "${dir}/codes" ]; then
  while read -r code; do
    total=$((total + 1))
    case "$code" in
      2*|3*) ok=$((ok + 1)) ;;
      *) fail=$((fail + 1)) ;;
    esac
  done < "${dir}/codes"
fi

elapsed=$(( $(date +%s) - start ))
echo "Waves ${wave}"
echo "Requests ${total}"
echo "Success ${ok}"
echo "Failed ${fail}"
echo "Elapsed ${elapsed} seconds"
rm -rf "$dir"

if [ "$fail" -ne 0 ]; then
  exit 1
fi
