#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bubbles.Access;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using static RimWorld.ColonistBar;

namespace Bubbles.Core
{
  public static class Bubbler
  {
    private const float LabelPositionOffset = -0.6f;

    private static readonly Dictionary<Pawn, List<Bubble>> Dictionary = new();
    private static readonly Regex RemoveColorTag = new("<\\/?color[^>]*>");

    private static bool ShouldShow() => Settings.Activated && !WorldRendererUtility.WorldRenderedNow && (Settings.AutoHideSpeed.Value is Settings.AutoHideSpeedDisabled || (int)Find.TickManager!.CurTimeSpeed < Settings.AutoHideSpeed.Value);

    public static void Add(LogEntry entry)
    {

      //Mod.Log($"Adding log entry: {entry.LogID}");

      if (!ShouldShow()) { return; }

      Pawn? initiator, recipient;

      switch (entry)
      {
        case PlayLogEntry_Interaction interaction:
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction);
          recipient = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(interaction);
          break;
        case PlayLogEntry_InteractionSinglePawn interaction:
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(interaction);
          recipient = null;
          break;
        default:
          return;
      }
      
      if (initiator is null || initiator.Map != Find.CurrentMap) { return; }

      if (!Settings.DoNonPlayer.Value && (!initiator.Faction?.IsPlayer ?? true)) { return; }
      if (!Settings.DoAnimals.Value && ((initiator.RaceProps?.Animal ?? false) || (recipient?.RaceProps?.Animal ?? false))) { return; }
      if (!Settings.DoDrafted.Value && ((initiator.drafter?.Drafted ?? false) || (recipient?.drafter?.Drafted ?? false))) { return; }

      if (!Dictionary.ContainsKey(initiator)) { Dictionary[initiator] = new List<Bubble>(); }

      //Mod.Log($"initiator name: {initiator.Name}");

      GetDialogue(initiator, recipient, entry);
    }

