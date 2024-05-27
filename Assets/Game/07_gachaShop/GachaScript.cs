using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using TMPro;

/// <summary>
/// ガチャ
/// </summary>
public class GachaScript : MonoBehaviour
{
    /*ガチャ結果表示*/
    [SerializeField] private GameObject slot;//スロットのプレハブ
    /// <summary>
    /// 作成したスロットを持つ
    /// </summary>
    private List<GameObject> slotList = new List<GameObject>();

    /*回している時*/
    /// <summary>
    /// ガチャを回したかどうかのフラグ
    /// </summary>
    private bool isGacha;
    /// <summary>
    /// ガチャの結果が返ってきたか
    /// true:(結果が帰ってきた) false:(結果を待っている)
    /// </summary>
    private bool isResultReturn;
    /// <summary>
    /// ガチャを引くために必要なゴールドorレベルポイントを持っているか
    /// </summary>
    private bool isHaveGoldOrPoint;
    /// <summary>
    /// ゴールドでまわすガチャを選んでいるか
    /// </summary>
    private bool isGoldGacha;

    /// <summary>
    /// ガチャ回数
    /// </summary>
    private int gachaCount;

    /*ガチャ説明*/
    /// <summary>
    /// ガチャ説明
    /// </summary>
    [SerializeField] private TextMeshProUGUI infomation;

    /// <summary>
    /// 送られてきたアイテム名
    /// </summary>
    private string[] resultItemName;

    /*パネル表示*/
    /// <summary>
    /// 画像を取得する
    /// </summary>
    [SerializeField] private GameObject myGold,myLevelPoint;
    /// <summary>
    /// 自分の持っている枚数を表示する
    /// </summary>
    private TextMeshProUGUI myGoldText,myLevelPointText;

    /*キー*/
    private MainManager mainManager;
    InputAction Navigate,Submit, Cancel;//移動,決定,キャンセル

    /*音*/
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound, submitSound, gachaSound, cancelSound;//移動音、決定音、回している時の音、キャンセル音

    /*アイテム*/
    /// <summary>
    /// アイテムデータベース
    /// </summary>
    [SerializeField] private ItemDataBase itemData;
    /// <summary>
    /// 自身が持っているアイテム
    /// </summary>
    private PlayerItem playerItem;
    /// <summary>
    /// プレイヤーのステータス
    /// </summary>
    private PlayerStartas playerStartas;

    /*結果*/
    /// <summary>
    /// 回した結果を表示するパネル
    /// </summary>
    [SerializeField] private GameObject gachaResultPanelObj;
    /// <summary>
    /// 結果の集計をテキストに
    /// </summary>
    [SerializeField] private TextMeshProUGUI resultText;

    /// <summary>
    /// 選択しているものの位置
    /// </summary>
    [SerializeField] private GameObject goldGacha, levelPointGacha;

    /*1回か10回、回すかを選択*/
    /// <summary>
    /// 回す回数を選択するパネルの表示非表示
    /// </summary>
    [SerializeField] private GameObject confirmObj;
    /// <summary>
    /// 選択しているものの位置
    /// </summary>
    private GameObject oneTurn, tenTurn;
    /// <summary>
    /// 説明テキスト(選択しているものに応じて変更する)
    /// </summary>
    [SerializeField] private TextMeshProUGUI infoConfirm;
    /// <summary>
    /// 一回回すほうを選んでいるか
    /// </summary>
    private bool isOneTurn;

    /// <summary>
    /// 一度だけプレイヤーのアイテムを取得する
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// 選んでいるオブジェクトに位置を合わせてフレームを表示
    /// </summary>
    [SerializeField] private GameObject selectFrame;

    /*拡大縮小*/
    [SerializeField] private Scaling scalingScript;

    /// <summary>
    /// お金down 経験値upを入れている
    /// </summary>
    private string rate;

    [SerializeField] private ItemDataLoadOrSave itemDataSave;
    [SerializeField] private StartasLoadOrSave startasLoadOrSave;

    private enum panelName
    {
        gacha,
        confirm,
        result,
    }
    private panelName drawType;

    private void OnEnable()
    {
        Debug.Log("ガチャがアクティブになった");
        if (isFirst) return;
        audioSource.volume = HoldVariable.SEVolume;
        Time.timeScale = 0f;

        //プレイヤータグを検索
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //もしあればアイテムを取得する
        foreach (var item in obj)
        {
            Debug.Log("いた");
            playerItem = item.GetComponent<PlayerItem>();
            myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
            playerStartas = item.GetComponent<PlayerStartas>();
            myLevelPointText.text = $"{playerStartas.GetLevelPoint()}P";
            break;
        }

        audioSource.PlayOneShot(submitSound);

        gachaCount = 1;

        drawType = panelName.gacha;

        rate = "coin";
    }
    //ゲームオブジェクトが非アクティブになった時
    private void OnDisable()
    {
        Debug.Log("ガチャが非アクティブになった");
        Time.timeScale = 1f;
        gachaResultPanelObj.SetActive(false);
        confirmObj.SetActive(false);
    }

