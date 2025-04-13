using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace CheckmateInnovations {
    public class Bishop : PlayerUnit {
        //*********************************************
        //* VARIABLES
        //*********************************************

        protected override void Start() {
            base.Start();

            unitType = UnitType.Bishop;
        }

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

            // Calculate diagonal path up & left
            nextCellPos = objCellPos + diagonalUpLeft;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += diagonalUpLeft;
            }

            // Calculate diagonal path up & right
            nextCellPos = objCellPos + diagonalUpRight;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += diagonalUpRight;
            }

            // Calculate diagonal path down & left
            nextCellPos = objCellPos + diagonalDownLeft;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += diagonalDownLeft;
            }

            // Calculate diagonal path down & right
            nextCellPos = objCellPos + diagonalDownRight;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += diagonalDownRight;
            }

            return movePath;
        }
    }
}