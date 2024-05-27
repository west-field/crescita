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
    private�@InputAction direction;//�A�N�V�����}�b�v����A�N�V�����̎擾

    //�v���C���[�̖��O��\������
    [SerializeField] private TextMeshProUGUI playerName;

    private RoomOptions roomOptions;

    public static int type;

    /// <summary>
    /// �V�[����ύX���邩�ǂ���
    /// </summary>
    private bool isChange;
    /// <summary>
    /// ���Ԃ��o�߂������ǂ���
    /// </summary>
    private bool isTime;

    /// <summary>
    /// �V�[���ύX�܂ł̎���
    /// </summary>
    private float maxTime;
    /// <summary>
    /// �o�ߎ���
    /// </summary>
    private float elapsedTime;

    /// <summary>
    /// ���ԕ\��
    /// </summary>
    [SerializeField] private GameObject timeLabel;
    private TextMeshProUGUI timeLabelText;

    /// <summary>
    /// ���[�h�摜
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

        //�V�[���J�ڂ�e�ɓ���������
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log(HoldVariable.playerName);
        //�v���C���[��
        PhotonNetwork.NickName = HoldVariable.playerName;

        roomOptions = new RoomOptions()
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true,
        };

        //PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        loadingPanel.SetActive(true);
    }

    //�}�X�^�[�T�[�o�[�ւ̐ڑ������������Ƃ��ɌĂ΂��
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("�}�X�^�[�T�[�o�[�ւ̐ڑ�");
        //�����_���ȃ��[���ɎQ������
        PhotonNetwork.JoinRandomRoom();
    }

    //�Q�[���T�[�o�[�ւ̐ڑ������������Ƃ��ɌĂ΂��
    public override void OnJoinedRoom()
    {
        PhotonNetwork.UseRpcMonoBehaviourCache = true; 

        //islink = true;
        base.OnJoinedRoom();
        Debug.Log("�Q�[���T�[�o�[�ւ̐ڑ�");

        //list.Add(0);//�E��
        type = 0;

        UpdateMemberList();//���O�̕\��

        //�v���C���[�����
        PhotonNetwork.Instantiate("Knight", new Vector3(0.0f, 0.0f, 5.0f), Quaternion.identity);

        loadingPanel.SetActive(false);
        if (GameObject.Find("Main Camera") != null)
        {
            GameObject.Find("Main Camera").gameObject.SetActive(false);
        }

        //���[���������ɂȂ�����A�ȍ~���̃��[���ւ̎Q����s���ɂ���
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            IsOpenRoom(false);
        }

        //���[�����쐬�����v���C���[�́A���݂̃T�[�o�[�������Q�[���̊J�n�����ɐݒ肷��
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
    }

    //�����_���ȕ����̓����Ɏ��s�����Ƃ��ɌĂ΂��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //���[���I�v�V�����ɐݒ肵���������쐬����
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    //���[���ւ̎Q���s�Q����ύX����
    void IsOpenRoom(bool isOpen)
    {
        PhotonNetwork.CurrentRoom.IsOpen = isOpen;
    }

    private void Update()
    {
        //�}�X�^�[�N���C�A���g�ł͂Ȃ��Ƃ�
        if (!PhotonNetwork.IsMasterClient)
        {
            return;//���������Ȃ�
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
        if (isChange) return;//�ύX�����Ƃ��͏������ł��Ȃ��悤�ɂ���
        // �܂����[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ�
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
            //�o�ߎ��Ԃ����߂āA�����_���ʂ܂ŕ\������
            timeLabelText.text = elapsedTime.ToString("f1");
            if (elapsedTime <= 0)
            {
                elapsedTime = 0;

                fadeOut = mainManager.OnlineFadeOut();
            }
        }

        //�}�X�^�[�N���C�A���g�ł͂Ȃ��Ƃ�
        if (!PhotonNetwork.IsMasterClient)
        {
            return;//���������Ȃ�
        }

        if(isTime)
        {
            if (fadeOut)
            {
                //�V�[����ύX���Ă��Ȃ��Ƃ� && ���Ԃ��o�߂����Ƃ�
                if (!isChange)
                {
                    IsOpenRoom(false);//���[���Q���ł��Ȃ��悤�ɂ���B

                    PhotonNetwork.IsMessageQueueRunning = false;
                    PhotonNetwork.AutomaticallySyncScene = true;

                    //�V�[���ύX
                    PhotonNetwork.LoadLevel("plain");
                    isChange = true;
                    return;
                }
            }
        }
    }


    //���v���C���[�����[���֎Q�������Ƃ��ɌĂ΂��
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("�Q��");
        UpdateMemberList();
    }

    //���v���C���[�����[���֑ޏo�����Ƃ��ɌĂ΂��
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log("�ޏo");
        UpdateMemberList();
    }

    /// <summary>
    /// �\�����閼�O���X�g��ύX����
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
