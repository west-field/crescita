using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// �v���C���[�A�C�e��
/// </summary>
public class PlayerItem : MonoBehaviour
{
    /// <summary>
    /// �A�C�e���f�[�^�x�[�X
    /// </summary>
    [SerializeField] private ItemDataBase itemData;

    //�A�C�e�������擾
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

    //�A�C�e�����擾
    public void CountItem(string itemId, int count)
    {
        for (int i = 0; i < HoldVariable.itemDataList.Count; i++)
        {
            //ID����v���Ă�����J�E���g
            if (HoldVariable.itemDataList[i].id == itemId)
            {
                HoldVariable.itemDataList[i].CountUp(count);
                return;
            }
        }

        //ID����v���Ȃ���΃A�C�e����ǉ�
        ItemData item = new ItemData(itemId, count);
        HoldVariable.itemDataList.Add(item);
    }

    //�A�C�e�����g�p
    public void UseItem(string itemId, int count)
    {
        for (int i = 0; i < HoldVariable.itemDataList.Count; i++)
        {
            //ID����v���Ă�����
            if (HoldVariable.itemDataList[i].id == itemId)
            {
                //�A�C�e�����J�E���g�_�E��
                HoldVariable.itemDataList[i].CountDown(count);
                return;
            }
        }
    }

    //�A�C�e���̌���
    public bool GetItemSourceData(string itemId)
    {
        //�A�C�e��������
        foreach (var data in HoldVariable.itemDataList)
        {
            //ID����v���Ă�����
            if (data.id == itemId)
            {
                //��������Ƃ�
                if(data.count != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //�����擾�ł���
    public int GetItemCountData(string itemId)
    {
        //�A�C�e��������
        foreach (var data in HoldVariable.itemDataList)
        {
            //ID����v���Ă�����
            if (data.id == itemId)
            {
                return data.count;
            }
        }
        return 0;
    }
}
