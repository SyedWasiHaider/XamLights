# XamLights
An embarrassingly bad attempt at using Phillips Hue and Xamarin.

## How to:
* If you have an actual bridge setup with lights, you'll want to build in release mode. Note that you must be on the same network as the bridge for this to work. When the app launches, make sure to click the hub button and the app will automatically connect.

* If you do not have actual bridge or lights, you can use the wonderful Hue Emulator found at http://steveyo.github.io/Hue-Emulator/
  * Just make sure to build the app in debug mode and change the ip address / port (if necessary)
  
## "Features" (if you can call it that):

* Select different lights
* Select pre-defined scenes
* Set a random color (just leave the yellow textbox blank and tap the set color button)
* Set a color by hex.
* Bonus: move the phone to change the compass direction and the lights will glow brighter/dimmer. This is a mostly random fun "feature".
* _Should_ work on Android but I've only tested it on iOS.


## Credit
Thanks to the folks at Q42 for their awesome C# Hue SDK and Steveyo!
