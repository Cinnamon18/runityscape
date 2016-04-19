﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CharacterPresenter {
    public Character Character { get; private set; }
    public PortraitView PortraitView { get; private set; }

    public CharacterPresenter(Character character, PortraitView portraitView) {
        this.Character = character;
        this.PortraitView = portraitView;
    }



    public void Tick() {
        //Attempt to set ResourceViews
        PortraitView.SetResources(Character.Resources.Keys.ToArray());

        foreach (KeyValuePair<AttributeType, Attribute> pair in Character.Attributes) {
            PortraitView.SetAttributes(pair.Key, new PortraitView.AttributeBundle { falseValue = pair.Value.False, trueValue = pair.Value.True });
        }

        //Update ResourceViews' values
        foreach (KeyValuePair<ResourceType, Resource> pair in Character.Resources) {
            ResourceView rv = PortraitView.ResourceViews[pair.Key].resourceView;
            ResourceType resType = pair.Key;
            Resource res = pair.Value;

            rv.Text = resType.DisplayFunction(res.False, res.True);
            rv.SetBarScale(res.False, res.True);
        }

        //Set buffs and durations
        PortraitView.SetBuffs(Character.Buffs.Select(s => s.Current).Where(b => b is TimedSpellComponent).Cast<TimedSpellComponent>().Select(b => new PortraitView.BuffParams { id = b.Spell.Id, sprite = b.Sprite, color = b.Color, duration = b.DurationText }).ToArray());
    }
}
