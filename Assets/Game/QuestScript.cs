using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestScript : MonoBehaviour
{
    /// <summary>
    /// メインマネージャー
    /// </summary>
    [SerializeField] private MainManager manager;
    /// <summary>
    /// アクションマップからアクションの取得
    /// </summary>
    private InputAction decision;

    /// <summary>
    /// 選択したときに表示する画像
    /// </summary>
    [SerializeField] private GameObject questPanel;
    /// <summary>
    /// 選択できるか、選択したか
    /// </summary>
    private bool isSelect, isChoice;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private string message;

    /// <summary>
    /// また選択できるようになるまでの時間
    /// </summary>
    private float time;

    private void Start()
    {
        decision = manager.GetPlayerInput().actions["fire"];

        isSelect = false;
        isChoice = false;
    }

    private void FixedUpdate()
    {
        //待機時間が0より大きいとき
        if(time > 0)
        {
            time -= Time.deltaTime;
            return;
        }

        //選択していたら処理をしない
        if (isChoice)
        {
            //選択できない位置にいるとき
            if (!isSelect)
            {
                //表示しないようにする
                isChoice = false;
                questPanel.SetActive(false);
            }

            //非表示になっている時
            if (!questPanel.activeSelf)
            {
                //選択しているフラグをfalseにする
                isChoice = false;
            }
        }
        //選択できるとき
        else if (isSelect)
        {
            isSelect = false;

            bool isStarted;
            isStarted = decision.IsPressed();

            //決定ボタンを押していないときは処理をしない
            if (!isStarted) return;
            //選択フラグをtrueに
            isChoice = true;
            time = 1.0f;
            //表示する
            questPanel.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //プレイヤーと当たっている時
        if(other.gameObject.tag == "Player")
        {
            isSelect = true;
        }
    }
}
