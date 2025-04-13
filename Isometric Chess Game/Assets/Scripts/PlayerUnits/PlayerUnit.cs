using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Codice.CM.Client.Differences;
using System.Text.RegularExpressions;

namespace CheckmateInnovations {
    public abstract class PlayerUnit : MonoBehaviour {

        public enum UnitType {
            Pawn,
            Rook,
            Knight,
            Bishop,
            Queen,
            King
        }

        public UnitType unitType;
        public TilemapBehavior tilemapBehavior;             // TilemapBehavior class instantiation associated with tilemap behavior

        private SpriteRenderer spriteRenderer;
        public Tilemap tilemap;                             // Tilemap that this unit is positioned on
        public GameObject playerObject;
        protected Vector3Int startingCell;

        public Player player;                           // The player that this unit belongs to

        private bool isSelected = false;
        private bool isHovering = false;

        public int maxBoundX;
        public int maxBoundY;
        public int minBoundX;
        public int minBoundY;

        private float transformScale = 1.2f;

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start() {
            // Assign Components
            tilemap = GetComponentInParent<Tilemap>();
            tilemapBehavior = GetComponentInParent<TilemapBehavior>();      //! Unit must be a child of the tilemap in the hierarchy for this to work
            spriteRenderer = GetComponent<SpriteRenderer>();
            player = playerObject.GetComponent<Player>();

            // Assign values of Unity Class Variables
            startingCell = tilemap.WorldToCell(gameObject.transform.position);

            // Assign simple variables
            maxBoundX = tilemap.cellBounds.xMax;
            maxBoundY = tilemap.cellBounds.yMax;
            minBoundX = tilemap.cellBounds.xMin;
            minBoundY = tilemap.cellBounds.yMin;
        }

        // Update is called once per frame
        void Update() {

        }

        //*********************************************
        //* STANDARD CLASS FUNCTIONS
        //*********************************************

        public virtual void MoveToCell(Vector3Int destCellPos) {
            // Move from current position to destination position "destCellPos"
            Vector3Int currCellPos = tilemap.WorldToCell(gameObject.transform.position);
            PlayerUnit targetPiece = tilemapBehavior.GetUnit(destCellPos);

            // Check if destination already has a unit on it, if there is a unit already there, check if capturable, if capturable, capture
            if (tilemapBehavior.HasUnit(destCellPos)) {
                if (IsCapturable(targetPiece)) {
                    CapturePiece(targetPiece);
                    TilemapBehavior.Instance.OnUnitCapture?.Invoke(this, targetPiece);
                } else {
                    Debug.Log("Cannot capture your own units!");
                    return;
                }
            }

            TilemapBehavior.Instance.OnUnitMoved?.Invoke();

            gameObject.transform.position = tilemap.GetCellCenterWorld(destCellPos);
            Debug.Log(gameObject + " moved to " + tilemap.GetCellCenterWorld(destCellPos));
        }

        public bool IsCapturable(PlayerUnit target) {
            Debug.Log(target + "Is Capturable!");
            // Check if target piece is from a different player, return true, otherwise return false
            if (this.playerObject != target.playerObject) {
                return true;
            } else {
                return false;
            }

            //TODO implement other conditions? what if piece is same player but swappable, etc.?
        }

        //TODO finish this function
        public void CapturePiece(PlayerUnit target) {
            Debug.Log("Capturing " + target);
            Vector3Int targetPos = tilemap.WorldToCell(target.transform.position);

            // Destroy the target object and then update game
            target.Death();

            //TODO some kind of UpdateGame() updating game points and captured pieces list for the score board
        }

        // Perform necessary functions to indicate that this piece has encountered Death (Destroyed)
        public void Death() {
            Debug.Log(this + "is calling Death()");
            //TODO DeathAnimation();
            Destroy(this.gameObject);
        }

        public abstract List<Vector3Int> CalculateMovementPath();

        //*********************************************
        //* EVENT FUNCTIONS
        //*********************************************

        // void OnMouseEnter() {
        //     //TODO 1. need visual feedback when the mouse is hovering over a chess piece
        //     //TODO 2. be able to click on the chess piece and "select" it
        //     //TODO 3. move the piece by clicking somewhere else
        //     //TODO 4. proper piece pathing

        //     // Give visual feedback when mouse hovers over object
        //     transform.localScale *= transformScale;
        //     isHovering = true;
        // }

        // void OnMouseExit() {
        //     transform.localScale /= transformScale;
        //     isHovering = false;
        // }

        // void OnMouseDown() {
        //     //TODO probably best place to handle unit selection in movement since it requires a collider (this prevents unneccessary duplicate calls)



        //     // Set the current piece as the currently selected piece on the board
        //     if (isHovering) {   // Check that this is the piece to receive MouseDown action
        //         if (!cursorResponse.hasSelectedPiece) {
        //             cursorResponse.hasSelectedPiece = true; // Let the associated tilemap know that a piece is selected
        //         }

        //         cursorResponse.selectedObject = this.gameObject; // Assign this piece as the selected piece on the tilemap
        //     }

        // }
    }
}