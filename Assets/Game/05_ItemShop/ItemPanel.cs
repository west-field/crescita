using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �A�C�e���̔��蔃��
/// </summary>
public class ItemPanel : MonoBehaviour
{
    /*�p�l���\��*/
    private enum PanelName
    {
        buy,
        sell,
        end,
        max
    }

    [SerializeField] private TextMeshProUGUI[] text = new TextMeshProUGUI[(int)PanelName.max];
    [SerializeField] private GameObject buyPanelObj, sellPanelObj;//�����p�l���A����p�l��
    [SerializeField] private ItemDataLoadOrSave itemSave;//�A�C�e���Z�[�u�p
    [SerializeField] private GameObject selectFrame;//�I�����Ă��镶���̏ꏊ�Ƀt���[�����ړ�������

    /*�L�[*/
    private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    /*�I��*/
    private int selectNum;//���I�����Ă������
    private int selectMaxNum;//�I���ł��鐔
    private PanelName drawType;//���ܕ\�����Ă������

    /*��*/
    private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip openingClosingSound, moveSound;//�J���Ƃ��̉�,�J�[�\���ړ���

    private bool isFirst = true;

    //�g��k��
    [SerializeField] private Scaling scalingScript;

    //�Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        Debug.Log("�V���b�v�p�l�����A�N�e�B�u�ɂȂ���");
        Time.timeScale = 0f;
        audioSource.volume = HoldVariable.SEVolume;

        if (isFirst) return;
        audioSource.PlayOneShot(openingClosingSound);
    }

    //�Q�[���I�u�W�F�N�g����A�N�e�B�u�ɂȂ�����
    private void OnDisable()
    {
        Debug.Log("�V���b�v�p�l������A�N�e�B�u�ɂȂ���");
        buyPanelObj.SetActive(false);
        sellPanelObj.SetActive(false);
        Time.timeScale = 1f;

        itemSave.SaveItemData();
    }

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    private void Start()
    {
        selectNum = (int)PanelName.end;
        selectMaxNum = (int)PanelName.max;
        text[selectNum].color = Color.red;
        selectFrame.transform.position = text[selectNum].transform.position;

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        drawType = PanelName.max;

        buyPanelObj.SetActive(false);
        sellPanelObj.SetActive(false);

        isFirst = false;

        scalingScript.Init(0.9f, 0.7f);

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (drawType)
        {
            case PanelName.buy:
                BuyDrawUpdate();
                break;
            case PanelName.sell:
                SellDrawUpdate();
                break;
            case PanelName.end:
                //�؂�ւ���
                this.gameObject.SetActive(false);
                break;
            default:
                GoldShopDrawUpdate();
                break;
        }
    }

    void GoldShopDrawUpdate()
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
                case (int)PanelName.buy:
                    buyPanelObj.SetActive(true);
                    drawType = PanelName.buy;
                    break;
                case (int)PanelName.sell:
                    sellPanelObj.SetActive(true);
                    drawType = PanelName.sell;
                    break;
                case (int)PanelName.end:
                    buyPanelObj.SetActive(false);
                    sellPanelObj.SetActive(false);

                    this.gameObject.SetActive(false);
                    break;
            }
        }
        //�L�����Z������������
        else if (Cancel.WasPressedThisFrame())
        {
            buyPanelObj.SetActive(false);
            sellPanelObj.SetActive(false);

            this.gameObject.SetActive(false);
        }
    }

    //�w������
    void BuyDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.buy;
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);
            //�A�C�e����ʂ������Ȃ�����
            buyPanelObj.SetActive(false);
            //�|�[�Y��ʂɖ߂�
            drawType = PanelName.max;

            return;
        }
    }

    //���p����
    void SellDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.sell;
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);
            //�A�C�e����ʂ������Ȃ�����
            sellPanelObj.SetActive(false);
            //�|�[�Y��ʂɖ߂�
            drawType = PanelName.max;
            return;
        }
    }
}
