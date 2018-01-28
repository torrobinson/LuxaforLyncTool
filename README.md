# LuxaforLyncTool

## What
A service for both sending current Lync status to your Luxafor LED notification light, and notifying you of new Lync messages (both without running the official Luxafor software)

## Why
Because the default bundled software, when setting your light to your Lync status, can't perform other actions like message notifications.
I plan to add more notification types and further customization in the future.

## How
This uses the Lync SDK to monitor the active Lync/Skype-for-Business instance for messages and state changes, and a HID library for communicating with the Luxafor via USB
