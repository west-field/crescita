using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public string id;//アイテムID
    public int count;//所持数

    //コンストラクタ
    public ItemData(string id,int count = 1)
    {
        this.id = id;
        this.count = count;
    }
    
    //所持数カウントアップ
    public void CountUp(int value = 1)
    {
        count += value;
    }

    //所持数カウントダウン
    public void CountDown(int value = 1)
    {
        count -= value;
    }
}

/// <summary>
/// アイテムマネージャー
/// </summary>
public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemDataBase itemDataBase;

    private List<PocketItem> itemDataBaseList;//アイテムリスト

    private List<ItemData> itemDataList = new List<ItemData>();
   
    private void Awake()
    {
        LoadItemData();
    }

    private void LoadItemData()
    {
        itemDataBaseList = itemDataBase.itemList;
    }

    public PocketItem GetItemSourceData(string id)
    {
        //アイテムを検索
        foreach(var data in itemDataBaseList)
        {
            //IDが一致していたら
            if(data.Id == id)
            {
                return data;
            }
        }
        return null;
    }

    public void CountItem(string itemId,int count)
    {
        for(int i = 0;i < itemDataList.Count;i++)
        {
            //IDが一致していたらカウント
            if(itemDataList[i].id == itemId)
            {
                itemDataList[i].CountUp(count);
                break;
            }
        }

        //IDが一致しなければアイテムを追加
        ItemData itemData = new ItemData(itemId, count);
        itemDataList.Add(itemData);

    }

    public void UseItem(string itemId,int count)
    {
        for (int i = 0; i < itemDataList.Count; i++)
        {
            //IDが一致していたら
            if (itemDataList[i].id == itemId)
            {
                //アイテムをカウントダウン
                itemDataList[i].CountDown(count);
                return;
            }
        }
    }
}
