using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {
    public class Pawn : PlayerUnit {
        //*********************************************
        //* VARIABLES
        //*********************************************
        private bool firstMove;     //? Should perform logic checking startingCell instead (found in PlayerUnit.cs)

        public Vector3Int pawnMoveDirection;

        private Dictionary<Vector3Int, PlayerUnit> movesEnPassant;

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        protected override void Start() {
            base.Start();
            firstMove = true;

            unitType = UnitType.Pawn;

            // Set the movement direction for pawns based on the player they are associated with
            //TODO this setup accounts for a static 2 player setting, take into account if including more than 2 players
            switch (playerObject.name) {
                case "Player_1":
                    pawnMoveDirection = Vector3Int.down;
                    break;
                case "Player_2":
                    pawnMoveDirection = Vector3Int.up;
                    break;
                default:
                    Debug.Log("No player found: " + this.gameObject);
                    break;
            }
        }

        //*********************************************
        //* STANDARD CLASS FUNCTIONS
        //*********************************************

        public override List<Vector3Int> CalculateMovementPath() {
            // Initialize relevant values
            Vector3Int nextCellPos;
            List<Vector3Int> movePath = new List<Vector3Int>();
            movesEnPassant = new Dictionary<Vector3Int, PlayerUnit>();

            // Diagonal movement values
            Vector3Int diagonalLeft = pawnMoveDirection + Vector3Int.left;
            Vector3Int diagonalRight = pawnMoveDirection + Vector3Int.right;

            // Get this GameObject's current position in cell coordinates
            Vector3Int objCellPos = tilemap.WorldToCell(gameObject.transform.position);

            // Check if pawn can move forward
            nextCellPos = objCellPos + pawnMoveDirection;
            if (tilemap.HasTile(nextCellPos) && !tilemapBehavior.HasUnit(nextCellPos)) {
                // Add the next forward cell to the path
                movePath.Add(nextCellPos);

                // If it's the pawn's first move, they can move forward an additional cell
                nextCellPos += pawnMoveDirection;
                if (firstMove && tilemap.HasTile(nextCellPos) && !tilemapBehavior.HasUnit(nextCellPos)) {
                    movePath.Add(nextCellPos);
                }
            }

            // Check if pawn can attack or perform "En Passant" diagonally (Up + Left)
            nextCellPos = objCellPos + diagonalLeft;
            if (tilemap.HasTile(nextCellPos)) {
                if (tilemapBehavior.HasUnit(nextCellPos) && IsCapturable(tilemapBehavior.GetUnit(nextCellPos))) {
                    Debug.Log(gameObject + "Can Attack Diagonal Left!");
                    movePath.Add(nextCellPos);
                } else if (tilemapBehavior.HasUnit(objCellPos + Vector3Int.left) && tilemapBehavior.GetUnit(objCellPos + Vector3Int.left).unitType == UnitType.Pawn && IsCapturable(tilemapBehavior.GetUnit(objCellPos + Vector3Int.left))) {
                    Debug.Log(gameObject + "Can En Passant Diagonal Left!");
                    movePath.Add(nextCellPos);
                    movesEnPassant.Add(nextCellPos, tilemapBehavior.GetUnit(objCellPos + Vector3Int.left));
                }
            }

            // Check if pawn can attack or perform "En Passant" diagonally (Up + Right)
            nextCellPos = objCellPos + diagonalRight;
            if (tilemap.HasTile(nextCellPos)) {
                if (tilemapBehavior.HasUnit(nextCellPos) && IsCapturable(tilemapBehavior.GetUnit(nextCellPos))) {
                    Debug.Log(gameObject + "Can Attack Diagonal Right!");
                    movePath.Add(nextCellPos);
                } else if (tilemapBehavior.HasUnit(objCellPos + Vector3Int.right) && tilemapBehavior.GetUnit(objCellPos + Vector3Int.right).unitType == UnitType.Pawn && IsCapturable(tilemapBehavior.GetUnit(objCellPos + Vector3Int.right))) {
                    Debug.Log(gameObject + "Can En Passant Diagonal Right!");
                    movePath.Add(nextCellPos);
                    movesEnPassant.Add(nextCellPos, tilemapBehavior.GetUnit(objCellPos + Vector3Int.right));
                }
            }

            return movePath;
        }

        // Class Override for when En Passant is performed
        public override void MoveToCell(Vector3Int destCellPos) {
            // If the movesEnPassant Dictionary contains destCellPos as a key, then move to that location and capture the piece associated in the pair
            if (movesEnPassant != null && movesEnPassant.ContainsKey(destCellPos)) {
                if (tilemapBehavior.HasUnit(destCellPos)) {
                    if (IsCapturable(movesEnPassant[destCellPos])) {
                        CapturePiece(movesEnPassant[destCellPos]);
                    } else {
                        Debug.Log("Cannot capture your own units!");
                        return;
                    }
                }
                gameObject.transform.position = tilemap.GetCellCenterWorld(destCellPos);

            } else {    // Otherwise just move as normally programmed
                base.MoveToCell(destCellPos);
            }

            firstMove = false;
        }
    }
}