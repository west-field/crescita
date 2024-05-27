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
    //選択できるImageを取得
    [SerializeField] private GameObject city, grassland, plain;//町、草原、草原

    private static int selectNum = 3;//選択できるステージの数
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

    // Start is called before the first frame update
    void Start()
    {
        select = mainManager.GetPlayerInput().actions["move"];
        direction = mainManager.GetPlayerInput().actions["fire"];

        value = select.ReadValue<Vector2>();

        selectCount = 0;
        ChangeSelect(selectCount);

        isSelect = false;

        scaling.Init(1.3f, 1.0f);
        scaling.ScalingObjPosition(selectFrame, city.transform.position);

        audioSource.volume = HoldVariable.SEVolume;
    }

    // Update is called once per frame
    void Update()
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
            if (value.x > 0.0f || value.y > 0.0f)
            {
                selectCount = (selectCount + 1) % selectNum;
                //移動音
                audioSource.PlayOneShot(moveSound);
            }
            //選択位置を変更する　前に
            else if (value.x < 0.0f || value.y < 0.0f)
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
    void ChangeSelect(int select)
    {
        switch (select)
        {
            case 0://シティを選択
                city.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, city.transform.position);

                grassland.SetActive(true);

                plain.SetActive(true);

                break;
            case 1://草原を選択
                city.SetActive(true);

                grassland.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, grassland.transform.position);

                plain.SetActive(true);
                break;
            case 2://平原を選択
                city.SetActive(true);

                grassland.SetActive(true);

                plain.SetActive(false);
                scaling.ScalingObjPosition(selectFrame, plain.transform.position);
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
            case 0://シティを選択
                mainManager.ChangeScene("city");
                //位置を変更
                HoldVariable.playerPosision = new Vector3(0.0f, 1.0f, 22.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 180, 0);
                break;
            case 1://草原を選択
                mainManager.ChangeScene("grassland");
                //位置を変更
                HoldVariable.playerPosision = new Vector3(0.0f, 0.01f, 0.0f);
                HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
                break;
            case 2://平原を選択
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