    private void Start()
    {
        //キー
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        //SEボリューム
        audioSource.volume = HoldVariable.SEVolume;

        //テキストを取得
        myGoldText = myGold.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        myLevelPointText = myLevelPoint.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        isGacha = false;
        isResultReturn = false;
        isHaveGoldOrPoint = false;
        isGoldGacha = true;
        infomation.text = $"1回　100G\nお金で回すことができる。";

        isFirst = false;

        isOneTurn = true;
        oneTurn = confirmObj.transform.GetChild(0).gameObject;
        tenTurn = confirmObj.transform.GetChild(1).gameObject;
        infoConfirm.text = "1回100Gで回します";
        confirmObj.SetActive(false);

        //見えないようにする
        gachaResultPanelObj.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch(drawType)
        {
            case panelName.confirm:
                ConfirmPanel();
                break;
            case panelName.result:
                ResultPanel();
                break;
            case panelName.gacha:
            default:
                GachaPanel();
                break;
        }
    }

    /*------------------ガチャパネル------------------*/
    /// <summary>
    /// ガチャのパネルでお金で回すか経験値で回すかを選ぶ
    /// </summary>
    private void GachaPanel()
    {
        //キャンセルを押したとき
        if (Cancel.WasPressedThisFrame())
        {
            //非表示にして　終了する
            this.gameObject.SetActive(false);
            audioSource.PlayOneShot(submitSound);
            return;
        }

        //ゴールドで回すかレベルポイントで回すかを変更
        if (Navigate.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(moveSound);
            isGoldGacha = !isGoldGacha;

            if (isGoldGacha)
            {
                rate = "coin";
                infomation.text = $"1回　100G\nお金で回すことができる。";
            }
            else
            {
                rate = "level";
                infomation.text = $"1回　10P\n経験値で回すことができる。けど気を付けて。使いすぎればLvが下がっちゃう。";
            }

            var scale = new Vector3(0.8f, 0.8f, 0.8f);

            goldGacha.transform.localScale = scale;
            levelPointGacha.transform.localScale = scale;
        }

        //お金で回すほうを選んでいる時
        if(isGoldGacha)
        {
            scalingScript.ScalingObjPosition(selectFrame.transform, goldGacha.transform.position);
        }
        else
        {
            scalingScript.ScalingObjPosition(selectFrame.transform, levelPointGacha.transform.position);
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //まだ回していなとき、決定ボタンを押したとき
        if (Submit.WasPressedThisFrame())
        {
            //持っているか
            ItemHave();

            audioSource.PlayOneShot(submitSound);
            
            drawType = panelName.confirm;
            confirmObj.SetActive(true);
            return;
        }
    }

    /// <summary>
    /// 1回回すか10回回すかを選ぶ
    /// </summary>
    private void ConfirmPanel()
    {
        //まだ回していなとき、キャンセルを押したら前に戻る
        if (!isGacha && Cancel.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            confirmObj.SetActive(false);
            drawType = panelName.gacha;
            isOneTurn = true;
            return;
        }

        if(Navigate.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(moveSound);

            isOneTurn = !isOneTurn;
            var scale = new Vector3(0.8f, 0.8f, 0.8f);

            oneTurn.transform.localScale = scale;
            tenTurn.transform.localScale = scale;

            if(isOneTurn)
            {
                gachaCount = 1;
            }
            else
            {
                gachaCount = 10;
            }
            //持っているか
            ItemHave();
        }

        if(isOneTurn)
        {
            //ChangeSize(oneTurn);
            scalingScript.ScalingObjPosition(selectFrame.transform,oneTurn.transform.position);
        }
        else
        {
            //ChangeSize(tenTurn);
            scalingScript.ScalingObjPosition(selectFrame.transform, tenTurn.transform.position);
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //アイテムを持っていないとき
        if (!isHaveGoldOrPoint)
        {
            //Debug.Log("持ってない");
            if (Submit.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(cancelSound);
            }
            return;
        }

        //まだ回していなとき、決定ボタンを押したとき
        if (!isGacha && Submit.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            isGacha = true;
            Gacha();
            return;
        }

        //ガチャを回し終えたとき
        if (isResultReturn)
        {
            drawType = panelName.result;
            confirmObj.SetActive(false);
            //結果を表示
            CreateItem();
            return;
        }

    }

    /// <summary>
    /// 回した結果を表示する
    /// </summary>
    private void ResultPanel()
    {
        //決定 キャンセル を押したら次へ進む
        if (Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(submitSound);
            //非表示にする
            gachaResultPanelObj.SetActive(false);

            drawType = panelName.gacha;
            return;
        }
    }

    /*------------------ガチャ------------------*/
    /// <summary>
    /// アイテムを持っているかを検索
    /// </summary>
    private void ItemHave()
    {
        Debug.Log("アイテムを持っているか検索");

        isHaveGoldOrPoint = false;
        if (isGoldGacha)
        {
            infoConfirm.text = $"{gachaCount}回　{100 * gachaCount}G で回す";

            //アイテムを持っているか
            if (playerItem.GetItemSourceData("gold"))
            {
                Debug.Log("ゴールドを持っている");
                //アイテムの数を確認する
                if (playerItem.GetItemCountData("gold") >= 100 * gachaCount)
                {
                    Debug.Log("ゴールドを100以上持っている");
                    isHaveGoldOrPoint = true;
                }
            }
        }
        else
        {
            infoConfirm.text = $"{gachaCount}回　{10 * gachaCount}P で回す";
            //レベルポイントを持っているか
            if (playerStartas.GetLevelPoint() >= 10 * gachaCount)
            {
                Debug.Log("レベルを10以上持っていない");
                isHaveGoldOrPoint = true;
            }
        }
    }

    /// <summary>
    /// ガチャ
    /// </summary>
    private void Gacha()
    {
        Debug.Log("ガチャ");
        audioSource.Stop();
        audioSource.PlayOneShot(gachaSound);

        string url = @"http://saya-2003.moo.jp/AccountDB/gachaResult.php";//@"http://saya-2003.moo.jp/AccountDB/Gacha.php";//@"http://localhost/Login/Gacha.php";//
        StartCoroutine(GachaRequest(url));
    }

    //ガチャの結果を返す通信
    IEnumerator GachaRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("count", gachaCount);
        form.AddField("rate", rate);

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
            Debug.Log(postRequest.downloadHandler.text);//送られてきたテキストを表示

            isResultReturn = true;

            var str = postRequest.downloadHandler.text;
            resultItemName = str.Split('\n');

            if(resultItemName[0] == "")
            {
                isResultReturn = false;
                StartCoroutine(GachaRequest(url));
            }
            else
            {
                if (isGoldGacha)
                {
                    playerItem.UseItem("gold", 100 * gachaCount);
                    myGoldText.text = $"{playerItem.GetItemCountData("gold")}G";
                }
                else
                {
                    playerStartas.UseLevelPoint(10 * gachaCount);
                    myLevelPointText.text = $"{playerStartas.GetLevelPoint()}P";
                }
            }
        }
    }

