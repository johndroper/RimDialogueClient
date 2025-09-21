//using HarmonyLib;
//using RimDialogue.Core.InteractionData;
//using RimDialogue.Core.InteractionRequests;
//using RimWorld;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using Verse;

//public static class PlayLogEntryFactory
//{
//  // Match the original ctor signature exactly.
//  // Return PlayLogEntry (base) so you can swap in any subclass.
//  public static PlayLogEntry_Interaction Create(
//      InteractionDef intDef,
//      Pawn initiator,
//      Pawn recipient,
//      List<RulePackDef> extraSentencePacks)
//  {
//    // Example switching. Use defName or direct reference comparisons.
//    // Return your preferred subclass per interaction.
//    switch (intDef.defName)
//    {
//      case "RecentIncidentChitchat":
//        dialogueRequest = new DialogueRequestIncident<DialogueDataIncident>(
//          __instance,
//          interactionDef,
//          initiator,
//          recipient);
//        break;
//      case "RecentBattleChitchat":
//        dialogueRequest = new DialogueRequestBattle_Recent(__instance, interactionDef, initiator, recipient);
//        break;
//      case "GameConditionChitchat":
//        dialogueRequest = new DialogueRequestCondition(__instance, interactionDef, initiator, recipient);
//        break;
//      case "MessageChitchat":
//        dialogueRequest = new DialogueRequestMessage(__instance, interactionDef, initiator, recipient);
//        break;
//      case "AlertChitchat":
//        dialogueRequest = new DialogueRequestAlert<DialogueDataAlert>(__instance, interactionDef, initiator, recipient);
//        break;
//      case "SameIdeologyChitchat":
//        dialogueRequest = new DialogueRequestIdeology<DialogueData>(__instance, interactionDef, initiator, recipient);
//        break;
//      case "SkillChitchat":
//        dialogueRequest = new DialogueRequestSkill(__instance, interactionDef, initiator, recipient);
//        break;
//      case "BestSkillChitchat":
//        dialogueRequest = new DialogueRequestBestSkill(__instance, interactionDef, initiator, recipient);
//        break;
//      case "WorstSkillChitchat":
//        dialogueRequest = new DialogueRequestWorstSkill(__instance, interactionDef, initiator, recipient);
//        break;
//      case "ColonistChitchat":
//        dialogueRequest = new DialogueRequestColonist<DialogueTargetData>(__instance, interactionDef, initiator, recipient);
//        break;
//      case "ColonyAnimalChitchat":
//        dialogueRequest = new DialogueRequestAnimal_Colony(__instance, interactionDef, initiator, recipient);
//        break;
//      case "WildAnimalChitchat":
//        dialogueRequest = new DialogueRequestAnimal_Wild(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorHealthChitchat":
//        dialogueRequest = new DialogueRequestHealthInitiator(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientHealthChitchat":
//        dialogueRequest = new DialogueRequestHealthRecipient(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorApparelChitchat":
//        dialogueRequest = new DialogueRequestApparel_Initiator(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientApparelChitchat":
//      case "SlightApparel":
//        dialogueRequest = new DialogueRequestApparel_Recipient(__instance, interactionDef, initiator, recipient);
//        break;
//      case "UnsatisfiedNeedChitchat":
//        dialogueRequest = new DialogueRequestNeed<DialogueDataNeed>(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorFamilyChitchat":
//        dialogueRequest = new DialogueRequestInitiatorFamily(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientFamilyChitchat":
//      case "SlightFamily":
//        dialogueRequest = new DialogueRequestRecipientFamily(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RoomChitchat":
//        dialogueRequest = new DialogueRequestRoom(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorBedroomChitchat":
//        dialogueRequest = new DialogueRequestRoom_InitiatorBedroom(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientBedroomChitchat":
//        dialogueRequest = new DialogueRequestRoom_RecipientBedroom(__instance, interactionDef, initiator, recipient);
//        break;
//      case "HostileFactionChitchat":
//        dialogueRequest = new DialogueRequestHostileFaction(__instance, interactionDef, initiator, recipient);
//        break;
//      case "AlliedFactionChitchat":
//        dialogueRequest = new DialogueRequestAlliedFaction(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RoyalFactionChitchat":
//        dialogueRequest = new DialogueRequestRoyalFaction(__instance, interactionDef, initiator, recipient);
//        break;
//      case "NeutralFactionChitchat":
//        dialogueRequest = new DialogueRequestNeutralFaction(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorWeaponChitchat":
//        dialogueRequest = new DialogueRequestWeapon_Initiator(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientWeaponChitchat":
//      case "SlightWeapon":
//        dialogueRequest = new DialogueRequestWeapon_Recipient(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorBeardChitchat":
//      case "InitiatorBodyTattooChitchat":
//      case "InitiatorFaceTattooChitchat":
//      case "InitiatorHairChitchat":
//        dialogueRequest = new DialogueRequestAppearance_Initiator(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientBeardChitchat":
//      case "RecipientBodyTattooChitchat":
//      case "RecipientFaceTattooChitchat":
//      case "RecipientHairChitchat":
//      case "SlightBeard":
//      case "SlightBodyTattoo":
//      case "SlightFaceTattoo":
//      case "SlightHair":
//        dialogueRequest = new DialogueRequestAppearance_Recipient(__instance, interactionDef, initiator, recipient);
//        break;
//      case "DeadColonistDeepTalk":
//        dialogueRequest = new DialogueRequestDeadColonist(__instance, interactionDef, initiator, recipient);
//        break;
//      case "InitiatorBattleChitchat":
//        dialogueRequest = new DialogueRequestBattle_Initiator(__instance, interactionDef, initiator, recipient);
//        break;
//      case "RecipientBattleChitchat":
//        dialogueRequest = new DialogueRequestBattle_Recipient(__instance, interactionDef, initiator, recipient);
//        break;
//      case "WeatherChitchat":
//        dialogueRequest = new DialogueRequestWeather(__instance, interactionDef, initiator, recipient);
//        break;
//      default:
//        return new PlayLogEntry_Interaction(intDef, initiator, recipient, extraSentencePacks);
//    }
//  }
//}

