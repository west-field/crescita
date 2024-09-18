using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �g��k��������
/// </summary>
public class Scaling : MonoBehaviour
{
    /*�g��k��*/
    /// <summary>
    /// �g��k���X�s�[�h
    /// </summary>
    private float changeSpeed = 0.0f;
    /// <summary>
    /// �g�傷�邩�ǂ���
    /// </summary>
    private bool isEnlarge = true;

    private float maxScale = 0.9f, minScale = 0.7f;
    public void Init(float max,float min)
    {
        this.maxScale = max;
        this.minScale = min;
    }

    /// <summary>
    /// �g�傳����I�u�W�F�N�g�̈ʒu��ύX������
    /// </summary>
    /// <param name="scalingTransform">�g�傳����I�u�W�F�N�g</param>
    /// <param name="position">�ύX�ʒu</param>
    public void ScalingObjPosition(Transform scalingTransform, Vector3 position)
    {
        //���I��ł���I�u�W�F�N�g�̈ʒu�ƃt���[���̈ʒu���Ⴄ�Ƃ�
        if (scalingTransform.position != position)
        {
            //�ʒu�����킹��
            scalingTransform.position = position;
            isEnlarge = true;
            //�g�嗦��������
            scalingTransform.localScale = new Vector3(minScale, minScale, minScale);
        }
    }

    /// <summary>
    /// �g��k��������
    /// </summary>
    /// <param name="scalingTransform">�g��k�����������I�u�W�F�N�g</param>
    public void ScalingObj(Transform scalingTransform)
    {
        //�t���[�����g��k��������
        changeSpeed = Time.unscaledDeltaTime * 0.2f;
        //�g��
        if (isEnlarge)
        {
            scalingTransform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
            //�t���[���̊g�嗦��0.8�ȏ�ɂȂ�����������������
            if (scalingTransform.localScale.x >= maxScale)
            {
                isEnlarge = false;
            }
        }
        //�k��
        else
        {
            scalingTransform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
            //�t���[���̊g�嗦��0.65�ȉ��ɂȂ�����傫��������
            if (scalingTransform.localScale.x <= minScale)
            {
                isEnlarge = true;
            }
        }
    }
}
