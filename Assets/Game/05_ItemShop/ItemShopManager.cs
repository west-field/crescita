using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムショップマネージャー
/// </summary>
public class ItemShopManager : MonoBehaviour
{
    private void Awake()
    {
        HoldVariable.playerPosision = new Vector3(9.5f, 1.0f, 3.0f);
        HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cubeプレハブを元に、インスタンスを生成、
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);

        HoldVariable.playerRotate = Quaternion.Euler(0, HoldVariable.playerRotate.eulerAngles.y + 180, 0);
    }

}
