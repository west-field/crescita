using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �^�C�g���}�l�[�W���[
/// (���O�ƃp�X���[�h����͌�A���̃V�[����)
/// </summary>
public class titleManager : MonoBehaviour
{
    [SerializeField] private MainManager manager;//���C���}�l�[�W���[
    private InputAction move,submit, cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    private bool isEnterName;//���O�C����ʂ�\�������邩�ǂ���
    private bool isCreate;//�A�J�E���g���쐬���邩�ǂ���
    private bool isSelect;//���������ǂ���

    [SerializeField] private new GameObject name;

    /*���O�A�p�X���[�h*/
    [SerializeField] private TMP_InputField enterName, enterPass;//���O�̓���,�p�X���[�h�̓���
    /// <summary>
    /// ���O�̓��͂�I�����Ă��邩
    /// </summary>
    private bool isName;
    private bool isLogin;//���O�C���ł��邩�ǂ���
    [SerializeField] private rogin roginScript;//�ʐM
    [SerializeField] private TextMeshProUGUI nextText;//InputField�̉��ɂ���e�L�X�g
    
    /*��*/
    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip directionSound, noGoodSound;//�������Ƃ��̉�,�_���Ȏ��̉�

    /// <summary>
    /// �t���[��
    /// </summary>
    [SerializeField] private GameObject selectFrame;
    /// <summary>
    /// �t���[���̊g��k��
    /// </summary>
    [SerializeField] private Scaling scalingSprit;
    /// <summary>
    /// �t���[���̈ʒu���擾
    /// </summary>
    [SerializeField] private Transform createPos, roginPos;
    /// <summary>
    /// �t���[���̈ʒu�����߂�
    /// </summary>
    [SerializeField] private bool isCreateButton;

    void Start()
    {
        move = manager.GetPlayerInput().actions["move"];
        submit = manager.GetPlayerInput().actions["fire"];
        cancel = manager.GetPlayerInput().actions["back"];

        audioSource.volume = HoldVariable.SEVolume;

        scalingSprit.Init(0.9f, 0.7f);

        isCreateButton = false;

        Init();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Init()
    {
        enterName.Select();
        isName = false;

        //enterName.text = "crescita";
        //enterPass.text = "12345";
        name.gameObject.SetActive(false);

        isEnterName = false;

        isCreate = false;

        isLogin = false;

        isSelect = false;

        nextText.enabled = false;
        nextText.text = "���ց@A�{�^�� or enter";
    }

    private void Update()
    {
        if (manager.IsChangeScene()) return;

        scalingSprit.ScalingObj(selectFrame.transform);//�g��k��

        if (isEnterName)
        {
            NameAndPassword();
            return;
        }

        //�I�����Ă���ق��Ƀt���[�����ړ�
        if (move.WasPressedThisFrame())
        {
            isCreateButton = !isCreateButton;

            if (isCreateButton)
            {
                scalingSprit.ScalingObjPosition(selectFrame.transform, createPos.position);
            }
            else
            {
                scalingSprit.ScalingObjPosition(selectFrame.transform, roginPos.position);
            }
        }

        //������������Ƃ�
        if (submit.WasPressedThisFrame())
        {
            if (isCreateButton)
            {
                CreateButtomClick();
            }
            else
            {
                LoginButtomClick();
            }
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

        if(Input.GetKeyDown(KeyCode.Tab))
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
            nextText.text = "���ց@A�{�^�� or enter";
            nextText.enabled = false;
            isSelect = false;
        }
    }

    /// <summary>
    /// �A�J�E���g���쐬����
    /// </summary>
    public void CreateButtomClick()
    {
        //����炷
        audioSource.PlayOneShot(directionSound);

        isCreate = true;
        name.gameObject.SetActive(true);
        isEnterName = true;
    }
    /// <summary>
    /// �A�J�E���g�̖��O�ƃp�X���[�h�Ń��O�C������
    /// </summary>
    public void LoginButtomClick()
    {
        //����炷
        audioSource.PlayOneShot(directionSound);

        isCreate = false;
        name.gameObject.SetActive(true);
        isEnterName = true;
    }
}