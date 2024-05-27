using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// ウィンドウモード変更パネルスクリプト
/// </summary>
public class ChangeWndowModeScript : MonoBehaviour
{
    [SerializeField] private GameObject yesObj, noObj;//はい、いいえのゲームオブジェクト
    private TextMeshProUGUI yes, no;//はい、いいえのテキスト

    private bool isYes;//yes,noどちらを選んでいるか

    /*キー*/
    private MainManager mainManager;
    private InputAction Navigate, Submit;//アクションマップからアクションの取得

    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip moveSound,submitSound;//カーソル移動音

    [SerializeField] private GameObject border;

    private void Awake()
    {
        isYes = false;//初めはnoから
    }
    private void Start()
    {
        //メインマネージャーからキーを取得する
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];

        yes = yesObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        yes.color = Color.black;
        no = noObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        no.color = Color.red;

        isYes = false;

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void Update()
    {
        bool isPress = false;

        if(Navigate.WasPressedThisFrame())
        {
            isYes = !isYes;
            isPress = true;
        }

        if (isPress)
        {
            Debug.Log("変更");
            //サウンドを鳴らす
            audioSource.PlayOneShot(moveSound);
            //選択している項目の色を変える
            if(isYes)
            {
                //赤色でないとき変更する
                if(yes.color != Color.red)
                {
                    yes.color = Color.red;
                }
                //黒色でないとき変更する
                if(no.color != Color.black)
                {
                    no.color = Color.black;
                }
            }
            else
            {
                //赤色でないとき変更する
                if (no.color != Color.red)
                {
                    no.color = Color.red;
                }
                //黒色でないとき変更する
                if (yes.color != Color.black)
                {
                    yes.color = Color.black;
                }
            }
            yesObj.transform.localScale = Vector3.one;
            noObj.transform.localScale = Vector3.one;
        }

        //選択している文字を拡大縮小する
        if(isYes)
        {
            ChangeSize(yesObj);
        }
        else
        {
            ChangeSize(noObj);
        }

        //決定ボタンが押されたら
        if(Submit.WasPressedThisFrame())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(submitSound);
            if (isYes)
            {
                Screen.fullScreen = !Screen.fullScreen;
            }

            this.gameObject.SetActive(false);
        }

    }

    //テキストの表示を拡大縮小する
    void ChangeSize(GameObject obj)
    {
        border.transform.position = obj.transform.position;
    }

}
