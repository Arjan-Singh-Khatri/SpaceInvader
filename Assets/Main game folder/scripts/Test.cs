using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public SpriteRenderer backgroundSpriteRenderer;
    public float padding = 0.1f; // Adjust padding as needed.

    void Start()
    {
        ScaleBackground();
    }

    void ScaleBackground()
    {
        if (backgroundSpriteRenderer == null)
        {
            Debug.LogError("Background SpriteRenderer not assigned.");
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }

        // Calculate the world space width and height of the screen edges at the depth of the background.
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Get the size of the background sprite.
        float spriteWidth = backgroundSpriteRenderer.bounds.size.x;
        float spriteHeight = backgroundSpriteRenderer.bounds.size.y;

        // Calculate the scaling factors for the background.
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        // Apply the smaller scaling factor to maintain the aspect ratio.
        float scale = Mathf.Min(scaleX, scaleY);

        // Apply padding to avoid the background being cut off at the screen edges.
        scale *= 1.0f - padding;

        // Set the background's scale to fit the screen.
        transform.localScale =  new Vector3(screenWidth+5, screenHeight+5, scale);

    }

}
