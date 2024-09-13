using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

//����
public class BuyScript : MonoBehaviour
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
    /// ����A�C�e���̃f�[�^������
    /// </summary>
    private List<PocketItem> items = new List<PocketItem>();
    /// <summary>
    /// ���g�������Ă���A�C�e��
    /// </summary>
    private PlayerItem playerItem;
    /// <summary>
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
    [SerializeField] private AudioClip pauseSound, moveSound, buySound, noSound;//�|�[�Y���J���Ƃ��̉�,�J�[�\���ړ���

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
            playerItem = item.GetComponent<PlayerItem>();//�����������Ă���A�C�e�����擾����
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";//�����̂������e�L�X�g�ɕ\������
            selectNum = 0;//��ԏ��߂�I������
            CreateSlot();//�X���b�g���쐬����
            //�g��k������t���[����I�����Ă���ʒu�ɕύX����
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            break;
        }

        Debug.Log("�w���p�l���A�N�e�B�u");

        audioSource.volume = HoldVariable.SEVolume;//���ݒ肵�Ă��鉹�ʂɂ���
        isFirst = true;//�\���ɂȂ������ɏ���������

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
        slotObj.Clear();//�쐬�����A�C�e���̃X���b�g���폜����
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

        //�A�C�e���f�[�^���烉���_���Ŕ���A�C�e�������
        foreach (var item in itemData.GetItemLists())
        {
            //�A�C�e���^�C�v���f�ނ̎�
            if (item.ItemType == ItemType.material)
            {
                //�w�����z��0�̂Ƃ��͏��������Ȃ�
                if (item.BuyingPrice == 0) continue;

                Debug.Log(item.ItemName);

                var rand = Random.Range(0, 10) % 2;

                //�����_���Ŋ���؂��Ƃ�
                if (rand == 0)
                {
                    items.Add(item);
                }
            }
        }

        selectNum = 0;
        selectMaxNum = items.Count;//�̔����Ă���A�C�e����

        foreach (var item in itemData.GetItemLists())
        {
            if (item.Id == "gold")
            {
                //�����̃X�N���v�g���Ƃ�
                myGold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
                break;
            }
        }

        //�����̂����̃e�L�X�g���擾����
        myGoldText = myGold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        //�g��k���̍ő�l�A�ŏ��l�����߂�
        scaling.Init(1.3f, 1.0f);
        Debug.Log("�w���p�l���쐬");
    }

    private void Update()
    {
        //��x�������
        if (isFirst)
        {
            isFirst = false;
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();//�I�����Ă���A�C�e���̐��������e�L�X�g�ɕ\������
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);//�I�����Ă���ʒu�ɕύX����
        }

        bool isPress = false;
        //�ړ��{�^���������Ă���Ƃ�
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
            //�T�E���h��炷
            audioSource.PlayOneShot(moveSound);

            //�I������Ă��Ȃ��Ƃ��͖��O���폜����
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
                            var gold = data.BuyingPrice;

                            //�������g��������K�v�ȕ������Ă��邩
                            if (playerItem.GetItemCountData("gold") >= gold)
                            {
                                audioSource.PlayOneShot(buySound);
                                //�����Ă���ꍇ�͎擾�ł���
                                playerItem.CountItem(itemId, 1);
                                playerItem.UseItem("gold", gold);

                                //�\���������ύX����
                                var slotInfo = itemObj.GetComponent<SlotInfo>();
                                slotInfo.ItemCount(playerItem.GetItemCountData(itemId));

                                break;
                            }
                            else
                            {
                                //�����Ă��Ȃ��Ƃ��͔����Ȃ�
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

    //�X���b�g���쐬����
    private void CreateSlot()
    {
        var i = 0;
        var child = this.transform.GetChild(1).gameObject;

        slotObj.Clear();

        foreach (var item in items)
        {
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
            instanceSlot.GetComponent<SlotInfo>().GoldText(true, true);

            //�I���ł���悤�Ɏ����Ă���
            slotObj.Add(instanceSlot);
        }
    }
}
