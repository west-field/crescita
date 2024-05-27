using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ステータスロード、セーブ
/// </summary>
public class StartasLoadOrSave : MonoBehaviour
{
    private bool isSave;

    //読み込み　(データ全てを取得する)
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

        //通信エラー処理
        //if(postRequest.isNetworkError || postRequest.isHttpError)
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //通信失敗　エラー内容を表示
            Debug.Log(postRequest.error);
        }
        else
        {
            //通信成功　レスポンスを表示
            Debug.Log(postRequest.downloadHandler.text);

            //テキストを取得
            var str = postRequest.downloadHandler.text;
            //\nで切り配列にする
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

        //通信エラー処理
        //if(postRequest.isNetworkError || postRequest.isHttpError)
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //通信失敗　エラー内容を表示
            Debug.Log(postRequest.error);
            isSave = false;
        }
        else
        {
            //通信成功　レスポンスを表示
            Debug.Log(postRequest.downloadHandler.text);
            isSave = true;
        }
    }
}
