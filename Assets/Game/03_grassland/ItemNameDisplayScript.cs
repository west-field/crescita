using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �\������UI�����X�ɓ��������A0.5�ɂȂ�����폜����
/// </summary>
public class ItemNameDisplayScript : MonoBehaviour
{
    //���l�ύX�X�s�[�h
    private const float alphaSpeed = 0.1f;
    //�ύX����
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
