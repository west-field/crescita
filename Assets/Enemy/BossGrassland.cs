using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// �����X�e�[�W�̃{�X
/// </summary>
public class BossGrassland : MonoBehaviourPunCallbacks, IPunObservable
{
    /*�X�e�[�^�X*/
    [SerializeField] private EnemyStartas enemyStartas;

    /*�p�[�e�B�N��*/
    [SerializeField] private BossParticle particle;

    /*�A�j���[�V����*/
    private Animator animator;//�A�j���[�^�[

    /*����*/
    private bool isDeath, isMove;//���񂾂��ǂ����A�ړ����Ă��邩�ǂ���
    private bool isReturnCenter;//���S�ɖ߂邩

    /*�ړ�*/
    static float moveSpeed = 0.01f;//�ړ��X�s�[�h
    private float speed;//���̃X�s�[�h

    private Vector3 centerPos;//���S�ʒu(�ړ��͈͂����߂�ۂ̒���)
    private Vector3 rot;//��]

    /*�U��*/
    static private float invincibleTime = 60.0f;//���G����
    private float invincibleCount;//���G�o�ߎ���

    private int attackCount = 0;//�U���ł���܂ł̎���

    private float attackScope;//�U���ł���͈�

    private BoxCollider tailBox,jawBox;//�U���ł���

    /*��*/
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip attackSound;//�U����
    [SerializeField] private AudioClip hitSound;//�����������̉�

    /*Hp*/
    [SerializeField] private GameObject hpBarObject;//HP�o�[�̃Q�[���I�u�W�F�N�g
    private bool isHpBarDraw;
    [SerializeField] private Image hpBars;//HP�o�[
    private int currentHp;
    private int maxHp;

    /*���S*/
    private int deathCount;

    /// <summary>
    /// �~�j�}�b�v�ɕ\������I�u�W�F�N�g���폜���邽��
    /// </summary>
    [SerializeField] private GameObject miniMap;

    private void Start()
    {
        animator = this.GetComponent<Animator>();

        hpBarObject.SetActive(false);

        enemyStartas.Init(100, 20, 1.0f, 200, 200);

        isDeath = false;
        isMove = false;
        isReturnCenter = false;
        speed = moveSpeed;

        centerPos = this.transform.position;
        rot = Vector3.zero;

        invincibleCount = 0;

        attackScope = 3.0f;//�A�^�b�N

        jawBox = this.transform.Find("Root_Pelvis/Spine/Chest/Neck/Head/Jaw/JawTip").GetComponent<BoxCollider>();
        jawBox.enabled = false;
        tailBox = this.transform.Find("Root_Pelvis/Tail01/Tail02/Tail03").GetComponent<BoxCollider>();
        tailBox.enabled = false;

        maxHp = enemyStartas.GetMaxHp();
        currentHp = enemyStartas.GetNowHp();

        deathCount = 0;

        HoldVariable.isDeathBoss = false;
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {
            //hp�o�[��\����\����ύX����
            if (isHpBarDraw != hpBarObject.activeSelf)
            {
                hpBarObject.SetActive(isHpBarDraw);
            }
            //���f������
            hpBars.fillAmount = (float)currentHp / (float)maxHp;
        }
        else
        {
            deathCount++;

            if (deathCount >= 120)
            {
                Debug.Log("���Ԍo�߂Ń{�X���폜");

                var root = this.transform.position;
                particle.ParticlePlay(root);

                GameObject item;
                //�A�C�e��
                var rand = Random.Range(0, 100);

                string[] itemName = { "frogHorn", "frog'sPearl", "frog'sGoldenBeads", "frog'sBlackJewel", "lotusLeaf" };

                Vector3[] position = new Vector3[]  {   new Vector3(root.x, root.y + 0.5f, root.z),
                                                        new Vector3(root.x + 0.5f, root.y + 0.5f, root.z),
                                                        new Vector3(root.x - 0.5f, root.y + 0.5f, root.z),
                                                        new Vector3(root.x, root.y + 0.5f, root.z + 0.5f),
                                                        new Vector3(root.x, root.y + 0.5f, root.z - 0.5f)};

                bool isCreate = false;//��x�ł��쐬�������ǂ���

                int divide = 2;
                int index = 0;
                foreach (var name in itemName)
                {
                    rand = Random.Range(0, 100) % divide;
                    if (rand == 0)
                    {
                        rand = Random.Range(1, 3);

                        for (int i = 0; i < rand; i++)
                        {
                            isCreate = true;
                            item = (GameObject)Resources.Load(name);
                            Instantiate(item, new Vector3(position[index].x, position[index].y + i * 0.3f, position[index].z), item.transform.localRotation);
                            Debug.Log(position[index]);
                        }
                        index++;
                    }

                    divide += 2;
                }

                if (!isCreate)
                {
                    item = (GameObject)Resources.Load("frogHorn");
                    Instantiate(item, new Vector3(), item.transform.localRotation);
                }

                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        Debug.Log("����");
                        PhotonNetwork.Destroy(this.gameObject);
                    }
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }

            return;
        }

