﻿using Scripts.Model.Characters;
using Scripts.Model.Stats;
using Scripts.View.Portraits;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Scripts.Model.Spells {

    public struct SpellParams {
        public readonly Characters.Stats Stats;
        public readonly Characters.Spells Spells;
        public readonly Characters.Buffs Buffs;

        public SpellParams(Character c) {
            this.Stats = c.Stats;
            this.Spells = c.Spells;
            this.Buffs = c.Buffs;
        }
    }

    public abstract class SpellBook {
        public readonly string Name;
        public readonly Sprite Icon;
        public readonly TargetType TargetType;
        public readonly SpellType SpellType;
        public readonly IDictionary<StatType, int> Costs;
        public readonly int Priority;

        public int CastTime;
        public int Cooldown;
        public bool IsSilenced;

        public SpellBook(string spellName, string spriteLoc, TargetType target, SpellType spell, int castTime, int cooldown) {
            this.Name = spellName;
            this.Icon = Util.LoadIcon(spriteLoc);
            this.Costs = new Dictionary<StatType, int>();
            this.TargetType = target;
            this.SpellType = spell;
            this.CastTime = castTime;
            this.Cooldown = cooldown;
        }

        public override bool Equals(object obj) {
            SpellBook item = obj as SpellBook;

            if (item == null) {
                return false;
            }

            return this.Name == item.Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public string CreateDescription(SpellParams caster) {
            return string.Format("{0}Target: {1}\nCost: {2}\n\n{3}",
                CasterHasResources(caster.Stats) ? string.Empty : Util.ColorString("Insufficient resource.\n\n", Color.red),
                TargetType.Name,
                Costs.Count == 0 ? "None" : GetCommaSeparatedCosts(caster.Stats),
                CreateDescriptionHelper(caster)
                );
        }

        public string CreateTargetDescription(Character caster, Character target) {
            return string.Format("{0} will use {1} on {2}.\n\n{3}", caster.Look.DisplayName, this.Name, target.Look.DisplayName, CreateDescription(new SpellParams(caster)));
        }

        public abstract string CreateDescriptionHelper(SpellParams caster);

        public bool CasterHasResource(StatType stat, Characters.Stats caster) {
            return caster.GetStatCount(stat) >= Costs[stat];
        }

        public bool CasterHasResources(Characters.Stats caster) {
            foreach (KeyValuePair<StatType, int> stat in Costs) {
                if (caster.GetStatCount(Value.MOD, stat.Key) < stat.Value) {
                    return false;
                }
            }
            return true;
        }

        public bool IsCastable(SpellParams caster, SpellParams target) {
            return CasterHasResources(caster.Stats) && IsMeetOtherCastRequirements(caster, target) && caster.Spells.HasSpellBook(this);
        }

        public Spell BuildSpell(Character caster, Character target) {
            Util.Assert(
                IsCastable(new SpellParams(caster), new SpellParams(target)),
                string.Format(
                "Attempted to cast spell without requirements fulfilled. Resources={0}, OtherRequirements={1}."
                , CasterHasResources(caster.Stats),
                IsMeetOtherCastRequirements(new SpellParams(caster), new SpellParams(target))
                ));

            // Consume resources
            foreach (KeyValuePair<StatType, int> pair in Costs) {
                caster.Stats.AddToStat(pair.Key, -pair.Value);
            }

            return ForceSpell(caster, target);
        }

        public Spell ForceSpell(Character caster, Character target) {
            Characters.Stats casterStats = caster.Stats;
            Characters.Stats targetStats = target.Stats;

            Result res = new Result();
            if (IsHit(new SpellParams(caster), new SpellParams(target))) {
                res.AddSFX(GetHitSFX(caster.Presenter.PortraitView, target.Presenter.PortraitView));
                if (IsCritical(new SpellParams(caster), new SpellParams(target))) {
                    res.Type = ResultType.CRITICAL;
                    res.Effects = GetCriticalEffects(new SpellParams(caster), new SpellParams(target));
                } else {
                    res.Type = ResultType.HIT;
                    res.Effects = GetHitEffects(new SpellParams(caster), new SpellParams(target));
                }
            } else {
                res.AddSFX(GetMissSFX(caster.Presenter.PortraitView, target.Presenter.PortraitView));
                res.Type = ResultType.MISS;
                res.Effects = GetMissEffects(new SpellParams(caster), new SpellParams(target));
            }

            return new Spell(this, res, caster, target);
        }

        protected virtual bool IsMeetOtherCastRequirements(SpellParams caster, SpellParams target) {
            return true;
        }

        protected virtual bool IsHit(SpellParams caster, SpellParams target) {
            return true;
        }

        protected abstract IList<SpellEffect> GetHitEffects(SpellParams caster, SpellParams target);

        protected virtual bool IsCritical(SpellParams caster, SpellParams target) {
            return false;
        }

        protected virtual IList<SpellEffect> GetCriticalEffects(SpellParams caster, SpellParams target) {
            return new SpellEffect[0];
        }

        protected virtual IList<SpellEffect> GetMissEffects(SpellParams caster, SpellParams target) {
            return new SpellEffect[0];
        }

        protected virtual IList<IEnumerator> GetHitSFX(PortraitView caster, PortraitView target) {
            return new IEnumerator[0];
        }

        protected virtual IList<IEnumerator> GetMissSFX(PortraitView caster, PortraitView target) {
            return new IEnumerator[0];
        }

        private string GetCommaSeparatedCosts(Characters.Stats caster) {
            string[] arr = new string[Costs.Count];

            int index = 0;
            foreach (KeyValuePair<StatType, int> pair in Costs) {
                arr[index++] = string.Format("{0} {1}",
                    Util.ColorString(pair.Value.ToString(), CasterHasResource(pair.Key, caster)),
                    Util.ColorString(pair.Key.Name, pair.Key.Color));
            }
            return string.Join(", ", arr);
        }
    }
}