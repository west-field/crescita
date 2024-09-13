using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;
using System.Linq;

/// <summary>
/// �|�[�Y�p�l��
/// </summary>
public class pausePanelScript : MonoBehaviourPunCallbacks
{
    /*�\���p�l��*/
    /// <summary>
    /// �|�[�Y�p�l��
    /// </summary>
    private GameObject pausePanel;
    /// <summary>
    /// �A�C�e���\���p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject itemPanelPrefab;
    /// <summary>
    /// �L�[�ݒ�\���p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject keyConfigPanelPrefab;
    /// <summary>
    /// �X�e�[�^�X�\���p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject startasPanelPrefab;
    /// <summary>
    /// �T�E���h�\���p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject soundPanelPrefab;
    /// <summary>
    /// �E�B���h�E�ύX�p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject windowPanelPrefab;
    /// <summary>
    /// �I�����Ă���ꏊ�Ƀt���[����\������
    /// </summary>
    [SerializeField] private GameObject selectFrameObj;

    /// <summary>
    /// �쐬�����p�l���v���n�u��ێ�
    /// </summary>
    private GameObject item, key,startas,sound, window;

    /*��*/
    /// <summary>
    /// �T�E���h
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// �|�[�Y���J���Ƃ��̉�,�J�[�\���ړ���
    /// </summary>
    [SerializeField] private AudioClip openingClosingSound, moveSound;

    /*�I��*/
    /// <summary>
    /// �|�[�Y��ʂőI���ł��鍀��
    /// </summary>
    private enum PauseItem
    {
        item,
        startas,
        keyConfig,
        sound,
        window,
        back,
        titleBack,
        gameEnd,

        max
    }
    /// <summary>
    /// �I�����Ă�����̂�����
    /// </summary>
    private int selectNum;
    /// <summary>
    /// �I�����ڕ������擾
    /// </summary>
    [SerializeField] private TextMeshProUGUI[] text;
    /// <summary>
    /// ���ܕ\�����Ă���(�I������)����
    /// </summary>
    private PauseItem drawType;

    /*�L�[*/
    /// <summary>
    /// ���C���}�l�[�W���[
    /// </summary>
    private MainManager mainManager;
    /// <summary>
    /// �A�N�V�����}�b�v����A�N�V�����̎擾
    /// </summary>
    private InputAction Navigate, Submit, Cancel,Pause;

    /// <summary>
    /// �|�[�Y��ʂ��ǂ���
    /// </summary>
    private bool isPause;

    //�g���̊g��k��
    [SerializeField] private Scaling scalingScript;

    void Start()
    {
        pausePanel = this.gameObject;

        audioSource = this.GetComponent<AudioSource>();
        //�T�E���h�̉��ʂ�ݒ肵�����ʂɕύX����
        audioSource.volume = HoldVariable.SEVolume;
        //�\�����ꂽ�Ƃ��ɉ���炷
        audioSource.PlayOneShot(openingClosingSound);

        //Canvas�Q�[���I�u�W�F�N�g���擾����
        GameObject canvas = GameObject.Find("MainCanvas");
        //���g(�|�[�Y�p�l��)��Canvas�̎q�I�u�W�F�N�g�ɂ���
        this.transform.SetParent(canvas.transform, false);

        scalingScript.Init(0.9f, 0.7f);
        //�t���[���̈ʒu��߂�̈ʒu�ɂ���
        scalingScript.ScalingObjPosition(selectFrameObj.transform, text[(int)PauseItem.back].transform.position);

        //���g(�|�[�Y�p�l��)��\��
        PanelChange(true);

        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (!photonView.IsMine)
            {
                //Debug.Log("�X�L�b�v����");
                return;
            }
            item = PhotonNetwork.Instantiate(itemPanelPrefab.name, Vector3.zero, Quaternion.identity);//�A�C�e���p�l��
            item.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            item.SetActive(false);//�ŏ��͌����Ȃ��悤��

            key = PhotonNetwork.Instantiate(keyConfigPanelPrefab.name, Vector3.zero, Quaternion.identity);//�L�[�R���t�B�O�p�l��
            key.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            key.SetActive(false);//�ŏ��͌����Ȃ��悤��

            startas = PhotonNetwork.Instantiate(startasPanelPrefab.name, Vector3.zero, Quaternion.identity);//�X�e�[�^�X�p�l��
            startas.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            startas.SetActive(false);//�ŏ��͌����Ȃ��悤��

            sound = PhotonNetwork.Instantiate(soundPanelPrefab.name, Vector3.zero, Quaternion.identity);//�T�E���h�p�l��
            sound.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            sound.SetActive(false);//�ŏ��͌����Ȃ��悤��

            window = PhotonNetwork.Instantiate(windowPanelPrefab.name, Vector3.zero, Quaternion.identity);//�E�B���h�E�ύX�p�l��
            window.transform.SetParent(canvas.transform, false);
            window.SetActive(false);
        }
        else
        {
            item = Instantiate(itemPanelPrefab, Vector3.zero, Quaternion.identity);//�A�C�e���p�l��
            item.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            item.SetActive(false);//�ŏ��͌����Ȃ��悤��

            key = Instantiate(keyConfigPanelPrefab, Vector3.zero, Quaternion.identity);//�L�[�R���t�B�O�p�l��
            key.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            key.SetActive(false);//�ŏ��͌����Ȃ��悤��

            startas = Instantiate(startasPanelPrefab, Vector3.zero, Quaternion.identity);//�X�e�[�^�X�p�l��
            startas.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            startas.SetActive(false);//�ŏ��͌����Ȃ��悤��

            sound = Instantiate(soundPanelPrefab, Vector3.zero, Quaternion.identity);//�X�e�[�^�X�p�l��
            sound.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
            sound.SetActive(false);//�ŏ��͌����Ȃ��悤��

            window = Instantiate(windowPanelPrefab, Vector3.zero, Quaternion.identity);//�E�B���h�E�ύX�p�l��
            window.transform.SetParent(canvas.transform, false);
            window.SetActive(false);
        }

