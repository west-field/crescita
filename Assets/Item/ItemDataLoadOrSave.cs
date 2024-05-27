using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// �A�C�e���̃��[�h�A�Z�[�u
/// </summary>
public class ItemDataLoadOrSave : MonoBehaviour
{
    private bool isSave;
    private bool isLoad;

    private void Start()
    {
        isLoad = false;
    }

    /// <summary>
    /// �ǂݍ��݁@(�f�[�^�S�Ă��擾����)
    /// </summary>
    public void GetItemData()
    {
        string url = @"http://saya-2003.moo.jp/AccountDB/loadItemData.php";//@"http://localhost/Login/loadItemData.php";
        StartCoroutine(LoadItemRequest(url));
    }
    public bool IsLoad()
    {
        return isLoad;
    }

    /// <summary>
    /// �A�C�e���̃��[�h
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator LoadItemRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", HoldVariable.id);

        Debug.Log($"{HoldVariable.id}�̃A�C�e�����[�h");

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);
        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
            isLoad = false;
        }
        else
        {
            //�ʐM�����@���X�|���X��\��
            Debug.Log(postRequest.downloadHandler.text);//�����Ă����e�L�X�g��\��

            var str = postRequest.downloadHandler.text;
            string[] splitText = str.Split('\n');

            for (int i = 0; i < HoldVariable.itemDataList.Count; i++)
            {
                HoldVariable.itemDataList[i].count = int.Parse(splitText[i]);
                Debug.Log(HoldVariable.itemDataList[i].count);
            }

            isLoad = true;
        }
    }

    public bool SaveItemData()
    {
        string url = @"http://saya-2003.moo.jp/AccountDB/saveItemData.php";// @"http://localhost/Login/saveItemData.php";
        StartCoroutine(SaveItemRequest(url));
        return isSave;
    }

    IEnumerator SaveItemRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", HoldVariable.id);

        foreach (var item in HoldVariable.itemDataList)
        {
            form.AddField("items[]", item.count);
        }

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);
        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        //if(postRequest.isNetworkError || postRequest.isHttpError)
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
            isSave = false;
        }
        else
        {
            //�ʐM�����@���X�|���X��\��
            Debug.Log(postRequest.downloadHandler.text);
            isSave = true;
        }
    }
}
