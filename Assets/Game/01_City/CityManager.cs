using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    void Start()
    {
        GameObject player = (GameObject)Resources.Load("Knight");
        // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);
    }
}
