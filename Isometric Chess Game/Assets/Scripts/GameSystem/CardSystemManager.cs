using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

namespace CheckmateInnovations {
    public class CardSystemManager : MonoBehaviour {

        public static CardSystemManager Instance;
        public UnityEvent<EffectCard> OnCardSelected = new UnityEvent<EffectCard>();
        public GameObject screenBlurPrefab;
        public GameObject cardPrefab;
        public Canvas screenBlurCanvas;
        public Transform draftPanel;
        public EffectCard[] allCardTypes;
        private List<GameObject> currentDraftOptions = new List<GameObject>();
        private List<EffectCard> randomCardList = new List<EffectCard>();
        private List<EffectCard> playerHand;
        public Player currentPlayer;

        public int cardDraftCount = 3;
        public float horizontalSpacing = 5f;

        private bool _awaitingSelection;
        private EffectCard _selectedCard;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            screenBlurCanvas.gameObject.SetActive(false);
        }

        void Update() {
            // UpdateDraftVisuals();
        }

        // Create draft panel for player to choose from
        public IEnumerator PromptPlayerDraftSelection(Player player) {
            _awaitingSelection = true;
            _selectedCard = null;
            currentPlayer = player;
            draftPanel = player.draftPanelPos;

            // Blur the background
            screenBlurCanvas.gameObject.SetActive(true);
            //TODO animate blur effect

            // Generate new draft options
            foreach (EffectCard card in randomCardList) {
                GameObject cardObj = Instantiate(cardPrefab, draftPanel);
                CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
                cardDisplay.Initialize(card, this, CardDisplay.CardState.Drafting);
                currentDraftOptions.Add(cardObj);
            }
            UpdateDraftVisuals();

            // Await card selection from player
            while (_awaitingSelection) {
                yield return null;
            }

            // Clear draft when done
            foreach (Transform child in draftPanel) {
                Destroy(child.gameObject);
            }
            currentDraftOptions.Clear();

            // Un-blur the background
            screenBlurCanvas.gameObject.SetActive(false);
            Debug.Log($"{player} chose: {_selectedCard.cardName}");
        }

        // Generates a draft of cards using random values
        public void GenerateCardDraft() {
            // Clear previous draft
            randomCardList.Clear();

            // Generate new draft randomly
            for (int i = 0; i < cardDraftCount; i++) {
                EffectCard randomCardType = allCardTypes[UnityEngine.Random.Range(0, allCardTypes.Length)];
                randomCardList.Add(randomCardType);
            }
        }

        public void UpdateDraftVisuals() {
            // If there are no items in the draft, return
            if (currentDraftOptions == null) {
                return;
            }

            // Position according to position in the draft
            for (int i = 0; i < currentDraftOptions.Count; i++) {
                float horizontalOffset = horizontalSpacing * (i - cardDraftCount - 1) / 2f;
                currentDraftOptions[i].transform.localPosition = new Vector3(horizontalOffset, 0f, 0f);
            }
        }

        public void RegisterCardSelection(EffectCard selectedCard) {
            if (!_awaitingSelection) return;

            _selectedCard = selectedCard;
            _awaitingSelection = false;
            OnCardSelected.Invoke(selectedCard);
        }


    }
}