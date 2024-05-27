using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// プレイヤーアイテム
/// </summary>
public class PlayerItem : MonoBehaviour
{
    /// <summary>
    /// アイテムデータベース
    /// </summary>
    [SerializeField] private ItemDataBase itemData;

    //アイテム名を取得
    public string ItemName(string itemId)
    {
        foreach(var item in itemData.itemList)
        {
            if(item.Id == itemId)
            {
                return item.ItemName;
            }
        }

        return "no";
    }

    //アイテムを取得
    public void CountItem(string itemId, int count)
    {
        for (int i = 0; i < HoldVariable.itemDataList.Count; i++)
        {
            //IDが一致していたらカウント
            if (HoldVariable.itemDataList[i].id == itemId)
            {
                HoldVariable.itemDataList[i].CountUp(count);
                return;
            }
        }

        //IDが一致しなければアイテムを追加
        ItemData item = new ItemData(itemId, count);
        HoldVariable.itemDataList.Add(item);
    }

    //アイテムを使用
    public void UseItem(string itemId, int count)
    {
        for (int i = 0; i < HoldVariable.itemDataList.Count; i++)
        {
            //IDが一致していたら
            if (HoldVariable.itemDataList[i].id == itemId)
            {
                //アイテムをカウントダウン
                HoldVariable.itemDataList[i].CountDown(count);
                return;
            }
        }
    }

    //アイテムの検索
    public bool GetItemSourceData(string itemId)
    {
        //アイテムを検索
        foreach (var data in HoldVariable.itemDataList)
        {
            //IDが一致していたら
            if (data.id == itemId)
            {
                //個数があるとき
                if(data.count != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //個数を取得できる
    public int GetItemCountData(string itemId)
    {
        //アイテムを検索
        foreach (var data in HoldVariable.itemDataList)
        {
            //IDが一致していたら
            if (data.id == itemId)
            {
                return data.count;
            }
        }
        return 0;
    }
}
