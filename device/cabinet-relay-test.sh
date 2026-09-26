#!/bin/bash
# Slow, obvious relay test: easy to watch and describe over the phone.
# Round 1 drives the line HIGH, round 2 drives it LOW. Each round: 3 x (2s on, 2s off).
CHIP=gpiochip0
PIN=${1:-17}

round() {
  local level=$1 label=$2
  echo "=== $label on line $PIN - watch the relay for 12 seconds"
  for i in 1 2 3; do
    echo "  $label $i/3: ON for 2s"
    timeout 3 gpioset --chip $CHIP --toggle 2s,0 $PIN=$level
    sleep 2
  done
}

round 1 "ROUND 1 (HIGH)"
echo "--- 5 second pause ---"
sleep 5
round 0 "ROUND 2 (LOW)"
echo "=== done"
