﻿using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class LifeTransmuter : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal LifeTransmuter()
    {
        var lifeTransmuterAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityLifeTransmuterHeightened",  DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetWarList(2,
                FalseLife, // necromancy
                MagicWeapon, // transmutation
                Blindness, // necromancy
                Fly, // transmutation
                BestowCurse, // necromancy
                VampiricTouch, // necromancy
                Blight, // necromancy
                CloudKill) // conjuration)
            .AddToDB();

        // Add transmuter stone like abilities.
        var transmuteForce = FeatureDefinitionPowerPoolBuilder
            .Create("PowerTransmuterHealingPool",  DefinitionBuilder.CENamespaceGuid)
            .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.RechargeRate.LongRest)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Make a power that grants darkvision
        var superiorDarkvision = BuildCondition(RuleDefinitions.DurationType.UntilLongRest,
                1, "ConditionPowerTransmuteDarkvision", FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .SetGuiPresentation("PowerTransmuteDarkvision", Category.Power,
                ConditionDefinitions.ConditionDarkvision.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerDarkvision = BuildActionTransmuteConditionPower(transmuteForce,
                RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None,
                RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, superiorDarkvision,
                "PowerTransmuteDarkvision")
            .SetGuiPresentation(Category.Power,
                FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference)
            .AddToDB();

        var poisonResistance = BuildCondition(
                RuleDefinitions.DurationType.UntilLongRest,
                1, "ConditionPowerTransmutePoison",
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionProtectedFromPoison.GuiPresentation.SpriteReference)
            .AddToDB();

        // Make a power that gives resistance to an elemental damage
        var powerPoison = BuildActionTransmuteConditionPower(
                transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None,
                RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, poisonResistance,
                "PowerTransmutePoison")
            .SetGuiPresentation("PowerTransmuteElementalResistance", Category.Power,
                FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference)
            .AddToDB();

        // Make a power that gives proficiency to constitution saves
        var constitutionProficiency = BuildCondition(RuleDefinitions.DurationType.UntilLongRest,
                1, "ConditionPowerTransmuteConstitution",
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerConstitution = BuildActionTransmuteConditionPower(transmuteForce,
                RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None,
                RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, constitutionProficiency,
                "PowerTransmuteConstitution")
            .SetGuiPresentation(Category.Power,
                FeatureDefinitionPowers.PowerPaladinAuraOfCourage.GuiPresentation.SpriteReference)
            .AddToDB();

        var transmuteForceExtra = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierTransmuterHealingPoolExtra",  DefinitionBuilder.CENamespaceGuid)
            .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                transmuteForce)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerFly = BuildActionTransmuteConditionPower(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.IndividualsUnique, ActionDefinitions.ItemSelectionType.None,
                RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence,
                ConditionDefinitions.ConditionFlying12, "PowerTransmuteFly")
            .SetGuiPresentation(Category.Power, Fly.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerTransmuteHeal",  DefinitionBuilder.CENamespaceGuid)
            .Configure(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1, false, false, AttributeDefinitions.Intelligence,
                MassHealingWord.EffectDescription, false /* unique instance */)
            .SetGuiPresentation(Category.Power, MassHealingWord.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerTransmuteRevive",  DefinitionBuilder.CENamespaceGuid)
            .Configure(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, false, false, AttributeDefinitions.Intelligence,
                Revivify.EffectDescription, false /* unique instance */)
            .SetGuiPresentation(Category.Power, Revivify.GuiPresentation.SpriteReference)
            .AddToDB();

        var transmuteForceExtraBonus = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierTransmuterHealingPoolBonus",  DefinitionBuilder.CENamespaceGuid)
            .Configure(4, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                transmuteForce)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("LifeTransmuter",  DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("TraditionLifeTransmuter", Category.Subclass,
                RoguishDarkweaver.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(lifeTransmuterAffinity, 2)
            .AddFeatureAtLevel(transmuteForce, 6)
            .AddFeatureAtLevel(powerDarkvision, 6)
            .AddFeatureAtLevel(powerPoison, 6)
            .AddFeatureAtLevel(powerConstitution, 6)
            .AddFeatureAtLevel(transmuteForceExtra, 10)
            .AddFeatureAtLevel(powerFly, 10)
            .AddFeatureAtLevel(powerHeal, 10)
            .AddFeatureAtLevel(powerRevive, 10)
            .AddFeatureAtLevel(transmuteForceExtraBonus, 14)
            .AddFeatureAtLevel(FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance, 14)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static ConditionDefinitionBuilder BuildCondition(RuleDefinitions.DurationType durationType,
        int durationParameter,
        string name, params FeatureDefinition[] conditionFeatures)
    {
        return ConditionDefinitionBuilder
            .Create(name,  DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(conditionFeatures)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(durationType, durationParameter, false); // No validation due to existing configuration
    }

    private static FeatureDefinitionPowerSharedPoolBuilder BuildActionTransmuteConditionPower(
        FeatureDefinitionPower poolPower,
        RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
        RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType, RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect, string abilityScore, ConditionDefinition condition,
        string name)
    {
        var effectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false)
            .CreatedByCharacter()
            .Build();

        var effectParticleParameters = new EffectParticleParameters();

        effectParticleParameters.Copy(MagicWeapon.EffectDescription.EffectParticleParameters);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, 1, 0,
                itemSelectionType)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .AddEffectForm(effectForm)
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(effectParticleParameters)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name,  DefinitionBuilder.CENamespaceGuid)
            .Configure(poolPower, recharge, activationTime, costPerUse, false, false, abilityScore,
                effectDescription, false /* unique instance */);
    }
}
