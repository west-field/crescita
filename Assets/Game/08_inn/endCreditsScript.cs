using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// エンドカード表示
/// </summary>
public class endCreditsScript : MonoBehaviour
{
    //テキストのスクロールスピード
    private const float textScrollSpeed = 30.0f;
    //テキストの制限位置
    private const float limitPosition = 900.0f;

    //エンドロールが終了したかどうか
    private bool isStopEndRoll;

    /*キー*/
    private MainManager mainManager;
    private InputAction Submit, Cancel;//アクションマップからアクションの取得

    private float startPosY;

    private void Start()
    {
        isStopEndRoll = false;
        startPosY = this.transform.position.y;
        this.transform.parent.gameObject.SetActive(false);

        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];
    }

    private void OnEnable()
    {
        isStopEndRoll = false;
        //初期位置に戻す
        this.transform.position = new Vector3(this.transform.position.x, startPosY, this.transform.position.z);
        Time.timeScale = 0f;
    }
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        //エンドロールが終了したとき
        if(isStopEndRoll)
        {
            //決定かキャンセルを押したとき
            if (Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
            {
                //非表示にする
                this.transform.parent.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            //エンドロール用テキストが制限位置を超えるまで動かす
            if(this.transform.position.y <= limitPosition)
            {
                this.transform.position += new Vector3(0.0f, textScrollSpeed, 0.0f);
                Debug.Log(this.transform.position);
            }
            else
            {
                isStopEndRoll = true;
            }
        }

        //決定かキャンセルを押したとき
        if(Submit.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
        {
            //強制終了
            Debug.Log("強制終了");
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