        //���C���}�l�[�W���[���擾
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        //�L�[�擾
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];
        Pause = mainManager.GetPlayerInput().actions["pause"];

        //�|�[�Y��\�����Ă���
        isPause = true;

        //������
        drawType = PauseItem.max;
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

        //���ʂ��Ⴄ�Ƃ��͉��ʂ����킹��
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //���ܕ\�����Ă���(�I������)���̂̃A�b�v�f�[�g������
        switch (drawType)
        {
            case PauseItem.item:
                ItemDrawUpdate();
                break;
            case PauseItem.startas:
                StartasDrawUpdate();
                break;
            case PauseItem.keyConfig:
                KeyConfigDrawUpdate();
                break;
            case PauseItem.sound:
                SoundDrawUpdate();
                break;
            case PauseItem.window:
                WindowDrawUpdate();
                break;
            case PauseItem.titleBack:
                TitleBackUpdate();
                break;
            case PauseItem.gameEnd:
                GameEndUpdate();
                break;
            default:
                PauseDrawUpdate();
                break;
        }
    }

    /// <summary>
    /// �|�[�Y���(�����w�肵�Ă��Ȃ��Ƃ�)�̃A�b�v�f�[�g
    /// </summary>
    private void PauseDrawUpdate()
    {
        //�ړ��{�^���������Ă�����
        if (Navigate.WasPressedThisFrame())
        {
            var val = Navigate.ReadValue<Vector2>();

            //���A��������Ă�����
            if (val.x < 0.0f || val.y > 0.0f)
            {
                //����
                selectNum = (selectNum + ((int)PauseItem.max - 1)) % (int)PauseItem.max;
            }
            //�E�A���������Ă�����
            else if (val.x > 0.0f || val.y < 0.0f)
            {
                //�����
                selectNum = (selectNum + 1) % (int)PauseItem.max;
            }

            //�T�E���h��炷
            audioSource.PlayOneShot(moveSound);
            //�I�����Ă��鍀�ڂ̐F��ς���
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    scalingScript.ScalingObjPosition(selectFrameObj.transform, text[i].transform.position);
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }

        scalingScript.ScalingObj(selectFrameObj.transform);

        //������������Ƃ�
        if (Submit.WasPressedThisFrame())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);

            //�I�������p�l����\��������
            switch (selectNum)
            {
                case (int)PauseItem.item:
                    item.SetActive(true);
                    drawType = PauseItem.item;
                    selectNum = 0;
                    break;
                case (int)PauseItem.startas:
                    startas.SetActive(true);
                    drawType = PauseItem.startas;
                    selectNum = 0;
                    break;
                case (int)PauseItem.keyConfig:
                    key.SetActive(true);
                    drawType = PauseItem.keyConfig;
                    selectNum = 0;
                    break;
                case (int)PauseItem.sound:
                    sound.SetActive(true);
                    drawType = PauseItem.sound;
                    selectNum = 0;
                    break;
                case (int)PauseItem.window:
                    window.SetActive(true);
                    drawType = PauseItem.window;
                    selectNum = 0;
                    break;
                case (int)PauseItem.back:
                    // ���[���ɎQ�����Ă���ꍇ�͏���������
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.Destroy(gameObject);
                        PhotonNetwork.Destroy(item);
                        PhotonNetwork.Destroy(key);
                        PhotonNetwork.Destroy(sound);
                    }
                    else
                    {
                        Destroy(gameObject);
                        Destroy(item);
                        Destroy(key);
                        Destroy(sound);
                    }

                    PanelChange(false);
                    break;
                case (int)PauseItem.titleBack:
                    //�A�C�e���A�X�e�[�^�X����ۑ�
                    this.transform.GetComponent<ItemDataLoadOrSave>().SaveItemData(); 
                    this.transform.GetComponent<StartasLoadOrSave>().SaveStartasData();
                    
                    drawType = PauseItem.titleBack;
                    break;
                case (int)PauseItem.gameEnd:
                    //Destroy(gameObject);
                    // ���[���ɎQ�����Ă���ꍇ�͏���������
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.Destroy(item);
                        PhotonNetwork.Destroy(key);
                        PhotonNetwork.Destroy(sound);
                    }
                    else
                    {
                        Destroy(item);
                        Destroy(key);
                        Destroy(sound);
                    }

                    this.transform.GetComponent<ItemDataLoadOrSave>().SaveItemData();
                    this.transform.GetComponent<StartasLoadOrSave>().SaveStartasData();

                    drawType = PauseItem.gameEnd;
                    break;
            }
        }
        //�L�����Z������������
        else if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(openingClosingSound);

            if (!isPause)
            {
                item.SetActive(false);
                isPause = true;
            }
            else
            {
                // ���[���ɎQ�����Ă���ꍇ�͏���������
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Destroy(gameObject);
                    PhotonNetwork.Destroy(item);
                    PhotonNetwork.Destroy(key);
                    PhotonNetwork.Destroy(sound);
                }
                else
                {
                    Destroy(gameObject);
                    Destroy(item);
                    Destroy(key);
                    Destroy(sound);
                }

                PanelChange(false);
            }
        }
    }

    /// <summary>
    /// �A�C�e���p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void ItemDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.item;
            //�T�E���h��炷
            //�A�C�e����ʂ������Ȃ�����
            item.SetActive(false);
            //�|�[�Y��ʂɖ߂�
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// �X�e�[�^�X�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void StartasDrawUpdate()
    {
        if(Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.startas;
            startas.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// �L�[�R���t�B�O�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void KeyConfigDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.keyConfig;
            key.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// �T�E���h�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void SoundDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.sound;
            sound.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// �E�B���h�E���[�h�ύX�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void WindowDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�    �E�B���h�E�ύX�p�l���������Ȃ��Ƃ��|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame() || window.activeSelf == false)
        {
            selectNum = (int)PauseItem.window;
            window.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// �^�C�g���֖߂�
    /// </summary>
    private void TitleBackUpdate()
    {
        if (mainManager.IsChangeScene()) return;
        Time.timeScale = 1f;

        mainManager.ChangeScene("title");
    }

    /// <summary>
    /// �Q�[���I����I�񂾎��̃A�b�v�f�[�g
    /// </summary>
    private void GameEndUpdate()
    {
        if(mainManager.GameEnd())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }
    }

    /// <summary>
    /// �|�[�Y�p�l�����\���ɂ���Ƃ��ɕK�v�Ȃ���
    /// (true �|�[�Y��ʂ�\��������)
    /// (false �|�[�Y��ʂ��\���ɂ���)
    /// </summary>
    /// <param name="activ">true:�\�� false:��\��</param>
    public void PanelChange(bool activ)
    {
        if (activ)
        {
            Time.timeScale = 0f;

            selectNum = (int)PauseItem.back;//�ŏ��͖߂�̏ꏊ

            //�t���[���̈ʒu��߂�̈ʒu�ɂ���
            scalingScript.ScalingObjPosition(selectFrameObj.transform, new Vector3(800, 275/*-175*/, 0));

            //�I�����Ă��鍀�ڂ̐F��ς���
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    //�t���[���̈ʒu��߂�̈ʒu�ɂ���
                    //scalingScript.ScalingObjPosition(selectFrameObj.transform, text[i].transform.position);
                    //Debug.Log(text[i].transform.position);
                    //scalingScript.ScalingObjPosition(selectFrameObj.transform, new Vector3(0, text[i].transform.position.y, 0));
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }
        else if (!activ)
        {
            Time.timeScale = 1f;
            //isChange = true;
        }

        scalingScript.ScalingObj(selectFrameObj.transform);

        //���܂̕\����ԂƈႤ�Ƃ�
        if (pausePanel.activeSelf != activ)
        {
            audioSource.PlayOneShot(openingClosingSound);
            //�؂�ւ���
            pausePanel.SetActive(activ);
        }
    }

}
