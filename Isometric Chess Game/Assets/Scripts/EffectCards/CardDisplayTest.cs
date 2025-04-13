using UnityEngine;
using UnityEngine.UI;

namespace CheckmateInnovations {
    public class CardDisplayTest : MonoBehaviour {

        public EffectCard cardData;

        public Image cardImage;
        public Text nameText;
        public Image[] typeImages;

        void Start() {
            UpdateCardDisplay();
        }

        public void UpdateCardDisplay() {
            // nameText.text = cardData.cardName;

        }
    }
}