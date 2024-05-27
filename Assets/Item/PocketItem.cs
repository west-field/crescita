using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//�A�C�e���̎��
public enum ItemType
{
    material,//�f��
    tool,//����
    wepon,//����
}

[Serializable]
public class PocketItem
{
    //�A�C�e�����ʗpid
    [SerializeField] private string id;
    //id���擾
    public string Id
    {
        get { return id; }
    }

    //�A�C�e���̖��O
    [SerializeField] private string itemName;
    //�A�C�e�������擾
    public string ItemName
    {
        get { return itemName; }
    }

    //�A�C�e���̎��
    [SerializeField] private ItemType itemType;
    //�A�C�e���̎�ނ��擾
    public ItemType ItemType
    {
        get { return itemType; }
    }

    //�A�C�e���̌�����
    [SerializeField] private Sprite sprite;
    //�A�C�e���̌����ڂ��擾
    public Sprite Sprite
    {
        get { return sprite; }
    }

    //���l
    [SerializeField] private int buyingPrice;
    //���l���擾
    public int BuyingPrice
    {
        get { return buyingPrice; }
    }

    //���l
    [SerializeField] private int sellingPrice;
    //���l���擾
    public int SellingPrice
    {
        get { return sellingPrice; }
    }

    //����
    [SerializeField] private string information;
    //���������擾
    public string Information
    {
        get { return information; }
    }
}
