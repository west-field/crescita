using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class weaponPanel : MonoBehaviour
{
    //アイテム情報のスロットのプレハブ
    [SerializeField] private GameObject slot;
    private List<GameObject> slotList = new List<GameObject>();//作成したスロットを持つ
    [SerializeField] private TextMeshProUGUI information;//説明

    /*アイテム*/
    [SerializeField] private ItemDataBase itemData;//アイテムデータベース
    private PlayerItem playerItem;//自身が持っているアイテム
    private PlayerStartas playerStartas;//プレイヤーのステータス

    private struct Material
    {
        public string name;
        public int count;
    }
    private List<Material> materialName = new List<Material>();//強化に必要な素材名

    [SerializeField] private GameObject myGold, gold;//自身が持っているお金、必要のお金
    private TextMeshProUGUI myGoldText, goldText;//お金表示用

    /*キー*/
    private MainManager mainManager;
    private InputAction Submit;//アクションマップからアクションの取得

    /*音*/
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip weaponSound, moveSound, levelUpSound, noSound;//開くときの音、カーソルの音

    private bool isFirst = true;

    /*武器レベルアップ*/
    private bool isLevelUp;
    [SerializeField] private GameObject levelUpPanelObj;

    //オブジェクトがアクティブになった時
    private void OnEnable()
    {
        if (isFirst) return;
        myGoldText.text = $"0G";
        Debug.Log("検索");
        //プレイヤータグを検索
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //もしあればアイテムを取得する
        foreach (var item in obj)
        {
            Debug.Log("いた");
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            playerStartas = item.GetComponent<PlayerStartas>();

            CreateSlot();
            break;
        }
        Debug.Log("武器強化パネルアクティブ");
        audioSource.volume = HoldVariable.SEVolume;
    }
    //オブジェクトが非表示になった時
    private void OnDisable()
    {
        foreach (var slot in slotList)
        {
            Destroy(slot.gameObject);
        }
        slotList.Clear();

        materialName.Clear();
        Debug.Log("武器強化パネル非アクティブ");
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

        Debug.Log("武器強化パネル作成");
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

        //決定を押したとき
        if (Submit.WasPressedThisFrame())
        {
            if(playerStartas == null)
            {
                return;
            }

            //必要な素材がそろっているかどうかを調べる
            bool isHaveMaterial = false;

            foreach (var item in materialName)
            {
                isHaveMaterial = HaveTheMaterial(item.name, item.count);

                if(item.name == "gold")
                {
                    if(item.count == 0)
                    {
                        isHaveMaterial = false;
                        //音を鳴らす　(ブブー)
                        audioSource.PlayOneShot(noSound);
                        return;
                    }
                }

                if (!isHaveMaterial) break;
                Debug.Log($"武器屋　{item.name}を{item.count}持っている");
            }

            if (isHaveMaterial)
            {
                foreach (var item in materialName)
                {
                   DeleteItem(item.name, item.count);
                }

                //音を鳴らす(カンカン)
                audioSource.PlayOneShot(levelUpSound);
                //必要な素材がそろっている時武器レベルを上げる
                playerStartas.WeponLevelUp();
                isLevelUp = true;
                levelUpPanelObj.SetActive(true);
            }
            else
            {
                //音を鳴らす　(ブブー)
                audioSource.PlayOneShot(noSound);
                information.text += "足りない。。";
            }
        }
    }

    //アイテムを必要な数持っているかどうかを確認する
    private bool HaveTheMaterial(string materialName,int num)
    {
        foreach (var item in itemData.GetItemLists())
        {
            //必要な素材ではないときは処理しない
            if (item.Id != materialName) continue;

            //アイテムを持っているか
            if (playerItem.GetItemSourceData(item.Id))
            {
                //アイテムの数を確認する
                if(playerItem.GetItemCountData(item.Id) >= num)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //アイテムを使用する
    private void DeleteItem(string materialName, int num)
    {
        foreach (var item in itemData.GetItemLists())
        {
            //必要な素材ではないときは処理しない
            if (item.Id != materialName) continue;

            //アイテムを持っているか
            if (playerItem.GetItemSourceData(item.Id))
            {
                //アイテムを使用する
                playerItem.UseItem(materialName, num);
                Debug.Log($"武器屋　{materialName}が{playerItem.GetItemCountData(item.Id)}個になった。");
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
            //素材ではないとき処理をしない
            if (item.ItemType != ItemType.material) continue;

            //必要な素材ではないときは処理しない
            foreach(var materialName in materialName)
            {
                if(item.Id == materialName.name)
                {
                    //スロットのインスタンス化
                    var instanceSlot = Instantiate(slot, child.transform);
                    //スロットのゲームオブジェクト名を設定
                    instanceSlot.transform.name = "ItemSlot" + i++;
                    //Scaleを設定しないと0になるので設定
                    instanceSlot.transform.localScale = Vector3.one;
                    //アイテム情報をスロットのSlotInfoに設定する
                    instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItem.GetItemCountData(item.Id));
                    instanceSlot.GetComponent<SlotInfo>().GoldText(false);

                    slotList.Add(instanceSlot);
                    break;
                }
            }
        }

        Debug.Log("作成" + i);
        information.text = "";
        foreach (var slot in materialName)
        {
            if (slot.name == "gold")
            {
                goldText.text = $"{slot.count}G";
                if(slot.count == 0)
                {
                    information.text = "これ以上強くできない。。。。";
                    return;
                }
                continue;
            }

            foreach (var item in itemData.GetItemLists())
            {
                if (slot.name == item.Id)
                {
                    information.text += $"{item.ItemName}が{slot.count}個,";
                }
            }
        }
        information.text += "必要\n";
    }
}