    private static async void GetDialogue(Pawn initiator, Pawn? recipient, LogEntry entry)
    {
      try
      {
        var logEntryText = RemoveColorTag.Replace(
          entry.ToGameStringFromPOV(initiator),
          string.Empty);
        Mod.Log(logEntryText);

        List<ISocialThought> initiatorThoughtsAboutRecipient = [];
        if (recipient != null)
          initiator.needs.mood.thoughts.GetSocialThoughts(recipient, initiatorThoughtsAboutRecipient);

        List<ISocialThought> recipientThoughtsAboutInitiator = [];
        if (recipient != null)
          recipient.needs.mood.thoughts.GetSocialThoughts(initiator, recipientThoughtsAboutInitiator);

        Mod.Log($"initiator is spawned: {initiator.Spawned}");
        Mod.Log($"initiator is destroyed: {initiator.Destroyed}");
        Mod.Log($"initiator kind: {initiator.KindLabel}");
        Mod.Log($"initiator age: {initiator.ageTracker.AgeBiologicalYears}");

        Mod.Log($"Has skills: {initiator.skills.skills != null}");
        Mod.Log($"Has traits: {initiator.story.traits.allTraits != null}");
        Mod.Log($"Has relations: {initiator.relations.DirectRelations != null}");
        Mod.Log($"Has health: {initiator.health.hediffSet != null}");
        Mod.Log($"Has mood: {initiator.needs?.mood != null}");

        var dialogueData = new DialogueData
        {
          interaction = logEntryText,
          initiatorFullName = initiator.Name?.ToStringFull ?? String.Empty,
          initiatorFactionName = initiator.Faction?.Name ?? String.Empty,
          initiatorDescription = initiator.DescriptionDetailed ?? String.Empty,
          initiatorRace = initiator.def?.defName ?? String.Empty,
          initiatorIsColonist = initiator.IsColonist,
          initiatorIsPrisoner = initiator.IsPrisoner,
          initiatorAge = initiator.ageTracker.AgeBiologicalYears,
          initiatorSkills = initiator.skills?.skills.Select(skill => new SkillData(skill.def?.description ?? string.Empty, skill.Level)).ToArray() ?? [],
          initiatorTraits = initiator.story?.traits?.allTraits?.Select(trait => trait.Label).ToArray() ?? [],
          initiatorChildhood = initiator.story?.Childhood?.FullDescriptionFor(initiator) ?? String.Empty,
          initiatorAdulthood = initiator.story?.Adulthood?.FullDescriptionFor(initiator) ?? String.Empty,
          initiatorRelations = initiator.relations?.DirectRelations?.Select(relation => new RelationshipData(relation.def.description, relation.otherPawn?.Name?.ToStringFull ?? string.Empty)).ToArray() ?? [],
          initiatorApparel = initiator.apparel?.WornApparel?.Select(apparel => apparel.DescriptionDetailed).ToArray() ?? [],
          initiatorWeapons = initiator.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.DescriptionDetailed).ToArray() ?? [],
          initiatorHediffs = initiator.health.hediffSet?.hediffs?.Select(hediff => hediff.Description).ToArray() ?? [],
          initiatorMemories = initiator.needs?.mood?.thoughts?.memories?.Memories?.Select(memory => memory.Description).ToArray() ?? [],
          initiatorOpinionOfRecipient = initiatorThoughtsAboutRecipient.Select(thought => thought.OpinionOffset()).ToArray() ?? [],
          initiatorMoodPercentage = initiator.needs?.mood?.CurLevelPercentage ?? -1f,
          initiatorComfortPercentage = initiator.needs?.comfort?.CurLevelPercentage ?? -1f,
          initiatorFoodPercentage = initiator.needs?.food?.CurLevelPercentage ?? -1f,
          initiatorRestPercentage = initiator.needs?.rest?.CurLevelPercentage ?? -1f,
          initiatorJoyPercentage = initiator.needs?.joy?.CurLevelPercentage ?? -1f,
          initiatorBeautyPercentage = initiator.needs?.beauty?.CurLevelPercentage ?? -1f,
          initiatorDrugsDesirePercentage = initiator.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          initiatorEnergyPercentage = initiator.needs?.energy?.CurLevelPercentage ?? -1f,
          recipientFullName = recipient?.Name?.ToStringFull ?? String.Empty,
          recipientFactionName = recipient?.Faction?.Name ?? String.Empty,
          recipientDescription = recipient?.DescriptionDetailed ?? String.Empty,
          recipientRace = recipient?.def.defName ?? String.Empty,
          recipientAge = recipient?.ageTracker.AgeBiologicalYears ?? -1,
          recipientIsColonist = recipient?.IsColonist ?? false,
          recipientIsPrisoner = recipient?.IsPrisoner ?? false,
          recipientSkills = recipient?.skills?.skills?.Select(skill => new SkillData(skill.def?.defName ?? string.Empty, skill.Level)).ToArray() ?? [],
          recipientTraits = recipient?.story?.traits?.allTraits?.Select(trait => trait.Label).ToArray() ?? [],
          recipientChildhood = recipient?.story?.Childhood?.FullDescriptionFor(recipient) ?? String.Empty,
          recipientAdulthood = recipient?.story?.Adulthood?.FullDescriptionFor(recipient) ?? String.Empty,
          recipientRelations = recipient?.relations?.DirectRelations?.Select(relation => new RelationshipData(relation.def.description, relation.otherPawn?.Name?.ToStringFull)).ToArray() ?? [],
          recipientApparel = recipient?.apparel?.WornApparel?.Select(apparel => apparel.DescriptionDetailed).ToArray() ?? [],
          recipientWeapons = recipient?.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.DescriptionDetailed).ToArray() ?? [],
          recipientHediffs = recipient?.health?.hediffSet?.hediffs?.Select(hediff => hediff.Description).ToArray() ?? [],
          recipientMemories = recipient?.needs.mood.thoughts.memories.Memories.Select(memory => memory.Description).ToArray() ?? [],
          recipientOpinionOfInitiator = recipientThoughtsAboutInitiator.Select(thought => thought.OpinionOffset()).ToArray() ?? [],
          recipientMoodPercentage = recipient?.needs?.mood?.CurLevelPercentage ?? -1f,
          recipientComfortPercentage = recipient?.needs?.comfort?.CurLevelPercentage ?? -1f,
          recipientFoodPercentage = recipient?.needs?.food?.CurLevelPercentage ?? -1f,
          recipientRestPercentage = recipient?.needs?.rest?.CurLevelPercentage ?? -1f,
          recipientJoyPercentage = recipient?.needs?.joy?.CurLevelPercentage ?? -1f,
          recipientBeautyPercentage = recipient?.needs?.beauty?.CurLevelPercentage ?? -1f,
          recipientDrugsDesirePercentage = recipient?.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          recipientEnergyPercentage = recipient?.needs?.energy?.CurLevelPercentage ?? -1f
        };

