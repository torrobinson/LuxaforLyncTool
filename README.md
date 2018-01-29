#  LuxaforLyncTool <img width="16" src="https://raw.githubusercontent.com/torrobinson/LuxaforLyncTool/master/LuxaforLyncTool/LuxaforLyncTool_Client/Resources/tray_icon.ico"> [![Build Status](https://travis-ci.org/torrobinson/LuxaforLyncTool.svg?branch=master)](https://travis-ci.org/torrobinson/LuxaforLyncTool) [![GitHub issues](https://img.shields.io/github/issues/torrobinson/LuxaforLyncTool.svg)](https://github.com/torrobinson/LuxaforLyncTool/issues)
## What
A service for both sending current Lync status to your Luxafor LED notification light, and notifying you of new Lync messages (both without running the official Luxafor software)

## Why
Because the default bundled software, when setting your light to your Lync status, can't perform other actions like message notifications.
I plan to add more notification types and further customization in the future.

## How
This uses the Lync SDK to monitor the active Lync/Skype-for-Business instance for messages and state changes, and a HID library for communicating with the Luxafor via USB
