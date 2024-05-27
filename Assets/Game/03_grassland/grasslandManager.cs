using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 草原ステージマネージャー
/// </summary>
public class grasslandManager : MonoBehaviour
{
    void Start()
    {
        HoldVariable.playerPosision = Vector3.zero;
        HoldVariable.playerRotate = Quaternion.identity;

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cubeプレハブを元に、インスタンスを生成、
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);
    }
}