    /// <summary>
    /// 回した結果を表示する
    /// </summary>
    private void CreateItem()
    {
        if(slotList.Count == 10)
        {
            foreach (var slot in slotList)
            {
                Destroy(slot.gameObject);
            }
            slotList.Clear();
        }

        gachaResultPanelObj.SetActive(true);
        Debug.Log("アイテムを作成");
        var i = 0;
        var child = gachaResultPanelObj.transform.GetChild(0).gameObject;

        foreach (var itemName in this.resultItemName)
        {
            foreach (var item in itemData.GetItemLists())
            {
                if (item.Id == itemName)
                {
                    //スロットのインスタンス化
                    var instanceSlot = Instantiate(slot, child.transform);
                    //スロットのゲームオブジェクト名を設定
                    instanceSlot.transform.name = "ItemSlot" + i++;
                    //Scaleを設定しないと0になるので設定
                    instanceSlot.transform.localScale = Vector3.one;
                    //アイテム情報をスロットのSlotInfoに設定する
                    instanceSlot.GetComponent<SlotInfo>().SetItemData(item, playerItem.GetItemCountData(item.Id));
                    instanceSlot.GetComponent<SlotInfo>().GoldText(false);

                    slotList.Add(instanceSlot);

                    playerItem.CountItem(itemName, 1);
                    break;
                }
            }
        }
        resultText.text = "";
        foreach (var item in itemData.GetItemLists())
        {
            int count = 0;
            foreach(var itemName in this.resultItemName)
            {
                if (item.Id == itemName)
                {
                    count++;
                }
            }

            if(count > 0)
            {
                resultText.text += $"{item.ItemName}×{count}\n";
            }

        }

        //また回せるように初期化
        isResultReturn = false;
        isGacha = false;

        //アイテムを保存
        itemDataSave.SaveItemData();
        startasLoadOrSave.SaveStartasData();
    }
}
