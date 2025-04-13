using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {
    public class Queen : PlayerUnit {

        //*********************************************
        //* VARIABLES
        //*********************************************

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        protected override void Start() {
            base.Start();

            unitType = UnitType.Queen;
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

            // Check forward moves until either a unit or tilemap bounds is reached
            nextCellPos = objCellPos + Vector3Int.up;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += Vector3Int.up;
            }

            // Check back moves until either a unit or tilemap bounds is reached        
            nextCellPos = objCellPos + Vector3Int.down;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += Vector3Int.down;
            }

            // Check left moves until either a unit or tilemap bounds is reached
            nextCellPos = objCellPos + Vector3Int.left;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += Vector3Int.left;
            }

            // Check left moves until either a unit or tilemap bounds is reached
            nextCellPos = objCellPos + Vector3Int.right;
            while (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
                if (tilemapBehavior.HasUnit(nextCellPos))
                    break;
                nextCellPos += Vector3Int.right;
            }

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