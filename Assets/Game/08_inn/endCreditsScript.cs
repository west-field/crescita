using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �G���h�J�[�h�\��
/// </summary>
public class endCreditsScript : MonoBehaviour
{
    //�e�L�X�g�̃X�N���[���X�s�[�h
    private const float textScrollSpeed = 30.0f;
    //�e�L�X�g�̐����ʒu
    private const float limitPosition = 900.0f;

    //�G���h���[�����I���������ǂ���
    private bool isStopEndRoll;

    /*�L�[*/
    private MainManager mainManager;
    private InputAction Submit, Cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    private float startPosY;

    private void Start()
    {
        isStopEndRoll = false;
        startPosY = this.transform.position.y;
        this.transform.parent.gameObject.SetActive(false);

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];
    }

    private void OnEnable()
    {
        isStopEndRoll = false;
        //�����ʒu�ɖ߂�
        this.transform.position = new Vector3(this.transform.position.x, startPosY, this.transform.position.z);
        Time.timeScale = 0f;
    }
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        //�G���h���[�����I�������Ƃ�
        if(isStopEndRoll)
        {
            //���肩�L�����Z�����������Ƃ�
            if (Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
            {
                //��\���ɂ���
                this.transform.parent.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            //�G���h���[���p�e�L�X�g�������ʒu�𒴂���܂œ�����
            if(this.transform.position.y <= limitPosition)
            {
                this.transform.position += new Vector3(0.0f, textScrollSpeed, 0.0f);
                Debug.Log(this.transform.position);
            }
            else
            {
                isStopEndRoll = true;
            }
        }

        //���肩�L�����Z�����������Ƃ�
        if(Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
        {
            //�����I��
            Debug.Log("�����I��");
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