//[HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
//public static class Patch_LastInteracted_ToFactory
//{
//  static readonly ConstructorInfo TargetCtor =
//      AccessTools.Constructor(typeof(PlayLogEntry_Interaction),
//          new[] { typeof(InteractionDef), typeof(Pawn), typeof(Pawn), typeof(List<RulePackDef>) });

//  static readonly MethodInfo Factory =
//      AccessTools.Method(typeof(PlayLogEntryFactory), nameof(PlayLogEntryFactory.Create),
//          new[] { typeof(InteractionDef), typeof(Pawn), typeof(Pawn), typeof(List<RulePackDef>) });

//  [HarmonyTranspiler]
//  public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instrs)
//    => NewPlayLogEntryToFactory.Run(instrs);

//public static class NewPlayLogEntryToFactory
//{
//  static readonly ConstructorInfo TargetCtor =
//      AccessTools.Constructor(typeof(PlayLogEntry_Interaction),
//          new[] { typeof(InteractionDef), typeof(Pawn), typeof(Pawn), typeof(List<RulePackDef>) });

//  static readonly MethodInfo Factory =
//      AccessTools.Method(typeof(PlayLogEntryFactory), nameof(PlayLogEntryFactory.Create),
//          new[] { typeof(InteractionDef), typeof(Pawn), typeof(Pawn), typeof(List<RulePackDef>) });

//  public static IEnumerable<CodeInstruction> Run(IEnumerable<CodeInstruction> instrs)
//  {
//    foreach (var ci in instrs)
//    {
//      if (ci.opcode == OpCodes.Newobj && ReferenceEquals(ci.operand, TargetCtor))
//      {
//        yield return new CodeInstruction(OpCodes.Call, Factory);
//        continue;
//      }
//      yield return ci;
//    }
//  }
//}

//// Example usage of the generic helper for another site:
//[HarmonyPatch(typeof(PlayLog), nameof(PlayLog.Add))]
//public static class Patch_PlayLog_Add_ToFactory
//{
//  [HarmonyTranspiler]
//  public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instrs)
//      => NewPlayLogEntryToFactory.Run(instrs);
//}

//// Example custom subclasses
//public class MyCustomInsultEntry : PlayLogEntry_Interaction
//{
//  public MyCustomInsultEntry(InteractionDef def, Pawn initiator, Pawn recipient, List<RulePackDef> packs)
//      : base(def, initiator, recipient, packs) { }
//}

//public class MyCustomKindWordsEntry : PlayLogEntry_Interaction
//{
//  public MyCustomKindWordsEntry(InteractionDef def, Pawn initiator, Pawn recipient, List<RulePackDef> packs)
//      : base(def, initiator, recipient, packs) { }
//}
