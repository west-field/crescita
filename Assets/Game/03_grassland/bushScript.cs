using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ���ɂ���X�N���v�g
/// </summary>
public class bushScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject bush;

    private bool isStart;

    private int count;//�q�I�u�W�F�N�g�����邩�ǂ����𐔂���

    private int rand;//�����_���ɃA�C�e����I��

    void Start()
    {
        isStart = false;
    }

    private void FixedUpdate()
    {
        count = transform.childCount;
        //�q�I�u�W�F�N�g��0�̂Ƃ��������폜����
        if(count <= 0)
        {
            switch(rand)
            {
                case 0:
                    // Cube�v���n�u��GameObject�^�Ŏ擾
                    GameObject lotusLeaf = (GameObject)Resources.Load("lotusLeaf");
                    Instantiate(lotusLeaf, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + lotusLeaf.transform.position.y, this.gameObject.transform.position.z), lotusLeaf.transform.localRotation);

                    break;
                case 1:
                    // Cube�v���n�u��GameObject�^�Ŏ擾
                    GameObject scarabSwallowtail = (GameObject)Resources.Load("scarabSwallowtail");
                    Instantiate(scarabSwallowtail, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + scarabSwallowtail.transform.position.y, this.gameObject.transform.position.z), scarabSwallowtail.transform.localRotation);

                    break;
                case 2:
                    // Cube�v���n�u��GameObject�^�Ŏ擾
                    GameObject gold = (GameObject)Resources.Load("gold");
                    Instantiate(gold, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + gold.transform.position.y, this.gameObject.transform.position.z), gold.transform.localRotation);

                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }
    }

    public void RandomItem()
    {
        rand = Random.Range(0, 100) % 4;
    }

    //weapon

    private void OnTriggerEnter(Collider other)
    {
        if (isStart) return;
        //����ɓ���������
        if(other.gameObject.tag == "weapon")
        {
            RandomItem();
            particle.Play();
            isStart = true;
            if (PhotonNetwork.InRoom) 
            {
                //�t�H�g���r���[�����邩�ǂ���
                if (this.gameObject.GetComponent<PhotonView>())
                {
                    Debug.Log("�u�b�V���폜");
                    this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.transform.root.GetComponent<PhotonView>().Owner);
                    Destroy(bush);
                }
            }
            else
            {
                Destroy(bush);
            }
        }
    }
}
