using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �h���}�l�[�W���[
/// </summary>
public class innManager : MonoBehaviour
{
    void Start()
    {
        HoldVariable.playerPosision = new Vector3(-11.5f, 1.0f, 2.0f);
        HoldVariable.playerRotate = Quaternion.Euler(0, 0, 0);
        Debug.Log("�ʒu�̕ύX");

        GameObject player = (GameObject)Resources.Load("Knight");
        // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
        Instantiate(player, HoldVariable.playerPosision, HoldVariable.playerRotate);

        HoldVariable.playerRotate = Quaternion.Euler(0, HoldVariable.playerRotate.eulerAngles.y + 180, 0);
    }
}
