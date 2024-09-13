using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

//売る
public class SellScript : MonoBehaviour
{
    /// <summary>
    /// アイテム情報のスロットのプレハブ
    /// </summary>
    [SerializeField] private GameObject slot;

    /*アイテム*/
    /// <summary>
    /// アイテムデータベース
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// 自身が持っているアイテム
    /// </summary>
    private PlayerItem playerItem;
    /// アイテム表示のために作成したスロット
    /// </summary>
    private List<GameObject> slotObj = new List<GameObject>();

    /// <summary>
    /// 自身が持っているお金
    /// </summary>
    [SerializeField] private GameObject myGold;
    /// <summary>
    /// 商品のお金表示用
    /// </summary>
    private TextMeshProUGUI myGoldText;

    /*選択*/
    /// <summary>
    /// 今選択しているアイテム
    /// </summary>
    private int selectNum;
    /// <summary>
    /// 選択できる数
    /// </summary>
    private int selectMaxNum;

    /*キー*/
    /// <summary>
    /// メインマネージャー
    /// </summary>
    private MainManager mainManager;
    /// <summary>
    /// アクションマップからアクションの取得
    /// </summary>
    InputAction Navigate, Submit, Cancel;

    /*音*/
    /// <summary>
    /// サウンド
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// 音
    /// </summary>
    [SerializeField] private AudioClip pauseSound, moveSound, sellSound, noSound;//ポーズを開くときの音,カーソル移動音

    /// <summary>
    /// 一度だけ
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// 選択フレーム
    /// </summary>
    [SerializeField] private Transform selectFrame;
    /// <summary>
    /// 枠の拡大縮小
    /// </summary>
    [SerializeField] private Scaling scaling;

    //オブジェクトがアクティブになった時
    private void OnEnable()
    {
        scaling.Init(1.3f, 1.0f);
        //プレイヤータグを検索
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //もしあればアイテムを取得する
        foreach (var item in obj)
        {
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            selectNum = 0;
            //アイテムスロットを作成
            CreateSlot();
            //拡大縮小するフレームを選択している位置に変更する
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            break;
        }

        Debug.Log("売却パネルアクティブ");

        audioSource.volume = HoldVariable.SEVolume;
        isFirst = true;

        //選択しているときに表示する名前を削除する
        foreach (var itemObj in slotObj)
        {
            itemObj.GetComponent<SlotInfo>().DestoroyInformation();
        }
    }

    //オブジェクトが非表示になった時
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
        selectMaxNum = 0;//自分が持っているアイテム数

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
        Debug.Log("売却パネル作成");
    }

    private void Update()
    {
        if(isFirst)
        {
            isFirst = false;
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);//選択している位置に変更する
        }

        bool isPress = false;
        if (Navigate.WasPressedThisFrame())
        {
            //Navigate.ReadValue<Vector2>();
            //Debug.Log(Navigate.ReadValue<Vector2>());

            if (Navigate.ReadValue<Vector2>().x < 0.0f)
            {
                //左
                selectNum = (selectNum + (selectMaxNum - 1)) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().y > 0.0f)
            {
                //上
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
                //右
                selectNum = (selectNum + 1) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().y < 0.0f)
            {
                //下
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
            //サウンドを鳴らす
            audioSource.PlayOneShot(moveSound);
            //選択している項目の色を変える
            foreach (var itemObj in slotObj)
            {
                itemObj.GetComponent<SlotInfo>().DestoroyInformation();
            }
            //選択している説明文を作成する、選択している位置に変更する
            scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
            slotObj[selectNum].GetComponent<SlotInfo>().CreateInfomation();
        }

        scaling.ScalingObj(selectFrame);

        //決定を押したとき
        if (Submit.WasPressedThisFrame())
        {
            //選択した項目のアイテムをプレイヤーアイテムに追加する
            int i = 0;
            foreach (var itemObj in slotObj)
            {
                if (i == selectNum)
                {
                    var itemId = itemObj.GetComponent<SlotInfo>().ItemData().Id;
                    foreach (var data in itemData.GetItemLists())
                    {
                        //同じIDの時
                        if (itemId == data.Id)
                        {
                            //値段を取得する
                            var gold = data.SellingPrice;

                            //売りたいアイテムの数が0よりも大きいとき
                            if (playerItem.GetItemCountData(itemId) > 0)
                            {
                                audioSource.volume = HoldVariable.SEVolume * 0.5f;
                                audioSource.PlayOneShot(sellSound);
                                //売ることができる
                                playerItem.UseItem(itemId, 1);
                                playerItem.CountItem("gold", gold);

                                //表示する情報を変更する
                                var slotInfo = itemObj.GetComponent<SlotInfo>();
                                slotInfo.ItemCount(playerItem.GetItemCountData(itemId));
                                break;
                            }
                            else
                            {
                                audioSource.volume = HoldVariable.SEVolume;
                                //持っていないときは売れない
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
            //素材ではないとき処理をしない
            if (item.ItemType != ItemType.material) continue;

            //アイテムを持っているか
            if(playerItem.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
                //スロットのインスタンス化
                var instanceSlot = Instantiate(slot, child.transform);
                //スロットのゲームオブジェクト名を設定
                instanceSlot.transform.name = "ItemSlot" + i++;
                //Scaleを設定しないと0になるので設定
                instanceSlot.transform.localScale = Vector3.one;
                //アイテム情報の初期化
                instanceSlot.GetComponent<SlotInfo>().Init();
                //アイテム情報をスロットのSlotInfoに設定する
                instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItem.GetItemCountData(item.Id));
                instanceSlot.GetComponent<SlotInfo>().GoldText(true);

                //選択できるように持っておく
                slotObj.Add(instanceSlot);
            }
        }

        Debug.Log("作成" + selectMaxNum);
    }

}
