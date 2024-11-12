# RimDialogueClient 

This project is a fork of https://github.com/Jaxe-Dev/Bubbles

RimDialogue is an extension to Rimworld’s builtin interactions system.   It takes the  basic interactions your pawns have with each other like, "Stomp chatted about board games with Mekkie." and uses a LLM to translate that into actual dialogue.  That dialogue is then shown in game above the characters head in a bubble.

The following base methods are patched with Harmony:
```
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Postfix : RimWorld.MapInterface.MapInterfaceOnGUI_BeforeMainTabs
Postfix : Verse.PlayLog.Add
Prefix  : Verse.Profile.MemoryUtility.ClearAllMapsAndWorld
```

