using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 取得したアイテムの名前を画面に表示する
/// </summary>
public class GetItemNameDisplay : MonoBehaviour
{
    //作成したいプレハブ
    [SerializeField] private GameObject itemNameDisplayPrefab;

    //作成した名前リスト
    private List<GameObject> itemNameDisplayList = new List<GameObject>();

    private void Start()
    {

    }

    /// <summary>
    /// アイテムを取得したとき名前を画面に表示させる
    /// </summary>
    /// <param name="itemName">アイテムの名前</param>
    public void ItemNameDisplay(string itemName)
    {
        //作成
        var create = Instantiate(itemNameDisplayPrefab,this.transform);
        //localScaleを設定しないと0になるので設定
        create.transform.localScale = Vector3.one;

        create.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        //追加する
        itemNameDisplayList.Add(create);
    }
}