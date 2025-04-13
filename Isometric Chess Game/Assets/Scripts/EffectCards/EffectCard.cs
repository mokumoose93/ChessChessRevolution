using UnityEngine;

namespace CheckmateInnovations {

    [CreateAssetMenu(fileName = "New Effect Card", menuName = "Effect Card")]

    public class EffectCard : ScriptableObject {

        public enum CardType {
            Firewall,
            Freeze,
            Ghost
        }

        public string cardName;
        public CardType cardType;
        public Sprite cardImage;
        public string description;
    }
}