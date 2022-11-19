﻿using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfGuts : AbstractSubclass
{
    internal CollegeOfGuts()
    {
        var proficiencyCollegeOfGutsArmor = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyClericArmor, "ProficiencyCollegeOfGutsArmor")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var proficiencyCollegeOfGutsWeapon = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyFighterWeapon, "ProficiencyCollegeOfGutsWeapon")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var f = FeatureDefinitionFeatureSetBuilder
            .Create("ProficiencyCollegeOfGutsFighting")
            .AddFeatureSet(proficiencyCollegeOfGutsArmor, proficiencyCollegeOfGutsWeapon)
            .AddToDB();

        var magicAffinityCollegeOfGutsCombatMagic = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCollegeOfGutsCombatMagic")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
            .AddToDB();

        var attributeModifierCollegeOfGutsExtraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierCollegeOfGutsExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

        var powerCollegeOfGutsWarMagic = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfGutsWarMagic")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSpellCast)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, validateDuration: false)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionCollegeOfGutsWarMagic")
                                    .SetGuiPresentationNoContent(true)
                                    .AddFeatures(FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .Build())
            .AddToDB();

        var replaceAttackWithCantripCollegeOfGuts = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripCollegeOfGuts")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfGuts")
            .SetGuiPresentation(Category.Subclass, DomainBattle)
            .AddFeaturesAtLevel(3,
                proficiencyCollegeOfGutsArmor,
                proficiencyCollegeOfGutsWeapon,
                magicAffinityCollegeOfGutsCombatMagic)
            .AddFeaturesAtLevel(6,
                attributeModifierCollegeOfGutsExtraAttack,
                replaceAttackWithCantripCollegeOfGuts)
            .AddFeaturesAtLevel(14,
                powerCollegeOfGutsWarMagic)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;
}
