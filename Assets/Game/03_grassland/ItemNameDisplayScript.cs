using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示したUIを徐々に透明化し、0.5になったら削除する
/// </summary>
public class ItemNameDisplayScript : MonoBehaviour
{
    //α値変更スピード
    private const float alphaSpeed = 0.1f;
    //変更する
    private CanvasGroup itemNameCanvas;
    
    private void Start()
    {
        itemNameCanvas = this.GetComponent<CanvasGroup>();
        itemNameCanvas.alpha = 1;
    }

    private void FixedUpdate()
    {
        itemNameCanvas.alpha -= alphaSpeed * Time.deltaTime;
        if(itemNameCanvas.alpha <= 0.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
