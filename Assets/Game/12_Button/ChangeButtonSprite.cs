using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �{�^���̃X�v���C�g��ύX����
/// </summary>
public class ChangeButtonSprite : MonoBehaviour
{
    //�X�v���C�g��ύX���邽��
    private Image spriteRenderer;
    //�ύX�������X�v���C�g�������Ă���
    [SerializeField] private Sprite secondSprite;
    private Sprite firstSprite;

    private void Start()
    {
        //SpriteRenderer���擾����
        spriteRenderer = GetComponent<Image>();
        //�ŏ��̃X�v���C�g��ێ����Ă���
        firstSprite = spriteRenderer.sprite;
    }

    public void ChangingSprite(bool isPressed)
    {
        if(isPressed)
        {
            if(spriteRenderer.sprite != secondSprite)
            {
                //������Ă���X�v���C�g�֕ύX����
                spriteRenderer.sprite = secondSprite;
            }
        }
        else
        {
            if (spriteRenderer.sprite != firstSprite)
            {
                //������Ă���X�v���C�g�֕ύX����
                spriteRenderer.sprite = firstSprite;
            }
        }
    }
}
