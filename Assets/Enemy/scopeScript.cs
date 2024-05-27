using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �͈�
/// </summary>
public class scopeScript : MonoBehaviour
{
    private bool isScope;//�͈͓��ɉ������邩�ǂ���
    private GameObject target;//�^�[�Q�b�g

    private int scorpeTime;//���Ԃ��o�߂����炢������X�R�[�v��false�ɂ���

    private SphereCollider scopeCollider;

    private void Start()
    {
        isScope = false;
        scorpeTime = 0;
        scopeCollider = this.GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        //isScope �� true �̎�
        if (isScope)
        {
            scopeCollider.enabled = false;
            //2�b��������
            if (scorpeTime-- <= 0)
            {
                //�������񏉊���
                scorpeTime = 0;
                isScope = false;
                target = null;
                scopeCollider.enabled = true;
            }
        }
    }

    /// <summary>
    /// �͈͓��Ƀ^�[�Q�b�g�����邩�ǂ���
    /// </summary>
    /// <returns>true ����: false ���Ȃ�</returns>
    public bool IsScope()
    {
        return isScope;
    }

    /// <summary>
    /// �^�[�Q�b�g�̃|�W�V�������擾����
    /// </summary>
    /// <returns>Vector3</returns>
    public Vector3 TargetPosition()
    {
        if (target == null)
            target = gameObject;
        return target.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        //�v���C���[�Ɠ���������
        if (other.gameObject.tag == "Player")
        {
            if (target == null)
            { 
                target = other.gameObject;
            }
            scorpeTime = 120;
            isScope = true;
        }
    }
}
