using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// �擾�����A�C�e���̖��O����ʂɕ\������
/// </summary>
public class GetItemNameDisplay : MonoBehaviour
{
    //�쐬�������v���n�u
    [SerializeField] private GameObject itemNameDisplayPrefab;

    //�쐬�������O���X�g
    private List<GameObject> itemNameDisplayList = new List<GameObject>();

    /// <summary>
    /// �A�C�e�����擾�����Ƃ����O����ʂɕ\��������
    /// </summary>
    /// <param name="itemName">�A�C�e���̖��O</param>
    public void ItemNameDisplay(string itemName)
    {
        //�쐬
        var create = Instantiate(itemNameDisplayPrefab,this.transform);
        //localScale��ݒ肵�Ȃ���0�ɂȂ�̂Őݒ�
        create.transform.localScale = Vector3.one;

        create.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        //�ǉ�����
        itemNameDisplayList.Add(create);
    }
}