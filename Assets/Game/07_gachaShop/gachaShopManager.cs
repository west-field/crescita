using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �K�`���V���b�v�}�l�[�W���[
/// </summary>
public class gachaShopManager : MonoBehaviour
{
    void Start()
    {
        HoldVariable.playerPosision = new Vector3(2.0f, 1.0f, 8.0f);
        HoldVariable.playerRotate = Quaternion.Euler(0, 90.0f, 0);

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);

        HoldVariable.playerRotate = Quaternion.Euler(0, HoldVariable.playerRotate.eulerAngles.y + 180, 0);
    }
}
