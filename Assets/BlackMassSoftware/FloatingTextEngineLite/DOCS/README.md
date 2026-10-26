###### version 1.1.2
# Floating Text Engine

A lightweight and modular floating text engine for 2D Unity games. Easy to integrate, highly customizable, and built for performance.

## Folder Structure
```
└─BlackMassSoftware
    └───FloatingTextEngine
        ├───Demo
        │   ├───DemoEntities
        │   │   ├───DemoEnemy
        │   │   ├───DemoTilemap
        │   │   └───Textures
        │   └───DemoScenes
        ├───DOCS
        ├───LICENSES
        └───Runtime
           ├───Fonts
           ├───Prefabs
           └───Scripts
               ├───FloatingBehaviors
               ├───FloatingPresets
               └───Utilities
```

## Setup

1. Create an empty GameObject
2. Attach the `FloatingTextEngine` component to it.
3. Using the Unity inspector, grab the `FloatingText` prefab from `Runtime\Prefabs` and drag it onto "Floating Text Object Prefab" parameter of the `FloatingTextEngine` component we just added. 
4. (Optional) Enable Spatial X Variance and change the Font Size.
5. Call `FloatingTextEngine.CreateTextAtScreenSpace(...)`/`FloatingTextEngine.CreateTextAtWorldSpace(...)` from your game code to create text on demand.

## Dependencies

- Requires **TextMeshPro** (automatically installed if imported from Unity Asset Store).


## Examples

To create a floating text objects we will be interacting with the `FloatingTextEngine` class.  There are two methods that you can call depending on which screen space you're working with:

`CreateTextAtScreenSpace` - This method is used for all 2D screen space text objects.\
`CreateTextAtWorldSpace` - This method is used for all 3D world space text objects.

These methods have numerous overrides so use whichever one matches the information you're wanting to display.

Here is an example of a World Space (3D) text object that moves up and fades out:

`FloatingTextEngine.CreateTextAtScreenWorldSpace(hitData.collider.transform.position, 10, Color.darkRed)`\
`.With(FloatingTextBehaviors.MoveUp(3f, 0.5f))`\
`.With(FloatingTextBehaviors.FadeOut(1f));`

This is an example of a Screen Space (2D) text object that does a Pop motion and move up.

`FloatingTextEngine.CreateTextAtScreenSpace(hitData.collider.transform.position, 30, Color.blueViolet)`\
`With(FloatingTextBehaviors.Pop(1.5f, 0.2f, 1f))`\
`.With(FloatingTextBehaviors.MoveUp(5f, 0.2f));`


### Adding Behaviors
Behaviors are what gives your floating text life.  To add them we're going to chain off the `CreateTextAtScreenSpace` or `CreateTextAtScreenWorldSpace` method with the `With()` method.
All of the available behaviors are going to be passed in by the `FloatingTextBehaviors` class.

Here is an example using the above text creation to make it move up 3 units on the y axis over 1 second, and fade out over 2 seconds.

`float damage = GetDamageValueFromWeapon();`\
`bool isCritical = GetCriticalRollValue();`\
`FloatingTextEngine.CreateFloatingTextAt(transform.position, damage, isCritical)`\
`.With(FloatingTextBehaviors.MoveUp(3f, 1f))`\
`.With(FloatingTextBehaviors.FadeOut(2f));`


### Adding Presets
Presets are just a class that encapsulates multiple behaviors.  Much like what you would do with the above example, the Presets are bundled with the library
as a way to make easy access to common effects one would use.  The interface to those presets are located in the following class `FloatingTextPresets`.  To add presets we're going to call the `WithPreset()` method again from `CreateFloatingTextAt`.

Here is an example of creating a critical strike and block texts.\
#### Critical strike:

`FloatingTextEngine.CreateFloatingTextAt(transform.position, randomDamageValue, Color.white)`
`.WithPreset(FloatingTextPresets.Critical());`

#### Block:

`FloatingTextEngine.CreateFloatingTextAt(transform.position, randomDamageValue, Color.white)`
`.WithPreset(FloatingTextPresets.Block());`


## Documentation

View the API Reference [here](https://blackmasssoftware.github.io/FloatingTextEngineDocs/) or visit: [www.blackmasssoftware.com](https://www.blackmasssoftware.com)

## ✉ Support :phone:

For support or feedback, contact: [**contact@blackmasssoftware.com**](mailto:contact@blackmasssoftware.com)