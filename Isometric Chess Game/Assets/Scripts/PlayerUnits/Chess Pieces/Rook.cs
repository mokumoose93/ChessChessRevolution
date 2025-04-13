using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace CheckmateInnovations {
    public class Rook : PlayerUnit {

        //*********************************************
        //* VARIABLES
        //*********************************************

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        protected override void Start() {
            base.Start();

            unitType = UnitType.Rook;
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

            return movePath;
        }

    }
}