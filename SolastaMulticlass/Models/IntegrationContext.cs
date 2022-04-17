﻿namespace SolastaMulticlass.Models
{
    internal static class IntegrationContext
    {
        //public const string CLASS_ALCHEMIST = "AlchemistClass";
        //public const string CLASS_BARD = "BardClass";
        //public const string CLASS_MONK = "MonkClass";
        public const string CLASS_TINKERER = "ClassTinkerer";
        public const string CLASS_WARDEN = "ClassWarden";
        public const string CLASS_WARLOCK = "ClassWarlock";
        public const string CLASS_WITCH = "ClassWitch";

        internal static CharacterClassDefinition DummyClass { get; private set; } = new()
        {
            name = "DummyClass"
        };

        //internal static CharacterClassDefinition AlchemistClass { get; private set; }
        //internal static CharacterClassDefinition BardClass { get; private set; }
        //internal static CharacterClassDefinition MonkClass { get; private set; }
        internal static CharacterClassDefinition TinkererClass { get; private set; }
        internal static CharacterClassDefinition WardenClass { get; private set; }
        internal static CharacterClassDefinition WarlockClass { get; private set; }
        internal static CharacterClassDefinition WitchClass { get; private set; }

        internal static void Load()
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            //dbCharacterClassDefinition.TryGetElement(CLASS_ALCHEMIST, out var unofficialAlchemist);
            //dbCharacterClassDefinition.TryGetElement(CLASS_BARD, out var unofficialBard);
            //dbCharacterClassDefinition.TryGetElement(CLASS_MONK, out var unofficialMonk);
            dbCharacterClassDefinition.TryGetElement(CLASS_TINKERER, out var unofficialTinkerer);
            dbCharacterClassDefinition.TryGetElement(CLASS_WARDEN, out var unofficialWarden);
            dbCharacterClassDefinition.TryGetElement(CLASS_WARLOCK, out var unofficialWarlock);
            dbCharacterClassDefinition.TryGetElement(CLASS_WITCH, out var unofficialWitch);

            // NOTE: don't use ?? here which bypasses Unity object lifetime check

            //AlchemistClass = unofficialAlchemist ? unofficialAlchemist : DummyClass;
            //BardClass = unofficialBard ? unofficialBard : DummyClass;
            //MonkClass = unofficialMonk ? unofficialMonk : DummyClass;
            TinkererClass = unofficialTinkerer ? unofficialTinkerer : DummyClass;
            WardenClass = unofficialWarden ? unofficialWarden : DummyClass;
            WitchClass = unofficialWitch ? unofficialWitch : DummyClass;
            WarlockClass = unofficialWarlock ? unofficialWarlock : DummyClass;
        }
    }
}
