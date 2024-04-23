# Exploding View Tool
A Unity package for exploding view functionality on 3D models.

## Quick Start
**Download & install .unitypackage from [releases](https://github.com/MinasKatsiokalis/ExplodingView/releases)**.

This package includes [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676#description) plugin that is used in this package.
Once downloaded an editor window will pop to install [UniTask](https://github.com/Cysharp/UniTask) package (essential for this tool), 
and [glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.4/manual/index.html) importer (used only in sample scenes).

**Make sure to install at UniTask in order this pacage to work as intented.**

Go to Tools/Exploding View/Add Components. Drag and drop the parent transform of any object you want to apply the exploding view.<br>
Click "Add Exploding View Component" to add the ExplodngViewComponent.cs onto the object.<br>
From the parent object you can adjust the properties from the ExplodingViewComponent inspector editor.<br>

***Exploding view is applied only to sub-objects that have a mesh.***

> [!TIP]
> Documentation of project structure: [Documentation](https://minaskatsiokalis.github.io/exploding-view/documentation/html/index.html). <br>
> A more detailed guide on how-to is coming soon.

## UPM Package
### Install via git URL

> [!IMPORTANT]
> This package is using [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676#description) plugin.<br>
> Make sure you installed it before proceeding with this package.

**You can add `https://github.com/MinasKatsiokalis/ExplodingView.git?path=Assets/Plugins/ExplodingViewTool` to Package Manager.**
> Requires a version of Unity that supports path query parameter for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1.a21).

![image](https://user-images.githubusercontent.com/46207/79450714-3aadd100-8020-11ea-8aae-b8d87fc4d7be.png)

**or add `"com.mk.exploding-view-tool": "https://github.com/MinasKatsiokalis/ExplodingView.git?path=Assets/Plugins/ExplodingViewTool"` to `Packages/manifest.json`.**
