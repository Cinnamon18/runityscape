﻿using Scripts.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Scripts.Model.Items {

    public sealed class EquipType : IComparable<EquipType>, INameable {
        private static readonly HashSet<EquipType> allTypes = new HashSet<EquipType>();

        public static readonly EquipType WEAPON = new EquipType("Weapon");
        public static readonly EquipType OFFHAND = new EquipType("Offhand");
        public static readonly EquipType ARMOR = new EquipType("Armor");
        public static readonly EquipType TRINKET = new EquipType("Trinket");

        private static int idCounter;

        public readonly string Name;
        private int id;

        private EquipType(string name) {
            this.Name = name;
            this.id = idCounter++;
            allTypes.Add(this);
        }

        public static ReadOnlyCollection<EquipType> AllTypes {
            get {
                return new ReadOnlyCollection<EquipType>(allTypes.ToArray());
            }
        }

        string INameable.Name {
            get {
                return Name;
            }
        }

        public override bool Equals(object obj) {
            return this == obj;
        }

        public override int GetHashCode() {
            return id;
        }

        public override string ToString() {
            return this.Name;
        }

        int IComparable<EquipType>.CompareTo(EquipType other) {
            return this.Name.CompareTo(other.Name);
        }
    }
}