using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �E�B���h�E���[�h�ύX�p�l���X�N���v�g
/// </summary>
public class ChangeWndowModeScript : MonoBehaviour
{
    [SerializeField] private GameObject yesObj, noObj;//�͂��A�������̃Q�[���I�u�W�F�N�g
    private TextMeshProUGUI yes, no;//�͂��A�������̃e�L�X�g

    private bool isYes;//yes,no�ǂ����I��ł��邩

    /*�L�[*/
    private MainManager mainManager;
    private InputAction Navigate, Submit;//�A�N�V�����}�b�v����A�N�V�����̎擾

    /*��*/
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip moveSound,submitSound;//�J�[�\���ړ���

    [SerializeField] private GameObject border;

    private void Awake()
    {
        isYes = false;//���߂�no����
    }
    private void Start()
    {
        //���C���}�l�[�W���[����L�[���擾����
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];

        yes = yesObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        yes.color = Color.black;
        no = noObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        no.color = Color.red;

        isYes = false;

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void Update()
    {
        bool isPress = false;

        if(Navigate.WasPressedThisFrame())
        {
            isYes = !isYes;
            isPress = true;
        }

        if (isPress)
        {
            Debug.Log("�ύX");
            //�T�E���h��炷
            audioSource.PlayOneShot(moveSound);
            //�I�����Ă��鍀�ڂ̐F��ς���
            if(isYes)
            {
                //�ԐF�łȂ��Ƃ��ύX����
                if(yes.color != Color.red)
                {
                    yes.color = Color.red;
                }
                //���F�łȂ��Ƃ��ύX����
                if(no.color != Color.black)
                {
                    no.color = Color.black;
                }
            }
            else
            {
                //�ԐF�łȂ��Ƃ��ύX����
                if (no.color != Color.red)
                {
                    no.color = Color.red;
                }
                //���F�łȂ��Ƃ��ύX����
                if (yes.color != Color.black)
                {
                    yes.color = Color.black;
                }
            }
            yesObj.transform.localScale = Vector3.one;
            noObj.transform.localScale = Vector3.one;
        }

        //�I�����Ă��镶�����g��k������
        if(isYes)
        {
            ChangeSize(yesObj);
        }
        else
        {
            ChangeSize(noObj);
        }

        //����{�^���������ꂽ��
        if(Submit.WasPressedThisFrame())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(submitSound);
            if (isYes)
            {
                Screen.fullScreen = !Screen.fullScreen;
            }

            this.gameObject.SetActive(false);
        }

    }

    //�e�L�X�g�̕\�����g��k������
    void ChangeSize(GameObject obj)
    {
        border.transform.position = obj.transform.position;
    }

}
