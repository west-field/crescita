using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// ステージ選択シーンマネージャー
/// </summary>
public class SelectStageManager : MonoBehaviour
{
    private enum Stage
    {
        Grassland,
        City,
        Plain,

        Max
    }

    //選択できるImageを取得
    [SerializeField] private GameObject[] stage = new GameObject[(int)Stage.Max];

    private const int selectNum = (int)Stage.Max;//選択できるステージの数
    private int selectCount;

    [SerializeField] private MainManager mainManager;
    private InputAction select,direction;//アクションマップからアクションの取得

    private Vector2 value;//アクションから入力値の取得

    private bool isSelect;//移動キーを押しているか,選択したか

    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip moveSound;//移動音
    [SerializeField] private AudioClip directionSound;//当たった時の音

    [SerializeField] private Transform selectFrame;
    [SerializeField] private Scaling scaling;

    private void Start()
    {
        select = mainManager.GetPlayerInput().actions["move"];
        direction = mainManager.GetPlayerInput().actions["fire"];

        value = select.ReadValue<Vector2>();

        selectCount = (int)Stage.City;
        ChangeSelect(selectCount);

        isSelect = false;

        scaling.Init(1.3f, 1.0f);

        audioSource.volume = HoldVariable.SEVolume;
    }

    // Update is called once per frame
    private void Update()
    {
        scaling.ScalingObj(selectFrame);
        //決定を押していたら処理をしない
        if (isSelect) return;

        //決定を押したら次へ移動する
        if (direction.WasPressedThisFrame())
        {
            //選択音
            audioSource.PlayOneShot(directionSound);
            Select(selectCount);//シーンを変更する
            isSelect = true;//決定を押した
            return;
        }

        if(select.WasPressedThisFrame())
        {
            value = select.ReadValue<Vector2>();//入力値の取得

            //選択位置を変更する　次へ
            if (value.x > 0.0f)
            {
                selectCount = (selectCount + 1) % selectNum;
                //移動音
                audioSource.PlayOneShot(moveSound);
            }
            //選択位置を変更する　前に
            else if (value.x < 0.0f)
            {
                selectCount = (selectCount + (selectNum - 1)) % selectNum;
                //移動音
                audioSource.PlayOneShot(moveSound);
            }
        }

        //選択している位置を分かりやすくする
        ChangeSelect(selectCount);
    }

    //選択している場所を分かりやすくする
    private void ChangeSelect(int select)
    {
        //リセットする
        for (int i = 0; i < selectNum; i++)
        {
            stage[i].SetActive(true);
        }

        switch (select)
        {
            case (int)Stage.City://シティを選択
                stage[(int)Stage.City].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.City].transform.position);
                break;
            case (int)Stage.Grassland://草原を選択
                stage[(int)Stage.Grassland].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.Grassland].transform.position);
                break;
            case (int)Stage.Plain://平原を選択
                stage[(int)Stage.Plain].SetActive(false);
                scaling.ScalingObjPosition(selectFrame, stage[(int)Stage.Plain].transform.position);
                break;
            default:
                break;
        }
    }

    //選択しているものによって移動するシーンを変更する
    private void Select(int select)
    {
        switch (select)
        {
            case (int)Stage.City://シティを選択
                mainManager.ChangeScene("city");
                //位置を変更
                HoldVariable.playerPosision = new Vector3(0.0f, 1.0f, 22.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 180, 0);
                break;
            case (int)Stage.Grassland://草原を選択
                mainManager.ChangeScene("grassland");
                //位置を変更
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            case (int)Stage.Plain://平原を選択
                mainManager.ChangeScene("onlineRoom");
                //位置を変更
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            default:
                break;
        }
    }
}
