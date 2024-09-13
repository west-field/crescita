using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

//����
public class SellScript : MonoBehaviour
{
    /// <summary>
    /// �A�C�e�����̃X���b�g�̃v���n�u
    /// </summary>
    [SerializeField] private GameObject slot;

    /*�A�C�e��*/
    /// <summary>
    /// �A�C�e���f�[�^�x�[�X
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// ���g�������Ă���A�C�e��
    /// </summary>
    private PlayerItem playerItem;
    /// �A�C�e���\���̂��߂ɍ쐬�����X���b�g
    /// </summary>
    private List<GameObject> slotObj = new List<GameObject>();

    /// <summary>
    /// ���g�������Ă��邨��
    /// </summary>
    [SerializeField] private GameObject myGold;
    /// <summary>
    /// ���i�̂����\���p
    /// </summary>
    private TextMeshProUGUI myGoldText;

    /*�I��*/
    /// <summary>
    /// ���I�����Ă���A�C�e��
    /// </summary>
    private int selectNum;
    /// <summary>
    /// �I���ł��鐔
    /// </summary>
    private int selectMaxNum;

    /*�L�[*/
    /// <summary>
    /// ���C���}�l�[�W���[
    /// </summary>
    private MainManager mainManager;
    /// <summary>
    /// �A�N�V�����}�b�v����A�N�V�����̎擾
    /// </summary>
    InputAction Navigate, Submit, Cancel;

    /*��*/
    /// <summary>
    /// �T�E���h
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// ��
    /// </summary>
    [SerializeField] private AudioClip pauseSound, moveSound, sellSound, noSound;//�|�[�Y���J���Ƃ��̉�,�J�[�\���ړ���

    /// <summary>
    /// ��x����
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// �I���t���[��
    /// </summary>
    [SerializeField] private Transform selectFrame;
    /// <summary>
    /// �g�̊g��k��
    /// </summary>
    [SerializeField] private Scaling scaling;

    //�I�u�W�F�N�g���A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        scaling.Init(1.3f, 1.0f);
        //�v���C���[�^�O������
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //��������΃A�C�e�����擾����
        foreach (var item in obj)
        {
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            selectNum = 0;
            //�A�C�e���X���b�g���쐬
            CreateSlot();
            //�g��k������t���[����I�����Ă���ʒu�ɕύX����
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            break;
        }

        Debug.Log("���p�p�l���A�N�e�B�u");

        audioSource.volume = HoldVariable.SEVolume;
        isFirst = true;

