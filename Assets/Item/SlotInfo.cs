using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// スロット
/// </summary>
public class SlotInfo : MonoBehaviour
{
    /// <summary>
    /// アイテム情報を表示するテキストUI
    /// </summary>
    private TextMeshProUGUI informationText;

    /// <summary>
    /// アイテムの名前を表示するテキストUIプレハブ
    /// </summary>
    [SerializeField] private GameObject itemSlotNameUI;

    /// <summary>
    /// itemSlotNameUIのインスタンス化したものを入れておく変数
    /// </summary>
    private GameObject itemSlotNameUIInstance;

    /// <summary>
    /// 自身のアイテムデータを入れておく
    /// </summary>
    private PocketItem itemData;
    private int itemCount = 0;

    /// <summary>
    /// ゴールドを表示させる
    /// </summary>
    [SerializeField] private TextMeshProUGUI myGoldText;

    /// <summary>
    /// 個数を表示させる
    /// </summary>
    [SerializeField] private TextMeshProUGUI myCount;

    //スロットが非アクティブになったら削除
    private void OnDisable()
    {
        //インスタンス化したものを削除
        if(itemSlotNameUIInstance != null)
        {
            Destroy(itemSlotNameUIInstance);
        }
        //自分自身を削除
        Destroy(this.gameObject);
    }

    /// <summary>
    /// アイテムのデータをセット
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="count"></param>
    public void SetItemData(PocketItem itemData,int count)
    {
        itemCount = count;
        this.itemData = itemData;
        //アイテムのスプライトを設定
        this.transform.GetChild(0).GetComponent<Image>().sprite = this.itemData.Sprite;
        //個数を表示
        myCount.text = itemCount + "個";
    }

    /// <summary>
    /// スクリプトの下に金額を表示する
    /// </summary>
    /// <param name="isEnable">表示するかどうか</param>
    /// <param name="isBuy">売値を表示するか</param>
    public void GoldText(bool isEnable,bool isBuy = false)
    {
        if(isEnable)
        {
            if (isBuy)
            {
                myGoldText.text = $"{itemData.BuyingPrice}G";
            }
            else
            {
                myGoldText.text = $"{itemData.SellingPrice}G";
            }
        }
        else
        {
            myGoldText.enabled = false;
        }
    }

    //初期化
    public void Init()
    {
        //アイテムスロットの親の親からInformationゲームオブジェクトを探しTextコンポーネントを取得する
        informationText = transform.parent.parent.Find("Information").GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //作成
    public void CreateInfomation()
    {
        if (itemSlotNameUIInstance != null)
        {
            Destroy(itemSlotNameUIInstance);
        }
        //アイテムの名前を表示するUIを位置を調整してインスタンス化
        itemSlotNameUIInstance = Instantiate(itemSlotNameUI, new Vector2(transform.position.x - 50, transform.position.y + 50), Quaternion.identity, transform.parent.parent);

        //　アイテム表示Textを取得
        var itemSlotTitleText = itemSlotNameUIInstance.GetComponentInChildren<TextMeshProUGUI>();
        //　アイテム表示テキストに自身のアイテムの名前を表示
        itemSlotTitleText.text = itemData.ItemName;

        informationText.text = itemCount + "個 持っている\n";
        //　情報表示テキストに自身のアイテムの情報を表示
        informationText.text += itemData.Information;
    }

    /// <summary>
    /// アイテムの個数が変更されたとき取得する
    /// </summary>
    /// <param name="count"></param>
    public void ItemCount(int count)
    {
        itemCount = count;

        //情報変更
        informationText.text = itemCount + "個 持っている\n";
        informationText.text += itemData.Information;
        //個数表示を変更
        myCount.text = itemCount + "個";
    }

    //選択から外れたらアイテム表示UIを削除する
    public void DestoroyInformation()
    {
        if(itemSlotNameUIInstance != null)
        {
            informationText.text = "";
            Destroy(itemSlotNameUIInstance);
        }
    }

    //アイテムのデータを取得する
    public PocketItem ItemData()
    {
        return itemData;
    }
}
