using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �^�C�g���}�l�[�W���[
/// (���O�ƃp�X���[�h����͌�A���̃V�[����)
/// </summary>
public class titleManager : MonoBehaviour
{
    private MainManager manager;//���C���}�l�[�W���[
    private InputAction move,submit, cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    private bool isSubmit;//��������������ǂ���
    private bool isCreate;//�A�J�E���g���쐬���邩�ǂ���
    private bool isSelect;//���������ǂ���

    [Header("Rogin")]
    /// <summary>
    /// ���O����
    /// </summary>
    [SerializeField] private GameObject nameInput;

    /*���O�A�p�X���[�h*/
    [SerializeField] private TMP_InputField enterName, enterPass;//���O�̓���,�p�X���[�h�̓���
    private bool isName;/// ���O�̓��͂�I�����Ă��邩
    private bool isLogin;//���O�C���ł��邩�ǂ���
    private rogin roginScript;//�ʐM
    [SerializeField] private TextMeshProUGUI nextText;//InputField�̉��ɂ���e�L�X�g

    /*��*/
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip directionSound;//�������Ƃ��̉�
    [SerializeField] private AudioClip noGoodSound;//�_���Ȏ��̉�

    [Header("Frame")]
    /// <summary>
    /// �t���[��
    /// </summary>
    [SerializeField] private GameObject selectFrame;
    /// <summary>
    /// �t���[���̊g��k��
    /// </summary>
    private Scaling scalingScprit;
    /// <summary>
    /// �t���[���̈ʒu���擾
    /// </summary>
    [SerializeField] private Transform createPos, roginPos, settingPos, endPos;

    [Header("Menu")]
    [SerializeField] private GameObject settingPrefab;//�ݒ�v���n�u
    private GameObject settingObject;//�ݒ���쐬
    private bool isSetting;//�ݒ���J���Ă��邩

    private bool isGameEnd;//�Q�[�����I�������邩�ǂ���

    //�I�񂾎��
    enum MenuItem
    { 
        create,
        rogin,
        setting,
        end,

        menuNum
    };

    private MenuItem menuItem;

    //ToDo ���j���[�̒��ɐݒ��ǉ�����
    //MenuItem.setting = 3
    //���ʐݒ�A�E�B���h�E���[�h�ύX�A�L�[�ݒ�A����A�Q�[���I��

    private void Start()
    {
        manager = this.GetComponent<MainManager>();

        move = manager.GetPlayerInput().actions["move"];
        submit = manager.GetPlayerInput().actions["fire"];
        cancel = manager.GetPlayerInput().actions["back"];

        roginScript = this.GetComponent<rogin>();

        audioSource.volume = HoldVariable.SEVolume;

        scalingScprit = this.GetComponent<Scaling>();
        scalingScprit.Init(0.9f, 0.7f);

        menuItem = MenuItem.create;
        scalingScprit.ScalingObjPosition(selectFrame.transform, createPos.position);

        settingObject = Instantiate(settingPrefab);
        settingObject.SetActive(false);
        isSetting = false;

        isGameEnd = false;

        Init();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Init()
    {
        enterName.Select();
        isName = false;

#if UNITY_EDITOR
        enterName.text = "crescita";
        enterPass.text = "12345";
#endif
        nameInput.SetActive(false);

        isSubmit = false;

        isCreate = false;

        isLogin = false;

        isSelect = false;

        nextText.enabled = false;
        nextText.text = "";
    }

    private void Update()
    {
        if (manager.IsChangeScene()) return;

        //�Q�[���I������Ƃ�
        if(isGameEnd)
        {
            GameEndUpdate();
            return;
        }

        //�ݒ��ʂ��J���Ă��鎞
        if (isSetting)
        {
            if(!settingObject.activeSelf)
            {
                isSetting = false;
                Debug.Log("�ݒ�I��");
            }
            return;
        }

        scalingScprit.ScalingObj(selectFrame.transform);//�g��k��

        if (isSubmit)
        {
            switch (menuItem)
            {
                case MenuItem.create:
                case MenuItem.rogin:
                    NameAndPassword();
                    return;
                case MenuItem.setting:
                default:
                    return;
            }

        }

        //�I��
        if (move.WasPressedThisFrame())
        {
            var value = move.ReadValue<Vector2>();
            var menuNum = (int)MenuItem.menuNum;
            if (value.x > 0 )
            {
                menuItem = (MenuItem)(((int)menuItem + 1) % menuNum);
            }
            else if(value.x < 0)
            {
                menuItem = (MenuItem)(((int)menuItem + (menuNum - 1)) % menuNum);
            }
            else if(value.y < 0)
            {
                menuItem = (MenuItem)(((int)menuItem + (menuNum / 2)) % menuNum);
            }
            else if(value.y > 0)
            {
                menuItem = (MenuItem)(((int)menuItem + (menuNum - (menuNum / 2))) % menuNum);
            }

            switch (menuItem)
            {
                case MenuItem.create:
                default:
                    scalingScprit.ScalingObjPosition(selectFrame.transform, createPos.position);
                    break;
                case MenuItem.rogin:
                    scalingScprit.ScalingObjPosition(selectFrame.transform, roginPos.position);
                    break;
                case MenuItem.setting:
                    scalingScprit.ScalingObjPosition(selectFrame.transform, settingPos.position);
                    break;
                case MenuItem.end:
                    scalingScprit.ScalingObjPosition(selectFrame.transform, endPos.position);
                    break;
            }

        }

        //������������Ƃ�
        if (submit.WasPressedThisFrame())
        {
            switch (menuItem)
            {
                case MenuItem.create:
                    Create();
                    break;
                case MenuItem.rogin:
                    Login();
                    break;
                case MenuItem.setting:
                    isSetting = true;
                    settingObject.SetActive(true);
                    break;
                case MenuItem.end:
                    isGameEnd = true;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// �Q�[���I����I�񂾎��̃A�b�v�f�[�g
    /// </summary>
    private void GameEndUpdate()
    {
        if (manager.GameEnd())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }
    }

    /// <summary>
    /// ���O�ƃp�X���[�h�̓���
    /// </summary>
    private void NameAndPassword()
    {
        //�߂���������Ƃ����̉�ʂɖ߂�
        if(cancel.WasPressedThisFrame())
        {
            Init();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //���O����͂���t�H�[����I�����Ă����ꍇ�̓p�X���[�h���͂ɕύX����
            if(isName)
            {
                enterPass.Select();
                isName = false;
            }
            //�p�X���[�h����͂���t�H�[����I�����Ă����ꍇ�͖��O����͂���t�H�[���ɕύX
            else
            {
                enterName.Select();
                isName = true;
            }
        }

        //���O��16�����܂œ��͂ł���
        if (enterName.text.Length > 16)
        {
            enterName.text = enterName.text[..16];
            //����炷
            audioSource.PlayOneShot(noGoodSound);
        }
        //�p�X���[�h��10�����܂œ��͂ł���
        if (enterPass.text.Length > 10)
        {
            enterPass.text = enterPass.text[..10];
            //����炷
            audioSource.PlayOneShot(noGoodSound);
        }

        //���͂��ꂽ��������2�ȏ�̎� && �p�X���[�h�̐���4�ȏ�̎�
        if (enterName.text.Length >= 2 && enterPass.text.Length >= 4)
        {
            nextText.enabled = true;//���ւ̕�����\��

            //�I�����Ă��Ȃ��@&& ���O�C�����Ă��Ȃ��@&& ����������Ă���
            if (!isSelect && !isLogin && submit.WasPressedThisFrame())
            {
                isSelect = true;

                //�쐬���Ȃ��Ƃ�
                if (!isCreate)
                { 
                    isLogin = roginScript.Login(enterName.text, enterPass.text);
                    Debug.Log("���O�C��"+isLogin);
                }
                else if (isCreate)
                {
                    isLogin = roginScript.Create(enterName.text, enterPass.text);
                    Debug.Log("�쐬"+isLogin);
                }
            }
            //���O�C���ł����Ƃ�
            else if (isLogin)
            {
                enterName.interactable = false;//�������͂��ł��Ȃ��悤��
                enterPass.interactable = false;//�������͂��ł��Ȃ��悤��

                //���͂������O���v���C���[�̖��O�ɂ���
                HoldVariable.playerName = enterName.text;

                //city�ɃV�[���ύX
                manager.ChangeScene("city");
            }

            //���O�C���o���ĂȂ� && ���ւ̃e�L�X�g���@(���O�C������ �܂��� �쐬)�ɕς���Ă�����
            if (!isLogin && (nextText.text == "���O�C������" || nextText.text == "�쐬"))
            {
                //���O�C��������
                isLogin = true;
                //����炷
                audioSource.PlayOneShot(directionSound);
            }
        }
        else
        {
            nextText.enabled = false;
            isSelect = false;
        }
    }

    /// <summary>
    /// �A�J�E���g���쐬����
    /// </summary>
    public void Create()
    {
        //����炷
        audioSource.PlayOneShot(directionSound);

        isCreate = true;
        nameInput.SetActive(true);
        isSubmit = true;
    }
    /// <summary>
    /// �A�J�E���g�̖��O�ƃp�X���[�h�Ń��O�C������
    /// </summary>
    public void Login()
    {
        //����炷
        audioSource.PlayOneShot(directionSound);

        isCreate = false;
        nameInput.SetActive(true);
        isSubmit = true;
    }
}
