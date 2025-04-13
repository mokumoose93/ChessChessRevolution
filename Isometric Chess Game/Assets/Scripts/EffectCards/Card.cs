using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {

    // [CreateAssetMenu(fileName = "New Card", menuName = "Card")]

    public class Card : ScriptableObject {
        public string cardName;
        public List<CardType> cardTypes;
        public int health;
        public int damageMin;
        public int damageMax;
        public List<DamageType> damageTypes;

        public enum CardType {
            Fire,
            Earth,
            Water,
            Dark,
            Light,
            Air
        }

        public enum DamageType {
            Fire,
            Earth,
            Water,
            Dark,
            Light,
            Air
        }
    }
}