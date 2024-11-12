# RimDialogueClient 

This project is a fork of https://github.com/Jaxe-Dev/Bubbles

The following base methods are patched with Harmony:
```
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Postfix : RimWorld.MapInterface.MapInterfaceOnGUI_BeforeMainTabs
Postfix : Verse.PlayLog.Add
Prefix  : Verse.Profile.MemoryUtility.ClearAllMapsAndWorld
```

