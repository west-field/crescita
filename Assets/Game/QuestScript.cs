using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestScript : MonoBehaviour
{
    /// <summary>
    /// ���C���}�l�[�W���[
    /// </summary>
    [SerializeField] private MainManager manager;
    /// <summary>
    /// �A�N�V�����}�b�v����A�N�V�����̎擾
    /// </summary>
    private InputAction decision;

    /// <summary>
    /// �I�������Ƃ��ɕ\������摜
    /// </summary>
    [SerializeField] private GameObject questPanel;
    /// <summary>
    /// �I���ł��邩�A�I��������
    /// </summary>
    private bool isSelect, isChoice;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private string message;

    /// <summary>
    /// �܂��I���ł���悤�ɂȂ�܂ł̎���
    /// </summary>
    private float time;

    private void Start()
    {
        decision = manager.GetPlayerInput().actions["fire"];

        isSelect = false;
        isChoice = false;
    }

    private void FixedUpdate()
    {
        //�ҋ@���Ԃ�0���傫���Ƃ�
        if(time > 0)
        {
            time -= Time.deltaTime;
            return;
        }

        //�I�����Ă����珈�������Ȃ�
        if (isChoice)
        {
            //�I���ł��Ȃ��ʒu�ɂ���Ƃ�
            if (!isSelect)
            {
                //�\�����Ȃ��悤�ɂ���
                isChoice = false;
                questPanel.SetActive(false);
            }

            //��\���ɂȂ��Ă��鎞
            if (!questPanel.activeSelf)
            {
                //�I�����Ă���t���O��false�ɂ���
                isChoice = false;
            }
        }
        //�I���ł���Ƃ�
        else if (isSelect)
        {
            isSelect = false;

            bool isStarted;
            isStarted = decision.IsPressed();

            //����{�^���������Ă��Ȃ��Ƃ��͏��������Ȃ�
            if (!isStarted) return;
            //�I���t���O��true��
            isChoice = true;
            time = 1.0f;
            //�\������
            questPanel.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //�v���C���[�Ɠ������Ă��鎞
        if(other.gameObject.tag == "Player")
        {
            isSelect = true;
        }
    }
}
