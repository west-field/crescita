using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �A�C�e���ɕt����X�N���v�g
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// �A�C�e��ID
    /// </summary>
    [SerializeField] public string id;

    /// <summary>
    /// ��]
    /// </summary>
    [SerializeField] private Vector3 rot;

    /// <summary>
    /// ������܂ł̎���
    /// </summary>
    private int deleteTime;

    // Start is called before the first frame update
    void Start()
    {
        deleteTime = 60 * 10;
    }

    private void FixedUpdate()
    {
        //��]������
        this.transform.localEulerAngles += rot;

        deleteTime--;
        if(deleteTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
