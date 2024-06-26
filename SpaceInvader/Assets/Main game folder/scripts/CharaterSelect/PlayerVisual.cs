using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    private Material material;

    private void Awake()
    {
        material = new Material(spriteRenderer.material);
        spriteRenderer.material = material;
    }



    public void SetPlayerColor(Color color )
    {
        material.color = color;
    }
}
