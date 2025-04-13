using CheckmateInnovations;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitMovement : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    private int currentState = 0;
    private Vector3 originalScale;
    private Material originalMaterial;
    private Player player;

    [SerializeField] private float selectScale = 1.5f;
    [SerializeField] private Material glowMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        originalScale = transform.localScale;
        originalMaterial = gameObject.GetComponent<SpriteRenderer>().material;
        player = gameObject.GetComponent<PlayerUnit>().player;
    }

    // Update is called once per frame
    void Update() {
        if (player.ToString() == GameManager.Instance.currentPlayer.ToString()) {
            switch (currentState) {
                case 1:
                    HandleHoverState();
                    break;
                case 2:
                    HandleClickState();
                    break;
            }
        } else {
            Debug.Log("player: " + player);
            Debug.Log("currentPlayer: " + GameManager.Instance.currentPlayer);
            Debug.Log("Values not equal!");
        }
    }

    //TODO
    private void TransitionToState0() {
        Debug.Log("Transition to State 0 event");
        currentState = 0;
        transform.localScale = originalScale;
        gameObject.GetComponent<SpriteRenderer>().material = originalMaterial;
    }

    //TODO
    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Pointer Enter Event");
        if (currentState == 0) {
            //? originalScale = transform.localScale;

            currentState = 1;
        }
    }

    //TODO
    public void OnPointerExit(PointerEventData eventData) {
        if (currentState == 1) {
            currentState = 0;   //? Possibly redundant, leaving here for now to avoid possible issues
            TransitionToState0();
        }
    }

    //TODO
    public void OnPointerDown(PointerEventData eventData) {
        if (currentState == 1) {
            currentState = 2;
        }
    }

    private void HandleHoverState() {
        gameObject.GetComponent<SpriteRenderer>().material = glowMaterial;
        transform.localScale = originalScale * selectScale;
    }

    private void HandleClickState() {
        transform.localScale = originalScale;
        TransitionToState0();
    }
}
