using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �t�F�[�h�p�l�����Ō���ɒu��
/// </summary>
public class fadePanel : MonoBehaviour
{
    private void OnEnable()
    {
        // �^�[�Q�b�g���Ō���ɔz�u
        this.transform.SetAsLastSibling();
    }
}