        string dialogueDataJson = JsonUtility.ToJson(dialogueData);

        //Mod.Log($"JSON: {dialogueDataJson}");

        WWWForm form = new WWWForm();
        form.AddField("dialogueDataJSON", dialogueDataJson);

        using (UnityWebRequest request = UnityWebRequest.Post("https://localhost:7293/home/GetDialogue", form))
        {
          //request.SetRequestHeader("Content-Type", "application/json");
          var asyncOperation =  request.SendWebRequest();

          while (!asyncOperation.isDone)
          {
            await Task.Yield(); // Yield control back to the main thread
          }

          if (request.isNetworkError || request.isHttpError)
          {
            throw new Exception($"Network error: {request.error}");
          }
          else
          {
            while(!request.downloadHandler.isDone) { await Task.Yield(); }

            var body = request.downloadHandler.text;

            //Mod.Log($"BODY: {body}");

            var dialogueResponse = JsonUtility.FromJson<DialogueResponse>(body);

            Mod.Log(dialogueResponse.text ?? "NULL");

            Dictionary[initiator]!.Add(new Bubble(initiator, entry, dialogueResponse.text ?? "NULL"));
          }
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"Deactivated because http post failed with error: [{ex.Source}: {ex.Message}]\n\nTrace:\n{ex.StackTrace}");
        Settings.Activated = false;
      }
    }

    private static void Remove(Pawn pawn, Bubble bubble)
    {
      Dictionary[pawn]!.Remove(bubble);
      if (Dictionary[pawn]!.Count is 0) { Dictionary.Remove(pawn); }
    }

    public static void Draw()
    {
      var altitude = GetAltitude();
      if (altitude <= 0 || altitude > Settings.AltitudeMax.Value) { return; }

      var scale = Settings.AltitudeBase.Value / altitude;
      if (scale > Settings.ScaleMax.Value) { scale = Settings.ScaleMax.Value; }

      var selected = Find.Selector!.SingleSelectedObject as Pawn;

      foreach (var pawn in Dictionary.Keys.OrderBy(pawn => pawn == selected).ThenBy(static pawn => pawn.Position.y).ToArray()) { DrawBubble(pawn, pawn == selected, scale); }
    }

    private static void DrawBubble(Pawn pawn, bool isSelected, float scale)
    {
      if (WorldRendererUtility.WorldRenderedNow || !pawn.Spawned || pawn.Map != Find.CurrentMap || pawn.Map!.fogGrid!.IsFogged(pawn.Position)) { return; }

      var pos = GenMapUI.LabelDrawPosFor(pawn, LabelPositionOffset);

      var offset = Settings.OffsetStart.Value;
      var count = 0;

      foreach (var bubble in Dictionary[pawn].OrderByDescending(static bubble => bubble.Entry.Tick).ToArray())
      {
        if (count > Settings.PawnMax.Value) { return; }
        if (!bubble.Draw(pos + GetOffset(offset), isSelected, scale)) { Remove(pawn, bubble); }
        offset += (Settings.OffsetDirection.Value.IsHorizontal ? bubble.Width : bubble.Height) + Settings.OffsetSpacing.Value;
        count++;
      }
    }

    private static float GetAltitude()
    {
      var altitude = Mathf.Max(1f, (float)Reflection.Verse_CameraDriver_RootSize.GetValue(Find.CameraDriver));
      Compatibility.Apply(ref altitude);

      return altitude;
    }

    private static Vector2 GetOffset(float offset)
    {
      var direction = Settings.OffsetDirection.Value.AsVector2;
      return new Vector2(offset * direction.x, offset * direction.y);
    }

    public static void Rebuild() => Dictionary.Values.Do(static list => list.Do(static bubble => bubble.Rebuild()));

    public static void Clear() => Dictionary.Clear();
  }


}
