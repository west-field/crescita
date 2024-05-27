using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// ����,�h��
/// ����
/// </summary>
public class WeaponShopPanel : MonoBehaviour
{
    /*�p�l���\��*/
    private enum PanelName
    {
        weapon,//����
        armor,//�h��
        end,//�I��
        max
    }

    [SerializeField] private TextMeshProUGUI[] text = new TextMeshProUGUI[(int)PanelName.max];//�e�L�X�g
    [SerializeField] private GameObject weaponPanelObj, armorPanelObj;//���틭���p�l���A�h����p�l��
    [SerializeField] private GameObject selectFrame;//�I�����Ă��镶���̏ꏊ�Ƀt���[�����ړ�������

    /*�L�[*/
    private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    /*�I��*/
    private int selectNum;//���I�����Ă������
    private int selectMaxNum;//�I���ł��鐔
    private PanelName drawType;//���ܕ\�����Ă������

    private bool isWorkShop;//�J�������ǂ���

    /*��*/
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip openingClosingSound, moveSound;//�|�[�Y���J���Ƃ��̉�,�J�[�\���ړ���

    private bool isFirst = true;

    //�g��k��
    [SerializeField] private Scaling scalingScript;

    //�Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        Debug.Log("����h������A�N�e�B�u�ɂȂ���");
        Time.timeScale = 0f;
        audioSource.volume = HoldVariable.SEVolume;

        if (isFirst) return;
        audioSource.PlayOneShot(openingClosingSound);
    }

    //�Q�[���I�u�W�F�N�g����A�N�e�B�u�ɂȂ�����
    private void OnDisable()
    {
        Debug.Log("����h�������A�N�e�B�u�ɂȂ���");
        weaponPanelObj.SetActive(false);
        armorPanelObj.SetActive(false);
        Time.timeScale = 1f;
    }

    void Start()
    {
        Debug.Log("����h����p�l��������");
        selectNum = (int)PanelName.end;
        selectMaxNum = (int)PanelName.max;
        text[selectNum].color = Color.red;
        selectFrame.transform.position = text[selectNum].transform.position;

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        drawType = PanelName.max;

        weaponPanelObj.SetActive(false);
        armorPanelObj.SetActive(false);

        GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
        GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
        isFirst = false;
        this.gameObject.SetActive(false);

        scalingScript.Init(0.9f, 0.7f);

        Debug.Log("����h����p�l������������");
    }

    void Update()
    {
        switch (drawType)
        {
            case PanelName.weapon:
                WeaponPanelDrawUpdate();
                break;
            case PanelName.armor:
                ArmorPanelDrawUpdate();
                break;
            case PanelName.end:
                //�؂�ւ���
                GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                this.gameObject.SetActive(false);
                break;
            default:
                WorkShopPanelDrawUpdate();
                break;
        }
    }

    private void WorkShopPanelDrawUpdate()
    {
        var isPress = false;

        if (Navigate.WasPressedThisFrame())
        {
            Navigate.ReadValue<Vector2>();
            //Debug.Log(Navigate.ReadValue<Vector2>());

            if (Navigate.ReadValue<Vector2>().x < 0.0f || Navigate.ReadValue<Vector2>().y > 0.0f)
            {
                //��
                selectNum = (selectNum + (selectMaxNum - 1)) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().x > 0.0f || Navigate.ReadValue<Vector2>().y < 0.0f)
            {
                //��
                selectNum = (selectNum + 1) % selectMaxNum;
            }

            isPress = true;
        }

        if (isPress)
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(moveSound);
            //�I�����Ă��鍀�ڂ̐F��ς���
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    scalingScript.ScalingObjPosition(selectFrame.transform, text[i].transform.position);
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //������������Ƃ�
        if (Submit.WasPressedThisFrame())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);
            switch (selectNum)
            {
                case (int)PanelName.weapon:
                    weaponPanelObj.SetActive(true);
                    drawType = PanelName.weapon;
                    isWorkShop = false;
                    break;
                case (int)PanelName.armor:
                    armorPanelObj.SetActive(true);
                    drawType = PanelName.armor;
                    isWorkShop = false;
                    break;
                case (int)PanelName.end:
                    weaponPanelObj.SetActive(false);
                    armorPanelObj.SetActive(false);

                    GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                    GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                    this.gameObject.SetActive(false);
                    break;
            }
        }
        //�L�����Z������������
        else if (Cancel.WasPressedThisFrame())
        {
            if (!isWorkShop)
            {
                weaponPanelObj.SetActive(false);
                armorPanelObj.SetActive(false);

                isWorkShop = true;
            }
            else
            {
                weaponPanelObj.SetActive(false);
                armorPanelObj.SetActive(false);

                GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                this.gameObject.SetActive(false);
            }
        }

    }

    private void WeaponPanelDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.weapon;
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);
            //�A�C�e����ʂ������Ȃ�����
            weaponPanelObj.SetActive(false);
            //�|�[�Y��ʂɖ߂�
            drawType = PanelName.max;

            return;
        }
    }

    private void ArmorPanelDrawUpdate()
    {       
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.armor;
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);
            //�A�C�e����ʂ������Ȃ�����
            armorPanelObj.SetActive(false);
            //�|�[�Y��ʂɖ߂�
            drawType = PanelName.max;

            return;
        }

    }

}
