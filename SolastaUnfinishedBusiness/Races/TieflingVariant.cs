﻿using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Races;

internal static class TieflingRaceBuilder
{
    internal static CharacterRaceDefinition RaceTiefling { get; } = BuildTiefling();

    [NotNull]
    private static CharacterRaceDefinition BuildTiefling()
    {
        #region subraces

        // BACKWARD COMPATIBILITY
        var attributeModifierTieflingIntelligenceAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingIntelligenceAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 1)
            .AddToDB();

        var castSpellTieflingAsmodeus = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingAsmodeus")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(SpellListDefinitionBuilder
                .Create("SpellListTieflingAsmodeus")
                .SetGuiPresentationNoContent(true)
                .ClearSpells()
                .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
                .SetSpellsAtLevel(1, SpellDefinitions.HellishRebuke)
                .SetSpellsAtLevel(2, SpellDefinitions.Darkness)
                .FinalizeSpells(true, 2)
                .AddToDB())
            .AddToDB();

        var raceTieflingAsmodeus = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingAsmodeus")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingIntelligenceAbilityScoreIncrease,
                castSpellTieflingAsmodeus)
            .AddToDB();

        raceTieflingAsmodeus.GuiPresentation.hidden = true;
        raceTieflingAsmodeus.contentPack = GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        // END BACKWARD COMPATIBILITY
        var castSpellTieflingDevilTongue = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingDevilTongue")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(SpellListDefinitionBuilder
                .Create("SpellListTieflingDevilTongue")
                .SetGuiPresentationNoContent(true)
                .ClearSpells()
                .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                .SetSpellsAtLevel(1, SpellDefinitions.CharmPerson)
                .SetSpellsAtLevel(2, SpellDefinitions.CalmEmotions)
                .FinalizeSpells(true, 2)
                .AddToDB())
            .AddToDB();

        var raceTieflingDevilTongue = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingDevilTongue")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingIntelligenceAbilityScoreIncrease,
                castSpellTieflingDevilTongue)
            .AddToDB();

        raceTieflingDevilTongue.contentPack = GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        //
        // Mephistopheles
        //

        var attributeModifierTieflingDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingDexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        var castSpellTieflingMephistopheles = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingMephistopheles")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(SpellListDefinitionBuilder
                .Create("SpellListTieflingMephistopheles")
                .SetGuiPresentationNoContent(true)
                .ClearSpells()
                .SetSpellsAtLevel(0, SpellDefinitions.FireBolt)
                .SetSpellsAtLevel(1, SpellDefinitions.BurningHands)
                .SetSpellsAtLevel(2, SpellDefinitions.FlameBlade)
                .FinalizeSpells(true, 2)
                .AddToDB())
            .AddToDB();

        var raceTieflingMephistopheles = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingMephistopheles")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingDexterityAbilityScoreIncrease,
                castSpellTieflingMephistopheles)
            .AddToDB();

        raceTieflingMephistopheles.contentPack = GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        //
        // Zariel
        //

        var attributeModifierTieflingStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var castSpellTieflingZariel = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingZariel")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(SpellListDefinitionBuilder
                .Create("SpellListTieflingZariel")
                .SetGuiPresentationNoContent(true)
                .ClearSpells()
                .SetSpellsAtLevel(0, SpellsContext.SunlightBlade)
                .SetSpellsAtLevel(1, SpellsContext.SearingSmite)
                .SetSpellsAtLevel(2, SpellDefinitions.BrandingSmite)
                .FinalizeSpells(true, 2)
                .AddToDB())
            .AddToDB();

        var raceTieflingZariel = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingZariel")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingStrengthAbilityScoreIncrease,
                castSpellTieflingZariel)
            .AddToDB();

        raceTieflingZariel.contentPack = GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        #endregion

        // BACKWARD COMPATIBILITY
        _ = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingCharismaAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Charisma, 2)
            .AddToDB();

        _ = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityTieflingHellishResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        _ = LanguageDefinitionBuilder
            .Create("LanguageInfernal")
            .SetGuiPresentation(Category.Language)
            .AddToDB();

        _ = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyTieflingLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "LanguageInfernal")
            .AddToDB();
        // END BACKWARD COMPATIBILITY

        for (var level = 17; level <= 20; level++)
        {
            castSpellTieflingAsmodeus.slotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = castSpellTieflingAsmodeus.slotsPerLevels[15].slots
            });

            castSpellTieflingMephistopheles.slotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = castSpellTieflingMephistopheles.slotsPerLevels[15].slots
            });

            castSpellTieflingZariel.slotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = castSpellTieflingZariel.slotsPerLevels[15].slots
            });
        }

        var raceTiefling = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTiefling")
            .SetOrUpdateGuiPresentation("Tiefling", Category.Race)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                FeatureSetTieflingHellishResistance,
                AttributeModifierTieflingAbilityScoreIncreaseCha,
                ProficiencyTieflingStaticLanguages)
            .AddToDB();

        raceTiefling.subRaces.SetRange(raceTieflingMephistopheles, raceTieflingZariel, raceTieflingDevilTongue);
        raceTiefling.contentPack = GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        return raceTiefling;
    }
}
