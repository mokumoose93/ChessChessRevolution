using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Client.Differences;
using log4net.DateFormatter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

//TODO implement system that handles player turns
//TODO write code that makes individual tiles in a tilemap respond when hovered over by cursor (move tile up or have sprite appear, etc.)
//TODO write code that interpolates piece movement over time to essentially animate the movement smoothly

namespace CheckmateInnovations {
    public class TilemapBehavior : MonoBehaviour {

        //*********************************************
        //* VARIABLES
        //*********************************************
        public static TilemapBehavior Instance { get; private set; }

        public UnityEvent OnUnitMoved;
        public UnityEvent<PlayerUnit, PlayerUnit> OnUnitCapture; //TODO UnityEvent OnUnitMoved also accounts for capture but that can be handled with a separate event

        public PlayerUnit selectedUnit;
        private Tilemap tilemap;
        public GameObject selectedObject;   //? Redundant to have selectedObject AND selectedUnit
        public GameObject hoveringObject;
        public GameObject pathTile;
        public Material activeMaterial;
        public Material inactiveMaterial;

        public List<Vector3Int> movePath;
        public List<Vector3Int> lastMovePath;
        public List<GameObject> movePathTiles;

        private Vector3Int currentMouseCellPos;
        private Vector3Int lastMouseCellPos;

        public float transformScale = 1.5f;
        public float translateAmount;
        public bool isHovering;
        public bool pathIsDrawn;
        public bool _inputEnabled;

        //*********************************************
        //* START + UPDATE FUNCTIONS
        //*********************************************

        void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            // Get Tilemap that is associated with GameObject that this script is attached as a component to
            tilemap = this.GetComponent<Tilemap>();
            selectedObject = null;
            selectedUnit = null;
            pathIsDrawn = false;
            _inputEnabled = true;
        }

        // Update is called once per frame
        void Update() {
            //TODO check for GameState (player turn, etc.)
            // GameManager.Instance.TestFunction();
            // GameManager.Instance.OnPlayer1TurnStart.Invoke();

            // Get mouse position from screen to world to tilemap cell coordinates
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);   //? probably obsolete

            // Update the Render Order for all the GameObjects on this tilemap
            UpdateRenderOrder();

            // Check that input on the tilemap is enabled
            if (_inputEnabled) {
                // Perform Actions for Left Mouse Click (GetMouseButtonDown(0))
                if (Input.GetMouseButtonDown(0)) {
                    MouseClickLeft(mouseWorldPos);
                }

                if (Input.GetMouseButtonDown(1)) {
                    MouseClickRight(mouseWorldPos);
                }
            }

