﻿using Scripts.Model.Characters;
using Scripts.Model.World.Serialization;
using Scripts.Model.Pages;
using Scripts.Model.Processes;
using Scripts.View.ActionGrid;
using System.Collections.Generic;
using System.IO;
using Scripts.Presenter;

namespace Scripts.Model.World.Pages {

    public class NewGamePage : ReadPage {
        private const int LOWER_CHARACTER_LIMIT = 4;

        public NewGamePage(Page back) : base(
            "Names must be at least 4 letters long.",
            "What is your name?",
            "Name Selection",
            true) {
            PlayerCharacter hero = new Hero();
            AddCharacters(false, hero);
            OnTickAction = () => {
                hero.Name = this.InputtedString;
            };

            ActionGrid[0] = new Process("Confirm", "Choose this name.", () => Game.Instance.CurrentPage = new IntroScenePage(new Party(hero)), () => nameMeetsRequirements);
            ActionGrid[ActionGridView.TOTAL_BUTTON_COUNT - 1] = new Process("Back", "", () => Game.Instance.CurrentPage = back);
        }

        private bool nameMeetsRequirements {
            get {
                return InputtedString.Length >= LOWER_CHARACTER_LIMIT;
            }
        }
    }
}