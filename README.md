# Companion-Display
Companion Display is an app designed to give life to a secondary monitor or old device. 
![CompanionDisplay](https://github.com/yagdev/Companion-Display/assets/53407061/9e9dfc31-f0f6-4a6c-aff6-2722d5ac6e07)

## How can I set up Companion Display?
It's really simple! Download the latest release and open CompanionDisplayWinUI.exe

## Getting started with Widgets
To add Widgets simply tap the + button and choose a widget. Currently there are the following Widgets available:
### Date & Time
![image](https://github.com/yagdev/Companion-Display/assets/53407061/e1ae5fcf-872a-4ee1-8476-37f143772bfc)
### Media controls with Spotify & Discord integration for synced lyrics
![image](https://github.com/yagdev/Companion-Display/assets/53407061/610aa0c9-5334-45ba-b26d-74d15d2bdf12)
### Launchpad
![image](https://github.com/yagdev/Companion-Display/assets/53407061/cb25992c-2238-4bb5-867b-ec7f8a9d9539)
### Image Slideshow
![image](https://github.com/yagdev/Companion-Display/assets/53407061/e14409e6-f7e5-4a1a-a336-3590dcbae7bc)
### Hardware Monitor
![image](https://github.com/yagdev/Companion-Display/assets/53407061/4c38aa0f-16af-43a4-a219-9f197bbbe60d)
### Monitor brightness controls
![image](https://github.com/yagdev/Companion-Display/assets/53407061/4e160220-1a50-4a8d-a45a-dcd213a1e008)
### Android control widget with Battery info, Brightness controls and shutdown/restart controls
![image](https://github.com/yagdev/Companion-Display/assets/53407061/c73dcff6-49eb-4225-806d-e7b806fff960)
### Virtual Numpad
![image](https://github.com/yagdev/Companion-Display/assets/53407061/9e3903c3-d212-4712-881f-6bad3804e206)

## Widget Configuration:
· To customize Widgets, right click the widget and press Edit and the configuration page for the respective widget will pop up. For now the Media Player widget requires a restart.

## App Configuration
· To configure Companion Display, press the settings button on the bottom left corner. Here you can find the available settings separated by category. While they should work without a restart, however this is a work in progress so a restart is recommended after changing settings

## Browser
· Companion Display has a built in browser based on WebView2. You can access it by pressing the Browser icon.

## Updating
· Whenever an update is available, it will automatically show up in the home screen. If you wanna force an update, you can do so by going to the settings, About & Update and pressing Update.

## Special note regarding the Hardware Monitor
· To use the Hardware Monitor, you have to run the app using admin rights, however you won't be able to reorder widgets in admin mode because of a WinUI bug regarding Drag & Drop. This is intended behavior because allowing Drag & Drop operations would cause an app crash. This is a known bug by Microsoft also.

## Reporting bugs
· To report bugs, it's really useful if you can use the source code and see what line causes a crash using Visual Studio.
