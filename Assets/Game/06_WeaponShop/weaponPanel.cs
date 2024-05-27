using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class weaponPanel : MonoBehaviour
{
    //�A�C�e�����̃X���b�g�̃v���n�u
    [SerializeField] private GameObject slot;
    private List<GameObject> slotList = new List<GameObject>();//�쐬�����X���b�g������
    [SerializeField] private TextMeshProUGUI information;//����

    /*�A�C�e��*/
    [SerializeField] private ItemDataBase itemData;//�A�C�e���f�[�^�x�[�X
    private PlayerItem playerItem;//���g�������Ă���A�C�e��
    private PlayerStartas playerStartas;//�v���C���[�̃X�e�[�^�X

    private struct Material
    {
        public string name;
        public int count;
    }
    private List<Material> materialName = new List<Material>();//�����ɕK�v�ȑf�ޖ�

    [SerializeField] private GameObject myGold, gold;//���g�������Ă��邨���A�K�v�̂���
    private TextMeshProUGUI myGoldText, goldText;//�����\���p

    /*�L�[*/
    private MainManager mainManager;
    private InputAction Submit;//�A�N�V�����}�b�v����A�N�V�����̎擾

    /*��*/
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip weaponSound, moveSound, levelUpSound, noSound;//�J���Ƃ��̉��A�J�[�\���̉�

    private bool isFirst = true;

    /*���탌�x���A�b�v*/
    private bool isLevelUp;
    [SerializeField] private GameObject levelUpPanelObj;

    //�I�u�W�F�N�g���A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        if (isFirst) return;
        myGoldText.text = $"0G";
        Debug.Log("����");
        //�v���C���[�^�O������
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //��������΃A�C�e�����擾����
        foreach (var item in obj)
        {
            Debug.Log("����");
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            playerStartas = item.GetComponent<PlayerStartas>();

            CreateSlot();
            break;
        }
        Debug.Log("���틭���p�l���A�N�e�B�u");
        audioSource.volume = HoldVariable.SEVolume;
    }
    //�I�u�W�F�N�g����\���ɂȂ�����
    private void OnDisable()
    {
        foreach (var slot in slotList)
        {
            Destroy(slot.gameObject);
        }
        slotList.Clear();

        materialName.Clear();
        Debug.Log("���틭���p�l����A�N�e�B�u");
    }

    private void Awake()
    {
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Submit = mainManager.GetPlayerInput().actions["fire"];

        foreach (var item in itemData.GetItemLists())
        {
            if (item.Id == "gold")
            {
                myGold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
                gold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
            }
        }

        myGoldText = myGold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        goldText = gold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        isFirst = false;

        isLevelUp = false;

        levelUpPanelObj.SetActive(false);

        Debug.Log("���틭���p�l���쐬");
    }

    private void Update()
    {
        if (isLevelUp)
        {
            if (Submit.WasPressedThisFrame())
            {
                levelUpPanelObj.SetActive(false);
                CreateSlot();
                isLevelUp = false;
            }

            return;
        }

        //������������Ƃ�
        if (Submit.WasPressedThisFrame())
        {
            if(playerStartas == null)
            {
                return;
            }

            //�K�v�ȑf�ނ�������Ă��邩�ǂ����𒲂ׂ�
            bool isHaveMaterial = false;

            foreach (var item in materialName)
            {
                isHaveMaterial = HaveTheMaterial(item.name, item.count);

                if(item.name == "gold")
                {
                    if(item.count == 0)
                    {
                        isHaveMaterial = false;
                        //����炷�@(�u�u�[)
                        audioSource.PlayOneShot(noSound);
                        return;
                    }
                }

                if (!isHaveMaterial) break;
                Debug.Log($"���퉮�@{item.name}��{item.count}�����Ă���");
            }

            if (isHaveMaterial)
            {
                foreach (var item in materialName)
                {
                   DeleteItem(item.name, item.count);
                }

                //����炷(�J���J��)
                audioSource.PlayOneShot(levelUpSound);
                //�K�v�ȑf�ނ�������Ă��鎞���탌�x�����グ��
                playerStartas.WeponLevelUp();
                isLevelUp = true;
                levelUpPanelObj.SetActive(true);
            }
            else
            {
                //����炷�@(�u�u�[)
                audioSource.PlayOneShot(noSound);
                information.text += "����Ȃ��B�B";
            }
        }
    }

    //�A�C�e����K�v�Ȑ������Ă��邩�ǂ������m�F����
    private bool HaveTheMaterial(string materialName,int num)
    {
        foreach (var item in itemData.GetItemLists())
        {
            //�K�v�ȑf�ނł͂Ȃ��Ƃ��͏������Ȃ�
            if (item.Id != materialName) continue;

            //�A�C�e���������Ă��邩
            if (playerItem.GetItemSourceData(item.Id))
            {
                //�A�C�e���̐����m�F����
                if(playerItem.GetItemCountData(item.Id) >= num)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //�A�C�e�����g�p����
    private void DeleteItem(string materialName, int num)
    {
        foreach (var item in itemData.GetItemLists())
        {
            //�K�v�ȑf�ނł͂Ȃ��Ƃ��͏������Ȃ�
            if (item.Id != materialName) continue;

            //�A�C�e���������Ă��邩
            if (playerItem.GetItemSourceData(item.Id))
            {
                //�A�C�e�����g�p����
                playerItem.UseItem(materialName, num);
                Debug.Log($"���퉮�@{materialName}��{playerItem.GetItemCountData(item.Id)}�ɂȂ����B");
                break;
            }
        }
    }

    private void CreateSlot()
    {
        var i = 0;
        var child = this.transform.GetChild(0).gameObject;

        myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";

        foreach (var slot in slotList)
        {
            Destroy(slot.gameObject);
        }
        slotList.Clear();

        materialName.Clear();
        Material material;
        switch (playerStartas.GetWeponPower())
        {
            case 2:
                material.name = "redStuffy";
                material.count = 5;
                materialName.Add(material);
                material.name = "gold";
                material.count = 100;
                materialName.Add(material);
                break;
            case 4:
                material.name = "redStuffy";
                material.count = 10; 
                materialName.Add(material);
                material.name = "redJigglyCore";
                material.count = 5;
                materialName.Add(material);
                material.name = "gold";
                material.count = 300;
                materialName.Add(material);
                break;
            case 8:
                material.name = "frogHorn";
                material.count = 10;
                materialName.Add(material);
                material.name = "frog'sPearl";
                material.count = 5;
                materialName.Add(material);
                material.name = "gold";
                material.count = 500;
                materialName.Add(material);
                break;
            case 16:
                material.name = "frog'sGoldenBeads";
                material.count = 5;
                materialName.Add(material);
                material.name = "frog'sBlackJewel";
                material.count = 5;
                materialName.Add(material);
                material.name = "scarabSwallowtail";
                material.count = 2;
                materialName.Add(material);
                material.name = "gold";
                material.count = 1000;
                materialName.Add(material);
                break;
            default:
                material.name = "gold";
                material.count = 0;
                materialName.Add(material);
                break;
        }

        foreach (var item in itemData.GetItemLists())
        {
            //�f�ނł͂Ȃ��Ƃ����������Ȃ�
            if (item.ItemType != ItemType.material) continue;

            //�K�v�ȑf�ނł͂Ȃ��Ƃ��͏������Ȃ�
            foreach(var materialName in materialName)
            {
                if(item.Id == materialName.name)
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
                    break;
                }
            }
        }

        Debug.Log("�쐬" + i);
        information.text = "";
        foreach (var slot in materialName)
        {
            if (slot.name == "gold")
            {
                goldText.text = $"{slot.count}G";
                if(slot.count == 0)
                {
                    information.text = "����ȏ㋭���ł��Ȃ��B�B�B�B";
                    return;
                }
                continue;
            }

            foreach (var item in itemData.GetItemLists())
            {
                if (slot.name == item.Id)
                {
                    information.text += $"{item.ItemName}��{slot.count}��,";
                }
            }
        }
        information.text += "�K�v\n";
    }
}
