using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �J����
/// </summary>
public class CameraScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject target;//�����_(�v���C���[)
    private Vector3 offset;//�^�[�Q�b�g����̃I�t�Z�b�g

    private float distance; // �㑱�̕��̂Ƃ̋���
    private float polarAngle; // y���Ƃ̊p�x
    private float azimuthalAngle; // x���Ƃ̊p�x

    void Start()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (!photonView.IsMine)
            {
                return;
            }
        }

        offset = new Vector3(0.0f, 0.1f, 0.0f);

        distance = 7.0f;//�^�[�Q�b�g�Ƃ̋���
        polarAngle = -40.0f;//y���̊p�x
        azimuthalAngle = 90.0f;//x���̊p�x
    }

    private void Update()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (!photonView.IsMine)
            {
                //Debug.Log("�X�L�b�v����");
                return;
            }
        }

        var lookPos = target.transform.position + offset;
        Position(lookPos);
        transform.LookAt(lookPos);
    }

    //�J�����̈ʒu��ύX����
    void Position(Vector3 lookAtPos)
    {
        var da = azimuthalAngle * Mathf.Deg2Rad;
        var dp = polarAngle * Mathf.Deg2Rad;
        transform.position = new Vector3(
            lookAtPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
            lookAtPos.y + distance * Mathf.Cos(dp),
            lookAtPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da));
    }

    public void ChangeTarget(GameObject target)
    {
        this.target = target;
    }
}
