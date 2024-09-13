using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using TMPro;

/// <summary>
/// ログイン
/// </summary>
public class rogin : MonoBehaviour
{
    //ホスト
    const string hostUrl = @"http://saya-2003.moo.jp/AccountDB/";//@"http://localhost/Login/";//

    //受信用URL

    //ログインURL
    const string loginUrl = @"loginAccount.php";

    //作成するためのURL
    const string crreateUrl = @"createAccountInfo.php";

    [SerializeField] private TextMeshProUGUI nextText;

    /// <summary>
    /// ログインできて他のデータを取得できたかどうか
    /// </summary>
    private bool isLogin;

    /// <summary>
    /// ログインできたか
    /// </summary>
    private bool isLoginData;

    /// <summary>
    /// データのロードをするため
    /// </summary>
    private ItemDataLoadOrSave itemData;
    /// <summary>
    /// ステータスをロードするため
    /// </summary>
    private StartasLoadOrSave startas;

    void Start()
    {
        isLogin = false;
        isLoginData = false;

        itemData = this.GetComponent<ItemDataLoadOrSave>();
        startas = this.GetComponent<StartasLoadOrSave>();
    }

    private void Update()
    {
        if (isLoginData)
        {
            if (itemData.IsLoad())
            {
                isLogin = true;
            }
        }
    }

    /// <summary>
    /// ログインする
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="pass">パスワード</param>
    /// <returns></returns>
    public bool Login(string name,string pass)
    {
        StartCoroutine(LoginRequest(hostUrl, loginUrl, name, pass));
        return isLogin;
    }

    /// <summary>
    /// 作成する
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="pass">パスワード</param>
    /// <returns></returns>
    public bool Create(string name, string pass)
    {
        StartCoroutine(CreateRequest(hostUrl,crreateUrl, name, pass));
        return isLogin;
    }

    //Post通信　ログイン
    IEnumerator LoginRequest(string host,string postUrl,string name,string pass)
    {
        //PostはURLのみの通信ではなく、URLに対してFormを渡すことで通信する
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("pass", pass);

        string url = host + postUrl;

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);

        yield return postRequest.SendWebRequest();

        //通信エラー処理
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //通信失敗　エラー内容を表示
            Debug.Log(postRequest.error);
        }
        else
        {
            //通信成功
            Debug.Log(postRequest.downloadHandler.text);
            var str = postRequest.downloadHandler.text;

            //送られてきたテキストを'\n'で区切る
            string[] splitText = str.Split('\n');

            nextText.text = splitText[0];
            if (splitText[0] == "全て一致")
            {
                Debug.Log("一致");
                HoldVariable.id = splitText[1];

                //アイテムデータを取得する
                itemData.GetItemData();
                //ステータスを取得する
                startas.GetStartasData();

                nextText.text = "ログイン成功";
                
                isLoginData = true;
            }
        }
    }

    //Post通信　アカウントを作成する
    IEnumerator CreateRequest(string host, string postUrl, string name, string pass)
    {
        //PostはURLのみの通信ではなく、URLに対してFormを渡すことで通信する
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("pass", pass);

        string url = host + postUrl;

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);

        yield return postRequest.SendWebRequest();

        //通信エラー処理
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //通信失敗　エラー内容を表示
            Debug.Log(postRequest.error);
        }
        else
        {
            //通信成功　レスポンスを表示
            Debug.Log(postRequest.downloadHandler.text);

            var str = postRequest.downloadHandler.text;
            string[] splitText = str.Split('\n');

            
            if (splitText[0] == "作成")
            {
                nextText.text = "作成";

                HoldVariable.id = splitText[1];

                ItemDataLoadOrSave itemData = GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>();
                itemData.GetItemData();
                StartasLoadOrSave startas = GameObject.Find("Manager").GetComponent<StartasLoadOrSave>();
                startas.GetStartasData();

                isLogin = true;
            }

            if (splitText[0] == "同じ名前があります")
            {
                nextText.text = "同じ名前があります。別の名前に変更してください";
                isLogin = false;
            }
        }
    }
}
