#!/usr/bin/env python3
"""Show what every button and stick direction on the cabinet actually sends.

Run this on the Pi, then press each button on the arcade panel. Every press prints the
device it came from and the key name, so we can map START and FIRE to the right buttons.

    ./cabinet-input-log.py            # watch until Ctrl-C
    ./cabinet-input-log.py --seconds 60

If pressing a button prints nothing at all, the panel is not reaching the Pi: check that
its USB cable is plugged into the Raspberry Pi (not the TV or another console).
"""

import argparse
import glob
import os
import select
import struct
import sys
import time

EVENT_SIZE = struct.calcsize("llHHi")
EV_KEY = 0x01

# Codes an arcade encoder is likely to send (from linux/input-event-codes.h)
KEY_NAMES = {
    1: "ESC", 2: "1", 3: "2", 4: "3", 5: "4", 6: "5", 7: "6", 8: "7", 9: "8", 10: "9",
    11: "0", 14: "BACKSPACE", 15: "TAB", 28: "ENTER", 29: "LEFTCTRL", 42: "LEFTSHIFT",
    44: "Z", 45: "X", 46: "C", 47: "V", 48: "B", 49: "N", 50: "M", 56: "LEFTALT",
    57: "SPACE", 97: "RIGHTCTRL", 100: "RIGHTALT", 103: "UP", 105: "LEFT",
    106: "RIGHT", 108: "DOWN", 125: "LEFTMETA",
    # gamepad style, in case the encoder presents as a joystick
    288: "BTN_TRIGGER/BTN_A", 289: "BTN_THUMB/BTN_B", 290: "BTN_THUMB2/BTN_C",
    291: "BTN_TOP/BTN_X", 292: "BTN_TOP2/BTN_Y", 293: "BTN_PINKIE/BTN_Z",
    294: "BTN_BASE", 295: "BTN_BASE2", 296: "BTN_BASE3", 297: "BTN_BASE4",
    304: "BTN_SOUTH/A", 305: "BTN_EAST/B", 307: "BTN_NORTH/X", 308: "BTN_WEST/Y",
    310: "BTN_TL", 311: "BTN_TR", 314: "BTN_SELECT", 315: "BTN_START", 316: "BTN_MODE",
}


def device_name(path):
    """Read the human-readable name of an event device."""
    base = os.path.basename(path)
    try:
        with open("/sys/class/input/%s/device/name" % base) as handle:
            return handle.read().strip()
    except OSError:
        return base


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--seconds", type=float, default=0, help="stop after this long")
    args = parser.parse_args()

    paths = sorted(glob.glob("/dev/input/event*"))
    handles = {}
    for path in paths:
        try:
            handles[os.open(path, os.O_RDONLY | os.O_NONBLOCK)] = (path, device_name(path))
        except OSError as error:
            print("cannot open %s (%s)" % (path, error))

    if not handles:
        print("No input devices could be opened - is this user in the 'input' group?")
        return 1

    print("Watching %d input devices. Press the buttons on the arcade panel now.\n" % len(handles))
    for _, (path, name) in sorted(handles.items()):
        print("  %-22s %s" % (os.path.basename(path), name))
    print("\n--- press buttons (Ctrl-C to stop) ---\n", flush=True)

    deadline = time.time() + args.seconds if args.seconds else None
    seen = set()
    try:
        while True:
            if deadline and time.time() > deadline:
                break
            ready, _, _ = select.select(list(handles), [], [], 0.5)
            for fd in ready:
                path, name = handles[fd]
                try:
                    data = os.read(fd, EVENT_SIZE * 32)
                except OSError:
                    continue
                for offset in range(0, len(data) - EVENT_SIZE + 1, EVENT_SIZE):
                    _, _, etype, code, value = struct.unpack(
                        "llHHi", data[offset:offset + EVENT_SIZE])
                    if etype != EV_KEY or value != 1:   # key down only
                        continue
                    key = KEY_NAMES.get(code, "code %d" % code)
                    print("%-34s %-22s %s" % (name, key, "(code %d)" % code), flush=True)
                    seen.add((name, key))
    except KeyboardInterrupt:
        pass

    print("\n--- summary: %d distinct buttons seen ---" % len(seen))
    for name, key in sorted(seen):
        print("  %-34s %s" % (name, key))
    if not seen:
        print("  NOTHING was pressed, or the panel is not connected to this Raspberry Pi.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
