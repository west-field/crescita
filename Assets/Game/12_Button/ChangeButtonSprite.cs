using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンのスプライトを変更する
/// </summary>
public class ChangeButtonSprite : MonoBehaviour
{
    //スプライトを変更するため
    private Image spriteRenderer;
    //変更したいスプライトを持っておく
    [SerializeField] private Sprite secondSprite;
    private Sprite firstSprite;

    private void Start()
    {
        //SpriteRendererを取得する
        spriteRenderer = GetComponent<Image>();
        //最初のスプライトを保持しておく
        firstSprite = spriteRenderer.sprite;
    }

    public void ChangingSprite(bool isPressed)
    {
        if(isPressed)
        {
            if(spriteRenderer.sprite != secondSprite)
            {
                //押されているスプライトへ変更する
                spriteRenderer.sprite = secondSprite;
            }
        }
        else
        {
            if (spriteRenderer.sprite != firstSprite)
            {
                //押されているスプライトへ変更する
                spriteRenderer.sprite = firstSprite;
            }
        }
    }
}
