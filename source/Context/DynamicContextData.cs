#nullable enable
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{
  public class DynamicContextData : ContextData, IExpirable
  {
    public event Action<IContext>? OnExpired;

    public static string GetApparelText(Apparel apparel, Pawn pawn)
    {
      return $"{pawn.Name?.ToStringShort ?? pawn.Label} is wearing a {apparel.Label}. {apparel.def.description.RemoveWhiteSpace()}";
    }

    public static DynamicContextData<Apparel>[] CreateApparelContexts(Pawn pawn)
    {
      return DynamicContextData<Apparel>.CreateContexts(
        pawn.apparel.WornApparel,
        pawn,
        (apparel) => GetApparelText(apparel, pawn));
    }

    public static string GetEquipmentText(ThingWithComps equipment, Pawn pawn)
    {
      return $"{pawn.Name?.ToStringShort ?? pawn.Label} has {equipment.Label} equipped. {equipment.def.description.RemoveWhiteSpace()}";
    }

    public static DynamicContextData<ThingWithComps>[] CreateEquipmentContexts(Pawn pawn)
    {
      return DynamicContextData<ThingWithComps>.CreateContexts(
        pawn.equipment.AllEquipmentListForReading,
        pawn,
        (equipment) => GetEquipmentText(equipment, pawn));
    }

    public static DynamicContextData<Hediff>[] CreateHediffsContexts(Pawn pawn)
    {
      return DynamicContextData<Hediff>.CreateContexts(
        pawn.health.hediffSet.hediffs,
        pawn,
        (hediff) => $"{pawn.Name?.ToStringShort ?? pawn.Label} has {hediff.Label} ({hediff.Severity.ToStringPercent()}).");
    }

    public static DynamicContextData<SkillRecord>[] CreateSkillContexts(Pawn pawn)
    {
      return DynamicContextData<SkillRecord>.CreateContexts(
        pawn.skills.skills,
        pawn,
        (skill) => $"{pawn.Name?.ToStringShort ?? pawn.Label} is a {skill.LevelDescriptor.ToLower()} in the skill '{skill.def.label}' - {skill.def.description.RemoveWhiteSpace()}");
    }

    public static DynamicContextData<Trait>[] CreateTraitContexts(Pawn pawn)
    {
      return DynamicContextData<Trait>.CreateContexts(
        pawn.story.traits.allTraits,
        pawn,
        (trait) => $"{pawn.Name?.ToStringShort ?? pawn.Label} has the '{trait.LabelCap}' trait. {trait.TipString(pawn).RemoveWhiteSpace()}");
    }

    public static ContextData[] CreateIdeo(Pawn pawn)
    {
      if (pawn.Ideo == null || !pawn.IsColonist)
        return [];

      var contexts = new List<ContextData>();

      var ideoContext = new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} follows the '{pawn.Ideo.name}' ideology. {pawn.Ideo.description.RemoveWhiteSpace()}",
        "ideo",
        1f,
        pawn);

      if (ideoContext != null)
        contexts.Add(ideoContext);

      contexts.AddRange(DynamicContextData<Precept>.CreateContexts(
        pawn.Ideo.PreceptsListForReading,
        pawn,
        (precept) => $"{pawn.Name?.ToStringShort ?? pawn.Label} follows the precept: '{precept.TipLabel}' - {precept.Description.RemoveWhiteSpace()}"));

      return contexts.ToArray();
    }
    public static ContextData[] CreateAppearance(Pawn pawn)
    {
      if (pawn == null || !pawn.IsColonist)
        return [];

      var contexts = new List<ContextData>();

      // Basic appearance description
      contexts.Add(new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} is {pawn.gender} {pawn.def.label} with {pawn.story?.HairColor.DescribeHairColor() ?? "unknown"} '{pawn.story?.hairDef?.label ?? string.Empty}' hair and {pawn.story?.SkinColor.DescribeSkinColor() ?? "unknown"} skin.",
        "appearance",
        1f,
        pawn));

      //tattoos
      contexts.Add(new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} has " + (pawn.style?.FaceTattoo == TattooDefOf.NoTattoo_Face ? "no" : $"a '{pawn.style?.FaceTattoo?.label}'") + " face tattoo.",
        "tattoos",
        1f,
        pawn));

      contexts.Add(new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} has " + (pawn.style?.BodyTattoo == TattooDefOf.NoTattoo_Body ? "no" : $"a '{pawn.style?.BodyTattoo?.label}'") + " body tattoo.",
        "tattoos",
        1f,
        pawn));

      //beard
      contexts.Add(new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} has " + (pawn.style?.beardDef == BeardDefOf.NoBeard ? "no" : $" a '{pawn.style?.beardDef?.label}'") + " beard.",
        "beard",
        1f,
        pawn));

      //Race
      contexts.Add(new BasicContextData(
        $"{pawn.Name?.ToStringShort ?? pawn.Label} is a {pawn.def.label}. {H.RemoveWhiteSpace(pawn.DescriptionDetailed)}",
        "race",
        1f,
        pawn));

      //role
      var role = pawn.Ideo?.GetRole(pawn);
      if (role != null)
      {
        contexts.Add(new DynamicContextData<Precept_Role>(
          role,
          (role) => $"{pawn.Name?.ToStringShort ?? pawn.Label} has the role of '{role.Label}' in their ideology. {H.RemoveWhiteSpaceAndColor(role.Description)}",
          "role",
          1f,
          pawn));
      }

      //royalty
      if (pawn.royalty != null && pawn.royalty.AllTitlesForReading.Any())
      {
        contexts.Add(new BasicContextData(
          $"{pawn.Name?.ToStringShort ?? pawn.Label} holds the title of '{pawn.royalty.MostSeniorTitle.def.label}' in the '{pawn.royalty.MostSeniorTitle.faction.Name}' faction. {H.RemoveWhiteSpaceAndColor(pawn.royalty.MostSeniorTitle.def.description)}",
          "royalty",
          1f,
          pawn));
      }

      return contexts.ToArray();
    }

    public static ContextData[] CreateFactions()
    {
      var contexts = new List<ContextData>();
      var playerFaction = Faction.OfPlayer;
      if (playerFaction != null)
      {
        contexts.Add(new BasicContextData(
          $"The player's faction is '{playerFaction.Name}'. {H.RemoveWhiteSpaceAndColor(playerFaction.def.description)}",
          "faction",
          1f));
        foreach (var faction in Find.FactionManager.AllFactions.Where(f => f != playerFaction))
        {
          contexts.Add(new BasicContextData(
            $"The '{faction.Name}' faction is {playerFaction.RelationKindWith(faction)} to the player's faction. {H.RemoveWhiteSpaceAndColor(faction.def.description)}",
            "faction",
            1f));
        }
      }
      return contexts.ToArray();
    }

    public DynamicContextData(
      string text,
      string type,
      float weight,
      params Pawn[] pawns) : base(type, weight, pawns)
    {
      Text = text;
    }

    public override string Text { get; protected set; }

    private bool _expired = false;
    public void Expire()
    {
      _expired = true;
    }

    public bool IsExpired => _expired;
  }


  public class DynamicContextData<K> : DynamicContextData
  {
    public static Dictionary<K, DynamicContextData<K>> Contexts = new();

    public static DynamicContextData<K>[] CreateContexts(
      IEnumerable<K> items,
      Pawn pawn,
      Func<K, string> textFunc)
    {
      if (!pawn.IsColonist || !items.Any())
        return Array.Empty<DynamicContextData<K>>();

      var contexts = items
        .Select(item => CreateContext(item, pawn, textFunc))
        .Where(context => context != null)
        .ToArray();

      return contexts!;
    }

    public static DynamicContextData<K>? CreateContext(
      K item,
      Pawn pawn,
      Func<K, string> textFunc)
    {
      if (!pawn.IsColonist || item == null)
        return null;
      var context = new DynamicContextData<K>(
        item,
        textFunc,
        typeof(K).Name,
        1f,
        pawn);
      Contexts[item] = context;
      return context;
    }

    public static void ExpireContext(K item)
    {
      if (Contexts.TryGetValue(item, out var context))
        context.Expire();
    }

    //public static void RefreshContext(K item)
    //{
    //  if (Contexts.TryGetValue(item, out var context))
    //  {
    //    context.Refresh();
    //  }
    //}

    public DynamicContextData(
      K item,
      Func<K, string> textFunc,
      string type,
      float weight,
      params Pawn[] pawns) : base(
        H.RemoveWhiteSpaceAndColor(textFunc(item)),
        type,
        weight,
        pawns)
    {
      TextFunc = textFunc;
      Item = item;
    }

    public K Item { get; }

    public Func<K, string> TextFunc { get; }

  }
}
