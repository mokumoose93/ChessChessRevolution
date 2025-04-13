using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {
    public class King : PlayerUnit {
        //*********************************************
        //* VARIABLES
        //*********************************************



        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        protected override void Start() {
            base.Start();

            unitType = UnitType.King;
        }

        //*********************************************
        //* STANDARD CLASS FUNCTIONS
        //*********************************************

        public override List<Vector3Int> CalculateMovementPath() {
            // Initialize relevant values
            Vector3Int nextCellPos;
            List<Vector3Int> movePath = new List<Vector3Int>();

            // Diagonal Directions
            Vector3Int diagonalUpLeft = Vector3Int.up + Vector3Int.left;
            Vector3Int diagonalUpRight = Vector3Int.up + Vector3Int.right;
            Vector3Int diagonalDownLeft = Vector3Int.down + Vector3Int.left;
            Vector3Int diagonalDownRight = Vector3Int.down + Vector3Int.right;

            // Get this GameObject's current position in cell coordinates
            Vector3Int objCellPos = tilemap.WorldToCell(gameObject.transform.position);

            // Check path forward
            nextCellPos = objCellPos + Vector3Int.up;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path backward
            nextCellPos = objCellPos + Vector3Int.down;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path left
            nextCellPos = objCellPos + Vector3Int.left;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path right
            nextCellPos = objCellPos + Vector3Int.right;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check diagonal path up & left
            nextCellPos = objCellPos + diagonalUpLeft;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check diagonal path up & right
            nextCellPos = objCellPos + diagonalUpRight;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check diagonal path down & left
            nextCellPos = objCellPos + diagonalDownLeft;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check diagonal path down & right
            nextCellPos = objCellPos + diagonalDownRight;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            return movePath;
        }

        //TODO Write code that allows king to perform a castle
        public void PerformCastle() {
            // Check if King can castle, then perform castle if possible
            if (CastleCheck()) {

            }
        }

        public bool CastleCheck() {
            // If King and relevant Rook are still in their starting positions, and there are no units in the way, King can castle
            Vector3Int placeholder;
            if (this.gameObject.transform.position == this.startingCell) {
                Debug.Log(this.gameObject + "at Starting Position"); //TODO
            }

            return true;    //! placeholder return statement
        }
    }
}