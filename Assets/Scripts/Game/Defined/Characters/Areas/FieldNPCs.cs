﻿using Scripts.Game.Defined.Serialized.Items;
using Scripts.Game.Defined.Serialized.Spells;
using Scripts.Game.Defined.Serialized.Statistics;
using Scripts.Game.Serialized;
using Scripts.Game.Serialized.Brains;
using Scripts.Game.Shopkeeper;
using Scripts.Model.Characters;
using Scripts.Model.Items;
using Scripts.Model.Pages;
using UnityEngine;

namespace Scripts.Game.Defined.Characters {

    public static class FieldNPCs {

        public static Shop AppleDealer(Page previous, Flags flags, Party party) {
            return new Shop(
                previous,
                "Apples",
                flags,
                party,
                0.5f,
                1f,
                Villager())
                .AddTalks(new Talk("Test", "<a>Buy some apples."))
                .AddBuys(new Buy(new Apple()));
        }

        public static Character Villager() {
            return CharacterUtil.StandardEnemy(
                new Stats(2, 1, 1, 1, 2),
                new Look(
                    "Ghost",
                    "haunting",
                    "A villager who didn't make it.",
                    Breed.SPIRIT
                    ),
                new Attacker())
                .AddItem(new Money(), Util.Range(0, 3));
        }

        public static Character Knight() {
            return CharacterUtil.StandardEnemy(
                new Stats(3, 1, 2, 2, 5),
                new Look(
                    "Spectre",
                    "spectre",
                    "A knight who didn't make it. May be armed.",
                    Breed.SPIRIT
                    ),
                new Attacker())
                .AddItem(new Item[] { new BrokenSword(), new GhostArmor() }.ChooseRandom(), Util.IsChance(.50f));
        }

        public static Character BigKnight() {
            return CharacterUtil.StandardEnemy(
                new Stats(3, 1, 2, 2, 15),
                new Look(
                    "Big Knight",
                    "spectre",
                    "It's a big guy.",
                    Breed.SPIRIT
                    ),
                new BigKnight())
                .AddStats(new Skill())
                .AddSpells(new SetupCounter());
        }

        public static Character Healer() {
            return CharacterUtil.StandardEnemy(
                new Stats(3, 1, 5, 5, 1),
                new Look(
                    "Spirit Healer",
                    "health-normal",
                    "Healer in life. Healer in death.",
                    Breed.SPIRIT
                    ),
                new Healer())
                .AddItem(new Apple())
                .AddSpells(new Heal());
        }

        public static Character Illusionist() {
            return CharacterUtil.StandardEnemy(
                new Stats(3, 2, 3, 8, 15),
                new Look(
                    "Illusionist",
                    "spectre",
                    "A wicked master of illusions.",
                    Breed.SPIRIT
                    ),
                new Illusionist())
                .AddSpells(new Blackout());
        }

        private static Look ReplicantLook() {
            return new Look(
                    "Replika",
                    "spectre",
                    string.Empty,
                    Breed.SPIRIT,
                    Color.magenta
                    );
        }

        public static Character Replicant() {
            return CharacterUtil.StandardEnemy(
                new Stats(10, 2, 5, 10, 30),
                ReplicantLook(),
                new Replicant()
                )
            .AddFlags(Model.Characters.Flag.PERSISTS_AFTER_DEFEAT)
            .AddSpells(new ReflectiveClone(), new SetupCounter());
        }
    }
}