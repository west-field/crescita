using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;

/// <summary>
/// �v���C���[
/// </summary>
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    /// <summary>
    /// ���C���J���o�X
    /// (���S���̃e�L�X�g�ύX�̂���)
    /// </summary>
    private GameObject mainCanvas;

    /// <summary>
    /// ���C���}�l�[�W���[
    /// </summary>
    private MainManager manager;

    /*�L�[*/
    /// <summary>
    /// �A�N�V����
    /// </summary>
    private InputAction move,run,fire,jump,block,sit, pause;

    /// <summary>
    /// �ړ��ʁ@�A�N�V����������͒l�̎擾
    /// </summary>
    private Vector2 value;

    /// <summary>
    /// �{�^������������
    /// </summary>
    private bool isFire/*�U��*/, isWalkAnim/*�����Ă��邩*/, isRunAnim/*�����Ă��邩*/, 
        isBlock/*�u���b�N���Ă��邩*/, isSit/*��������*/, isDeth/*���񂾂��ǂ���*/,isPause/*�|�[�Y��ʂɍs�������ǂ���*/;

    /// <summary>
    /// �n�ʂɂ��Ă��邩�ǂ���
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// ���g��Rigidbody
    /// </summary>
    private Rigidbody rigid;

    ///*�_���[�W*/
    private float damageTime;
    private bool isHit;
    /*-----�_�Ŏ���-----*/
    /// <summary>
    /// �q��Renderer�̔z��
    /// </summary>
    private Renderer[] childrenRenderer;
    /// <summary>
    /// childrenRenderers���L����������
    /// </summary>
    private bool isEnabledRenderers;
    /// <summary>
    /// �_���[�W���󂯂Ă��邩
    /// </summary>
    private bool isDamaged;
    /// <summary>
    /// ���Z�b�g����Ƃ��̂��߂ɃR���[�`����ۑ����Ă���
    /// </summary>
    private Coroutine flicker;
    /// <summary>
    /// �_�Ŏ��Ԃ̒���
    /// </summary>
    private float flickerDuration;
    /// <summary>
    /// �_���[�W�_�ł̍��v����
    /// </summary>
    private float flickerTotalElapsedTime;
    /// <summary>
    /// �_���\�W�_�ł�Renderer
    /// </summary>
    private float flickerElapsedTime;
    /// <summary>
    /// �_���[�W�_�ł�Renderer�̗L���E�����؂�ւ��p�̃C���^�[�o���B
    /// </summary>
    private float flickerInterval;

    /*�ړ�*/
    /// <summary>
    /// �����ړ��X�s�[�h
    /// </summary>
    private static float moveSpeed = 4.5f;
    /// <summary>
    /// ���݂̈ړ��X�s�[�h
    /// </summary>
    private float speed;

    /// <summary>
    /// �ړ��A��]
    /// </summary>
    private Vector3 vel, lastPos,diff, rot;//�ړ��A�Ō�̈ʒu�A�ړ����Ă��邩�ǂ����̌v�Z���ʂ��擾�A��]

    /// <summary>
    /// �A�j���[�^�[���g���ăA�j���[�V������ύX����
    /// </summary>
    private Animator animator;

    /*�U��*/
    /// <summary>
    /// �U�����镐����擾����
    /// </summary>
    [SerializeField] private GameObject weapon;
    /// <summary>
    /// ����̓����蔻��(�R���C�_�[)���擾����
    /// </summary>
    private BoxCollider weaponCollider;

    /// <summary>
    /// �U�����ł��鎞��
    /// </summary>
    float attackTime;
    /// <summary>
    /// �U���̃C���^�[�o��
    /// </summary>
    private static float attackMaxTime = 35.0f;
    /// <summary>
    /// ����R���C�_�[�̗L������
    /// </summary>
    private bool isWeponEnable;

    /*�A�C�e��*/
    /// <summary>
    /// �A�C�e���X�N���v�g
    /// </summary>
    private PlayerItem item;
    /// <summary>
    /// ���g�̃X�e�[�^�X�X�N���v�g
    /// </summary>
    private PlayerStartas startas;

    /// <summary>
    /// ���x���A�b�v�I�u�W�F�N�g
    /// </summary>
    [SerializeField] private GameObject levelUpObj;
    /// <summary>
    /// ���x���A�b�v�e�L�X�g
    /// </summary>
    private TextMeshProUGUI levelUpText;
    /// <summary>
    /// �e�L�X�g�\������
    /// </summary>
    private int levelUpCount;

    /*�G�t�F�N�g*/
    /// <summary>
    /// �U�����󂯂��Ƃ��̃p�[�e�B�N��
    /// </summary>
    [SerializeField] private ParticleSystem hitParticle;
    /// <summary>
    /// ���x�����オ�������̃p�[�e�B�N��
    /// </summary>
    [SerializeField] private ParticleSystem levelUpParticle;
    /// <summary>
    /// ��������Ƃ��̃G�t�F�N�g
    /// </summary>
    [SerializeField] private ParticleSystem revivalParticle;

    /*�|�[�Y*/
    /// <summary>
    /// �|�[�Y�̃X�N���v�g
    /// </summary>
    [SerializeField] private GameObject pausePanelObj;

    /*��*/
    /// <summary>
    /// �T�E���h��炷
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// ���ʉ�
    /// </summary>
    [SerializeField] private AudioClip attackSound, hitSound,itemGetSound,levelUpSound;//�U����,�����������̉�,�A�C�e�����艹�A���x���A�b�v���̉�

    /*HP*/
    /// <summary>
    /// HP
    /// </summary>
    private int nowHp, maxHp;

    /// <summary>
    /// �X�ɖ߂�܂ł̎���
    /// </summary>
    private float backCityTime;

    /// <summary>
    /// �������邽�߂̎���
    /// </summary>
    private float revivalTime;
    /// <summary>
    /// �������邽�߂ɕK�v�Ȏ���
    /// </summary>
    private float revivaMaxTime;

    /// <summary>
    /// �擾�����A�C�e���̖��O����ʂɕ\������
    /// </summary>
    private GetItemNameDisplay itemNameDisplay;

    void Start()
    {
        //�����Collider���擾���A������Ȃ��悤�ɖ����ɂ��Ă���
        weaponCollider = weapon.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        isWeponEnable = false;

        //���x���A�b�v���t�F�[�h�C���t�F�[�h�A�E�g������e�L�X�g���擾���A�����Ȃ��悤�ɂ���
        levelUpText = levelUpObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelUpObj.SetActive(false);
        levelUpCount = 0;

        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            // ���g���������Ă��Ȃ��I�u�W�F�N�g�̎�
            if (!photonView.IsMine)
            {
                //�����ȊO�̃J�����ƃX�e�[�^�X�X�N���v�g�𖳌��ɂ���
                this.transform.Find("Camera").gameObject.SetActive(false);
                this.gameObject.GetComponent<PlayerStartas>().enabled = false;
                return;
            }
        }

        //MainCanvas���擾����
        mainCanvas = GameObject.Find("MainCanvas");
        //MainManager���擾����
        manager = GameObject.Find("Manager").GetComponent<MainManager>();

        //�A�N�V�����}�b�v����A�N�V�������擾����
        move = manager.GetPlayerInput().actions["move"];
        run = manager.GetPlayerInput().actions["run"];
        fire = manager.GetPlayerInput().actions["fire"];
        jump = manager.GetPlayerInput().actions["jump"];
        block = manager.GetPlayerInput().actions["block"];
        sit = manager.GetPlayerInput().actions["sit"];
        pause = manager.GetPlayerInput().actions["pause"];

        //�A�N�V����������͒l�̎擾
        value = move.ReadValue<Vector2>();
        
        //�{�^���������Ă��邩��������
        isFire = false;
        isWalkAnim = false;
        isRunAnim = false;
        isBlock = false;
        isSit = false;
        isDeth = false;
        isPause = false;

        //�ŏ��͒n�ʂɂ��Ă��邱�Ƃɂ���
        isGrounded = true;

        //���g��Rigidbody���擾
        rigid = this.gameObject.GetComponent<Rigidbody>();

        //���G������
        damageTime = 0;
        isHit = false;

        //�S�Ă�Renderer���擾
        childrenRenderer = GetComponentsInChildren<Renderer>();

        //�_���[�W�͎󂯂Ă��Ȃ�
        isDamaged = false;

        //�R���[�`����������
        flicker = null;

        //�_�ł̒���
        flickerDuration = 1.5f;

        flickerTotalElapsedTime = 0;
        flickerElapsedTime = 0;

        //�C���^�[�o��
        flickerInterval = 0.075f;

        //�����X�s�[�h�ɏ�����
        speed = moveSpeed;

        //������
        vel = Vector3.zero;//�ړ��x�N�g��
        lastPos = this.transform.position;//�Ō�̈ʒu
        diff = Vector3.zero;//�ړ��̌v�Z����
        rot = Vector3.zero;//��]

        //�A�j���[�^�[���擾����
        animator = this.GetComponent<Animator>();

        //�U��������
        attackTime = 0.0f;

        //���g�̃I�u�W�F�N�g����X�N���v�g���擾
        item = this.gameObject.GetComponent<PlayerItem>();
        startas = this.gameObject.GetComponent<PlayerStartas>();

        audioSource = this.GetComponent<AudioSource>();
        //�T�E���h�̉��ʂ�ύX����
        audioSource.volume = HoldVariable.SEVolume;

        //�ő�HP���擾����
        maxHp = 0;
        if (PhotonNetwork.InRoom)
        {
            //�S�Ẵ��[���Q���҂ɑ���
            photonView.RPC(nameof(MaxHp), RpcTarget.All, startas.GetMaxHp());
        }
        else
        {
            maxHp = startas.GetMaxHp();
        }
        //���݂�HP���擾����
        nowHp = startas.GetNowHp();

        //�X�ɖ߂�܂ł̎��Ԃ��擾
        backCityTime = 5;

        //�������邽�߂̎��Ԃ�������
        revivalTime = 0;

        //�������邽�߂ɕK�v�Ȏ���
        revivaMaxTime = 10;

        //GetItemNameDisplay��ǉ������I�u�W�F�N�g�����邩�ǂ���
        if (GameObject.Find("ItemNameDisplay") != null)
        {
            //����Ƃ�GetItemNameDisplay���擾����
            itemNameDisplay = GameObject.Find("ItemNameDisplay").GetComponent<GetItemNameDisplay>();
            Debug.Log("ItemNameDisplay��������");
        }
    }

    private void Update()
    {
        //���x���A�b�v�e�L�X�g��\�����Ă��鎞
        if (levelUpCount != 0)
        {
            //��]������������
            levelUpObj.transform.rotation = Quaternion.identity;
        }

        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            //���g�����������I�u�W�F�N�g�ł͂Ȃ��Ƃ��������s��Ȃ�
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //����
        Sit();

        //�u���b�N���Ă��鎞�͓����Ȃ�
        if (isBlock) return;
        //�W�����v����
        Jump();
    }

    private void FixedUpdate()
    {
        //����̓����蔻���ύX����
        weaponCollider.enabled = isWeponEnable;

        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            //���g�����������I�u�W�F�N�g�ł͂Ȃ��Ƃ��������s��Ȃ�
            if (!photonView.IsMine)
            {
                return;
            }
        }
        
        //���ʂ��Ⴄ�Ƃ��͉��ʂ����킹��
        if(audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //hp��0�ɂȂ��āA�����Ă��Ȃ��Ƃ�
        if (!startas.IsAlive())
        {
            Death();

            return;
        }

        //�A�j���[�V�����ύX
        animator.SetBool("move", isWalkAnim);
        animator.SetBool("run", isRunAnim);
        animator.SetBool("block", isBlock);

        //�V�[����ύX���Ă��鎞�͏��������Ȃ�
        if (manager.IsChangeScene())
        {
            //�ړ��A�j���[�V�������~�߂�
            animator.SetBool("move", false);
            return;
        }

        LevelUp();

        //�|�[�Y
        if (!isPause && pause.IsPressed())
        {
            // ���[���ɎQ�����Ă��Ȃ��ꍇ�͏���������
            if (!PhotonNetwork.InRoom)
            {
                isPause = true;
                Instantiate(pausePanelObj, Vector3.zero, Quaternion.identity);
            }
            return;
        }
        else if (isPause && !pause.IsPressed())
        {
            isPause = false;
        }

        //�_���[�W���󂯂��Ƃ��͉����ł��Ȃ��悤��
        if (isDamaged)
        {
            damageTime -= Time.deltaTime;
            if (damageTime > 0.0f)
            {
                //�ړ��ʂ�������
                value = Vector2.zero;
                return;
            }
        }

        //�����Ă��鎞�̓W�����v�ȊO�ł��Ȃ�
        if (isSit) return;
        Block();
        Fire();
        Move();
        //�u���b�N���Ă��鎞�͑���Ȃ�
        if (isBlock) return;
        Run();
    }

    /// <summary>
    /// ���x���A�b�v
    /// </summary>
    private void LevelUp()
    {
        //���x���A�b�v�����Ƃ�
        if (startas.IsLevelUp())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(levelUpSound);

            //�p�[�e�B�N�����Đ�
            levelUpParticle.Play();

            //���x���A�b�v�e�L�X�g�̃I�u�W�F�N�g
            levelUpObj.transform.rotation = new Quaternion(levelUpObj.transform.rotation.x, 0, 0, 0);//��]��������
            levelUpObj.transform.position = this.transform.position + new Vector3(0.0f, 3.0f, 0.0f);//�ʒu��������
            levelUpObj.SetActive(true);//�\������

            //�e�L�X�g�̃��l���ő�ɂ���
            Color color = levelUpText.color;
            color.a = 1.0f;
            levelUpText.color = color;

            //�e�L�X�g��\�����Ă�������
            levelUpCount = 60 * 5;
        }

        //���x���A�b�v�e�L�X�g��\�����Ă��鎞
        if(levelUpObj.activeSelf)
        {
            //���񂾂񓧖��ɂ���
            Color color = levelUpText.color;
            color.a -= 0.01f;
            levelUpText.color = color;

            //���Ԃ����炷
            levelUpCount--;
            //Debug.Log(levelUpCount);

            //���l��0�����������Ƃ�
            if (color.a <= 0.0f)
            {
                //�\�����I���
                levelUpCount = 0;
            }

            //�\�����Ԃ�0��菬�����Ȃ�����
            if (levelUpCount <= 0)
            {
                levelUpCount = 0;

                color.a = 0.0f;
                levelUpText.color = color;
                //��\���ɂ���
                levelUpObj.SetActive(false);
                Debug.Log("���x���A�b�v�e�L�X�g:false");
            }
        }
    }


    /// <summary>
    /// �ړ�
    /// </summary>
    private void Move()
    {
        //������
        value = Vector2.zero;
        vel = Vector3.zero;

        //�A�N�V�����J�n����I���܂�
        isWalkAnim = move.IsPressed();

        //�ړ��{�^���������Ă��Ȃ��Ƃ�
        if (!isWalkAnim )
        {
            return;
        }

        //���͒l�̎擾
        value = move.ReadValue<Vector2>();

        //�ړ��ʌv�Z
        vel = new Vector3(value.x, 0.0f, value.y) * Time.deltaTime * speed;
        //�ړ��ʂ𑫂�
        this.transform.position += vel;

        //�O�񂩂�i�񂾂����擾
        diff = this.transform.position - lastPos;
        //y���͕ύX���Ȃ�
        diff.y = 0;
        //�O��̈ʒu�̍X�V
        lastPos = this.transform.position;

        //�ړ��ʂ����Ȃ��Ƃ��̓v���C���[�̕�����ς��Ȃ�
        if(diff.sqrMagnitude <= 0.001f)
        {
            return;
        }

        var playerRot = Quaternion.LookRotation(diff, Vector3.up);//�v���C���[�̐i�s�����������N�H�[�^�j�I��
        var diffAngle = Vector3.Angle(transform.forward, diff);//���݂̌����Ɛi�s�����̊p�x
        float currentAngularVel = 0.0f;
        var rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngularVel, 0.03f, Mathf.Infinity);//���݂̉�]����p�x
        var nextRot = Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);//�ǂ̂��炢��]���邩
        transform.rotation = nextRot;

    }

    /// <summary>
    /// ����
    /// </summary>
    private void Run()
    {
        //������
        speed = moveSpeed;
        //�A�N�V�����J�n����I���܂�
        isRunAnim = run.IsPressed();

        //����{�^���������Ă��Ȃ��@|| �����Ă��Ȃ��Ƃ�
        if (!isRunAnim || !isWalkAnim)
        {
            return;
        }
        //�����X�s�[�h��1.5�{�ɂ���
        speed *= 1.5f;
    }

    /// <summary>
    /// �U��
    /// </summary>
    private void Fire()
    {
        if(manager.NowGameScene() != "grassland" && manager.NowGameScene() != "plain")
        {
            return;
        }

        //�C���^�[�o��
        attackTime -= 1.0f;
        //���ɍU���ł���܂ł̃C���^�[�o����0�����������Ƃ��@&&�@�U�����̎�
        if (attackTime <= 0.0f && isFire)
        {
            //0�ɂ���
            attackTime = 0.0f;
            //����̓����蔻�肪����Ƃ�
            if (isWeponEnable)
            {
                //�����蔻�������
                isWeponEnable = false;
            }
            //�U�����I��
            isFire = false;
        }
        else if(attackTime == 15.0f)
        {
            //�U����
            audioSource.PlayOneShot(attackSound);
            //����̓����蔻���true�ɂ���
            isWeponEnable = true;
            //�U�����Ă��鎞�͖h�䔻����s��Ȃ��悤��
            isBlock = false;
            return;
        }
        //���ɍU���ł���܂ł̎��Ԃ��܂�����Ƃ�
        else if(attackTime > 0.0f)
        {
            return;
        }

        //�A�N�V�����̊J�n����I���܂�
        isFire = fire.IsPressed();
        //�U�������Ƃ�
        if(isFire)
        {
            animator.SetBool("attack", true);
            attackTime = attackMaxTime;
        }
    }

    /// <summary>
    /// �W�����v
    /// </summary>
    private void Jump()
    {
        //�n�ʂɂ��Ă���Ƃ�
        if(isGrounded)
        {
            //�W�����v�L�[��������Ă���Ƃ�
            if (jump.WasPressedThisFrame())
            {
                //�����Ă�����
                if (isSit)
                {
                    isSit = false;
                    animator.SetBool("sit", isSit);
                }

                //�W�����v�̑��x���v�Z
                var jumpVelocity = Vector3.up * (moveSpeed * 1.0f);
                
                rigid.AddForce(jumpVelocity, ForceMode.Impulse);
                animator.SetBool("jump", true);

                isGrounded = false;
            }
        }
    }

    /// <summary>
    /// �u���b�N
    /// </summary>
    private void Block()
    {
        //�A�N�V�����̊J�n����I���܂�
        isBlock = block.IsPressed();

        //�u���b�N�{�^���������Ă��Ȃ�������
        if (!isBlock)
        {
            //�����𔲂���
            return;
        }

        isRunAnim = false;
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Sit()
    {
        //�����Ă�����
        if (sit.WasPerformedThisFrame())
        {
            //����ւ���
            isSit = !isSit;
            animator.SetBool("sit", isSit);
        }
    }

    /// <summary>
    /// ���񂾂Ƃ��̏���
    /// </summary>
    private void Death()
    {
        //isDeth��false�̎�
        if (!isDeth)
        {
            //���񂾂��Ƃɂ���
            isDeth = true;
            //�U�����ꂽ�Ƃ��̓_�Ŏ��Ԃ��I��点��
            flickerTotalElapsedTime = flickerDuration;
            //���񂾃A�j���[�V�������Đ�
            animator.SetBool("death", true);

            //�����l�v���C�̎���
            if(PhotonNetwork.InRoom)
            {
                //�����܂ł̃G�t�F�N�g���Đ�
                revivalParticle.Play();
            }
        }
        //���g�̃^�O��ύX���āA�G��������ɗ��Ȃ��悤�ɂ���
        if (this.tag == "Player")
        {
            this.tag = "Untagged";
        }

        // ���[���ɎQ�����Ă���ꍇ
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (!photonView.IsMine)
            {
                return;
            }

            //�����܂ł̎��Ԃ�i�߂�
            revivalTime += Time.deltaTime;

            //��������܂ł̎��Ԉȏ�ɂȂ�����
            if (revivalTime >= revivaMaxTime)
            {
                //���Ԃ�������
                revivalTime = 0;
                
                //����ł��Ȃ����Ƃɂ���
                isDeth = false;
                
                //�A�j���[�V������ύX
                animator.SetBool("death", false);
                animator.SetBool("attack", true);
                
                //����
                startas.Heals(100);
                
                //���݂�HP���擾����
                nowHp = startas.GetNowHp();
                
                //���g�̃^�O��ύX����
                if (this.tag != "Player")
                {
                    this.tag = "Player";
                }
            }
            
            return;
        }
        else
        {
            //�X�ɖ߂�܂ł̎��Ԃ����炵�Ă���
            backCityTime -= Time.deltaTime;
            
            //�e�L�X�g�ɕ\������
            var str = $"����{(int)backCityTime}�b��\n�X�ɂ��ǂ�܂��B";
            mainCanvas.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = str;
            
            //�X�ɖ߂�
            if (backCityTime < 0.0f)
            {
                manager.ChangeScene("city");
                return;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
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

        //�����Ă��Ȃ��Ƃ��͉������Ȃ�
        if (!startas.IsAlive())
        {
            return;
        }

        if (!isGrounded)
        {
            if (collision.gameObject.tag != "wall")
            {
                Debug.Log("Player �W�����v�ł���悤��");
                isGrounded = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
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

        //�����Ă��Ȃ��Ƃ��͉������Ȃ�
        if (!startas.IsAlive())
        {
            return;
        }

        //if (other.gameObject.tag != "wall")
        //{
        //    Debug.Log("Player �W�����v�ł���悤��");
        //    isGrounded = true;
        //}

        //�A�C�e���^�O���ǂ���
        if (other.gameObject.tag == "item")
        {
            //�A�C�e���X�N���v�g�����邩
            Debug.Log("�A�C�e���X�N���v�g");
            Item itemScript = other.gameObject.GetComponent<Item>();

            if (itemScript != null)
            {
                item.CountItem(itemScript.id, 1);//���g�̃A�C�e���ɒǉ�����
                Debug.Log("�A�C�e���ǉ�");
                //GetItemNameDisplay�����邩���m�F
                if (itemNameDisplay != null)
                {
                    //���������͎擾�����A�C�e������n��
                    itemNameDisplay.ItemNameDisplay(item.ItemName(itemScript.id));
                }

                //�擾��
                audioSource.PlayOneShot(itemGetSound);
                //�ǉ������A�C�e���͏���
                Destroy(other.gameObject);
            }
        }

        //�G�Ɠ���������
        if (other.gameObject.tag == "enemy")
        {
            //�����������͈�u�d��
            isWalkAnim = false;

            //�ړ��ʂ�������
            value = Vector2.zero;

            ////���G���Ԓ��͉������Ȃ�
            if (isDamaged) return;

            //�G�̕���������   �^�[�Q�b�g - ���g�̈ʒu
            rot = other.gameObject.transform.position - this.transform.position;
            rot.y = 0.0f;
            this.transform.rotation = Quaternion.LookRotation(rot);
            //�m�b�N�o�b�N
            rigid.AddForce(-transform.forward.normalized * (moveSpeed * 0.2f), ForceMode.VelocityChange);//�����X�s�[�h��

            //�U�������Ă��Ȃ��Ƃ��͓��������A�j���[�V����������
            if (!isWeponEnable)
            {
                if(!isHit)
                {
                    //���������A�j���[�V�����ɂ���
                    animator.SetBool("hit", true);
                    isHit = true;
                }
            }
            //�U�������Ă��鎞�͍U�����󂯂Ȃ�
            else
            {
                animator.SetBool("hit", false);
                return;
            }

            //�q�b�g��
            audioSource.PlayOneShot(hitSound);

            //�u���b�N���Ă�����HP�͌���Ȃ�
            if (isBlock)
            {
                damageTime = 0;
                return;
            }
            //�_���[�W
            Damaged();
            
            //���������G�t�F�N�g���Đ�
            hitParticle.Play();
            
            //parent �e�I�u�W�F�N�g�@root ��ԏ�̐e�@GetChild(0)�@�q���擾
            int hit = other.gameObject.transform.root.GetComponent<EnemyStartas>().GetAttackPower() - startas.GetDefensePower();
            startas.Hit(hit);
            nowHp = startas.GetNowHp();

            //���G����
            damageTime = 0.5f;

            //�m�b�N�o�b�N
            rigid.AddForce(-transform.forward.normalized * (moveSpeed * 0.5f), ForceMode.VelocityChange);//�u���b�N���Ă��Ȃ��Ƃ��͂���Ƀm�b�N�o�b�N����

            this.transform.position -= diff * Time.deltaTime * moveSpeed;
        }
    }

    //HP�o�[����M
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���̃N���C�A���g��PhotonView�̏��L�҂ł���ꍇ
        if (stream.IsWriting)
        {
            //���M����
            stream.SendNext(nowHp);//���݂�HP
            stream.SendNext(isWeponEnable);//����̓����蔻��
        }
        else
        {
            //��M����
            nowHp = (int)stream.ReceiveNext();//���݂�HP��ύX����
            isWeponEnable = (bool)stream.ReceiveNext();//����̓����蔻���ύX����
        }
    }

    /// <summary>
    /// �ő�HP�𑗂�
    /// </summary>
    /// <param name="hp">�ő�HP</param>
    [PunRPC]
    private void MaxHp(int hp)
    {
        maxHp = hp;
    }

    //--------------------------�_���[�W--------------------------//
    /// <summary>
    /// �_���[�W���󂯂��Ƃ��ɌĂяo��
    /// </summary>
    private void Damaged()
    {
        //�_���[�W�_�Œ��͓�d�Ɏ��s���Ȃ��B
        if (isDamaged)
            return;

        StartFlicker();
    }
    /// <summary>
    /// �\�����邩���Ȃ�����ύX����֐�
    /// </summary>
    /// <param name="b">true:�\���ɂ��� false:��\���ɂ���</param>
    private void SetEnabledRenderers(bool b)
    {
        //����\����\����ύX����
        for (int i = 0; i < childrenRenderer.Length; i++)
        {
            childrenRenderer[i].enabled = b;
        }
    }

    /// <summary>
    /// �񓯊��œ_�ł��n�߂�
    /// </summary>
    private void StartFlicker()
    {
        //isDamaged�ő��d���s��h���ł���̂ŁA�����ő��d���s��e���Ȃ��Ă�OK�B        
        flicker = StartCoroutine(Flicker());
    }

    /// <summary>
    /// �񓯊��œ_��
    /// </summary>
    /// <returns></returns>
    IEnumerator Flicker()
    {
        //�_�Œ��̓_���[�W���󂯂Ă���ݒ��
        isDamaged = true;
        //������
        flickerTotalElapsedTime = 0;
        flickerElapsedTime = 0;

        while (true)
        {
            //���Ԃ�i�߂�
            flickerTotalElapsedTime += Time.deltaTime;
            flickerElapsedTime += Time.deltaTime;


            //�_���[�W�_�ł̃C���^�[�o��
            if (flickerInterval <= flickerElapsedTime)
            {
                //��������_���[�W�_�ł̏����B

                flickerElapsedTime = 0;
                //Renderer�̗L���A�����̔��]�B
                isEnabledRenderers = !isEnabledRenderers;
                SetEnabledRenderers(isEnabledRenderers);

            }

            //�_�Ŏ��Ԃ̒���
            if (flickerDuration <= flickerTotalElapsedTime)
            {
                //��������_���[�W�_�ł̏I�����̏����B
                Debug.Log("��_���[�W�I��");
                isDamaged = false;
                isHit = false;

                //�Ō�ɂ͕K��Renderer��L���ɂ���(�������ςȂ��ɂȂ�̂�h��)�B
                isEnabledRenderers = true;
                SetEnabledRenderers(true);

                flicker = null;
                ResetFlicker();
                yield break;
            }

            yield return null;
        }

    }

    /// <summary>
    /// �R���[�`���̃��Z�b�g�p
    /// </summary>
    private void ResetFlicker()
    {
        if (flicker != null)
        {
            StopCoroutine(flicker);
            flicker = null;
        }
    }
}
