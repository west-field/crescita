using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// �X�e�[�W�I���V�[���}�l�[�W���[
/// </summary>
public class SelectStageManager : MonoBehaviour
{
    private enum Stage
    {
        Grassland,
        City,
        Plain,

        Max
    }

    //�I���ł���Image���擾
    [SerializeField] private GameObject[] stage = new GameObject[(int)Stage.Max];

    private const int selectNum = (int)Stage.Max;//�I���ł���X�e�[�W�̐�
    private int selectCount;

    [SerializeField] private MainManager mainManager;
    private InputAction select,direction;//�A�N�V�����}�b�v����A�N�V�����̎擾

    private Vector2 value;//�A�N�V����������͒l�̎擾

    private bool isSelect;//�ړ��L�[�������Ă��邩,�I��������

    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip moveSound;//�ړ���
    [SerializeField] private AudioClip directionSound;//�����������̉�

    [SerializeField] private Transform selectFrame;
    [SerializeField] private Scaling scaling;

    private void Start()
    {
        select = mainManager.GetPlayerInput().actions["move"];
        direction = mainManager.GetPlayerInput().actions["fire"];

        value = select.ReadValue<Vector2>();

        selectCount = (int)Stage.City;
        ChangeSelect(selectCount);

        isSelect = false;

        scaling.Init(1.3f, 1.0f);

        audioSource.volume = HoldVariable.SEVolume;
    }

    // Update is called once per frame
    private void Update()
    {
        scaling.ScalingObj(selectFrame);
        //����������Ă����珈�������Ȃ�
        if (isSelect) return;

        //������������玟�ֈړ�����
        if (direction.WasPressedThisFrame())
        {
            //�I����
            audioSource.PlayOneShot(directionSound);
            Select(selectCount);//�V�[����ύX����
            isSelect = true;//�����������
            return;
        }

        if(select.WasPressedThisFrame())
        {
            value = select.ReadValue<Vector2>();//���͒l�̎擾

            //�I���ʒu��ύX����@����
            if (value.x > 0.0f)
            {
                selectCount = (selectCount + 1) % selectNum;
                //�ړ���
                audioSource.PlayOneShot(moveSound);
            }
            //�I���ʒu��ύX����@�O��
            else if (value.x < 0.0f)
            {
                selectCount = (selectCount + (selectNum - 1)) % selectNum;
                //�ړ���
                audioSource.PlayOneShot(moveSound);
            }
        }

        //�I�����Ă���ʒu�𕪂���₷������
        ChangeSelect(selectCount);
    }

    //�I�����Ă���ꏊ�𕪂���₷������
    private void ChangeSelect(int select)
    {
        //���Z�b�g����
        for (int i = 0; i < selectNum; i++)
        {
            stage[i].SetActive(true);
        }

        switch (select)
        {
            case (int)Stage.City://�V�e�B��I��
                stage[(int)Stage.City].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.City].transform.position);
                break;
            case (int)Stage.Grassland://������I��
                stage[(int)Stage.Grassland].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.Grassland].transform.position);
                break;
            case (int)Stage.Plain://������I��
                stage[(int)Stage.Plain].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.Plain].transform.position);
                break;
            default:
                break;
        }
    }

    //�I�����Ă�����̂ɂ���Ĉړ�����V�[����ύX����
    private void Select(int select)
    {
        switch (select)
        {
            case (int)Stage.City://�V�e�B��I��
                mainManager.ChangeScene("city");
                //�ʒu��ύX
                HoldVariable.playerPosision = new Vector3(0.0f, 1.0f, 22.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 180, 0);
                break;
            case (int)Stage.Grassland://������I��
                mainManager.ChangeScene("grassland");
                //�ʒu��ύX
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            case (int)Stage.Plain://������I��
                mainManager.ChangeScene("onlineRoom");
                //�ʒu��ύX
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            default:
                break;
        }
    }
}
