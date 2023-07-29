using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite defaultSprite, altSprite;

    // Start is called before the first frame update
    void Start()
    {
        defaultSprite = spriteRenderer.sprite;
    }
    public void DefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }
    public void AltSprite()
    {
        spriteRenderer.sprite = altSprite;
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
