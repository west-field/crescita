using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public static class GameRoomTimePropatly
{
    private const string KeyStartTime = "StartTime";

    private static readonly Hashtable propsToSet = new Hashtable();

    // ゲームの開始時刻が設定されていれば取得する
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {
        //Room room = PhotonNetwork.CurrentRoom;
        if (room.CustomProperties[KeyStartTime] is int value)
        {
            timestamp = value;
            return true;
        }
        else
        {
            timestamp = 0;
            return false;
        }
    }

    // ゲームの開始時刻を設定する
    public static void SetStartTime(this Room room, int timestamp)
    {
        //Room room = PhotonNetwork.CurrentRoom;
        propsToSet[KeyStartTime] = timestamp;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}

/// <summary>
/// 平野マネージャー
/// (オンライン時のステージ)
/// </summary>
public class PlainManager : MonoBehaviourPunCallbacks
{
    private int type = onlineJoin.type;

    [SerializeField] private TextMeshProUGUI timeLabel;//制限時間を表示するテキスト

    private const float maxTime = 100.0f;//制限時間
    private float elapsedTime;//現在の時間

    private string playerCharName;//使うキャラクター名
    private GameObject myObject;

    private bool isPlaying;//ゲームを続けられるかどうか
    [SerializeField] private GameObject timeUpTextObj;//時間切れの時表示するオブジェクト

    [SerializeField] private MainManager mainManager;//メインマネージャー
    private bool isChangeScene;//シーンを変更しているか

    private bool isBossDeath;//ボスを倒したかどうか

    private void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        timeUpTextObj.SetActive(false);

        //自身がマスタークライアントの時
        if (PhotonNetwork.IsMasterClient)
        {
            //生成する
            PhotonNetwork.InstantiateRoomObject("plainMap", Vector3.zero, Quaternion.identity);
        }
    }

    private void Start()
    {
        isPlaying = true;
        isChangeScene = false;

        var pos = new Vector3[4];//スポーンする位置を指定
        pos[0] = new Vector3(0.0f, 1.0f, 0.0f);
        pos[1] = new Vector3(-6.5f, 1.0f, 34.0f);
        pos[2] = new Vector3(20.0f, 1.0f, 10.0f);
        pos[3] = new Vector3(-24.0f, 1.0f, 20.0f);

        //var num = photonView.OwnerActorNr - 1;//自分のナンバー
        var i = type;//キャラクタータイプ

        var index = Random.Range(1, 100) % 4;//位置を決める
        var position = pos[index];//位置指定

        Debug.Log(index +":" + position);

        playerCharName = "Knight";

        //生成する
        myObject = PhotonNetwork.Instantiate(playerCharName, position, Quaternion.identity);

        //ルームを作成したプレイヤーは、現在のサーバー時刻をゲームの開始時刻に設定する
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
    }

    private void FixedUpdate()
    {
        //シーンを変更したら更新しないように
        if(isChangeScene) { return; }
        // まだルームに参加していない場合は更新しない
        if (!PhotonNetwork.InRoom) { return; }
        // まだゲームの開始時刻が設定されていない場合は更新しない
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        //時間切れになっていない。ボスを倒してない。
        if (isPlaying && !isBossDeath)
        {
            GameObject[] enemyes = GameObject.FindGameObjectsWithTag("enemy");
            foreach (var enemy in enemyes)
            {
                //ボスがいるかどうか
                if(enemy.GetComponent<BossGrassland>() != null)
                {
                    isBossDeath = enemy.GetComponent<BossGrassland>().IsDeath();
                    if(isBossDeath)
                    {
                        elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
                        timeUpTextObj.SetActive(true);
                        timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "クリア";
                        return;
                    }
                }
            }

            if(HoldVariable.isDeathBoss)
            {
                isBossDeath = true;
                elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
                timeUpTextObj.SetActive(true);
                timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "クリア";
                return;
            }
        }
        //時間切れになった　or　ボスを倒した
        else if (!isPlaying || isBossDeath)
        {
            float time = (elapsedTime + 5.0f) - Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
            timeLabel.text = "あと" + time.ToString("f1") + "秒で戻ります";
            Debug.Log("あと"+time);
            if (time <= 0)
            {
                // ルームから退出する
                PhotonNetwork.LeaveRoom();
                // ロビーから退出する
                PhotonNetwork.LeaveLobby();
                //Photonから切断
                PhotonNetwork.Disconnect();

                isChangeScene = true;
                mainManager.ChangeScene("selectScene");
            }
            return;
        }

        // ゲームの経過時間を求めて、小数第一位まで表示する
        elapsedTime = maxTime - Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        timeLabel.text = elapsedTime.ToString("f1");

        //制限時間が0になった時
        if(elapsedTime <= 0 && isPlaying)
        {
            timeLabel.text = "0.0";

            myObject.GetComponent<Player>().enabled = false;
            isPlaying = false;

            timeUpTextObj.SetActive(true);
            timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "時間切れ";

            elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

            return;
        }
    }
}