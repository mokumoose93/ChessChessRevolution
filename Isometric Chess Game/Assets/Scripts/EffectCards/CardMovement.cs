using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CheckmateInnovations {
    public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 originalLocalPointerPosition;
        private Vector3 originalPanelLocalPosition;
        private Vector3 originalScale;
        private int currentState = 0;
        private Quaternion originalRotation;
        private Quaternion originalRotationInHand;
        private Vector3 originalPosition;
        private Vector3 originalPositionInHand;
        private HandManager hand;

        [SerializeField] private float animDuration = 0.2f;
        [SerializeField] private float selectScale = 1.5f;
        [SerializeField] private Vector2 cardPlay;
        [SerializeField] private Vector3 playPosition;
        [SerializeField] private GameObject glowEffect;
        [SerializeField] private GameObject playArrow;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            originalScale = rectTransform.localScale;
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            // hand = GetComponentInParent<HandManager>();

            // Get HandManager component from sibling of parent
            int index = transform.parent.GetSiblingIndex();
            hand = transform.parent.parent.GetChild(index - 1).GetComponent<HandManager>();
        }

        void Update() {
            switch (currentState) {
                case 1:
                    HandleHoverState();
                    break;
                case 2:
                    HandleDragState();
                    if (!Input.GetMouseButton(0)) {
                        TransitionToState0();
                    }
                    break;
                case 3:
                    HandlePlayState();
                    if (!Input.GetMouseButton(0)) {
                        TransitionToState0();
                    }
                    break;
                case 4:
                    // HandleActiveState();
                    break;
            }
        }

        private void TransitionToState0() {
            currentState = 0;
            rectTransform.localScale = originalScale;
            rectTransform.localRotation = originalRotation;
            rectTransform.localPosition = originalPosition;
            glowEffect.SetActive(false);
            playArrow.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (currentState == 0) {
                originalPosition = rectTransform.localPosition;
                originalRotation = rectTransform.localRotation;
                originalScale = rectTransform.localScale;

                currentState = 1;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (currentState == 1) {
                currentState = 0;
                TransitionToState0();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (currentState == 1) {
                currentState = 3;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
                originalPanelLocalPosition = rectTransform.localPosition;
            }
        }

        public void OnDrag(PointerEventData eventData) {
            if (currentState == 2) {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition)) {
                    localPointerPosition /= canvas.scaleFactor;

                    Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                    rectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;

                    // Animate to position using LERP
                    // Vector3.Lerp

                    if (rectTransform.localPosition.y > cardPlay.y) {
                        currentState = 3;
                        playArrow.SetActive(true);
                        rectTransform.localPosition = playPosition;
                    }
                }
            }
        }

        private void HandleHoverState() {
            glowEffect.SetActive(true);
            rectTransform.localScale = originalScale * selectScale;
        }

        private void HandleDragState() {
            // Set the card's rotation to zero
            rectTransform.localRotation = Quaternion.identity;
        }

        private void HandlePlayState() {
            // Get card information
            CardDisplay selectedCard = gameObject.GetComponent<CardDisplay>();

            // Handle cards differently based on their state
            switch (selectedCard.cardState) {
                case CardDisplay.CardState.Drafting:
                    HandleDraftSelection(selectedCard);
                    break;
                case CardDisplay.CardState.InHand:
                    HandleHandSelection(selectedCard);
                    break;
                case CardDisplay.CardState.Active:
                    HandleActiveSelection(selectedCard);
                    break;
            }

            //? old code for handling drag state, but planning to remove it - current system skips over state 2 to go directly to the click state
            // if (Input.mousePosition.y < cardPlay.y) {
            //     currentState = 2;
            //     playArrow.SetActive(false);
            // }
        }

        // Add the card to the player's hand
        private void HandleDraftSelection(CardDisplay selectedCard) {
            currentState = 4;
            selectedCard.cardState = CardDisplay.CardState.InHand;

            // Add card to hand
            EffectCard cardData = selectedCard.cardData;
            CardSystemManager.Instance.RegisterCardSelection(cardData);

            // Destroy this instance of the card
            Destroy(gameObject);
        }

        // Handles cards that are selected from hand
        private void HandleHandSelection(CardDisplay selectedCard) {
            currentState = 4;

            if (hand.activeCard == null) {
                // Store position and rotation data from hand if active card needs to be returned to hand
                originalPositionInHand = originalPosition;
                originalRotationInHand = originalRotation;

                // Set the new original position and rotation to play position
                originalPosition = playPosition;
                originalRotation = Quaternion.identity;

                //? hand.activeCard.GetComponent<CardMovement>().HandleActiveSelection(hand.activeCard.GetComponent<CardDisplay>());
                hand.activeCard = selectedCard.gameObject;

                selectedCard.cardState = CardDisplay.CardState.Active;
                StartCoroutine(LerpAnimation(playPosition, Quaternion.identity));
            } else {
                TransitionToState0();
            }
        }

        // Handles cards that are selected in the active position
        private void HandleActiveSelection(CardDisplay selectedCard) {
            currentState = 4;

            // Set the new original position and rotation back to the original position and rotation before set to play position
            originalPosition = originalPositionInHand;
            originalRotation = originalRotationInHand;

            selectedCard.cardState = CardDisplay.CardState.InHand;
            hand.activeCard = null;
            StartCoroutine(LerpAnimation(originalPositionInHand, originalRotationInHand));
        }

        private IEnumerator LerpAnimation(Vector3 targetPosition, Quaternion targetRotation) {
            float elapsedTime = 0f;

            Vector3 startPosition = rectTransform.localPosition;
            Quaternion startRotation = rectTransform.localRotation;

            while (elapsedTime < animDuration) {
                // Lerp position
                rectTransform.localPosition = Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    elapsedTime / animDuration
                );

                // Lerp rotation
                rectTransform.localRotation = Quaternion.Lerp(
                    startRotation,
                    targetRotation,
                    elapsedTime / animDuration
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final values are correct
            rectTransform.localPosition = targetPosition;
            rectTransform.localRotation = targetRotation;

            TransitionToState0();
        }
    }
}