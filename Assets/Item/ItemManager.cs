using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public string id;//�A�C�e��ID
    public int count;//������

    //�R���X�g���N�^
    public ItemData(string id,int count = 1)
    {
        this.id = id;
        this.count = count;
    }
    
    //�������J�E���g�A�b�v
    public void CountUp(int value = 1)
    {
        count += value;
    }

    //�������J�E���g�_�E��
    public void CountDown(int value = 1)
    {
        count -= value;
    }
}

/// <summary>
/// �A�C�e���}�l�[�W���[
/// </summary>
public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemDataBase itemDataBase;

    private List<PocketItem> itemDataBaseList;//�A�C�e�����X�g

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
        //�A�C�e��������
        foreach(var data in itemDataBaseList)
        {
            //ID����v���Ă�����
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
            //ID����v���Ă�����J�E���g
            if(itemDataList[i].id == itemId)
            {
                itemDataList[i].CountUp(count);
                break;
            }
        }

        //ID����v���Ȃ���΃A�C�e����ǉ�
        ItemData itemData = new ItemData(itemId, count);
        itemDataList.Add(itemData);

    }

    public void UseItem(string itemId,int count)
    {
        for (int i = 0; i < itemDataList.Count; i++)
        {
            //ID����v���Ă�����
            if (itemDataList[i].id == itemId)
            {
                //�A�C�e�����J�E���g�_�E��
                itemDataList[i].CountDown(count);
                return;
            }
        }
    }
}
