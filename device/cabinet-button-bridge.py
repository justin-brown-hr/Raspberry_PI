#!/usr/bin/env python3
"""Make the arcade panel's buttons work inside the game.

The cabinet panel is a DragonRise encoder. Its stick arrives as axes and already works,
but its buttons arrive as legacy joystick codes (BTN_TOP, BTN_THUMB2) which Waydroid's
Android does not turn into the gamepad buttons Unity listens for - so pressing them did
nothing, while ENTER on a keyboard worked.

This bridge reads the panel directly and injects the equivalent key into the USB keyboard
device, which Android already handles correctly:

    BTN_TOP    (291)  ->  ENTER   = START / credit
    BTN_THUMB2 (290)  ->  SPACE   = FIRE the ball

Swap the two codes in BUTTON_MAP if the physical buttons turn out to be the other way
round. Started by cabinet-start.sh; needs no root (pi5 is in the `input` group).
"""

import argparse
import glob
import os
import select
import struct
import sys
import time

EVENT_SIZE = struct.calcsize("llHHi")
EV_SYN, EV_KEY = 0x00, 0x01

KEY_ENTER, KEY_SPACE = 28, 57

# Panel button code -> key to inject
BUTTON_MAP = {
    291: (KEY_ENTER, "START"),
    290: (KEY_SPACE, "FIRE"),
}

PANEL_NAME_HINT = "DragonRise"
KEYBOARD_NAME_HINT = "Usb KeyBoard Usb KeyBoard"
LOG_FILE = "/home/pi5/cabinet-buttons.log"


def log(message):
    line = "%s %s" % (time.strftime("%Y-%m-%d %H:%M:%S"), message)
    print(line, flush=True)
    try:
        with open(LOG_FILE, "a") as handle:
            handle.write(line + "\n")
    except OSError:
        pass


def device_name(path):
    base = os.path.basename(path)
    try:
        with open("/sys/class/input/%s/device/name" % base) as handle:
            return handle.read().strip()
    except OSError:
        return ""


def find_device(hint, exact=False):
    """Find an event device by (part of) its name. Device numbers move between boots."""
    for path in sorted(glob.glob("/dev/input/event*")):
        name = device_name(path)
        if (name == hint) if exact else (hint.lower() in name.lower()):
            return path, name
    return None, None


def emit(handle, type_, code, value):
    now = time.time()
    handle.write(struct.pack("llHHi", int(now), int((now % 1) * 1e6), type_, code, value))
    handle.flush()


def tap(keyboard_path, key_code):
    """Inject a key press+release into the keyboard the game already listens to."""
    with open(keyboard_path, "wb") as handle:
        emit(handle, EV_KEY, key_code, 1)
        emit(handle, EV_SYN, 0, 0)
        time.sleep(0.05)
        emit(handle, EV_KEY, key_code, 0)
        emit(handle, EV_SYN, 0, 0)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--dry-run", action="store_true", help="log presses without injecting")
    args = parser.parse_args()

    panel_path = keyboard_path = None
    while panel_path is None or keyboard_path is None:
        panel_path, panel_name = find_device(PANEL_NAME_HINT)
        keyboard_path, keyboard_name = find_device(KEYBOARD_NAME_HINT, exact=True)
        if panel_path is None or keyboard_path is None:
            log("waiting for devices (panel=%s keyboard=%s)" % (panel_path, keyboard_path))
            time.sleep(5)

    log("bridge up: panel %s (%s) -> keyboard %s" % (panel_path, panel_name, keyboard_path))

    fd = os.open(panel_path, os.O_RDONLY)
    while True:
        select.select([fd], [], [])
        try:
            data = os.read(fd, EVENT_SIZE * 32)
        except OSError as error:
            log("panel read failed (%s) - unplugged?" % error)
            return 1
        for offset in range(0, len(data) - EVENT_SIZE + 1, EVENT_SIZE):
            _, _, etype, code, value = struct.unpack("llHHi", data[offset:offset + EVENT_SIZE])
            if etype != EV_KEY or value != 1:
                continue
            mapped = BUTTON_MAP.get(code)
            if mapped is None:
                log("panel button %d pressed (not mapped)" % code)
                continue
            key_code, label = mapped
            if args.dry_run:
                log("panel %d -> %s (dry-run)" % (code, label))
            else:
                tap(keyboard_path, key_code)
                log("panel %d -> %s" % (code, label))


if __name__ == "__main__":
    sys.exit(main())
