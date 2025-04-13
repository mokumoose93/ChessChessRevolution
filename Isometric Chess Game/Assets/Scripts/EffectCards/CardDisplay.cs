using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace CheckmateInnovations {
    public class CardDisplay : MonoBehaviour {

        public enum CardState {
            Drafting,
            InHand,
            Active
        }

        public CardState cardState;
        public EffectCard cardData;
        public Image cardImage;
        private Player _assignedPlayer;
        private EffectCard _assignedCard;
        private CardSystemManager _selector;

        public void Initialize(Player player, EffectCard card, CardSystemManager selector, CardState state) {
            _assignedPlayer = player;
            cardData = card;
            _selector = selector;
            cardState = state;

            cardImage.sprite = card.cardImage;

            // _button.onClick.AddListener(OnCardClicked);
        }

        public void Initialize(EffectCard card, CardSystemManager selector, CardState state) {
            cardData = card;
            _selector = selector;
            cardState = state;

            cardImage.sprite = card.cardImage;
        }

        // private void OnCardClicked() {
        //     // Notify selector
        //     _selector.RegisterCardSelection(_assignedPlayer, _assignedCard);
        // }
    }
}