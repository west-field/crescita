using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    public List<PocketItem> itemList = new List<PocketItem>();

    //�A�C�e���̃��X�g��Ԃ�
    public List<PocketItem> GetItemLists()
    {
        return itemList;
    }
}
