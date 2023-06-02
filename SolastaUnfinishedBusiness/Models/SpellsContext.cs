﻿using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Spells.SpellBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellsContext
{
    internal static readonly Dictionary<SpellListDefinition, SpellListContext> SpellListContextTab = new();

    internal static readonly SpellListDefinition EmptySpellList = SpellListDefinitionBuilder
        .Create("SpellListEmpty")
        .SetGuiPresentationNoContent(true)
        .ClearSpells()
        .FinalizeSpells(false)
        .AddToDB();

    // ReSharper disable once InconsistentNaming
    private static readonly SortedList<string, SpellListDefinition> spellLists = new();
    private static readonly Dictionary<SpellDefinition, List<SpellListDefinition>> SpellSpellListMap = new();

    internal static readonly SpellDefinition BanishingSmite = BuildBanishingSmite();
    internal static readonly SpellDefinition BlindingSmite = BuildBlindingSmite();
    internal static readonly SpellDefinition BurstOfRadiance = BuildBurstOfRadiance();
    internal static readonly SpellDefinition ColorBurst = BuildColorBurst();
    internal static readonly SpellDefinition ElementalWeapon = BuildElementalWeapon();
    internal static readonly SpellDefinition EnduringSting = BuildEnduringSting();
    internal static readonly SpellDefinition EnsnaringStrike = BuildEnsnaringStrike();
    internal static readonly SpellDefinition FarStep = BuildFarStep();
    internal static readonly SpellDefinition SearingSmite = BuildSearingSmite();
    internal static readonly SpellDefinition StaggeringSmite = BuildStaggeringSmite();
    internal static readonly SpellDefinition SunlightBlade = BuildSunlightBlade();
    internal static readonly SpellDefinition ThunderousSmite = BuildThunderousSmite();
    internal static readonly SpellDefinition Web = BuildWeb();
    internal static readonly SpellDefinition Wrack = BuildWrack();
    internal static readonly SpellDefinition WrathfulSmite = BuildWrathfulSmite();

    // ReSharper disable once MemberCanBePrivate.Global
    internal static HashSet<SpellDefinition> Spells { get; set; } = new();

    [NotNull]
    internal static SortedList<string, SpellListDefinition> SpellLists
    {
        get
        {
            if (spellLists.Count != 0)
            {
                return spellLists;
            }

            // only this sub matters for spell selection. this might change if we add additional subs to mod
            var characterSubclass = DatabaseHelper.CharacterSubclassDefinitions.TraditionLight;

            var title = characterSubclass.FormatTitle();

            var featureDefinitions = characterSubclass.FeatureUnlocks
                .Select(x => x.FeatureDefinition)
                .Where(x => x is FeatureDefinitionCastSpell or FeatureDefinitionMagicAffinity);

            foreach (var featureDefinition in featureDefinitions)
            {
                switch (featureDefinition)
                {
                    case FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity
                        when featureDefinitionMagicAffinity.ExtendedSpellList != null &&
                             !spellLists.ContainsValue(featureDefinitionMagicAffinity.ExtendedSpellList):
                        spellLists.Add(title, featureDefinitionMagicAffinity.ExtendedSpellList);

                        foreach (var spell in featureDefinitionMagicAffinity.ExtendedSpellList.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.ContainsKey(spell))
                            {
                                SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                            }

                            SpellSpellListMap[spell].Add(featureDefinitionMagicAffinity.ExtendedSpellList);
                        }

                        break;
                    case FeatureDefinitionCastSpell featureDefinitionCastSpell
                        when featureDefinitionCastSpell.SpellListDefinition != null &&
                             !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition):
                        spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                        foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.ContainsKey(spell))
                            {
                                SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                            }

                            SpellSpellListMap[spell].Add(featureDefinitionCastSpell.SpellListDefinition);
                        }

                        break;
                }
            }

            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            foreach (var characterClass in dbCharacterClassDefinition)
            {
                title = characterClass.FormatTitle();

                var featureDefinitionCastSpell = characterClass.FeatureUnlocks
                    .Select(x => x.FeatureDefinition)
                    .OfType<FeatureDefinitionCastSpell>()
                    .FirstOrDefault();

                // NOTE: don't use featureDefinitionCastSpell?. which bypasses Unity object lifetime check
                if (!featureDefinitionCastSpell
                    || !featureDefinitionCastSpell.SpellListDefinition
                    || spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                {
                    continue;
                }

                spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                             x => x.Spells))
                {
                    if (!SpellSpellListMap.ContainsKey(spell))
                    {
                        SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                    }

                    SpellSpellListMap[spell].Add(featureDefinitionCastSpell.SpellListDefinition);
                }
            }

            return spellLists;
        }
    }

    internal static bool IsAllSetSelected()
    {
        return SpellListContextTab.Values.All(spellListContext => spellListContext.IsAllSetSelected);
    }

    internal static bool IsSuggestedSetSelected()
    {
        return SpellListContextTab.Values.All(spellListContext => spellListContext.IsSuggestedSetSelected);
    }

    internal static void SelectAllSet(bool toggle)
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.SelectAllSetInternal(toggle);
        }
    }

    internal static void SelectSuggestedSet(bool toggle)
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.SelectSuggestedSetInternal(toggle);
        }
    }

    internal static void LateLoad()
    {
        // init collections
        foreach (var spellList in SpellLists.Values)
        {
            var name = spellList.Name;

            SpellListContextTab.Add(spellList, new SpellListContext(spellList));

            Main.Settings.SpellListSpellEnabled.TryAdd(name, new List<string>());
            Main.Settings.DisplaySpellListsToggle.TryAdd(name, true);
            Main.Settings.SpellListSliderPosition.TryAdd(name, 4);
        }

        var spellListInventorClass = InventorClass.SpellList;

        // MUST COME BEFORE ANY MOD REGISTERED SPELL
        AllowAssigningOfficialSpells();

        // Dead Master Spells
        WizardDeadMaster.DeadMasterSpells.Do(x => RegisterSpell(x));

        // cantrips
        RegisterSpell(BuildAcidClaw(), 0, SpellListDruid);
        RegisterSpell(BuildAirBlast(), 0, SpellListBard, SpellListCleric, SpellListDruid, SpellListSorcerer,
            SpellListWizard);
        RegisterSpell(BuildBladeWard(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BurstOfRadiance, 0, SpellListCleric);
        RegisterSpell(EnduringSting, 0, SpellListWizard);
        RegisterSpell(BuildIlluminatingSphere(), 0, SpellListBard, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMindSpike(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildMinorLifesteal(), 0, SpellListBard, SpellListSorcerer, SpellListWizard, SpellListWarlock);
        RegisterSpell(BuildResonatingStrike(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(SunlightBlade, 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildSwordStorm(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildTollTheDead(), 0, SpellListCleric, SpellListWizard, SpellListWarlock);
        RegisterSpell(BuildThornyVines(), 0, SpellListDruid, spellListInventorClass);
        RegisterSpell(BuildThunderStrike(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(Wrack, 0, SpellListCleric);

        // 1st level
        RegisterSpell(BuildCausticZap(), 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildChromaticOrb(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildEarthTremor(), 0, SpellListBard, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildFindFamiliar(), 0, SpellListWizard);
        RegisterSpell(BuildGiftOfAlacrity(), 0, SpellListWizard);
        RegisterSpell(EnsnaringStrike, 0, SpellListRanger);
        RegisterSpell(BuildMagnifyGravity(), 0, SpellListWizard);
        RegisterSpell(BuildMule(), 0, SpellListWizard);
        RegisterSpell(BuildRadiantMotes(), 0, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildSanctuary(), 0, SpellListCleric, spellListInventorClass);
        RegisterSpell(SearingSmite, 0, SpellListPaladin, SpellListRanger);
        RegisterSpell(BuildSkinOfRetribution(), 0, SpellListWarlock);
        RegisterSpell(ThunderousSmite, 0, SpellListPaladin);
        RegisterSpell(WrathfulSmite, 0, SpellListPaladin);

        // 2nd level
        RegisterSpell(BuildBindingIce(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(ColorBurst, 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildPetalStorm(), 0, SpellListDruid);
        RegisterSpell(BuildProtectThreshold(), 0, SpellListCleric, SpellListDruid, SpellListPaladin);
        RegisterSpell(BuildMirrorImage(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildShadowBlade(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(Web, 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);

        // 3rd level
        RegisterSpell(BlindingSmite, 0, SpellListPaladin);
        RegisterSpell(BuildCrusadersMantle(), 0, SpellListPaladin);
        RegisterSpell(ElementalWeapon, 0, SpellListCleric, SpellListPaladin);
        RegisterSpell(BuildPulseWave(), 0, SpellListWizard);
        RegisterSpell(BuildSpiritShroud(), 0, SpellListCleric, SpellListPaladin, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildWinterBreath(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 4th level
        RegisterSpell(BuildBrainBulwark(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildGravitySinkhole(), 0, SpellListWizard);
        RegisterSpell(StaggeringSmite, 0, SpellListPaladin);

        //5th level
        RegisterSpell(BanishingSmite, 0, SpellListPaladin);
        RegisterSpell(FarStep, 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildMantleOfThorns(), 0, SpellListDruid);
        RegisterSpell(BuildSonicBoom(), 0, SpellListSorcerer, SpellListWizard);

        // 6th level
        // RegisterSpell(BuildGravityFissure(), 0, SpellListWizard);

        // 7th level
        RegisterSpell(BuildReverseGravity(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 8th level
        RegisterSpell(BuildMindBlank(), 0, SpellListBard, SpellListWizard);

        // 9th level
        RegisterSpell(BuildForesight(), 0, SpellListBard, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMassHeal(), 0, SpellListCleric);
        RegisterSpell(BuildMeteorSwarmSingleTarget(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildPowerWordHeal(), 0, SpellListBard, SpellListCleric);
        RegisterSpell(BuildPowerWordKill(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildTimeStop(), 0, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildShapechange(), 0, SpellListDruid, SpellListWizard);
        RegisterSpell(BuildWeird(), 0, SpellListWizard);

        Spells = Spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.FormatTitle()).ToHashSet();

        foreach (var kvp in SpellListContextTab)
        {
            // caches which spells are toggleable per spell list
            var spellListContext = kvp.Value;

            spellListContext.CalculateAllSpells();

            // settings paring
            var spellListName = kvp.Key.Name;

            foreach (var name in Main.Settings.SpellListSpellEnabled[spellListName]
                         .Where(name => Spells.All(x => x.Name != name))
                         .ToList())
            {
                Main.Settings.SpellListSpellEnabled[spellListName].Remove(name);
            }
        }
    }

    internal static void SwitchAllowAssigningOfficialSpells()
    {
        if (Main.Settings.AllowAssigningOfficialSpells)
        {
            return;
        }

        foreach (var spellList in SpellLists.Values)
        {
            var name = spellList.Name;

            Main.Settings.SpellListSpellEnabled[name].RemoveAll(x =>
                DatabaseHelper.GetDefinition<SpellDefinition>(x).ContentPack != CeContentPackContext.CeContentPack);
        }
    }

    private static void AllowAssigningOfficialSpells()
    {
        foreach (var kvp in SpellSpellListMap)
        {
            RegisterSpell(kvp.Key, kvp.Value.Count, kvp.Value.ToArray());
        }
    }

    private static void RegisterSpell(
        SpellDefinition spellDefinition,
        int suggestedStartsAt = 0,
        params SpellListDefinition[] registeredSpellLists)
    {
        if (Spells.Contains(spellDefinition))
        {
            return;
        }

        Spells.Add(spellDefinition);

        for (var i = 0; i < registeredSpellLists.Length; i++)
        {
            var spellList = registeredSpellLists[i];

            if (i < suggestedStartsAt)
            {
                SpellListContextTab[spellList].MinimumSpells.Add(spellDefinition);
            }
            else
            {
                SpellListContextTab[spellList].SuggestedSpells.Add(spellDefinition);
            }
        }

        foreach (var spellList in SpellLists.Values)
        {
            if (!Main.Settings.SpellListSpellEnabled.ContainsKey(spellList.Name))
            {
                continue;
            }

            if (SpellListContextTab[spellList].MinimumSpells.Contains(spellDefinition))
            {
                continue;
            }

            var enable = Main.Settings.SpellListSpellEnabled[spellList.Name].Contains(spellDefinition.Name);

            SpellListContextTab[spellList].Switch(spellDefinition, enable);
        }

        var isActiveInAtLeastOneRepertoire = SpellLists.Values.Any(x => x.ContainsSpell(spellDefinition));

        if (!isActiveInAtLeastOneRepertoire || spellDefinition.contentPack != CeContentPackContext.CeContentPack)
        {
            return;
        }

        //Add cantrips to `All Cantrips` list, so that Warlock's `Pact of the Tome` and Loremaster's `Arcane Professor` would see them
        if (spellDefinition.SpellLevel == 0)
        {
            SpellListAllCantrips.AddSpell(spellDefinition);
        }

        //Add spells to `All Spells` list, so that Warlock's `Book of Ancient Secrets` and Bard's `Magic Secrets` would see them
        SpellListAllSpells.AddSpell(spellDefinition);

        //Add spells to Snipers lists
        var spellSniperClasses = new List<CharacterClassDefinition>
        {
            Cleric,
            Druid,
            Sorcerer,
            Warlock,
            Wizard
        };

        foreach (var spellSniperClass in spellSniperClasses)
        {
            if (spellDefinition.SpellLevel == 0 &&
                DatabaseHelper.TryGetDefinition<SpellListDefinition>(
                    $"SpellListFeatSpellSniper{spellSniperClass.Name}", out var spellListSniper))
            {
                spellListSniper.AddSpell(spellDefinition);
            }
        }
    }

    internal sealed class SpellListContext
    {
        internal SpellListContext(SpellListDefinition spellListDefinition)
        {
            SpellList = spellListDefinition;
            AllSpells = new HashSet<SpellDefinition>();
            MinimumSpells = new HashSet<SpellDefinition>();
            SuggestedSpells = new HashSet<SpellDefinition>();
        }

        private List<string> SelectedSpells => Main.Settings.SpellListSpellEnabled[SpellList.Name];
        private SpellListDefinition SpellList { get; }
        internal HashSet<SpellDefinition> AllSpells { get; }
        internal HashSet<SpellDefinition> MinimumSpells { get; }
        internal HashSet<SpellDefinition> SuggestedSpells { get; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsAllSetSelected => SelectedSpells.Count == AllSpells
            .Count(x => Main.Settings.AllowAssigningOfficialSpells ||
                        x.ContentPack == CeContentPackContext.CeContentPack);

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsSuggestedSetSelected => SelectedSpells.Count == SuggestedSpells.Count
                                                && SuggestedSpells.All(x => SelectedSpells.Contains(x.Name));

        internal void CalculateAllSpells()
        {
            var minSpellLevel = SpellList.HasCantrips ? 0 : 1;
            var maxSpellLevel = SpellList.MaxSpellLevel;

            AllSpells.Clear();

            foreach (var spell in Spells
                         .Where(x => x.SpellLevel >= minSpellLevel && x.SpellLevel <= maxSpellLevel &&
                                     !MinimumSpells.Contains(x)))
            {
                AllSpells.Add(spell);
            }
        }

        internal void SelectAllSetInternal(bool toggle)
        {
            foreach (var spell in AllSpells)
            {
                Switch(spell, toggle);
            }
        }

        internal void SelectSuggestedSetInternal(bool toggle)
        {
            if (toggle)
            {
                SelectAllSetInternal(false);
            }

            foreach (var spell in SuggestedSpells)
            {
                Switch(spell, toggle);
            }
        }

        internal void Switch([NotNull] SpellDefinition spellDefinition, bool active)
        {
            var spellListName = SpellList.Name;
            var spellName = spellDefinition.Name;

            if (!SpellList.HasCantrips && spellDefinition.SpellLevel == 0)
            {
                return;
            }

            InventorClass.SwitchSpellStoringItemSubPower(spellDefinition, active);

            if (!Main.Settings.AllowAssigningOfficialSpells &&
                spellDefinition.ContentPack != CeContentPackContext.CeContentPack)
            {
                return;
            }

            if (active)
            {
                SpellList.AddSpell(spellDefinition);

                Main.Settings.SpellListSpellEnabled[spellListName].TryAdd(spellName);
            }
            else
            {
                foreach (var spellsByLevel in SpellList.SpellsByLevel)
                {
                    spellsByLevel.Spells.RemoveAll(x => x == spellDefinition);
                }

                Main.Settings.SpellListSpellEnabled[spellListName].Remove(spellName);
            }
        }
    }
}
