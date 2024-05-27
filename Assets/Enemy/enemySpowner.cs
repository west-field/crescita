using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// エネミースポナー
/// </summary>
public class enemySpowner : MonoBehaviour
{
    [SerializeField]　private GameObject prefab;//スポーンさせたいエネミー
    private GameObject enemy;

    /*次に敵が出現するまでの時間*/
    [SerializeField]　private float nextTime = 60.0f;
    //待ち時間計測
    private float elapsedTime;
    //敵の最大数
    private const int maxNumOfEnemiys = 1;

    //今何人の数を出現させたか
    private int numberOfEnemys;

    // Start is called before the first frame update
    void Start()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            //マスタークライアントじゃないとき操作しない
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }
        AppearEnemy();
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

        if (enemy == null)
        {
            numberOfEnemys--;
        }

        //出現する最大数を超えていたら何もしない
        if (numberOfEnemys >= maxNumOfEnemiys)
        {
            return;
        }

        //経過時間を足す
        elapsedTime += Time.deltaTime;

        //経過時間がたったら
        if (elapsedTime >= nextTime)
        {
            elapsedTime = 0.0f;
            AppearEnemy();
        }
    }

    /// <summary>
    /// 敵出現メソッド
    /// </summary>
    void AppearEnemy()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            enemy = PhotonNetwork.InstantiateRoomObject(prefab.name, this.transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(prefab, this.transform.position, Quaternion.identity);
        }

        numberOfEnemys++;
        elapsedTime = 0.0f;
    }
}
