Sensoray Model 2255 JPEG-enabled firmware
=========================================

This package includes the firmware that provides JPEG capture support for model 2255.
Please follow the instructions below based on the operating system and the driver model used.
The board has to be unplugged prior and replugged after the file replacements.

1. Windows.
Replace the file \windows\system32\drivers\f2255usb.bin with the one from this package.

2. Linux (non-V4L driver).
Replace the file f2255usb.h with the one from this package. 
Recompile driver and reload it before unplugging and replugging in the board.

3. Linux (V4L driver).
Replace the file /lib/firmware/f2255usb.bin with the one from this package.
Eg.  "cp f2255usb.bin /lib/firmware/f2255usb.bin"