            // Variable Post-Condition Updates
            lastMouseCellPos = cellPosition;
            if (movePath != null) {
                lastMovePath = movePath;
            }
        }

        //*********************************************
        //* STANDARD CLASS FUNCTIONS
        //*********************************************

        // Get position of all GameObjects on Tilemap in Tilemap Cell Coordinates and calculate render order based on position
        void UpdateRenderOrder() {
            // Vector3Int cellPosition;
            int spriteOrder = 0;    // Ordering starts at zero and counts upwards (anything < 0 will render behind the tilemap)

            // Retrieve all children into a list
            List<Transform> childList = new List<Transform>();

            foreach (Transform child in tilemap.transform) {
                childList.Add(child);
            }

            // Sort the list first by Y, then by X
            childList.Sort((t1, t2) => {
                Vector3Int t1Cell = tilemap.WorldToCell(t1.position);
                Vector3Int t2Cell = tilemap.WorldToCell(t2.position);
                int yComparison = t1Cell.y.CompareTo(t2Cell.y);
                if (yComparison != 0) {
                    return yComparison;
                } else {
                    return t1Cell.x.CompareTo(t2Cell.x);
                }
            });

            childList.Reverse();    // Lowest values should be rendered last as they "appear closest"; Reverse the order

            // Assign Render Order in order of the sorted list
            foreach (Transform child in childList) {
                SpriteRenderer spriteRenderer = child.gameObject.GetComponent<SpriteRenderer>();    // Get the Associated Sprite Renderer
                if (spriteRenderer != null) {   // Assign sprite order value and increment
                    spriteRenderer.sortingOrder = spriteOrder;
                    spriteOrder++;
                }
            }
        }

        //TODO transforming the localScale seems to cause issues with collider detection, how to resolve this?
        void SelectObject(GameObject obj) {
            if (selectedObject != null) {
                DeselectObject();
            }
            selectedObject = obj;
            selectedObject.GetComponent<SpriteRenderer>().material = activeMaterial;
            // selectedObject.transform.localScale *= transformScale;
            Debug.Log("Selected: " + selectedObject.name);
        }

        void DeselectObject() {
            if (selectedObject != null) {
                Debug.Log("Deselected: " + selectedObject.name);
                selectedObject.GetComponent<SpriteRenderer>().material = inactiveMaterial;
                // selectedObject.transform.localScale /= transformScale;
                selectedObject = null;
            }
            ClearMovementPath();
        }

        // Calculate the Path of Cells between the Mouse and the selected GameObject
        void CalculateMovementPathGeneric() {
            // Get cell positions from both the selected GameObject and the mouse
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 objWorldPos = selectedObject.transform.position;
            Vector3Int mouseCellPos = tilemap.WorldToCell(mouseWorldPos);
            Vector3Int objCellPos = tilemap.WorldToCell(objWorldPos);

            // Calculate the distance in (x, y) coordinates from GameObject cell to mouse cell
            int xDiff = mouseCellPos.x - objCellPos.x;
            int yDiff = mouseCellPos.y - objCellPos.y;

            // Calculate pathing direction, starting position, and number of steps to destination
            Vector3Int xDir = xDiff >= 0 ? Vector3Int.right : Vector3Int.left;
            Vector3Int yDir = yDiff >= 0 ? Vector3Int.up : Vector3Int.down;
            int xSteps = Mathf.Abs(xDiff);
            int ySteps = Mathf.Abs(yDiff);

            // movePathTiles = new Vector3Int[xSteps + ySteps];
            movePath = new List<Vector3Int>();

            //TODO have decision to draw either x or y first depending on which is longer (or other condition)
            // Collect the Cell Positions for the path on the x-axis starting from the GameObject
            for (int x = 1; x <= xSteps; x++) {
                Vector3Int cellCoords = objCellPos + (xDir * x);
                //? movePathTiles.Append(cellCoords);
                movePath.Add(objCellPos + (xDir * x));
            }
            // Collect the Cell Positions for the path on the y-axis starting from the end of the x-path
            for (int y = 1; y <= ySteps; y++) {
                //? movePathTiles.Append(objCellPos + (xDir * xSteps) + (yDir * y));
                movePath.Add(objCellPos + (xDir * xSteps) + (yDir * y));
            }
        }

        // Draw the path formed from CalculateMovementPath()
        void DrawMovementPath() {
            if (selectedObject != null && selectedObject.tag == "Unit") {
                // Clear old path drawing
                ClearMovementPath();

                selectedUnit = selectedObject.GetComponent<PlayerUnit>();
                movePath = selectedUnit.CalculateMovementPath();

                if (movePath != null) {
                    movePathTiles = new List<GameObject>();
                    foreach (Vector3Int tilePos in movePath) {
                        movePathTiles.Add(Instantiate(pathTile, tilemap.GetCellCenterWorld(tilePos), Quaternion.identity));
                    }
                    pathIsDrawn = true;
                }
            }

        }

        void ClearMovementPath() {
            if (movePathTiles != null && pathIsDrawn) {
                foreach (GameObject obj in movePathTiles) {
                    Destroy(obj);
                }
                pathIsDrawn = false;
                movePath = null;
                movePathTiles = null;
            }
        }

        bool PathChanged() {
            // Check if either list is null
            if (lastMovePath == null || movePath == null) {
                return lastMovePath != movePath;
            }

            // Check for equal size
            if (lastMovePath.Count != movePath.Count) {
                return true;
            }

            // Check for equal elements
            for (int i = 0; i < lastMovePath.Count; i++) {
                if (lastMovePath[i] != movePath[i]) {
                    return true;
                }
            }

            // if all elements are equal, return false
            return false;
        }

        bool MouseInBounds() {
            int xBound = tilemap.cellBounds.xMax;
            int yBound = tilemap.cellBounds.yMax;
            Vector3Int mouseCellPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (mouseCellPos.x >= tilemap.origin.x && mouseCellPos.x < xBound && mouseCellPos.y >= tilemap.origin.y && mouseCellPos.y < yBound) {
                return true;
            } else {
                return false;
            }
        }

        // Check if the passed in cell position "cellPos" has a unit positioned on it
        public bool HasUnit(Vector3Int cellPos) {
            // Check for object using OverlapPoint            
            Collider2D[] hits = OverlapPointAllSorted(tilemap.GetCellCenterWorld(cellPos));

            // Check if any hits return as Unit
            foreach (Collider2D hit in hits) {
                Vector3Int actualPos = tilemap.WorldToCell(hit.gameObject.transform.position);
                if (hit.tag == "Unit" && actualPos == cellPos) { // Check if hit is a Unit and that Unit is actually positioned on cell being checked, NOT just that the collider is overlapping that point
                    return true;
                }
            }

            return false;
        }

        public PlayerUnit GetUnit(Vector3Int cellPos) {
            // Check for object using OverlapPoint
            Collider2D[] hits = OverlapPointAllSorted(tilemap.GetCellCenterWorld(cellPos));

            // Find first hit that is a Unit
            foreach (Collider2D hit in hits) {
                if (hit.tag == "Unit") {
                    return hit.GetComponent<PlayerUnit>();
                }
            }

            return null;
        }

        // Modified use of OverlapPointAll that includes sorting by spriteRenderer.sortingOrder
        public Collider2D[] OverlapPointAllSorted(Vector2 point) {
            // Retrieve the hits from specified point
            Collider2D[] hits = Physics2D.OverlapPointAll(point);

            // Remove all elements of array that do not have a SpriteRenderer component
            hits = hits.Where(n => n.GetComponent<SpriteRenderer>() != null).ToArray();

            // Sort the hits in ASCENDING order specified by spriteRenderer.sortingOrder            
            if (hits != null) {
                Array.Sort(hits, (t1, t2) => {
                    int sortOrder1 = t1.GetComponent<SpriteRenderer>().sortingOrder;
                    int sortOrder2 = t2.GetComponent<SpriteRenderer>().sortingOrder;
                    return sortOrder1.CompareTo(sortOrder2);
                });
            }

            return hits;
        }

        //TODO check if unit selected belongs to current player
        private bool BelongsToCurrentPlayer(GameObject targetObject) {
            // Get PlayerUnit script from passed in GameObject
            PlayerUnit targetUnit = targetObject.GetComponent<PlayerUnit>();

            // Check if the selected unit belongs to current player
            if (targetUnit.player.name == "Player_1" && GameManager.Instance.CurrentState == GameManager.GameState.Player1Turn) {
                return true;
            }

            if (targetUnit.player.name == "Player_2" && GameManager.Instance.CurrentState == GameManager.GameState.Player2Turn) {
                return true;
            }

            return false;
        }

        public void SetInputEnabled(bool enabled) {
            _inputEnabled = enabled;
        }

        //*********************************************
        //* EVENT FUNCTIONS
        //*********************************************

        void MouseClickLeft(Vector3 mousePos) {
            Vector3Int cellPosition = tilemap.WorldToCell(mousePos);

            // Check if there's a unit at the cell position
            Collider2D hit = Physics2D.OverlapPoint(tilemap.GetCellCenterWorld(cellPosition));
            if (hit != null) {
                switch (hit.tag) {
                    case "Unit":
                        if (BelongsToCurrentPlayer(hit.gameObject)) {
                            SelectObject(hit.gameObject);
                            DrawMovementPath();
                        }
                        break;
                    case "Path":
                        if (selectedObject != null && selectedUnit != null) {
                            selectedUnit.MoveToCell(cellPosition);
                            DeselectObject();
                        }
                        break;
                    default:
                        ClearMovementPath();
                        break;
                }
            } else {
                DeselectObject();
            }

        }

        void MouseClickRight(Vector3 mousePos) {
            // Deselect unit if one is currently selected
            if (selectedObject != null && selectedUnit != null) {
                DeselectObject();
            }
        }

        //! Currently avoiding OnMouse events because they require interaction with a collider associated with the tilemap which is currently an issue
        void OnMouseOver() {
            // // Convert the mouse position to world coordinates
            // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // mouseWorldPos.z = 0; // Ensure the z-coordinate is in front of all possible tilemap target z-coordinates

            // // Output some Debug.Logs() for better understanding cell-space, world-space, and local-space
            // Debug.Log("mouseScreenPos: " + Input.mousePosition); // get the position of the mouse in screen coordinates
            // Debug.Log("mouseWorldPos: " + mouseWorldPos); // get the position of the mouse in world coordinates

            // // Get the cell position in the Tilemap
            // Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            // // Check if there is a tile at hovered over tile
            // if (tilemap.HasTile(cellPosition)) {
            //     Debug.Log("cellPosition: " + cellPosition); // get the position of the cell as an object in the grid

            //     // Get the GameObject that's instantiated at the cell position where mouse is hovering
            //     GameObject selectedTile = tilemap.GetInstantiatedObject(cellPosition);

            //     // Make the selected tile hover slightly
            //     if (selectedTile != null) {
            //         selectedTile.transform.Translate(Vector3.up * translateAmount);
            //     }
            //     else {
            //         Debug.Log("No GameObject Here");
            //     }
            // }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            // Check if there's a unit at the cell position
            Collider2D hit = Physics2D.OverlapPoint(tilemap.GetCellCenterWorld(cellPosition));
            if (hit != null && hit.tag == "Unit") {
                //AddHighlightObject(hit.gameObject);
            } else {
                //RemoveHighlightObject();
            }
        }

        void OnMouseDown() {
            //! THIS FUNCTION ONLY CALLS WHILE MOUSE IS OVER THE COLLIDER OF ASSOCIATED GAMEOBJECT
            //TODO Get mouse position when mouse is clicked and this function is called
            //TODO Check on what is actually being clicked (tile, gameObject, what kind of gameObject?)
            //TODO if a gameobject is currently selected and a tile is clicked on, move to that tile
            //TODO if a gameobject is NOT currently selected and a tile is clicked on, do nothing
            //TODO if a gameobject is clicked on, set it to currently selected

            // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            // // Check if there's a unit at the cell position
            // Collider2D hit = Physics2D.OverlapPoint(tilemap.GetCellCenterWorld(cellPosition));
            // if (hit != null && hit.tag == "Unit") {
            //     SelectObject(hit.gameObject);
            // }
            // else {
            //     DeselectObject();
            // }
        }
    }
}
