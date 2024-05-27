using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���V���b�v�}�l�[�W���[
/// </summary>
public class ItemShopManager : MonoBehaviour
{
    private void Awake()
    {
        HoldVariable.playerPosision = new Vector3(9.5f, 1.0f, 3.0f);
        HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);

        HoldVariable.playerRotate = Quaternion.Euler(0, HoldVariable.playerRotate.eulerAngles.y + 180, 0);
    }

}
