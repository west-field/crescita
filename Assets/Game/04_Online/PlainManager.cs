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

    // �Q�[���̊J�n�������ݒ肳��Ă���Ύ擾����
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

    // �Q�[���̊J�n������ݒ肷��
    public static void SetStartTime(this Room room, int timestamp)
    {
        //Room room = PhotonNetwork.CurrentRoom;
        propsToSet[KeyStartTime] = timestamp;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}

/// <summary>
/// ����}�l�[�W���[
/// (�I�����C�����̃X�e�[�W)
/// </summary>
public class PlainManager : MonoBehaviourPunCallbacks
{
    private int type = onlineJoin.type;

    [SerializeField] private TextMeshProUGUI timeLabel;//�������Ԃ�\������e�L�X�g

    private const float maxTime = 100.0f;//��������
    private float elapsedTime;//���݂̎���

    private string playerCharName;//�g���L�����N�^�[��
    private GameObject myObject;

    private bool isPlaying;//�Q�[���𑱂����邩�ǂ���
    [SerializeField] private GameObject timeUpTextObj;//���Ԑ؂�̎��\������I�u�W�F�N�g

    [SerializeField] private MainManager mainManager;//���C���}�l�[�W���[
    private bool isChangeScene;//�V�[����ύX���Ă��邩

    private bool isBossDeath;//�{�X��|�������ǂ���

    private void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        timeUpTextObj.SetActive(false);

        //���g���}�X�^�[�N���C�A���g�̎�
        if (PhotonNetwork.IsMasterClient)
        {
            //��������
            PhotonNetwork.InstantiateRoomObject("plainMap", Vector3.zero, Quaternion.identity);
        }
    }

    private void Start()
    {
        isPlaying = true;
        isChangeScene = false;

        var pos = new Vector3[4];//�X�|�[������ʒu���w��
        pos[0] = new Vector3(0.0f, 1.0f, 0.0f);
        pos[1] = new Vector3(-6.5f, 1.0f, 34.0f);
        pos[2] = new Vector3(20.0f, 1.0f, 10.0f);
        pos[3] = new Vector3(-24.0f, 1.0f, 20.0f);

        //var num = photonView.OwnerActorNr - 1;//�����̃i���o�[
        var i = type;//�L�����N�^�[�^�C�v

        var index = Random.Range(1, 100) % 4;//�ʒu�����߂�
        var position = pos[index];//�ʒu�w��

        Debug.Log(index +":" + position);

        playerCharName = "Knight";

        //��������
        myObject = PhotonNetwork.Instantiate(playerCharName, position, Quaternion.identity);

        //���[�����쐬�����v���C���[�́A���݂̃T�[�o�[�������Q�[���̊J�n�����ɐݒ肷��
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
    }

    private void FixedUpdate()
    {
        //�V�[����ύX������X�V���Ȃ��悤��
        if(isChangeScene) { return; }
        // �܂����[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.InRoom) { return; }
        // �܂��Q�[���̊J�n�������ݒ肳��Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        //���Ԑ؂�ɂȂ��Ă��Ȃ��B�{�X��|���ĂȂ��B
        if (isPlaying && !isBossDeath)
        {
            GameObject[] enemyes = GameObject.FindGameObjectsWithTag("enemy");
            foreach (var enemy in enemyes)
            {
                //�{�X�����邩�ǂ���
                if(enemy.GetComponent<BossGrassland>() != null)
                {
                    isBossDeath = enemy.GetComponent<BossGrassland>().IsDeath();
                    if(isBossDeath)
                    {
                        elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
                        timeUpTextObj.SetActive(true);
                        timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "�N���A";
                        return;
                    }
                }
            }

            if(HoldVariable.isDeathBoss)
            {
                isBossDeath = true;
                elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
                timeUpTextObj.SetActive(true);
                timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "�N���A";
                return;
            }
        }
        //���Ԑ؂�ɂȂ����@or�@�{�X��|����
        else if (!isPlaying || isBossDeath)
        {
            float time = (elapsedTime + 5.0f) - Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
            timeLabel.text = "����" + time.ToString("f1") + "�b�Ŗ߂�܂�";
            Debug.Log("����"+time);
            if (time <= 0)
            {
                // ���[������ޏo����
                PhotonNetwork.LeaveRoom();
                // ���r�[����ޏo����
                PhotonNetwork.LeaveLobby();
                //Photon����ؒf
                PhotonNetwork.Disconnect();

                isChangeScene = true;
                mainManager.ChangeScene("selectScene");
            }
            return;
        }

        // �Q�[���̌o�ߎ��Ԃ����߂āA�������ʂ܂ŕ\������
        elapsedTime = maxTime - Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        timeLabel.text = elapsedTime.ToString("f1");

        //�������Ԃ�0�ɂȂ�����
        if(elapsedTime <= 0 && isPlaying)
        {
            timeLabel.text = "0.0";

            myObject.GetComponent<Player>().enabled = false;
            isPlaying = false;

            timeUpTextObj.SetActive(true);
            timeUpTextObj.GetComponent<TextMeshProUGUI>().text = "���Ԑ؂�";

            elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

            return;
        }
    }
}