        //���G�o�ߎ���
        if (invincibleCount > 0.0f)
        {
            invincibleCount--;
        }

        //�����Ă��Ȃ��Ƃ�
        if (!enemyStartas.IsAlive())
        {
            isDeath = true;
            HoldVariable.isDeathBoss = true;
            animator.SetBool("die", isDeath);

            this.gameObject.tag = "Untagged";

            tailBox.enabled = false;
            jawBox.enabled = false;
            if (hpBarObject != null)
            {
                Destroy(hpBarObject);
            }
            if(miniMap != null)
            {
                Destroy(miniMap);
            }
            return;
        }

        //���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
            {
                return;
            }
        }

        //���ʂ��Ⴄ�Ƃ��͉��ʂ����킹��
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        animator.SetBool("move", isMove);

        //�͈͓��Ƀv���C���[������Ƃ�
        if (enemyStartas.GetScopescript().IsScope())
        {
            Debug.Log("�ǂ�������");
            isHpBarDraw = true;
            //�v���C���[��ǂ�������
            ChaseThePlayer();
        }
        else if (isReturnCenter)
        {
            Debug.Log("�߂�");
            isHpBarDraw = false;
            ReturnToCenterPos();
        }
        else
        {
            //����H
            isHpBarDraw = false;
        }
    }

    /// <summary>
    /// �v���C���[��ǂ�������
    /// </summary>
    private void ChaseThePlayer()
    {
        isMove = true;//�ړ����Ă���

        //�v���C���[�̕���������
        rot = enemyStartas.GetScopescript().TargetPosition() - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (attackCount > 0)
        {
           attackCount--;
        }

        //���̋����ȓ��Ƀv���C���[������Ƃ�

        //�ʏ�U���͈͓̔��ɂ���Ƃ�
        float magnitude = rot.magnitude;
        if(magnitude > -attackScope && magnitude < attackScope)
        {
            isMove = false;
            //�U���J�E���g��0�̎��@�U�����ł���
            if (attackCount <= 0)
            {
                jawBox.enabled = true;
                animator.SetBool("attack", true);
                attackCount = 120;
            }
            else if(attackCount == 60)
            {
                jawBox.enabled = false;
            }
        }
        //���Ȃ��Ƃ��͈ړ�����
        else
        {
            this.transform.position += rot.normalized * speed;
        }

        if (!isReturnCenter)
        {
            isReturnCenter = true;
        }
    }

    /// <summary>
    /// ���S�ɖ߂�
    /// </summary>
    private void ReturnToCenterPos()
    {
        isMove = true;//�ړ����Ă���

        //���̈ʒu�̕���������
        rot = centerPos - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (rot.magnitude < 1.0f)
        {
            isMove = false;
            isReturnCenter = false;
        }

        this.transform.position += rot.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)//�ڐG�����Ƃ�
    {
        //����ł��鎞�������Ȃ�
        if (isDeath) return;

        if (PhotonNetwork.InRoom)
        {
            //�}�X�^�[�N���C�A���g�ł͂Ȃ��Ƃ��U���������������������Ȃ�
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        //���G���Ԃ�0���傫���Ƃ��������Ȃ�
        if (invincibleCount > 0.0f) return;
        
        //�U��������������
        if (other.gameObject.tag == "weapon")
        {
            var playerObj = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;

            //�����Ă��鎞
            if (enemyStartas.IsAlive())
            {
                rot = this.transform.position - other.gameObject.transform.position;
                //���킪���������ʒu���玩���������āA�G�t�F�N�g�𔭐�������
                particle.HitParticle(other.gameObject.transform.position, rot);
            }
            else
            {
                //�v���C���[�̃��x�����グ��
                playerObj.GetComponent<PlayerStartas>().LevelUp(enemyStartas.LevelPoint());
                playerObj.GetComponent<PlayerItem>().CountItem("gold", enemyStartas.Gold());
            }

            //HP�����炷
            enemyStartas.Hit(other.gameObject.transform.root.GetComponent<PlayerStartas>().GetAttackPower());
            currentHp = enemyStartas.GetNowHp();

            //����炷
            audioSource.PlayOneShot(hitSound);
            //�_���[�W�A�j���[�V����
            animator.SetBool("damage", true);
            invincibleCount = invincibleTime;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHp);
            stream.SendNext(jawBox.enabled);
            stream.SendNext(isHpBarDraw);
            stream.SendNext(isDeath);
        }
        else
        {
            currentHp = (int)stream.ReceiveNext();
            jawBox.enabled = (bool)stream.ReceiveNext();
            isHpBarDraw = (bool)stream.ReceiveNext();
            isDeath = (bool)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// �|���ꂽ���ǂ���
    /// </summary>
    /// <returns>true �|���ꂽ: false �܂������Ă���</returns>
    public bool IsDeath()
    {
        return isDeath;
    }
}
