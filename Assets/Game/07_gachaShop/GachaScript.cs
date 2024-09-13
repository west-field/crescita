using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using TMPro;

/// <summary>
/// �K�`��
/// </summary>
public class GachaScript : MonoBehaviour
{
    /*�K�`�����ʕ\��*/
    [SerializeField] private GameObject slot;//�X���b�g�̃v���n�u
    /// <summary>
    /// �쐬�����X���b�g������
    /// </summary>
    private List<GameObject> slotList = new List<GameObject>();

    /*�񂵂Ă��鎞*/
    /// <summary>
    /// �K�`�����񂵂����ǂ����̃t���O
    /// </summary>
    private bool isGacha;
    /// <summary>
    /// �K�`���̌��ʂ��Ԃ��Ă�����
    /// true:(���ʂ��A���Ă���) false:(���ʂ�҂��Ă���)
    /// </summary>
    private bool isResultReturn;
    /// <summary>
    /// �K�`�����������߂ɕK�v�ȃS�[���hor���x���|�C���g�������Ă��邩
    /// </summary>
    private bool isHaveGoldOrPoint;
    /// <summary>
    /// �S�[���h�ł܂킷�K�`����I��ł��邩
    /// </summary>
    private bool isGoldGacha;

    /// <summary>
    /// �K�`����
    /// </summary>
    private int gachaCount;

    /*�K�`������*/
    /// <summary>
    /// �K�`������
    /// </summary>
    [SerializeField] private TextMeshProUGUI infomation;

    /// <summary>
    /// �����Ă����A�C�e����
    /// </summary>
    private string[] resultItemName;

    /*�p�l���\��*/
    /// <summary>
    /// �摜���擾����
    /// </summary>
    [SerializeField] private GameObject myGold,myLevelPoint;
    /// <summary>
    /// �����̎����Ă��閇����\������
    /// </summary>
    private TextMeshProUGUI myGoldText,myLevelPointText;

    /*�L�[*/
    private MainManager mainManager;
    InputAction Navigate,Submit, Cancel;//�ړ�,����,�L�����Z��

    /*��*/
    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound, submitSound, gachaSound, cancelSound;//�ړ����A���艹�A�񂵂Ă��鎞�̉��A�L�����Z����

    /*�A�C�e��*/
    /// <summary>
    /// �A�C�e���f�[�^�x�[�X
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// ���g�������Ă���A�C�e��
    /// </summary>
    private PlayerItem playerItem;
    /// <summary>
    /// �v���C���[�̃X�e�[�^�X
    /// </summary>
    private PlayerStartas playerStartas;

    /*����*/
    /// <summary>
    /// �񂵂����ʂ�\������p�l��
    /// </summary>
    [SerializeField] private GameObject gachaResultPanelObj;
    /// <summary>
    /// ���ʂ̏W�v���e�L�X�g��
    /// </summary>
    [SerializeField] private TextMeshProUGUI resultText;

    /// <summary>
    /// �I�����Ă�����̂̈ʒu
    /// </summary>
    [SerializeField] private GameObject goldGacha, levelPointGacha;

    /*1��10��A�񂷂���I��*/
    /// <summary>
    /// �񂷉񐔂�I������p�l���̕\����\��
    /// </summary>
    [SerializeField] private GameObject confirmObj;
    /// <summary>
    /// �I�����Ă�����̂̈ʒu
    /// </summary>
    private GameObject oneTurn, tenTurn;
    /// <summary>
    /// �����e�L�X�g(�I�����Ă�����̂ɉ����ĕύX����)
    /// </summary>
    [SerializeField] private TextMeshProUGUI infoConfirm;
    /// <summary>
    /// ���񂷂ق���I��ł��邩
    /// </summary>
    private bool isOneTurn;

    /// <summary>
    /// ��x�����v���C���[�̃A�C�e�����擾����
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// �I��ł���I�u�W�F�N�g�Ɉʒu�����킹�ăt���[����\��
    /// </summary>
    [SerializeField] private GameObject selectFrame;

    /*�g��k��*/
    [SerializeField] private Scaling scalingScript;

    /// <summary>
    /// ����down �o���lup�����Ă���
    /// </summary>
    private string rate;

    [SerializeField] private ItemDataLoadOrSave itemDataSave;
    [SerializeField] private StartasLoadOrSave startasLoadOrSave;

    private enum panelName
    {
        gacha,
        confirm,
        result,
    }
    private panelName drawType;

