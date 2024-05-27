using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// �X�e�[�^�X���[�h�A�Z�[�u
/// </summary>
public class StartasLoadOrSave : MonoBehaviour
{
    private bool isSave;

    //�ǂݍ��݁@(�f�[�^�S�Ă��擾����)
    public void GetStartasData()
    {
        string url = @"http://saya-2003.moo.jp/AccountDB/loadStartas.php";//@"http://localhost/Login/loadStartas.php";
        StartCoroutine(LoadStartasRequest(url));
    }
    IEnumerator LoadStartasRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", HoldVariable.id);

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);
        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        //if(postRequest.isNetworkError || postRequest.isHttpError)
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
        }
        else
        {
            //�ʐM�����@���X�|���X��\��
            Debug.Log(postRequest.downloadHandler.text);

            //�e�L�X�g���擾
            var str = postRequest.downloadHandler.text;
            //\n�Ő؂�z��ɂ���
            string[] splitText = str.Split('\n');

            HoldVariable.startas.maxHp = int.Parse(splitText[0]);
            HoldVariable.startas.attack = int.Parse(splitText[1]);
            HoldVariable.startas.defense = int.Parse(splitText[2]);
            HoldVariable.startas.weponAttack = int.Parse(splitText[3]);
            HoldVariable.startas.armorDefense = int.Parse(splitText[4]);
            HoldVariable.startas.level = int.Parse(splitText[5]);
            HoldVariable.startas.levelPoint = int.Parse(splitText[6]);
        }
    }

    public bool SaveStartasData()
    {
        string url = @"http://saya-2003.moo.jp/AccountDB/saveStartas.php";// @"http://localhost/Login/saveStartas.php";
        StartCoroutine(SaveStartasRequest(url));
        return isSave;
    }

    IEnumerator SaveStartasRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", HoldVariable.id);

        form.AddField("startas[]", HoldVariable.startas.maxHp);
        form.AddField("startas[]", HoldVariable.startas.attack);
        form.AddField("startas[]", HoldVariable.startas.defense);
        form.AddField("startas[]", HoldVariable.startas.weponAttack);
        form.AddField("startas[]", HoldVariable.startas.armorDefense);
        form.AddField("startas[]", HoldVariable.startas.level);
        form.AddField("startas[]", HoldVariable.startas.levelPoint);

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
