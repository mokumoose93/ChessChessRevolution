using UnityEngine;

public class SpriteBorderHover : MonoBehaviour {
    public Color borderColor = Color.red; // Color of the border
    public float borderWidth = 0.01f;    // Width of the border

    private Material material;
    private Color originalColor;
    private float originalWidth;

    void Start() {
        // Get the sprite's material
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        // Save the original material properties
        originalColor = material.GetColor("_BorderColor");
        originalWidth = material.GetFloat("_BorderWidth");
    }

    void OnMouseEnter() {
        // Enable the border
        material.SetColor("_BorderColor", borderColor);
        material.SetFloat("_BorderWidth", borderWidth);
    }

    void OnMouseExit() {
        // Disable the border
        material.SetColor("_BorderColor", originalColor);
        material.SetFloat("_BorderWidth", originalWidth);
    }
}