using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// 武器,防具
/// 強化
/// </summary>
public class WeaponShopPanel : MonoBehaviour
{
    /*パネル表示*/
    private enum PanelName
    {
        weapon,//武器
        armor,//防具
        end,//終了
        max
    }

    [SerializeField] private TextMeshProUGUI[] text = new TextMeshProUGUI[(int)PanelName.max];//テキスト
    [SerializeField] private GameObject weaponPanelObj, armorPanelObj;//武器強化パネル、防具強化パネル
    [SerializeField] private GameObject selectFrame;//選択している文字の場所にフレームを移動させる

    /*キー*/
    private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel;//アクションマップからアクションの取得

    /*選択*/
    private int selectNum;//今選択しているもの
    private int selectMaxNum;//選択できる数
    private PanelName drawType;//いま表示しているもの

    private bool isWorkShop;//開いたかどうか

    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip openingClosingSound, moveSound;//ポーズを開くときの音,カーソル移動音

    private bool isFirst = true;

    //拡大縮小
    [SerializeField] private Scaling scalingScript;

    //ゲームオブジェクトがアクティブになった時
    private void OnEnable()
    {
        Debug.Log("武器防具強化がアクティブになった");
        Time.timeScale = 0f;
        audioSource.volume = HoldVariable.SEVolume;

        if (isFirst) return;
        audioSource.PlayOneShot(openingClosingSound);
    }

    //ゲームオブジェクトが非アクティブになった時
    private void OnDisable()
    {
        Debug.Log("武器防具強化が非アクティブになった");
        weaponPanelObj.SetActive(false);
        armorPanelObj.SetActive(false);
        Time.timeScale = 1f;
    }

    void Start()
    {
        Debug.Log("武器防具強化パネル初期化");
        selectNum = (int)PanelName.end;
        selectMaxNum = (int)PanelName.max;
        text[selectNum].color = Color.red;
        selectFrame.transform.position = text[selectNum].transform.position;

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];

        drawType = PanelName.max;

        weaponPanelObj.SetActive(false);
        armorPanelObj.SetActive(false);

        GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
        GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
        isFirst = false;
        this.gameObject.SetActive(false);

        scalingScript.Init(0.9f, 0.7f);

        Debug.Log("武器防具強化パネル初期化完了");
    }

    void Update()
    {
        switch (drawType)
        {
            case PanelName.weapon:
                WeaponPanelDrawUpdate();
                break;
            case PanelName.armor:
                ArmorPanelDrawUpdate();
                break;
            case PanelName.end:
                //切り替える
                GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                this.gameObject.SetActive(false);
                break;
            default:
                WorkShopPanelDrawUpdate();
                break;
        }
    }

    private void WorkShopPanelDrawUpdate()
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
                case (int)PanelName.weapon:
                    weaponPanelObj.SetActive(true);
                    drawType = PanelName.weapon;
                    isWorkShop = false;
                    break;
                case (int)PanelName.armor:
                    armorPanelObj.SetActive(true);
                    drawType = PanelName.armor;
                    isWorkShop = false;
                    break;
                case (int)PanelName.end:
                    weaponPanelObj.SetActive(false);
                    armorPanelObj.SetActive(false);

                    GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                    GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                    this.gameObject.SetActive(false);
                    break;
            }
        }
        //キャンセルを押した時
        else if (Cancel.WasPressedThisFrame())
        {
            if (!isWorkShop)
            {
                weaponPanelObj.SetActive(false);
                armorPanelObj.SetActive(false);

                isWorkShop = true;
            }
            else
            {
                weaponPanelObj.SetActive(false);
                armorPanelObj.SetActive(false);

                GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>().SaveItemData();
                GameObject.Find("Manager").GetComponent<StartasLoadOrSave>().SaveStartasData();
                this.gameObject.SetActive(false);
            }
        }

    }

    private void WeaponPanelDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.weapon;
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);
            //アイテム画面を見えなくする
            weaponPanelObj.SetActive(false);
            //ポーズ画面に戻す
            drawType = PanelName.max;

            return;
        }
    }

    private void ArmorPanelDrawUpdate()
    {       
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame())
        {
            selectNum = (int)PanelName.armor;
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);
            //アイテム画面を見えなくする
            armorPanelObj.SetActive(false);
            //ポーズ画面に戻す
            drawType = PanelName.max;

            return;
        }

    }

}
