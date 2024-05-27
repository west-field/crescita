using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宿屋マネージャー
/// </summary>
public class innManager : MonoBehaviour
{
    void Start()
    {
        HoldVariable.playerPosision = new Vector3(-11.5f, 1.0f, 2.0f);
        HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
        Debug.Log("位置の変更");

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cubeプレハブを元に、インスタンスを生成、
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);

        HoldVariable.playerRotate = Quaternion.Euler(0, HoldVariable.playerRotate.eulerAngles.y + 180, 0);
    }
}
