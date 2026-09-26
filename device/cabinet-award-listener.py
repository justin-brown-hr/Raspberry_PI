#!/usr/bin/env python3
"""Credit the claw machine with the tries won in SmashFest.

The game runs inside Waydroid and cannot touch the Pi's GPIO, so when a game ends it
sends one UDP line to the host across the Waydroid bridge:

    AWARD <tries> <score>

This listener pulses a relay once per try won - exactly as if that many credits had been
put into the claw machine. It runs as the ordinary pi5 user (who is in the `gpio` group),
so it needs no root and no system service; cabinet-start.sh launches it at boot.

Wiring: relay module IN -> GPIO line 17 (header pin 11), plus 5V and GND. The relay's
normally-open contacts go across the claw's credit input.

Check it without a relay attached:
    ./cabinet-award-listener.py --dry-run
    # then, from another shell:
    echo -n "AWARD 2 2660" | nc -u -w1 127.0.0.1 47801
"""

import argparse
import socket
import sys
import time

try:
    import gpiod
except ImportError:
    gpiod = None

LISTEN_HOST = "0.0.0.0"       # the game reaches us on 192.168.240.1 (waydroid0)
LISTEN_PORT = 47801
LOG_FILE = "/home/pi5/cabinet-award.log"

GPIO_CHIP = "/dev/gpiochip0"   # Pi 5 header = pinctrl-rp1 (confirmed with gpiodetect)
GPIO_PIN = 17                  # line offset; header pin 11

# This cabinet's relay board is ACTIVE LOW: the relay is released when IN sits HIGH and
# engages when IN is pulled to 0V. Confirmed on the machine - holding the line low left the
# relay engaged the whole time. So we hold the line HIGH for as long as this service runs
# and a credit is a short pulse LOW. Use --active-high for a board wired the other way.
ACTIVE_LOW = True

# Open-drain: at rest we let the line go and the board's own pull-up (to whatever VCC it
# runs on) releases the relay; a credit pulls it to 0V. This is what lets the board keep
# its proper 5V supply while the Pi only ever drives 0V - a 3.3V "high" is not high enough
# to release a 5V-referenced input, which is why the relay was stuck on at first.
OPEN_DRAIN = True
PULSE_MS = 500                 # contact closed time for one credit (client asked for 0.5 s)
GAP_SECONDS = 0.7              # gap so the claw counts credits separately
MAX_TRIES = 3                  # never credit more than a game can award


def log(message):
    line = "%s %s" % (time.strftime("%Y-%m-%d %H:%M:%S"), message)
    print(line, flush=True)
    try:
        with open(LOG_FILE, "a") as handle:
            handle.write(line + "\n")
    except OSError:
        pass


class Relay:
    """Holds the relay line at rest and pulses it to credit the claw.

    The line is held for the lifetime of this object: releasing it lets the board's own
    pull-up switch the relay on, which would hand out free credits.
    """

    def __init__(self, dry_run):
        self.dry_run = dry_run
        self.request = None
        self.idle = 0 if not ACTIVE_LOW else 1
        self.active = 1 - self.idle
        if dry_run:
            log("relay: dry-run, no GPIO will be touched")
            return
        if gpiod is None:
            log("relay: python3-gpiod missing - cannot drive the relay")
            return
        try:
            settings = gpiod.LineSettings(
                direction=gpiod.line.Direction.OUTPUT,
                output_value=self._value(self.idle),
            )
            if OPEN_DRAIN:
                settings.drive = gpiod.line.Drive.OPEN_DRAIN
                # No internal bias: any pull-down fights the board's pull-up and leaves IN
                # at an in-between voltage, which keeps the relay engaged (seen on the machine)
                settings.bias = gpiod.line.Bias.DISABLED
            self.request = gpiod.request_lines(
                GPIO_CHIP, consumer="cabinet-award", config={GPIO_PIN: settings},
            )
            log("relay: line %d at rest (%s%s)" % (
                GPIO_PIN, "low" if self.idle == 0 else "high",
                ", open-drain" if OPEN_DRAIN else ""))
        except Exception as error:                      # noqa: BLE001 - report anything
            log("relay: could not claim line %d (%s)" % (GPIO_PIN, error))

    @staticmethod
    def _value(level):
        return gpiod.line.Value.ACTIVE if level else gpiod.line.Value.INACTIVE

    def pulse(self, count):
        """Close the relay `count` times, one pulse per try won."""
        for index in range(count):
            if self.dry_run or self.request is None:
                log("  [dry-run] pulse %d/%d" % (index + 1, count))
            else:
                self.request.set_value(GPIO_PIN, self._value(self.active))
                time.sleep(PULSE_MS / 1000.0)
                self.request.set_value(GPIO_PIN, self._value(self.idle))
                log("  pulse %d/%d sent" % (index + 1, count))
            if index < count - 1:
                time.sleep(GAP_SECONDS)


def handle(message, relay):
    parts = message.split()
    if len(parts) < 2 or parts[0] != "AWARD":
        log("ignored: %r" % message[:80])
        return
    try:
        tries = int(parts[1])
        score = int(parts[2]) if len(parts) > 2 else 0
    except ValueError:
        log("bad numbers in: %r" % message[:80])
        return

    if not 1 <= tries <= MAX_TRIES:
        log("refusing out-of-range award: %d tries" % tries)
        return

    log("award: %d tries (score %d)" % (tries, score))
    relay.pulse(tries)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--dry-run", action="store_true", help="log pulses instead of driving GPIO")
    parser.add_argument("--active-high", action="store_true",
                        help="relay switches when IN is driven to 3.3V (default is active low)")
    parser.add_argument("--push-pull", action="store_true",
                        help="drive both levels instead of open-drain")
    parser.add_argument("--test", type=int, metavar="N",
                        help="fire N pulses now and exit, for checking the wiring")
    args = parser.parse_args()

    global ACTIVE_LOW, OPEN_DRAIN
    if args.active_high:
        ACTIVE_LOW = False
    if args.push_pull:
        OPEN_DRAIN = False

    relay = Relay(args.dry_run)

    if args.test:
        log("wiring test: %d pulse(s), active_low=%s" % (args.test, ACTIVE_LOW))
        relay.pulse(args.test)
        time.sleep(1)
        return 0

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    sock.bind((LISTEN_HOST, LISTEN_PORT))
    log("listener up on %s:%d (pin %d, dry_run=%s)" % (LISTEN_HOST, LISTEN_PORT, GPIO_PIN, args.dry_run))

    while True:
        try:
            data, sender = sock.recvfrom(256)
        except KeyboardInterrupt:
            log("listener stopping")
            return 0
        message = data.decode("utf-8", "replace").strip()
        log("from %s: %s" % (sender[0], message))
        handle(message, relay)


if __name__ == "__main__":
    sys.exit(main())
