using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����X�e�[�W�}�l�[�W���[
/// </summary>
public class grasslandManager : MonoBehaviour
{
    void Start()
    {
        HoldVariable.playerPosision = Vector3.zero;
        HoldVariable.playerRotate = Quaternion.identity;

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);
    }
}
