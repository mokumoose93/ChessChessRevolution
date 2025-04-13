using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

//TODO implement turn handling for players

namespace CheckmateInnovations {
    public class GameManager : MonoBehaviour {

        public Canvas canvas;

        public enum GameState {
            Player1Turn,
            Player2Turn,
            CardDraft,
            GameOver
        }

        public enum GameMode {
            Standard,
            CaptureAll,
            KingHealth
        }

        public static GameManager Instance { get; private set; }

        public GameState CurrentState;
        public GameMode SelectedGameMode;

        public UnityEvent OnPlayer1TurnStart;
        public UnityEvent OnPlayer2TurnStart;
        public UnityEvent OnGameOver;

        List<Player> players = new List<Player>();
        public GameObject player1Obj, player2Obj;
        private Player player1, player2;
        public Player currentPlayer;

        public int round;
        public int draftInterval = 2;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            round = 0;

            player1 = player1Obj.GetComponent<Player>();
            player2 = player2Obj.GetComponent<Player>();

            players.Add(player1);
            players.Add(player2);

            InitializeGame();
        }

        private void OnEnable() {
            // AnnouncementManager.Instance.OnPieceSelected.AddListener();
            TilemapBehavior.Instance.OnUnitMoved.AddListener(HandleUnitMove);
            TilemapBehavior.Instance.OnUnitCapture.AddListener(HandleUnitCapture);
            CardSystemManager.Instance.OnCardSelected.AddListener(HandleCardSelected);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void InitializeGame() {
            // Start with Player 1's turn
            StartPlayer1Turn();
        }

        public void StartPlayer1Turn() {
            CurrentState = GameState.Player1Turn;
            SetPlayerUIActive(player1);
            OnPlayer1TurnStart?.Invoke();
            currentPlayer = player1;
            // EnablePlayerInput(true);
        }

        public void StartPlayer2Turn() {
            CurrentState = GameState.Player2Turn;
            SetPlayerUIActive(player2);
            OnPlayer2TurnStart?.Invoke();
            currentPlayer = player2;
            // EnablePlayerInput(false);
        }

        public void EndGame() {
            CurrentState = GameState.GameOver;
            OnGameOver?.Invoke();
            Debug.Log("Game has ended.");
        }

        private bool CheckWinCondition(Player targetPlayer) {
            // Switch Case according to GameMode
            switch (SelectedGameMode) {
                case GameMode.Standard:
                    // Check if the King was captured
                    if (!targetPlayer.HasKing()) {
                        return true;
                    }
                    break;
                case GameMode.KingHealth:
                    //TODO implement winning conditions for KingHealth
                    break;
                case GameMode.CaptureAll:
                    if (targetPlayer.units.Count == 0) {
                        return true;
                    }
                    break;
                default:
                    Debug.Log("No Game Mode Detected");
                    break;
            }

            return false;
        }

        // End a Players turn, Switch to other player
        public void EndPlayerTurn() {
            // Announce End of Turn
            AnnouncementManager.Instance.AnnounceEndTurn();

            // Switch GameState from current player to the next player
            if (CurrentState == GameState.Player1Turn) {
                StartPlayer2Turn();
            } else if (CurrentState == GameState.Player2Turn) {
                StartNewRound();
                StartPlayer1Turn();
            }
        }

        private void EnablePlayerMovement(bool enabled) {
            TilemapBehavior.Instance.SetInputEnabled(enabled);
        }

        // Starts a new Round
        public void StartNewRound() {
            // Increment the Round Counter
            round += 1;

            // Check for Card Draft this round
            if (round % draftInterval == 0) {
                // StartCardDraft();
                StartCoroutine(StartCardDraft());
            }
        }

        // Starts a card drafting sequence
        // TODO 1. disable input for tilemap OR preferably disable input for all systems other than CardSystemManager
        // TODO 2. create a screen for card selection that allows players to select a card after checking if the player has appropriate resources
        // TODO 3. re-enable player movement input
        public IEnumerator StartCardDraft() {
            // Initiate Card Draft Game State + Announce that it is time for a Card Draft!
            CurrentState = GameState.CardDraft;
            AnnouncementManager.Instance.Announce("Starting Card Draft!");

            // Temporarily disable player movement from tilemap input
            EnablePlayerMovement(false);

            // Generate a draft of effect cards for each player to choose
            CardSystemManager.Instance.GenerateCardDraft();

            // Player 1 Draft Sequence
            currentPlayer = player1;
            SetPlayerUIActive(player1);
            AnnouncementManager.Instance.Announce("Player 1 Select From Card Draft!");
            yield return CardSystemManager.Instance.PromptPlayerDraftSelection(player1);

            // Player 2 Draft Sequence
            currentPlayer = player2;
            SetPlayerUIActive(player2);
            AnnouncementManager.Instance.Announce("Player 2 Select From Card Draft!");
            yield return CardSystemManager.Instance.PromptPlayerDraftSelection(player2);

            // Re-enable player movement from tilemap input
            SetPlayerUIActive(player1);
            currentPlayer = player1;
            EnablePlayerMovement(true);
        }

        public void SetPlayerUIActive(Player player) {
            ClearAllActivePlayerUI();
            player.handCanvas.gameObject.SetActive(true);
        }

        public void ClearAllActivePlayerUI() {
            foreach (Player player in players) {
                player.handCanvas.gameObject.SetActive(false);
            }
        }

        //****************
        //* EVENT HANDLING
        //****************

        // What the Game System needs to do when a unit is moved
        private void HandleUnitMove() {

            EndPlayerTurn();
        }

        // What the Game System needs to do when a unit is captured
        private void HandleUnitCapture(PlayerUnit attackUnit, PlayerUnit targetUnit) {
            // Announce that a Unit was captured
            AnnouncementManager.Instance.AnnounceCapture(attackUnit, targetUnit);

            // Check for winning condition, End game if condition is met
            if (CheckWinCondition(targetUnit.player)) {
                EndGame();
            }

            //TODO implement other code to be called when a unit is captured

        }

        private void HandleCardSelected(EffectCard selectedCard) {
            // Add card to player's hand
            //? player.hand.Add(selectedCard);
            //? player.hand.AddCardToHand(selectedCard);
            currentPlayer.hand.AddCardToHand(selectedCard);
        }
    }
}