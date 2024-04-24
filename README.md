# Exploding View Tool
A Unity package for exploding view functionality on 3D models.
> [!TIP]
> Documentation of project structure: [Documentation](https://minaskatsiokalis.github.io/exploding-view/documentation/html/index.html). <br>

## Quick Start
### 1. Install .unitypackage
**Download & install .unitypackage from [releases](https://github.com/MinasKatsiokalis/ExplodingView/releases)**.

This package includes [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676#description) plugin that is used in this package.
Once downloaded an editor window will pop to install [UniTask](https://github.com/Cysharp/UniTask) package (essential for this tool), 
and [glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.4/manual/index.html) importer (used only in sample scenes).

![image](https://github.com/MinasKatsiokalis/ExplodingView/assets/9119948/40c65b83-f04e-48d0-854c-06cf762b3583)

**Make sure to install at UniTask in order this pacage to work as intented.**

### 2. Clone repo
Clone this repo using: 
```
$ git clone https://github.com/MinasKatsiokalis/ExplodingView.git
```
If you want to test the sample scenes go to *ExplodingViewTool/Samples*

### 3. Install via [Package Manager](https://github.com/MinasKatsiokalis/ExplodingView?tab=readme-ov-file#install-via-git-url) 

## Editor Usage
![image](https://github.com/MinasKatsiokalis/ExplodingView/assets/9119948/7cfb354a-0ce3-466c-a7ec-f7f2c6d96f64)

- Go to Tools/Exploding View/Add Components. Drag and drop the parent transform of any object you want to apply the exploding view.<br>
Click "Add Exploding View Component" to add the ExplodngViewComponent.cs onto the object.<br>

![image](https://github.com/MinasKatsiokalis/ExplodingView/assets/9119948/60f58452-d60e-47d8-a6ac-1aa61c35a0ae)

- From the parent object you can adjust the properties from the ExplodingViewComponent inspector editor. Each property has a tooltip that indigates how is used, read them carefully.<br>
- You can leave the default option to automatically add the exploding parts of the model (including any game object that is children of the transform) or add manualy only the transforms you want to be exploded by deselecting the **"Add Explodables Automatically"**.<br>
***Exploding view is applied only to sub-objects that have a mesh.***
  
![image](https://github.com/MinasKatsiokalis/ExplodingView/assets/9119948/51a9343e-418e-49e1-a621-6cbb6b3a451e)

- You can adjust the behaviour of any explodable part, by adding the **ExplodableModifier** component to it. This allows the part to ignore the exploding direction of the **ExplodingViewComponent** higher in the hierarchy and behave with its own properties. Through ExplodableModifier can be adjusted the order in which the part/parts will be exploded by changing their order (e.g. order 2 will explode before order 1 etc.).

![image](https://github.com/MinasKatsiokalis/ExplodingView/assets/9119948/d862e102-236e-4834-bd97-a7999ead4d4f)

> [!NOTE]
> **ExplodableModifier** should be added before **ExplodingViewComponent** initialization, which happens in Start method.
> **ExplodablePart** can be added anytime manually, otherwise they are added during initialization automatically.
> Re-Calculation has to be executed in order to apply changes to explodables and exploding view properties.   
- In **ExplodingViewTool/Runtime** you can find all the core components to apply them manualy as you see fit.

## Script Usage
- Initialize a new ExplodingViewComponent into your script:
```C#
  ExplodingViewComponent explodingViewComponent;

  void Init()
  {
      //Cretae a new ExplodingViewComponent atteched to the game object
      explodingViewComponent = gameObject.AddComponent<ExplodingViewComponent>();
      //Set exploding view parameters
      explodingViewComponent.AddExplodablesAutomatically = true;
      explodingViewComponent.Direction = Direction.FromCenter;
      explodingViewComponent.ExplosionSpeed = 1.0f;
      explodingViewComponent.DistanceFactor = DistanceFactor.DistanceFromCenter;
      explodingViewComponent.DistanceFactorMultiplier = 0.5f;
      //Initialize the exploding view component with the set parameters
      explodingViewComponent.Init();
  }
```
- Initialize with manual import of ExplodableParts:
```C#
void AddExplodables(List<ExplodablePart> parts)
{
    //Disable auto import
    explodingViewComponent.AddExplodablesAutomatically = false;
    //Add custom explodables to the Explodables list and initialize
    explodingViewComponent.Explodables = parts;
    explodingViewComponent.Init();
}
```
- Change properties and re-calculate an ExplodingViewComponent that already exists into the scene:
```C#
ExplodingViewComponent explodingViewComponent;

void Recalibrate()
{   
    //Set exploding view parameters
    explodingViewComponent.ExplosionSpeed = 1.0f;
    explodingViewComponent.Direction = Direction.FromAxis;
    explodingViewComponent.NormalAxis = Axis.Y;
    explodingViewComponent.DistanceFactor = DistanceFactor.DistanceFromProjectionPoint;
    explodingViewComponent.DistanceFactorMultiplier = 0.5f;
    //Recalibrate the exploding view component with the set parameters
    explodingViewComponent.CalculateExplodingParameters();
}
```
- Toggle exploding view with waitable async method:
```C#
async void AwaitableExplode()
{   
    await explodingViewComponent.ExplodingViewAsyncTask();
    //This code will be executed after the explosion is finished
}
```
- Toggle exploding view with non-waitable async method:
```C#
void Explode()
{
    explodingViewComponent.ExplodingView();
    //This code will be executed before explosion is finished
}
```
- Add ExplodableModifier to any game object affected by exploding view:
```C#
void AddExplodableModifier(GameObject part)
{
    //Add component
    var modifier = part.AddComponent<ExplodableModifier>();
    //Set properties
    modifier.Order = 1;
    modifier.ModifierProperty = ModifierProperty.Axis;
    modifier.Axis = ModifierAxis.NegX;
    modifier.AffectChildren = false;
}
```

## UPM Package
### Install via git URL

> [!IMPORTANT]
> This package is using [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676#description) plugin.<br>
> Make sure you installed it before proceeding with this package.

**You can add `https://github.com/MinasKatsiokalis/ExplodingView.git?path=Assets/Plugins/ExplodingViewTool` to Package Manager.**
> Requires a version of Unity that supports path query parameter for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1.a21).

![image](https://user-images.githubusercontent.com/46207/79450714-3aadd100-8020-11ea-8aae-b8d87fc4d7be.png)

**or add `"com.mk.exploding-view-tool": "https://github.com/MinasKatsiokalis/ExplodingView.git?path=Assets/Plugins/ExplodingViewTool"` to `Packages/manifest.json`.**
