using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// カメラ
/// </summary>
public class CameraScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject target;//注視点(プレイヤー)
    private Vector3 offset;//ターゲットからのオフセット

    private const float distance = 7.0f; // 後続の物体との距離
    private const float polarAngle = -40.0f; // y軸との角度
    private const float azimuthalAngle = 90.0f; // x軸との角度

    void Start()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                return;
            }
        }

        offset = new Vector3(0.0f, 0.1f, 0.0f);
    }

    private void Update()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                //Debug.Log("スキップする");
                return;
            }
        }

        var lookPos = target.transform.position + offset;
        Position(lookPos);
        transform.LookAt(lookPos);
    }

    //カメラの位置を変更する
    void Position(Vector3 lookAtPos)
    {
        var da = azimuthalAngle * Mathf.Deg2Rad;
        var dp = polarAngle * Mathf.Deg2Rad;
        transform.position = new Vector3(
            lookAtPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
            lookAtPos.y + distance * Mathf.Cos(dp),
            lookAtPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da));
    }

    public void ChangeTarget(GameObject target)
    {
        this.target = target;
    }
}
