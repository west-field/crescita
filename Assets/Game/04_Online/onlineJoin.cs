using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine.InputSystem;

using TMPro;

public class onlineJoin : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private MainManager mainManager;
    private　InputAction direction;//アクションマップからアクションの取得

    //プレイヤーの名前を表示する
    [SerializeField] private TextMeshProUGUI playerName;

    private RoomOptions roomOptions;

    public static int type;

    /// <summary>
    /// シーンを変更するかどうか
    /// </summary>
    private bool isChange;
    /// <summary>
    /// 時間が経過したかどうか
    /// </summary>
    private bool isTime;

    /// <summary>
    /// シーン変更までの時間
    /// </summary>
    private float maxTime;
    /// <summary>
    /// 経過時間
    /// </summary>
    private float elapsedTime;

    /// <summary>
    /// 時間表示
    /// </summary>
    [SerializeField] private GameObject timeLabel;
    private TextMeshProUGUI timeLabelText;

    /// <summary>
    /// ロード画像
    /// </summary>
    [SerializeField] private GameObject loadingPanel;

    private void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        direction = mainManager.GetPlayerInput().actions["fire"];

        isChange = false;
        isTime = false;

        maxTime = 5.0f;

        timeLabelText = timeLabel.GetComponent<TextMeshProUGUI>();
        timeLabel.SetActive(false);

        //シーン遷移を親に同期させる
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log(HoldVariable.playerName);
        //プレイヤー名
        PhotonNetwork.NickName = HoldVariable.playerName;

        roomOptions = new RoomOptions()
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true,
        };

        //PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        loadingPanel.SetActive(true);
    }

    //マスターサーバーへの接続が成功したときに呼ばれる
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("マスターサーバーへの接続");
        //ランダムなルームに参加する
        PhotonNetwork.JoinRandomRoom();
    }

    //ゲームサーバーへの接続が成功したときに呼ばれる
    public override void OnJoinedRoom()
    {
        PhotonNetwork.UseRpcMonoBehaviourCache = true; 

        //islink = true;
        base.OnJoinedRoom();
        Debug.Log("ゲームサーバーへの接続");

        //list.Add(0);//職業
        type = 0;

        UpdateMemberList();//名前の表示

        //プレイヤーを作る
        PhotonNetwork.Instantiate("Knight", new Vector3(0.0f, 0.0f, 5.0f), Quaternion.identity);

        loadingPanel.SetActive(false);
        if (GameObject.Find("Main Camera") != null)
        {
            GameObject.Find("Main Camera").gameObject.SetActive(false);
        }

        //ルームが満員になったら、以降そのルームへの参加を不許可にする
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            IsOpenRoom(false);
        }

        //ルームを作成したプレイヤーは、現在のサーバー時刻をゲームの開始時刻に設定する
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
    }

    //ランダムな部屋の入室に失敗したときに呼ばれる
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //ルームオプションに設定した部屋を作成する
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    //ルームへの参加不参加を変更する
    void IsOpenRoom(bool isOpen)
    {
        PhotonNetwork.CurrentRoom.IsOpen = isOpen;
    }

    private void Update()
    {
        //マスタークライアントではないとき
        if (!PhotonNetwork.IsMasterClient)
        {
            return;//処理をしない
        }
        if (direction.WasPressedThisFrame())
        {
            isTime = !isTime;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isTime);
            }
        }

        if (!stream.IsWriting)
        {
            isTime = (bool)stream.ReceiveNext();
        }
    }

    private void FixedUpdate()
    {
        if (isChange) return;//変更したときは処理をできないようにする
        // まだルームに参加していない場合は更新しない
        if (!PhotonNetwork.InRoom) { return; }

        if (isTime)
        {
            if (!timeLabel.activeSelf)
            {
                elapsedTime = maxTime;
                timeLabel.SetActive(true);
            }
        }
        else
        {
            if (timeLabel.activeSelf)
            {
                timeLabel.SetActive(false);
            }
        }

        var fadeOut = false;
        if (isTime)
        {
            elapsedTime -= Time.deltaTime;
            //経過時間を求めて、小数点第一位まで表示する
            timeLabelText.text = elapsedTime.ToString("f1");
            if (elapsedTime <= 0)
            {
                elapsedTime = 0;

                fadeOut = mainManager.OnlineFadeOut();
            }
        }

        //マスタークライアントではないとき
        if (!PhotonNetwork.IsMasterClient)
        {
            return;//処理をしない
        }

        if(isTime)
        {
            if (fadeOut)
            {
                //シーンを変更していないとき && 時間が経過したとき
                if (!isChange)
                {
                    IsOpenRoom(false);//ルーム参加できないようにする。

                    PhotonNetwork.IsMessageQueueRunning = false;
                    PhotonNetwork.AutomaticallySyncScene = true;

                    //シーン変更
                    PhotonNetwork.LoadLevel("plain");
                    isChange = true;
                    return;
                }
            }
        }
    }


    //他プレイヤーがルームへ参加したときに呼ばれる
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("参加");
        UpdateMemberList();
    }

    //他プレイヤーがルームへ退出したときに呼ばれる
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log("退出");
        UpdateMemberList();
    }

    /// <summary>
    /// 表示する名前リストを変更する
    /// </summary>
    private void UpdateMemberList()
    {
        playerName.text = "";

        foreach(var p in PhotonNetwork.PlayerList)
        {
            playerName.text += p.NickName + "\n";
        }
    }
}
