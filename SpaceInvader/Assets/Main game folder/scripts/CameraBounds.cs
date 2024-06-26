using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public float targetAspectRatio = 1920f/1080f; // You can set your desired aspect ratio here.

    void Start()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;
            float scaleHeight = currentAspectRatio / targetAspectRatio;

            if (scaleHeight < 1.0f)
            {
                mainCamera.orthographicSize *= scaleHeight;
            }
        }
    }
}

