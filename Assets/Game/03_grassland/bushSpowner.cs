using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 草を出現させる
/// </summary>
public class bushSpowner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;//スポーンさせたいオブジェクト
    private GameObject obj;//実際に作ったオブジェクト

    /*次に出現するまでの時間*/
    private const float nextTime = 60.0f;
    //待ち時間計測
    private float elapsedTime;
    //スポーン数(最大数)
    private const int maxSpown = 1;

    //今いくつ出現させたか
    private int spown;

    void Start()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom) 
        {
            //マスタークライアントじゃないとき操作しない
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }
        SpoenObj();
    }

    private void FixedUpdate()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom) 
        {
            //マスタークライアントじゃないとき操作しない
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        if (obj == null)
        {
            if (spown - 1 >= 0)
            {
                spown--;
            }
        }

        //出現する最大数を超えていたら何もしない
        if (spown >= maxSpown)
        {
            return;
        }

        //経過時間を足す
        elapsedTime += Time.deltaTime;

        //経過時間がたったら
        if (elapsedTime >= nextTime)
        {
            elapsedTime = 0.0f;
            SpoenObj();
        }
    }

    //　スポーンさせる
    private void SpoenObj()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            obj = PhotonNetwork.Instantiate(prefab.name, this.transform.position, Quaternion.identity);
        }
        else
        {
            obj = Instantiate(prefab, this.transform.position, Quaternion.identity);
        }

        spown++;
        elapsedTime = 0.0f;
    }
}
