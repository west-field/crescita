using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;

public class SettingPanelScript : MonoBehaviour
{
    /*�\���p�l��*/
    /// <summary>
    /// �|�[�Y�p�l��
    /// </summary>
    private GameObject pausePanel;
    /// <summary>
    /// �L�[�ݒ�\���p�l���v���n�u
    /// </summary>
    [SerializeField] private GameObject keyConfigPanelPrefab;
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
    private GameObject  key, sound, window;

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
        keyConfig,
        sound,
        window,
        back,

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
    private InputAction Navigate, Submit, Cancel, Pause;

    /// <summary>
    /// �|�[�Y��ʂ��ǂ���
    /// </summary>
    private bool isPause;

    //�g���̊g��k��
    [SerializeField] private Scaling scalingScript;

    private void Start()
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

        scalingScript.Init(1.1f, 0.9f);
        //�t���[���̈ʒu��߂�̈ʒu�ɂ���
        scalingScript.ScalingObjPosition(selectFrameObj.transform, text[(int)PauseItem.back].transform.position);

        //���g(�|�[�Y�p�l��)��\��
        PanelChange(true);

        key = Instantiate(keyConfigPanelPrefab, Vector3.zero, Quaternion.identity);//�L�[�R���t�B�O�p�l��
        key.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
        key.SetActive(false);//�ŏ��͌����Ȃ��悤��

        sound = Instantiate(soundPanelPrefab, Vector3.zero, Quaternion.identity);//�X�e�[�^�X�p�l��
        sound.transform.SetParent(canvas.transform, false);//Canvas�̎q�I�u�W�F�N�g�ɂ���
        sound.SetActive(false);//�ŏ��͌����Ȃ��悤��

        window = Instantiate(windowPanelPrefab, Vector3.zero, Quaternion.identity);//�E�B���h�E�ύX�p�l��
        window.transform.SetParent(canvas.transform, false);
        window.SetActive(false);

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
        //���ʂ��Ⴄ�Ƃ��͉��ʂ����킹��
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //���ܕ\�����Ă���(�I������)���̂̃A�b�v�f�[�g������
        switch (drawType)
        {
            case PauseItem.keyConfig:
                KeyConfigDrawUpdate();
                break;
            case PauseItem.sound:
                SoundDrawUpdate();
                break;
            case PauseItem.window:
                WindowDrawUpdate();
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
                    Debug.Log(text[i].transform.position);
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

                    PanelChange(false);
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
                isPause = true;
            }
            else
            {
                Destroy(key);
                Destroy(sound);
                Destroy(window);

                PanelChange(false);
            }
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
            scalingScript.ScalingObjPosition(selectFrameObj.transform, new Vector3(800, 205, 0));

            //�I�����Ă��鍀�ڂ̐F��ς���
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;

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
