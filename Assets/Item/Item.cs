using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// アイテムに付けるスクリプト
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// アイテムID
    /// </summary>
    [SerializeField] public string id;

    /// <summary>
    /// 回転
    /// </summary>
    [SerializeField] private Vector3 rot;

    /// <summary>
    /// 消えるまでの時間
    /// </summary>
    private int deleteTime;

    // Start is called before the first frame update
    void Start()
    {
        deleteTime = 60 * 10;
    }

    private void FixedUpdate()
    {
        //回転させる
        this.transform.localEulerAngles += rot;

        deleteTime--;
        if(deleteTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
