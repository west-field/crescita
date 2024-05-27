using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// アイテムの売り買い
/// </summary>
public class ItemPanel : MonoBehaviour
{
    /*パネル表示*/
    private enum PanelName
    {
        buy,
        sell,
        end,
        max
    }

    [SerializeField] private TextMeshProUGUI[] text = new TextMeshProUGUI[(int)PanelName.max];
    [SerializeField] private GameObject buyPanelObj, sellPanelObj;//買うパネル、売るパネル
    [SerializeField] private ItemDataLoadOrSave itemSave;//アイテムセーブ用
    [SerializeField] private GameObject selectFrame;//選択している文字の場所にフレームを移動させる

    /*キー*/
    private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel;//アクションマップからアクションの取得

    /*選択*/
    private int selectNum;//今選択しているもの
    private int selectMaxNum;//選択できる数
    private PanelName drawType;//いま表示しているもの

    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip openingClosingSound, moveSound;//開くときの音,カーソル移動音

    private bool isFirst = true;

    //拡大縮小
    [SerializeField] private Scaling scalingScript;

    //ゲームオブジェクトがアクティブになった時
    private void OnEnable()
    {
        Debug.Log("ショップパネルがアクティブになった");
        Time.timeScale = 0f;
        audioSource.volume = HoldVariable.SEVolume;

        if (isFirst) return;
        audioSource.PlayOneShot(openingClosingSound);
    }

    //ゲームオブジェクトが非アクティブになった時
    private void OnDisable()
    {
        Debug.Log("ショップパネルが非アクティブになった");
        buyPanelObj.SetActive(false);
        sellPanelObj.SetActive(false);
        Time.timeScale = 1f;

        itemSave.SaveItemData();
    }

    private void Start()
    {
        selectNum = (int)PanelName.end;
        selectMaxNum = (int)PanelName.max;
        text[selectNum].color = Color.red;
        selectFrame.transform.position = text[selectNum].transform.position;

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        drawType = PanelName.max;

        buyPanelObj.SetActive(false);
        sellPanelObj.SetActive(false);

        isFirst = false;

        scalingScript.Init(0.9f, 0.7f);

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (drawType)
        {
            case PanelName.buy:
                BuyDrawUpdate();
                break;
            case PanelName.sell:
                SellDrawUpdate();
                break;
            case PanelName.end:
                //切り替える
                this.gameObject.SetActive(false);
                break;
            default:
                GoldShopDrawUpdate();
                break;
        }
    }

    void GoldShopDrawUpdate()
    {
        var isPress = false;

        if (Navigate.WasPressedThisFrame())
        {
            Navigate.ReadValue<Vector2>();
            //Debug.Log(Navigate.ReadValue<Vector2>());

            if (Navigate.ReadValue<Vector2>().x < 0.0f || Navigate.ReadValue<Vector2>().y > 0.0f)
            {
                //上
                selectNum = (selectNum + (selectMaxNum - 1)) % selectMaxNum;
            }
            else if (Navigate.ReadValue<Vector2>().x > 0.0f || Navigate.ReadValue<Vector2>().y < 0.0f)
            {
                //下
                selectNum = (selectNum + 1) % selectMaxNum;
            }

            isPress = true;
        }

        if (isPress)
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(moveSound);
            //選択している項目の色を変える
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    scalingScript.ScalingObjPosition(selectFrame.transform, text[i].transform.position);
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }

        scalingScript.ScalingObj(selectFrame.transform);

        //決定を押したとき
        if (Submit.WasPressedThisFrame())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);
            switch (selectNum)
            {
                case (int)PanelName.buy:
                    buyPanelObj.SetActive(true);
                    drawType = PanelName.buy;
                    break;
                case (int)PanelName.sell:
                    sellPanelObj.SetActive(true);
                    drawType = PanelName.sell;
                    break;
                case (int)PanelName.end:
                    buyPanelObj.SetActive(false);
                    sellPanelObj.SetActive(false);

                    this.gameObject.SetActive(false);
                    break;
            }
        }
        //キャンセルを押した時
        else if (Cancel.WasPressedThisFrame())
        {
            buyPanelObj.SetActive(false);
            sellPanelObj.SetActive(false);

            this.gameObject.SetActive(false);
        }
    }

    //購入する
    void BuyDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.buy;
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);
            //アイテム画面を見えなくする
            buyPanelObj.SetActive(false);
            //ポーズ画面に戻す
            drawType = PanelName.max;

            return;
        }
    }

    //売却する
    void SellDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.sell;
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);
            //アイテム画面を見えなくする
            sellPanelObj.SetActive(false);
            //ポーズ画面に戻す
            drawType = PanelName.max;
            return;
        }
    }
}
