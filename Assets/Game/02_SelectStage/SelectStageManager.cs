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
    //�I���ł���Image���擾
    [SerializeField] private GameObject city, grassland, plain;//���A�����A����

    private static int selectNum = 3;//�I���ł���X�e�[�W�̐�
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

    // Start is called before the first frame update
    void Start()
    {
        select = mainManager.GetPlayerInput().actions["move"];
        direction = mainManager.GetPlayerInput().actions["fire"];

        value = select.ReadValue<Vector2>();

        selectCount = 0;
        ChangeSelect(selectCount);

        isSelect = false;

        scaling.Init(1.3f, 1.0f);
        scaling.ScalingObjPosition(selectFrame, city.transform.position);

        audioSource.volume = HoldVariable.SEVolume;
    }

    // Update is called once per frame
    void Update()
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
            if (value.x > 0.0f || value.y > 0.0f)
            {
                selectCount = (selectCount + 1) % selectNum;
                //�ړ���
                audioSource.PlayOneShot(moveSound);
            }
            //�I���ʒu��ύX����@�O��
            else if (value.x < 0.0f || value.y < 0.0f)
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
    void ChangeSelect(int select)
    {
        switch (select)
        {
            case 0://�V�e�B��I��
                city.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, city.transform.position);

                grassland.SetActive(true);

                plain.SetActive(true);

                break;
            case 1://������I��
                city.SetActive(true);

                grassland.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, grassland.transform.position);

                plain.SetActive(true);
                break;
            case 2://������I��
                city.SetActive(true);

                grassland.SetActive(true);

                plain.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, plain.transform.position);
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
            case 0://�V�e�B��I��
                mainManager.ChangeScene("city");
                //�ʒu��ύX
                HoldVariable.playerPosision = new Vector3(0.0f, 1.0f, 22.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 180, 0);
                break;
            case 1://������I��
                mainManager.ChangeScene("grassland");
                //�ʒu��ύX
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            case 2://������I��
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