    private void OnEnable()
    {
        Debug.Log("�K�`�����A�N�e�B�u�ɂȂ���");
        if (isFirst) return;
        audioSource.volume = HoldVariable.SEVolume;
        Time.timeScale = 0f;

        //�v���C���[�^�O������
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //��������΃A�C�e�����擾����
        foreach (var item in obj)
        {
            Debug.Log("����");
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            playerStartas = item.GetComponent<PlayerStartas>();
            myLevelPointText.text = $"{playerStartas.GetLevelPoint()}P";
            break;
        }

        audioSource.PlayOneShot(submitSound);

        gachaCount = 1;

        drawType = panelName.gacha;

        rate = "coin";
    }
    //�Q�[���I�u�W�F�N�g����A�N�e�B�u�ɂȂ�����
    private void OnDisable()
    {
        Debug.Log("�K�`������A�N�e�B�u�ɂȂ���");
        Time.timeScale = 1f;
        gachaResultPanelObj.SetActive(false);
        confirmObj.SetActive(false);
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //�L�[
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        //SE�{�����[��
        audioSource.volume = HoldVariable.SEVolume;

        //�e�L�X�g���擾
        myGoldText = myGold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        myLevelPointText = myLevelPoint.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        isGacha = false;
        isResultReturn = false;
        isHaveGoldOrPoint = false;
        isGoldGacha = true;
        infomation.text = $"1��@100G\n�����ŉ񂷂��Ƃ��ł���B";

        isFirst = false;

        isOneTurn = true;
        oneTurn = confirmObj.transform.GetChild(0).gameObject;
        tenTurn = confirmObj.transform.GetChild(1).gameObject;
        infoConfirm.text = "1��100G�ŉ񂵂܂�";
        confirmObj.SetActive(false);

        //�����Ȃ��悤�ɂ���
        gachaResultPanelObj.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch(drawType)
        {
            case panelName.confirm:
                ConfirmPanel();
                break;
            case panelName.result:
                ResultPanel();
                break;
            case panelName.gacha:
            default:
                GachaPanel();
                break;
        }
    }

