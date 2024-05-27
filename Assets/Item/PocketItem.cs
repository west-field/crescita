using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//アイテムの種類
public enum ItemType
{
    material,//素材
    tool,//道具
    wepon,//武器
}

[Serializable]
public class PocketItem
{
    //アイテム識別用id
    [SerializeField] private string id;
    //idを取得
    public string Id
    {
        get { return id; }
    }

    //アイテムの名前
    [SerializeField] private string itemName;
    //アイテム名を取得
    public string ItemName
    {
        get { return itemName; }
    }

    //アイテムの種類
    [SerializeField] private ItemType itemType;
    //アイテムの種類を取得
    public ItemType ItemType
    {
        get { return itemType; }
    }

    //アイテムの見た目
    [SerializeField] private Sprite sprite;
    //アイテムの見た目を取得
    public Sprite Sprite
    {
        get { return sprite; }
    }

    //買値
    [SerializeField] private int buyingPrice;
    //買値を取得
    public int BuyingPrice
    {
        get { return buyingPrice; }
    }

    //売値
    [SerializeField] private int sellingPrice;
    //売値を取得
    public int SellingPrice
    {
        get { return sellingPrice; }
    }

    //説明
    [SerializeField] private string information;
    //説明文を取得
    public string Information
    {
        get { return information; }
    }
}
