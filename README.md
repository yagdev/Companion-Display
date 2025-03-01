# Companion Display
Companion Display is an app designed to give life to a secondary monitor or old device. 
![image](https://github.com/user-attachments/assets/14cc8733-3a51-42da-ac87-74bcb564280b)

## How can I set up Companion Display?
It's really simple! Download the latest release here:

https://github.com/yagdev/Companion-Display/releases

Open CompanionDisplayWinUI.exe. It works on Windows 10 x64 (Build 1809 and later) and Windows 11.
If you use Windows 10, I recommend you install the Segoe Fluent Icons font as the app will not display icons correctly on Windows 10 without it. A download can be found here:

https://aka.ms/SegoeFluentIcons

Another thing that does not work on Windows 10 builds is the Mica Backdrop.

## Getting started with Widgets
To add Widgets simply tap the + button and choose a widget. Currently there are the following Widgets available:
### Date & Time
![image](https://github.com/user-attachments/assets/4383fc7a-5570-4545-a347-e13844eaaf07)
### Media controls with Spotify & Discord integration for synced lyrics
![image](https://github.com/user-attachments/assets/75f54a8b-3f0c-4036-912c-28a46c43ccbb)
### Launchpad
![image](https://github.com/yagdev/Companion-Display/assets/53407061/cb25992c-2238-4bb5-867b-ec7f8a9d9539)
### Image Slideshow
![image](https://github.com/yagdev/Companion-Display/assets/53407061/e14409e6-f7e5-4a1a-a336-3590dcbae7bc)
### Hardware Monitor
![image](https://github.com/user-attachments/assets/83a89f41-4730-481b-9755-4be5897ecfdb)
### Monitor brightness controls
![image](https://github.com/yagdev/Companion-Display/assets/53407061/4e160220-1a50-4a8d-a45a-dcd213a1e008)
### Android control widget with Battery info, Brightness controls and shutdown/restart controls
![image](https://github.com/yagdev/Companion-Display/assets/53407061/c73dcff6-49eb-4225-806d-e7b806fff960)
### Virtual Numpad
![image](https://github.com/yagdev/Companion-Display/assets/53407061/9e3903c3-d212-4712-881f-6bad3804e206)
### Notepad Widget
![image](https://github.com/user-attachments/assets/95724ebe-532a-4175-8a2e-4a2432029d7a)
### Volume Mixer
![image](https://github.com/user-attachments/assets/0819c656-2127-49db-9075-56bb372b3ad8)
### Twitch Integration Widgets
![image](https://github.com/user-attachments/assets/34715216-18c2-4209-8e32-29898a9736d9)
### OBS Integration Widgets
![image](https://github.com/user-attachments/assets/6617f1c9-94b6-4d22-8f61-793f8f90585b)

## Pins
· Pins allow you to keep selected Widgets on top while letting you scroll unpinned ones at the same time:
![image](https://github.com/user-attachments/assets/58dbbb64-e248-4fa5-8be6-b6c12d034fba)

## Picture-in-Picture support
· Companion Display allows you to open Widgets in Picture-in-Picture mode, allowing you to keep the Widget pinned above other apps too like so:
![image](https://github.com/user-attachments/assets/bdd342be-694c-48d7-9cfe-e58114d4cc91)

## Widget Configuration:
· To customize Widgets, right click the widget and press Edit and the configuration page for the respective widget will pop up. Making changes in the Media Player widget requires a restart.

## App Configuration
· To configure Companion Display, press the settings button on the bottom left corner. Here you can find the available settings separated by category. A restart is recommended after making changes but not mandatory.
![image](https://github.com/user-attachments/assets/9dbe0c72-1fa4-4a2a-841f-44859b9d731b)
### Theme
· This category allows you to change the color scheme, background, accent color and font used across the interface.
### Behavior
· This category allows you to toggle behaviors such as disabling focus takeover, hiding the Add Widget button, disabling the ability to reorder Widgets (Lock Layout) and launching on startup.
### Browser
· This category allows you to change your search engine and behavior when opening a new tab.
### Integrations
· Here you can manage integrations with Twitch and OBS.
### Sleep Mode
· Here you can change the Sleep Mode text opacity and color.
### About & Updates
· Here you can check the current version, toggle participation in the Beta Program, manage updates and donate.

## Lyric Viewer
· By pressing the pop-out button in the Media Player Widget, you can access a pane with the lyrics of the currently playing song with synced lyrics and autoscroll.
![image](https://github.com/user-attachments/assets/125c0431-e099-4562-9079-76516e0c65c2)

## Browser
· Companion Display has a built in browser based on WebView2. You can access it by pressing the Browser icon.
· The Browser also includes Launchpad, a place to bookmark websites that can be edited by right-clicking an empty space while on a new tab.
![image](https://github.com/user-attachments/assets/9b3e8d6e-d3fe-4b98-86b0-3c824b819151)

## Spotify
· The Spotify tab is the web version of Spotify easily accessible.

## Sleep mode
· Companion Display offers a customizeable sleep mode, which is a minimalistic interface that displays the time, date and current song info. You can customize the text color and opacity in Settings and it features built-in burn-in prevention for OLED displays.
![image](https://github.com/user-attachments/assets/b2c954df-bdb3-4d2a-8827-b82b9d193eab)

## Updating
· Whenever an update is available, it will automatically show up in the home screen. If you wanna force an update, you can do so by going to the settings, About & Update and pressing Update.

## Special note regarding the Hardware Monitor
· To use the Hardware Monitor, you have to run the app using admin rights, however you won't be able to reorder widgets in admin mode because of a WinUI bug regarding Drag & Drop. This is intended behavior because allowing Drag & Drop operations would cause an app crash. This is a known bug by Microsoft also.

## Reporting bugs
· To report bugs, it's really useful if you can use the source code and see what line causes a crash using Visual Studio.
