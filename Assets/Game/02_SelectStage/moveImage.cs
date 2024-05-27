using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X�e�[�W��I�����邽�߂̉摜
/// </summary>
public class moveImage : MonoBehaviour
{
    /// <summary>
    /// �I������Ă��Ȃ��Ƃ��ɕ\������e�L�X�g
    /// </summary>
    [SerializeField] private GameObject textObj;
    /// <summary>
    /// �\������G
    /// </summary>
    [SerializeField] private Image imageFirst, imageSecond;

    /// <summary>
    /// �ő�p�x
    /// </summary>
    [SerializeField] private int maxAngle = 360;
    /// <summary>
    /// ���݂̊p�x
    /// </summary>
    private float currentAngle;

    /// <summary>
    /// ������������
    /// </summary>
    private bool isAdd;

    /// <summary>
    /// �҂���
    /// </summary>
    [SerializeField] private float waitTime = 60;
    /// <summary>
    /// �o�ߎ���
    /// </summary>
    private float time;

    private void Start()
    {
        currentAngle = maxAngle;
        isAdd = false;
        time = waitTime;
    }

    private void FixedUpdate()
    {
        //�e�L�X�g���\������Ă��鎞�͏������Ȃ�
        if(textObj.activeSelf)
        {
            if (currentAngle != maxAngle)
            {
                currentAngle = maxAngle;
            }
            return;
        }

        //0���傫���Ƃ��͎��Ԃ����炷
        if(time >= 0)
        {
            time -= Time.deltaTime * 100;
            return;
        }

        //���炷�Ƃ�
        if (!isAdd)
        {
            //���݂̊p�x�����炷
            currentAngle -= Time.deltaTime * 100;
            if (0 == imageFirst.fillAmount)
            {
                time = waitTime;
                isAdd = true;
                imageFirst.fillClockwise = false;
            }
        }
        else
        {
            currentAngle += Time.deltaTime * 100;
            if (1 == imageFirst.fillAmount)
            {
                time = waitTime;
                isAdd = false;
                imageFirst.fillClockwise = true;
            }
        }

        //Image�R���|�[�l���g��fillAmount���擾���đ��삷��
        imageFirst.fillAmount = currentAngle / maxAngle;
    }

}
