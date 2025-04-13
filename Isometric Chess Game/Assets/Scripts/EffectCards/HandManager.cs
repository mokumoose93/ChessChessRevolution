using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace CheckmateInnovations {
    public class HandManager : MonoBehaviour {

        public Player player;           //? Player that this handmanager is associated with
        public DeckManager deckManager;
        public GameObject cardPrefab;
        public GameObject activeCard;
        public Transform handTransform; // Root of the hand position
        public float fanSpread = 5f;    // How much hand is spread out
        public List<GameObject> cardsInHand = new List<GameObject>();   // Holds a list of the card objects in player's hand
        public float cardSpacing = -350f;
        public float verticalSpacing = 100f;
        public float animDuration = 0.2f;

        void Start() {
            activeCard = null;

            //? Add cards to hand for debugging purposes
            AddCardToHand();
            AddCardToHand();
            AddCardToHand();
            AddCardToHand();
            AddCardToHand();
        }

        void Update() {
            // UpdateHandVisuals();
        }

        public void AddCardToHand() {
            // Generate Card types randomly since type is not specified
            EffectCard[] allCardTypes = CardSystemManager.Instance.allCardTypes;
            EffectCard randomCardType = allCardTypes[UnityEngine.Random.Range(0, allCardTypes.Length)];

            // Instantiate the card and initialize the display
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
            cardDisplay.Initialize(randomCardType, CardSystemManager.Instance, CardDisplay.CardState.InHand);

            cardsInHand.Add(newCard);

            UpdateHandVisuals();
        }

        public void AddCardToHand(EffectCard card) {
            //Instantiate the card with specified type
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
            cardDisplay.Initialize(card, CardSystemManager.Instance, CardDisplay.CardState.InHand);

            cardsInHand.Add(newCard);

            UpdateHandVisuals();
        }

        private void UpdateHandVisuals() {
            int cardCount = cardsInHand.Count;

            // Special Case for when there is only 1 card in player's hand
            if (cardCount == 1) {
                cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
                return;
            }

            for (int i = 0; i < cardCount; i++) {
                float rotationAngle = fanSpread * (i - cardCount - 1) / 2f;
                // cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);

                float horizontalOffset = cardSpacing * (i - cardCount - 1) / 2f;

                float normalizedPosition = 2f * i / (cardCount - 1) - 1f;   // Normalize card position between -1 and 1
                float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

                // Set card position
                // cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
                Vector3 targetPosition = new Vector3(horizontalOffset, verticalOffset, 0f);

                StartCoroutine(LerpAnimation(cardsInHand[i], targetPosition, targetRotation));
            }
        }

        private IEnumerator LerpAnimation(GameObject cardObj, Vector3 targetPosition, Quaternion targetRotation) {
            float elapsedTime = 0f;

            Transform cardTransform = cardObj.transform;
            Vector3 startPosition = cardTransform.localPosition;
            Quaternion startRotation = cardTransform.localRotation;

            while (elapsedTime < animDuration) {
                // Lerp position
                cardTransform.localPosition = Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    elapsedTime / animDuration
                );

                // Lerp rotation
                cardTransform.localRotation = Quaternion.Lerp(
                    startRotation,
                    targetRotation,
                    elapsedTime / animDuration
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final values are correct
            cardTransform.localPosition = targetPosition;
            cardTransform.localRotation = targetRotation;
        }
    }
}