    /*------------------�K�`���p�l��------------------*/
    /// <summary>
    /// �K�`���̃p�l���ł����ŉ񂷂��o���l�ŉ񂷂���I��
    /// </summary>
    private void GachaPanel()
    {
        //�L�����Z�����������Ƃ�
        if (Cancel.WasPressedThisFrame())
        {
            //��\���ɂ��ā@�I������
            this.gameObject.SetActive(false);
            audioSource.PlayOneShot(submitSound);
            return;
        }

        //�S�[���h�ŉ񂷂����x���|�C���g�ŉ񂷂���ύX
        if (Navigate.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(moveSound);
            isGoldGacha = !isGoldGacha;

            if (isGoldGacha)
            {
                rate = "coin";
                infomation.text = $"1��@100G\n�����ŉ񂷂��Ƃ��ł���B";
            }
            else
            {
                rate = "level";
                infomation.text = $"1��@10P\n�o���l�ŉ񂷂��Ƃ��ł���B���ǋC��t���āB�g���������Lv�����������Ⴄ�B";
            }

            var scale = new Vector3(0.8f, 0.8f, 0.8f);

            goldGacha.transform.localScale = scale;
            levelPointGacha.transform.localScale = scale;
        }

        //�����ŉ񂷂ق���I��ł��鎞
        if(isGoldGacha)
        {
            scalingScript.ScalingObjPosition(selectFrame.transform, goldGacha.transform.position);
        }
        else
        {
            scalingScript.ScalingObjPosition(selectFrame.transform, levelPointGacha.transform.position);
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //�܂��񂵂Ă��ȂƂ��A����{�^�����������Ƃ�
        if (Submit.WasPressedThisFrame())
        {
            //�����Ă��邩
            ItemHave();

            audioSource.PlayOneShot(submitSound);
            
            drawType = panelName.confirm;
            confirmObj.SetActive(true);
            return;
        }
    }

    /// <summary>
    /// 1��񂷂�10��񂷂���I��
    /// </summary>
    private void ConfirmPanel()
    {
        //�܂��񂵂Ă��ȂƂ��A�L�����Z������������O�ɖ߂�
        if (!isGacha && Cancel.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            confirmObj.SetActive(false);
            drawType = panelName.gacha;
            isOneTurn = true;
            return;
        }

        if(Navigate.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(moveSound);

            isOneTurn = !isOneTurn;
            var scale = new Vector3(0.8f, 0.8f, 0.8f);

            oneTurn.transform.localScale = scale;
            tenTurn.transform.localScale = scale;

            if(isOneTurn)
            {
                gachaCount = 1;
            }
            else
            {
                gachaCount = 10;
            }
            //�����Ă��邩
            ItemHave();
        }

        if(isOneTurn)
        {
            //ChangeSize(oneTurn);
            scalingScript.ScalingObjPosition(selectFrame.transform,oneTurn.transform.position);
        }
        else
        {
            //ChangeSize(tenTurn);
            scalingScript.ScalingObjPosition(selectFrame.transform, tenTurn.transform.position);
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //�A�C�e���������Ă��Ȃ��Ƃ�
        if (!isHaveGoldOrPoint)
        {
            //Debug.Log("�����ĂȂ�");
            if (Submit.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(cancelSound);
            }
            return;
        }

        //�܂��񂵂Ă��ȂƂ��A����{�^�����������Ƃ�
        if (!isGacha && Submit.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            isGacha = true;
            Gacha();
            return;
        }

        //�K�`�����񂵏I�����Ƃ�
        if (isResultReturn)
        {
            drawType = panelName.result;
            confirmObj.SetActive(false);
            //���ʂ�\��
            CreateItem();
            return;
        }

    }

    /// <summary>
    /// �񂵂����ʂ�\������
    /// </summary>
    private void ResultPanel()
    {
        //���� �L�����Z�� ���������玟�֐i��
        if (Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            //��\���ɂ���
            gachaResultPanelObj.SetActive(false);

            drawType = panelName.gacha;
            return;
        }
    }

    /*------------------�K�`��------------------*/
    /// <summary>
    /// �A�C�e���������Ă��邩������
    /// </summary>
    private void ItemHave()
    {
        Debug.Log("�A�C�e���������Ă��邩����");

        isHaveGoldOrPoint = false;
        if (isGoldGacha)
        {
            infoConfirm.text = $"{gachaCount}��@{100 * gachaCount}G �ŉ�";

            //�A�C�e���������Ă��邩
            if (playerItem.GetItemSourceData("gold"))
            {
                Debug.Log("�S�[���h�������Ă���");
                //�A�C�e���̐����m�F����
                if (playerItem.GetItemCountData("gold") >= 100 * gachaCount)
                {
                    Debug.Log("�S�[���h��100�ȏ㎝���Ă���");
                    isHaveGoldOrPoint = true;
                }
            }
        }
        else
        {
            infoConfirm.text = $"{gachaCount}��@{10 * gachaCount}P �ŉ�";
            //���x���|�C���g�������Ă��邩
            if (playerStartas.GetLevelPoint() >= 10 * gachaCount)
            {
                Debug.Log("���x����10�ȏ㎝���Ă��Ȃ�");
                isHaveGoldOrPoint = true;
            }
        }
    }

    /// <summary>
    /// �K�`��
    /// </summary>
    private void Gacha()
    {
        Debug.Log("�K�`��");
        audioSource.Stop();
        audioSource.PlayOneShot(gachaSound);

        string url = @"http://saya-2003.moo.jp/AccountDB/gachaResult.php";//@"http://saya-2003.moo.jp/AccountDB/Gacha.php";//@"http://localhost/Login/Gacha.php";//
        StartCoroutine(GachaRequest(url));
    }

    //�K�`���̌��ʂ�Ԃ��ʐM
    IEnumerator GachaRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("count", gachaCount);
        form.AddField("rate", rate);

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);
        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        //if(postRequest.isNetworkError || postRequest.isHttpError)
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
        }
        else
        {
            //�ʐM�����@���X�|���X��\��
            Debug.Log(postRequest.downloadHandler.text);//�����Ă����e�L�X�g��\��

            isResultReturn = true;

            var str = postRequest.downloadHandler.text;
            resultItemName = str.Split('\n');

            if(resultItemName[0] == "")
            {
                isResultReturn = false;
                StartCoroutine(GachaRequest(url));
            }
            else
            {
                if (isGoldGacha)
                {
                    playerItem.UseItem("gold", 100 * gachaCount);
                    myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
                }
                else
                {
                    playerStartas.UseLevelPoint(10 * gachaCount);
                    myLevelPointText.text = $"{playerStartas.GetLevelPoint()}P";
                }
            }
        }
    }

    /// <summary>
    /// �񂵂����ʂ�\������
    /// </summary>
    private void CreateItem()
    {
        if(slotList.Count == 10)
        {
            foreach (var slot in slotList)
            {
                Destroy(slot.gameObject);
            }
            slotList.Clear();
        }

        gachaResultPanelObj.SetActive(true);
        Debug.Log("�A�C�e�����쐬");
        var i = 0;
        var child = gachaResultPanelObj.transform.GetChild(0).gameObject;

        foreach (var itemName in this.resultItemName)
        {
            foreach (var item in itemData.GetItemLists())
            {
                if (item.Id == itemName)
                {
                    //�X���b�g�̃C���X�^���X��
                    var instanceSlot = Instantiate(slot, child.transform);
                    //�X���b�g�̃Q�[���I�u�W�F�N�g����ݒ�
                    instanceSlot.transform.name = "ItemSlot" + i++;
                    //Scale��ݒ肵�Ȃ���0�ɂȂ�̂Őݒ�
                    instanceSlot.transform.localScale = Vector3.one;
                    //�A�C�e�������X���b�g��SlotInfo�ɐݒ肷��
                    instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItem.GetItemCountData(item.Id));
                    instanceSlot.GetComponent<SlotInfo>().GoldText(false);

                    slotList.Add(instanceSlot);

                    playerItem.CountItem(itemName, 1);
                    break;
                }
            }
        }
        resultText.text = "";
        foreach (var item in itemData.GetItemLists())
        {
            int count = 0;
            foreach(var itemName in this.resultItemName)
            {
                if (item.Id == itemName)
                {
                    count++;
                }
            }

            if(count > 0)
            {
                resultText.text += $"{item.ItemName}�~{count}\n";
            }

        }

        //�܂��񂹂�悤�ɏ�����
        isResultReturn = false;
        isGacha = false;

        //�A�C�e����ۑ�
        itemDataSave.SaveItemData();
        startasLoadOrSave.SaveStartasData();
    }
}
