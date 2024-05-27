using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

//Plan Main �ɃA�^�b�`
/// <summary>
/// �A�C�e���X���b�g
/// </summary>
public class ItemSlotScript : MonoBehaviour
{
    /*�A�C�e��*/
    /// <summary>
    /// �A�C�e�����̃X���b�g�v���n�u
    /// </summary>
    [SerializeField] private GameObject slot;
    /// <summary>
    /// �������Ă���A�C�e���Ɛ�
    /// </summary>
    [SerializeField] private PlayerItem playerItems;
    /// <summary>
    /// �A�C�e���f�[�^�x�[�X
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// �����\��
    /// </summary>
    [SerializeField] private GameObject gold;

    /// <summary>
    /// �A�C�e���\���̂��߂ɍ쐬�����X���b�g��ێ�
    /// </summary>
    private List<GameObject> slotObj = new List<GameObject>();

    /*�I��*/
    /// <summary>
    /// ���I�����Ă���
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
    private InputAction Navigate;

    /*��*/
    /// <summary>
    /// �T�E���h
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// �J�[�\���ړ���
    /// </summary>
    [SerializeField] private AudioClip moveSound;

    /// <summary>
    /// ��ԏ��߂���
    /// </summary>
    private bool isFirst;

    /// <summary>
    /// �I���t���[��
    /// </summary>
    [SerializeField] private Transform selectFrame;
    [SerializeField] private Scaling scaling;

    private void Start()
    {
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];

        selectNum = 0;
        selectMaxNum = 0;
        foreach (var item in itemData.itemList)
        {
            //�A�C�e���̃^�C�v���f�ނł͂Ȃ��Ƃ����������Ȃ�
            if (item.ItemType != ItemType.material) continue;
            //�A�C�e���̃f�[�^������Ƃ�
            if (playerItems.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
            }
        }
        scaling.Init(1.3f, 1.0f);
        scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
        Debug.Log("�쐬" + selectMaxNum);
    }

    //�A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        isFirst = true;
        //�v���C���[�^�O������
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //��������΃A�C�e�����擾����
        foreach (var item in obj)
        {
            //Debug.Log("����");
            playerItems = item.GetComponent<PlayerItem>();
            break;
        }

        selectNum = 0;
        CreateSlot(itemData.itemList);

        //�I������Ă��Ȃ��Ƃ��͖��O���폜����
        var i = 0;
        foreach (var itemObj in slotObj)
        {
            if (i != selectNum)
            {
                itemObj.GetComponent<SlotInfo>().DestoroyInformation();
            }
            i++;
        }
        scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
    }

    //�A�C�e���X���b�g�̍쐬
    private void CreateSlot(List<PocketItem> itemList)
    {
        var i = 0;
        
        slotObj.Clear();

        selectMaxNum = 0;

        foreach (var item in itemList)
        {
            //�����͉E��ɕ\������̂ŕK�v�Ȃ��̂��擾����
            if(item.Id == "gold")
            {
                var itemCount = playerItems.GetItemCountData(item.Id);
                //�A�C�e���̃X�v���C�g��ݒ�
                gold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
                //�A�C�e���̌���\��
                gold.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{itemCount}G";
            }
            //�A�C�e���̃^�C�v���f�ނł͂Ȃ��Ƃ����������Ȃ�
            if (item.ItemType != ItemType.material) continue;
            //�A�C�e���̃f�[�^������Ƃ�
            if(playerItems.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
                //�@�X���b�g�̃C���X�^���X��
                var instanceSlot = Instantiate<GameObject>(slot, transform);
                //�@�X���b�g�Q�[���I�u�W�F�N�g�̖��O��ݒ�
                instanceSlot.name = "ItemSlot" + i++;
                //�@Scale��ݒ肵�Ȃ���0�ɂȂ�̂Őݒ�
                instanceSlot.transform.localScale = new Vector3(1f, 1f, 1f);
                //�A�C�e�����̏�����
                instanceSlot.GetComponent<SlotInfo>().Init();
                //�@�A�C�e�������X���b�g��SlotInfo�ɐݒ肷��
                instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItems.GetItemCountData(item.Id));
                instanceSlot.GetComponent<SlotInfo>().GoldText(false);

                //�I���ł���悤�Ɏ����Ă���
                slotObj.Add(instanceSlot);
            }
        }
    }

    private void Update()
    {
        if(isFirst)
        {
            isFirst = false;
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
        }
        //����A�C�e�����Ȃ��Ƃ�
        if (selectMaxNum == 0)
        {
            return;
        }

        bool isPress = false;
        if (Navigate.WasPressedThisFrame())
        {
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
            var i = 0;
            foreach(var itemObj in slotObj)
            {
                if(i != selectNum)
                {
                    itemObj.GetComponent<SlotInfo>().DestoroyInformation();
                }
                i++;
            }

            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
        }

        scaling.ScalingObj(selectFrame);
    }
}
