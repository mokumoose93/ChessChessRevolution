using UnityEngine;
using UnityEngine.Events;

namespace CheckmateInnovations {
    public class AnnouncementManager : MonoBehaviour {

        // Singleton pattern
        public static AnnouncementManager Instance { get; private set; }

        public UnityEvent OnPieceSelected;

        private TextMesh textMesh;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void Announce(string message) {
            //? using Debug.Log() for now until I get TextMeshPro to work
            Debug.Log("ANNOUNCEMENT: " + message);
        }

        //TODO Announce Capture
        public void AnnounceCapture(PlayerUnit attackUnit, PlayerUnit targetUnit) {

        }

        public void AnnounceEndTurn() {

        }

        public void AnnounceCardDraft() {

        }
    }
}