        //�I�����Ă���Ƃ��ɕ\�����閼�O���폜����
        foreach (var itemObj in slotObj)
        {
            itemObj.GetComponent<SlotInfo>().DestoroyInformation();
        }
    }

    //�I�u�W�F�N�g����\���ɂȂ�����
    private void OnDisable()
    {
        scaling.Init(0.9f, 0.7f);
        slotObj.Clear();
    }

    private void Awake()
    {
        audioSource = this.GetComponent<Transform>().parent.GetComponent<AudioSource>();
    }

    private void Start()
    {
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        selectNum = 0;
        selectMaxNum = 0;//�����������Ă���A�C�e����

        foreach (var item in itemData.GetItemLists())
        {
            if(item.Id == "gold")
            {
                myGold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
                break;
            }
        }

        myGoldText = myGold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        scaling.Init(1.3f, 1.0f);
        Debug.Log("���p�p�l���쐬");
    }

    private void Update()
    {
        if(isFirst)
        {
            isFirst = false;
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);//�I�����Ă���ʒu�ɕύX����
        }

        bool isPress = false;
        if (Navigate.WasPressedThisFrame())
        {
            //Navigate.ReadValue<Vector2>();
            //Debug.Log(Navigate.ReadValue<Vector2>());

            if (Navigate.ReadValue<Vector2>().x < 0.0f)
            {
                //��
                selectNum = (selectNum + (selectMaxNum - 1)) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().y > 0.0f)
            {
                //��
                if (selectNum + -4 < 0)
                {
                    selectNum += 4;
                    //selectNum += (selectMaxNum - 1);
                }
                else
                {
                    selectNum += (selectMaxNum - 4);
                }
                selectNum = (selectNum) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().x > 0.0f)
            {
                //�E
                selectNum = (selectNum + 1) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().y < 0.0f)
            {
                //��
                if (selectNum + 4 > selectMaxNum)
                {
                    selectNum += (selectMaxNum - 4);
                    //selectNum += 1;
                }
                else
                {
                    selectNum += 4;
                }
                selectNum = (selectNum) % selectMaxNum;
            }

            isPress = true;
        }

        if (isPress)
        {
            audioSource.volume = HoldVariable.SEVolume;
            //�T�E���h��炷
            audioSource.PlayOneShot(moveSound);
            //�I�����Ă��鍀�ڂ̐F��ς���
            foreach (var itemObj in slotObj)
            {
                itemObj.GetComponent<SlotInfo>().DestoroyInformation();
            }
            //�I�����Ă�����������쐬����A�I�����Ă���ʒu�ɕύX����
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
        }

        scaling.ScalingObj(selectFrame);

        //������������Ƃ�
        if (Submit.WasPressedThisFrame())
        {
            //�I���������ڂ̃A�C�e�����v���C���[�A�C�e���ɒǉ�����
            int i = 0;
            foreach (var itemObj in slotObj)
            {
                if (i == selectNum)
                {
                    var itemId = itemObj.GetComponent<SlotInfo>().ItemData().Id;
                    foreach (var data in itemData.GetItemLists())
                    {
                        //����ID�̎�
                        if (itemId == data.Id)
                        {
                            //�l�i���擾����
                            var gold = data.SellingPrice;

                            //���肽���A�C�e���̐���0�����傫���Ƃ�
                            if (playerItem.GetItemCountData(itemId) > 0)
                            {
                                audioSource.volume = HoldVariable.SEVolume * 0.5f;
                                audioSource.PlayOneShot(sellSound);
                                //���邱�Ƃ��ł���
                                playerItem.UseItem(itemId, 1);
                                playerItem.CountItem("gold", gold);

                                //�\���������ύX����
                                var slotInfo = itemObj.GetComponent<SlotInfo>();
                                slotInfo.ItemCount(playerItem.GetItemCountData(itemId));
                                break;
                            }
                            else
                            {
                                audioSource.volume = HoldVariable.SEVolume;
                                //�����Ă��Ȃ��Ƃ��͔���Ȃ�
                                audioSource.PlayOneShot(noSound);
                            }
                        }
                    }
                }
                i++;
            }
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
        }
    }

    private void CreateSlot()
    {
        var i = 0;
        var child = this.transform.GetChild(1).gameObject;

        slotObj.Clear();

        selectMaxNum = 0;

        foreach (var item in itemData.GetItemLists())
        {
            //�f�ނł͂Ȃ��Ƃ����������Ȃ�
            if (item.ItemType != ItemType.material) continue;

            //�A�C�e���������Ă��邩
            if(playerItem.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
                //�X���b�g�̃C���X�^���X��
                var instanceSlot = Instantiate(slot, child.transform);
                //�X���b�g�̃Q�[���I�u�W�F�N�g����ݒ�
                instanceSlot.transform.name = "ItemSlot" + i++;
                //Scale��ݒ肵�Ȃ���0�ɂȂ�̂Őݒ�
                instanceSlot.transform.localScale = Vector3.one;
                //�A�C�e�����̏�����
                instanceSlot.GetComponent<SlotInfo>().Init();
                //�A�C�e�������X���b�g��SlotInfo�ɐݒ肷��
                instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItem.GetItemCountData(item.Id));
                instanceSlot.GetComponent<SlotInfo>().GoldText(true);

                //�I���ł���悤�Ɏ����Ă���
                slotObj.Add(instanceSlot);
            }
        }

        Debug.Log("�쐬" + selectMaxNum);
    }

}
