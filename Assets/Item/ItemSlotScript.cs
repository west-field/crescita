using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

//Plan Main にアタッチ
/// <summary>
/// アイテムスロット
/// </summary>
public class ItemSlotScript : MonoBehaviour
{
    /*アイテム*/
    /// <summary>
    /// アイテム情報のスロットプレハブ
    /// </summary>
    [SerializeField] private GameObject slot;
    /// <summary>
    /// 今持っているアイテムと数
    /// </summary>
    [SerializeField] private PlayerItem playerItems;
    /// <summary>
    /// アイテムデータベース
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// お金表示
    /// </summary>
    [SerializeField] private GameObject gold;

    /// <summary>
    /// アイテム表示のために作成したスロットを保持
    /// </summary>
    private List<GameObject> slotObj = new List<GameObject>();

    /*選択*/
    /// <summary>
    /// 今選択している
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
    private InputAction Navigate;

    /*音*/
    /// <summary>
    /// サウンド
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// カーソル移動音
    /// </summary>
    [SerializeField] private AudioClip moveSound;

    /// <summary>
    /// 一番初めだけ
    /// </summary>
    private bool isFirst;

    /// <summary>
    /// 選択フレーム
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
            //アイテムのタイプが素材ではないとき処理をしない
            if (item.ItemType != ItemType.material) continue;
            //アイテムのデータがあるとき
            if (playerItems.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
            }
        }
        scaling.Init(1.3f, 1.0f);
        scaling.ScalingObjPosition(selectFrame, slotObj[selectNum].transform.position);
        Debug.Log("作成" + selectMaxNum);
    }

    //アクティブになった時
    private void OnEnable()
    {
        isFirst = true;
        //プレイヤータグを検索
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //もしあればアイテムを取得する
        foreach (var item in obj)
        {
            //Debug.Log("いた");
            playerItems = item.GetComponent<PlayerItem>();
            break;
        }

        selectNum = 0;
        CreateSlot(itemData.itemList);

        //選択されていないときは名前を削除する
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

    //アイテムスロットの作成
    private void CreateSlot(List<PocketItem> itemList)
    {
        var i = 0;
        
        slotObj.Clear();

        selectMaxNum = 0;

        foreach (var item in itemList)
        {
            //お金は右上に表示するので必要なものを取得する
            if(item.Id == "gold")
            {
                var itemCount = playerItems.GetItemCountData(item.Id);
                //アイテムのスプライトを設定
                gold.transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
                //アイテムの個数を表示
                gold.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{itemCount}G";
            }
            //アイテムのタイプが素材ではないとき処理をしない
            if (item.ItemType != ItemType.material) continue;
            //アイテムのデータがあるとき
            if(playerItems.GetItemSourceData(item.Id))
            {
                selectMaxNum++;
                //　スロットのインスタンス化
                var instanceSlot = Instantiate<GameObject>(slot, transform);
                //　スロットゲームオブジェクトの名前を設定
                instanceSlot.name = "ItemSlot" + i++;
                //　Scaleを設定しないと0になるので設定
                instanceSlot.transform.localScale = new Vector3(1f, 1f, 1f);
                //アイテム情報の初期化
                instanceSlot.GetComponent<SlotInfo>().Init();
                //　アイテム情報をスロットのSlotInfoに設定する
                instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItems.GetItemCountData(item.Id));
                instanceSlot.GetComponent<SlotInfo>().GoldText(false);

                //選択できるように持っておく
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
        //一つもアイテムがないとき
        if (selectMaxNum == 0)
        {
            return;
        }

        bool isPress = false;
        if (Navigate.WasPressedThisFrame())
        {
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
            //サウンドを鳴らす
            audioSource.PlayOneShot(moveSound);

            //選択されていないときは名前を削除する
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
