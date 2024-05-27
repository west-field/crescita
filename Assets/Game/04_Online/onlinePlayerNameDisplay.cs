using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

/// <summary>
/// �I�����C�����Ƀv���C���[�̖��O��\������
/// </summary>
public class onlinePlayerNameDisplay : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI nameLabel;

    [SerializeField]�@private GameObject player;
    private GameObject parent;
    private�@Vector3 add;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        nameLabel = this.gameObject.GetComponent<TextMeshProUGUI>();
        // �܂����[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.InRoom)
        {
            nameLabel.text = ""; 
            parent.SetActive(false);
            return;
        }
        add = new Vector3(0.0f, 2.0f, 0.0f);
        //�v���C���[���ƃv���C���[ID��\������ ({photonView.OwnerActorNr})�@
        nameLabel.text = $"{photonView.Owner.NickName}";
    }

    private void FixedUpdate()
    {
        // �܂����[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.InRoom)
        {
            return;
        }
        parent.transform.position = player.transform.position;
        parent.transform.position += add;

        parent.transform.rotation = Quaternion.identity;
    }
}
