using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// �X���C��
/// </summary>
public class SlimeScript : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject slime;//�{��
    [SerializeField] private EnemyStartas enemyScript;//�X�e�[�^�X

    /*�p�[�e�B�N��*/
    [SerializeField] private ParticleSystem deathParticle;//�p�[�e�B�N��
    [SerializeField] private GameObject hitParticleObject;//�����������̃G�t�F�N�g�I�u�W�F�N�g
    private ParticleSystem hitParticle;//�U�����󂯂��Ƃ��̃p�[�e�B�N��

    /*�A�j���[�V����*/
    private Animator animator;//�A�j���[�^�[

    /*����*/
    private bool isDeath,isMove;//���񂾂��ǂ����A�ړ����Ă��邩�ǂ����A�����Ă��邩�ǂ���
    private bool isWallOnTrigger, isReturnCenter;//, isRotating;//�ǂɓ������Ă��邩�ǂ���,���S�ɖ߂邩,��]���Ă��邩

    /*�ړ�*/
    static float moveSpeed = 0.01f;
    private float speed;

    private Vector3 centerPos;//���S�ʒu(�ړ��͈͂����߂�ۂ̒���)
    private Vector3 rot;//��]

    private float chargeTime;//�ړ�������ς��鎞��
    private float timeCount;//�o�ߎ���

    /*�U��*/
    private int attackCount = 0;

    static private float invincibleTime = 60.0f;//���G����
    private float invincibleCount;//���G�o�ߎ���

    /*��*/
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip attackSound;//�U����
    [SerializeField] private AudioClip hitSound;//�����������̉�

    /*HP*/
    [SerializeField] private GameObject hpBarObject;//HP�o�[�̃Q�[���I�u�W�F�N�g
    private bool isHpBarDraw;//hp�o�[��\������
    [SerializeField] private Image hpBars;//HP�o�[
    private int currentHp;
    private int maxHp;

    [SerializeField] private GameObject miniMap;

    void Start()
    {
        enemyScript.Init(10, 5, 1.3f, 50, 20);

        animator = slime.GetComponent<Animator>();
        animator.SetBool("strong", enemyScript.IsStrong());

        hpBarObject.SetActive(false);

        isDeath = false;
        isMove = false;

        isWallOnTrigger = false;

        speed = moveSpeed;

        centerPos = this.transform.position;

        chargeTime = Random.Range(3.0f, 6.0f);
        timeCount = 0;

        isReturnCenter = false;

        invincibleCount = 0;
        hitParticle = hitParticleObject.GetComponent<ParticleSystem>();

        maxHp = enemyScript.GetMaxHp();
        currentHp = enemyScript.GetNowHp();

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void FixedUpdate()
    {
        if (isDeath) 
        {
            var count = this.transform.childCount;
            if(count <= 0)
            {
                if (PhotonNetwork.InRoom)
                {
                    if(photonView.IsMine)
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

        //hp�o�[��\����\����ύX����
        if (isHpBarDraw != hpBarObject.activeSelf)
        {
            hpBarObject.SetActive(isHpBarDraw);
        }
        //hpBarObject������Ƃ�
        if (hpBarObject != null)
        {
            //���f������
            hpBars.fillAmount = (float)currentHp / (float)maxHp;
        }

        //���G�o�ߎ���
        if (invincibleCount > 0.0f)
        {
            invincibleCount--;
        }

        //���񂾂Ƃ�
        if (currentHp <= 0)
        {
            Debug.Log("����");
            isDeath = true;

            animator.SetBool("die", isDeath);
            //�����蔻�������
            BoxCollider box = slime.GetComponentInChildren<BoxCollider>();
            box.enabled = false;

            if(hpBarObject != null)
            {
                Destroy(hitParticleObject);
                Destroy(hpBarObject);
                Destroy(miniMap);

                deathParticle.Play();
                Destroy(slime);
            }

            GameObject item;
            //�A�C�e��
            var rand = Random.Range(1, 100) % 4;
            switch (rand)
            {
                case 0:
                case 1:
                    Debug.Log("�v���v�� �A�C�e������");
                    item = (GameObject)Resources.Load("redStuffy");
                    Instantiate(item, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z), item.transform.localRotation);
                    break;
                case 2:
                    Debug.Log("�R�A �A�C�e������");
                    item = (GameObject)Resources.Load("redJigglyCore");
                    Instantiate(item, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z), item.transform.localRotation);
                    break;
                default:
                    break;
            }
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //���ʂ��Ⴄ�Ƃ��͉��ʂ����킹��
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //�A�j���[�^�[���j������Ă��鎞
        if (animator == null)
        {
            //�������Ȃ�
            return;
        }

        animator.SetBool("move", isMove);
        //�͈͓��Ƀv���C���[������Ƃ�
        if (enemyScript.GetScopescript().IsScope())
        {
            isHpBarDraw = true;
            //�v���C���[��ǂ�������
            ChaseThePlayer();
        }
        else if (isReturnCenter)
        {
            isHpBarDraw = false;
            //���̈ʒu�ɖ߂�
            ReturnToCenterPos();
        }
        else
        {
            isHpBarDraw = false;
            //�㉺�Ɉړ�����
            ForwardAndBackwardMovement();
        }
        //Hp�o�[��\�����Ă��鎞
        if (hpBarObject.activeSelf)
        {
            //�J�����̕���������
            hpBarObject.transform.rotation = Quaternion.identity;
        }
    }

    // <summary>
    /// �v���C���[��ǂ�������
    /// </summary>
    private void ChaseThePlayer()
    {
        isMove = true;//�ړ����Ă���

        //�v���C���[�̕���������
        rot = enemyScript.GetScopescript().TargetPosition() - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (attackCount > 0)
        {
            attackCount--;
        }

        //���̋����ȓ��Ƀv���C���[������Ƃ�
        float magnitude = rot.magnitude;
        if (magnitude > -enemyScript.GetScope() && magnitude < enemyScript.GetScope())
        {
            isMove = false;
            //�U���J�E���g��0�̎��@�U�����ł���
            if (attackCount <= 0)
            {
                audioSource.PlayOneShot(attackSound);
                if(!enemyScript.IsStrong())
                {
                    this.transform.position += rot.normalized * (speed * 10);
                }

                animator.SetBool("attack", true);
                attackCount = 120;
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
    /// ���̈ʒu�ɖ߂�
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
            isReturnCenter = false;
        }

        this.transform.position += rot.normalized * speed;
    }

    /// <summary>
    /// �O��Ɉړ�����
    /// </summary>
    private void ForwardAndBackwardMovement()
    {
        isMove = true;//�ړ����Ă���

        timeCount += Time.deltaTime;//���Ԍo��
        bool isChange = false;

        //�����_���Ō��܂������Ԃ����傫���Ȃ�����
        if (timeCount > chargeTime)
        {
            timeCount = 0;//�J�E���g���O�ɂ���
            isChange = true;//�ύX����
        }
        //�ǂɓ���������
        else if (isWallOnTrigger)
        {
            isWallOnTrigger = false;//�������d�Ȃ�Ȃ��悤�� false�ɂ���
            isChange = true;//�ύX����
        }


        if (isChange)
        {
            float angle = 0;//�����]��
            if (this.transform.localRotation.y == 0)
            {
                angle = 180;
            }
            this.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }

        this.transform.position += this.transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)//�ڐG�����Ƃ�
    {
        if (isDeath) return;

        if (PhotonNetwork.InRoom)
        {
            //�}�X�^�[�N���C�A���g�ł͂Ȃ��Ƃ��U���������������������Ȃ�
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        //���G���Ԃ�0���傫���Ƃ��������Ȃ�
        if (invincibleCount > 0.0f) return;

        //�U��������������
        if (other.gameObject.tag == "weapon")
        {
            var pos = other.gameObject.transform.position;

            rot = pos - this.transform.position;
            rot.y = 0.0f;

            //�v���C���[�̕���������            
            this.transform.rotation = Quaternion.LookRotation(rot);

            audioSource.PlayOneShot(hitSound);

            //HP�����炷
            enemyScript.Hit(other.gameObject.transform.root.GetComponent<PlayerStartas>().GetAttackPower());
            currentHp = enemyScript.GetNowHp();

            //�_���[�W�A�j���[�V����
            animator.SetBool("damage", true);

            //���G����
            invincibleCount = invincibleTime;
            attackCount += 10;

            //�m�b�N�o�b�N
            var rigid = this.gameObject.GetComponent<Rigidbody>();
            rigid.AddForce(-transform.forward * (moveSpeed * 2), ForceMode.VelocityChange);

            //�����Ă��Ȃ��Ƃ�
            if (currentHp <= 0)
            {
                //���[���ɎQ�����Ă���ꍇ�͏���������
                if (PhotonNetwork.InRoom)
                {
                    //�������I�[�i�[�ł͂Ȃ��Ƃ�
                    if (!photonView.IsMine)
                    {
                        this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.transform.root.GetComponent<PhotonView>().Owner);
                        Debug.Log("�I�[�i�[�ύX");
                        return;
                    }
                }

                GameObject playerObj = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;
                //�v���C���[�̃��x�����グ��
                playerObj.GetComponent<PlayerStartas>().LevelUp(enemyScript.LevelPoint());
                playerObj.GetComponent<PlayerItem>().CountItem("gold", enemyScript.Gold());
            }
            else
            {
                //���킪���������ʒu���玩��������
                hitParticle.transform.position = pos;

                rot = this.transform.position - hitParticle.transform.position;
                hitParticle.transform.rotation = Quaternion.LookRotation(rot);

                //�G�t�F�N�g�𔭐�������
                hitParticle.Play();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)//�ڐG�����Ƃ�
    {
        if (isDeath) return;
        if (collision.gameObject.tag == "wall")
        {
            isWallOnTrigger = true;
            Debug.Log("��������");
        }

    }

    //�f�[�^����M
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���̃I�u�W�F�N�g��PhotonView�̏��L�҂ł���ꍇ
        if (stream.IsWriting)
        {
            stream.SendNext(currentHp);//���݂�HP
            stream.SendNext(isHpBarDraw);//HP�o�[�̕\����\��
            stream.SendNext(isDeath);//���񂾂��ǂ���
        }
        else
        {
            currentHp = (int)stream.ReceiveNext();//���݂�HP��ύX����
            isHpBarDraw = (bool)stream.ReceiveNext();//HP�o�[�̕\����\����ύX
            isDeath = (bool)stream.ReceiveNext();//���񂾂��ǂ����̃t���O��ύX
        }
    }
}
