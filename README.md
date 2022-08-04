# Ready Player Me Unity Vuplex Example

This repository contains a Unity project that uses the paid Vuplex Web Browser plugin to run the Ready Player Me Avatar creator and load the Ready Player Me Avatar into the application at runtime. This can be used as a reference for anybody wanting to add Ready Player Me Avatars and embed the Ready Player Me avatar creator directly into their Unity application.

## What is Vuplex?

Vuplex is actually the name of a publisher on the Unity Asset store with a number of different paid "3D WebView" plugins separated for different platforms. Note that when we say Vuplex throughout this document we are referring to their Unity Plugins. As all of their plugins are architected in a similar way the general approach and logic is quite the same for each one.  

## Project Requirements

Because the Vuplex plugins are not free we not included it in the repository. To fully run and test the project you need to have purchased one of the [Vuplex 3 WebView plugins](https://assetstore.unity.com/publishers/40309). If you haven't purchased one already you can use the `ReadyPlayerMe` discount code with [this link here](https://store.vuplex.com/cart/?coupon=ReadyPlayerMe). For any questions specifically about the Vuplex plugins please contact the developer.

For the Vuplex VR Test Scene you also need to import the (XR Interaction Toolkit)[https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html] plugin from the package manager. This is because the scene uses the XR Origin prefab and some scripts provided with the plugin that help with UI interaction in VR.

# Desktop
## Quick Start

Open up the `VuplexDesktopScene` found in `Assets/Vuplex/Ready Player Me` folder.

To test in the editor you can just hit the Play button and the Vuplex browser should display automatically and load the demo.readyplayer.me website.

After you complete the avatar creation process the WebView should hide itself automatically and load the avatar into the scene.

To test on a built application first you need to make sure that you have added the `VuplexDesktopScene` in the build settings and it is set to as the first active scene in the Scenes In Build section. Also make sure you have set the target platform as PC, Mac, Linux as shown below.

Now just click Build or Build And Run.

# VR
## Quick Start

*To run the VR Test scene you need to install the XR Interaction Toolkit plugin from the Package Manager*

Open up the `Vuplex VR Test Scene` found in `Assets/Vuplex/Ready Player Me` folder.

To test in the editor you can just hit the Play button and the Vuplex browser should display automatically and load the demo.readyplayer.me website.

After you complete the avatar creation process the WebView should hide itself automatically and load the avatar into the scene.

To test on a built application first you need to make sure that you have added the `Vuplex VR Test Scene` in the build settings and it is set to as the first active scene in the Scenes In Build section. If you are building for Oculus Quest or another mobile VR platform make sure the platform is set to Android. For Desktop VR set the target platform as PC, Mac, Linux as shown below.

Now just click Build or Build And Run.

# How It works

## Common Features

This project includes 2 example scenes **Vuplex Desktop Scene** and **Vuplex VR Test Scene** which both include the use of a `CanvasWebViewPrefab` and and object with the `VuplexWebviewTest.cs` script. See below for more information on these.

#### Canvas Web View Prefab

This is a UI prefab we provide that can be used to display the Ready Player Me avatar creator using Unity's default UI Canvas. When using this prefab make sure it is placed into the scene as the child of a Canvas GameObject. If you select the prefab or game object in the scene and look at the inspector you will see it has a `CanvasWebviewPrefab.cs` script attached to it.

*In the Vuplex VR Test Scene the Canvas Render Mode is set to World Space, so it is rendered in 3D space which works better for VR applications*

This is a script provided with the Vuplex plugin and it is important as we use this to interact and configure the browser later when we discuss the VuplexWebViewTest.cs script.

#### Vuplex WebView Test

In both example scenes you can find an object called **Vuplex WebView Test** and if you select it in the hierarchy you can see in the Inspector that it has a custom script attached to it called VuplexWebViewTest.cs. We provide this custom script as it assists with the setup of the Vuplex Web Browser and can be used as an example for adding the Ready Player Me Avatar Creation process directly into your Unity application. In particular the We
As you can see, this script has some properties that can be assigned in the editor.

The `Loading` property is just a `GameObject` you can assign, that will display while the avatar is loading. The `Base Web View` property needs to be set as this is the instance of the WebView we will be interacting with, it is of type `BaseWebViewPrefab` so that it will work with all the different types of WebView prefabs that Vuplex provides.

Now open up the `VuplexWebViewTest` script

### Vuplex Web View

While this is not visible in the scene the `VuplexWebView.cs` script it is a used in `VuplexWebviewTest.cs` mentioned previously. It adds some additional functionality to the different Vuplex WebView Prefabs. In particular it inserts some custom Javascript that enables us to listen to events that are fired from the `readyplayer.me/avatar` website. Using these event listeners we are able to automatically retrieve the avatar GLB Url after the creation is complete and subsequently load the avatar into the Unity Scene.

# VR Only

If you open up the `Vuplex VR Test Scene` the hierarchy will look something like this.

Similar to the Desktop Scene, the main things that make this work is the `CanvasWebViewPrefab` and the `Vuplex WebView Test` with the addition of the XR Origin which is part of the XR Interaction Toolkit that you can download from the package manager. As mentioned previously the Canvas in this scene has the Canvas Render Mode set to World Space so that the UI is visible in 3D space, which works better for VR applications.

#### XR Origin

This is a prefab that comes with the XR Interaction Toolkit and is used as a VR Controller that handles tracking from VR devices (head and hand movement) as well as input from the controllers. If you would like to more about the XR Origin or the XR Interaction Toolkit please refer to the documentation [here](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html).

## Links
**Ready Player Me Unity SDK**
- [Documentation](https://docs.readyplayer.me/ready-player-me/integration-guides/unity)
- [Download](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/unity-sdk-download)
- [Support](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/troubleshooting)

**Resources**
- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl-develop.html)
- [Vuplex](https://developer.vuplex.com/webview/overview)
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html)
