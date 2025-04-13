using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {
    public class Knight : PlayerUnit {
        //*********************************************
        //* VARIABLES
        //*********************************************

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        protected override void Start() {
            base.Start();

            unitType = UnitType.Knight;
        }

        //*********************************************
        //* STANDARD CLASS FUNCTIONS
        //*********************************************

        public override List<Vector3Int> CalculateMovementPath() {
            // Initialize relevant values
            Vector3Int nextCellPos;
            List<Vector3Int> movePath = new List<Vector3Int>();

            // Get this GameObject's current position in cell coordinates
            Vector3Int objCellPos = tilemap.WorldToCell(gameObject.transform.position);

            // Check path forward and left
            nextCellPos = objCellPos + (Vector3Int.up * 2) + Vector3Int.left;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path forward and right
            nextCellPos = objCellPos + (Vector3Int.up * 2) + Vector3Int.right;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path left and forward
            nextCellPos = objCellPos + (Vector3Int.left * 2) + Vector3Int.up;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path left and backward
            nextCellPos = objCellPos + (Vector3Int.left * 2) + Vector3Int.down;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path right and forward
            nextCellPos = objCellPos + (Vector3Int.right * 2) + Vector3Int.up;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path right and backward
            nextCellPos = objCellPos + (Vector3Int.right * 2) + Vector3Int.down;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path backward and left
            nextCellPos = objCellPos + (Vector3Int.down * 2) + Vector3Int.left;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            // Check path backward and right
            nextCellPos = objCellPos + (Vector3Int.down * 2) + Vector3Int.right;
            if (tilemap.HasTile(nextCellPos)) {
                movePath.Add(nextCellPos);
            }

            return movePath;
        }
    }
}