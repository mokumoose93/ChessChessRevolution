using System.Collections.Generic;
using UnityEngine;

namespace CheckmateInnovations {
    public class Player : MonoBehaviour {

        //*********************************************
        //* VARIABLES
        //*********************************************

        public List<PlayerUnit> units;     // The List of units that this player has
        //? public List<EffectCard> hand;
        public HandManager hand;
        public Canvas handCanvas;
        public int resourceCount;
        string playerName;
        public Transform draftPanelPos;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            playerName = gameObject.name;
        }

        // Update is called once per frame
        void Update() {

        }

        //*********************************************
        //* FUNCTIONS
        //*********************************************

        void PassTurn() {

        }

        // Checks if the player still has a King
        public bool HasKing() {
            foreach (PlayerUnit unit in units) {
                if (unit.unitType == PlayerUnit.UnitType.King) {
                    return true;
                }
            }

            return false;
        }
    }
}