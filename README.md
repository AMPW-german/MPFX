# MPFX

A very simple post processing mod.

## Currently included effects in their default execution order:
- Color balance using lift, gamma and gain. Either for all channels or per channel.
- Color Temperature
- Hue shift in YIQ color format
- Brightness
- Contrast
- Saturation
- Single color color overlay
- Vignette
- (RGB2HSV: converts from rgb to hsv and shows the result in rgb)
- (HSV2RGB: converts the rgb input from hsv to rgb and shows the result)
- (Single color: only keeps the largest rgb channels for each pixel)
- (Single color smallest)
- (Single solid color: same as single color but sets largest channels to max value)

The execution order can be changed by changing the subpass in the MPFXAssets.xml file.\
All effects are separated into pre and post imgui.

Effects in parathenses are just some stuff I wanted to try during development, they are not meant for real use but I wanted to include them anyway.

### Warning

Currently all settings are lost when closing the game.\
Effect profiles is most likely coming in the next release.

## Installation

- Requires [StarMap](https://github.com/StarMapLoader/StarMap/releases)
- Requires [ModMenu](https://github.com/MrJeranimo/ModMenu/releases)
- Requires [KittenExtensions >= v0.4.0](https://github.com/tsholmes/KittenExtensions/releases)
- Requires [ShaderExtensions](https://github.com/AMPW-german/ShaderExtensions/releases)
- Download zip from releases and extract it into KSAs local mod folder, usually located at "Documents/My Games/Kitten Space Agency/mods"

It's recommended to install all mods in the local mod folder and not KSAs Content folder.\
The manifest.toml doesn't need to be manually modified for mods installed there but a restart is necessary after the first launch.

---

Do not expect anything like KSP's TUFX, this is just a small side project of a side project.\
Contributions are always welcome, bug reports should be done on github.

This mod is licensed under the MIT license unless stated otherwise.
