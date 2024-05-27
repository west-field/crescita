using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���C���}�l�[�W���[
/// </summary>
public class MainManager : MonoBehaviour
{
    /// <summary>
    /// PlayerInput�̎擾
    /// </summary>
    [SerializeField] private PlayerInput input;

    /// <summary>
    /// �t�F�[�h�p�l��
    /// </summary>
    [SerializeField] private GameObject panelfade;

    /// <summary>
    /// �t�F�[�h�p�l���̓����x��ς��邽��
    /// </summary>
    private Image fadealpha;

    /// <summary>
    /// �p�l���̃A���t�@�l
    /// </summary>
    private float alpha;

    /// <summary>
    /// �t�F�[�h�X�s�[�h
    /// </summary>
    private float fadeSpeed;

    /// <summary>
    /// �t�F�[�h�C���t���O
    /// </summary>
    private bool fadein;
    /// <summary>
    /// �t�F�[�h�A�E�g�t���O
    /// </summary>
    private bool fadeout;

    /// <summary>
    /// ���̃V�[����
    /// </summary>
    private string nextSceneName;

    /// <summary>
    /// �T�E���h
    /// </summary>
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    private float fadeSeconds;

    /// <summary>
    /// �I�����C�����I�t���C�������w�肷��
    /// </summary>
    private OfflineMode offlineMode;

    /// <summary>
    /// ���[�h�̐i���󋵂�\������UI�Ȃ�
    /// </summary>
    [SerializeField]�@private GameObject loadingUI;

    /// <summary>
    /// ���[�h�̐i���󋵂��Ǘ����邽�߂̕ϐ�
    /// </summary>
    private AsyncOperation async;

    /// <summary>
    /// �V�[�������[�h���Ă��邩�ǂ���
    /// </summary>
    private bool isSceneLoading;

    /// <summary>
    /// ���[�h��ʎ��\���ł���摜�̖���
    /// </summary>
    [SerializeField] private int randMax = 5;

    /// <summary>
    /// �V�[���ύX���ɐ�������UI
    /// </summary>
    private GameObject map, button, startas;

    private void Awake()
    {
        //�T�[�o�[�ɐڑ��@or�@�I�t���C�����[�h��L���ɂ���
        offlineMode = this.gameObject.GetComponent<OfflineMode>();
        offlineMode.Connect();

        //���[�h�摜�����߂�
        int rand = Random.Range(0, 100) % randMax;
        //�摜��Resources����擾����
        Sprite sprite = Resources.Load<Sprite>($"roding_{rand}");
        //�摜��\������UI���擾����
        Image image = loadingUI.GetComponent<Image>();
        //�摜�������ւ���
        image.sprite = sprite;

        //���[�h�摜�͌����Ȃ��悤�ɂ���
        loadingUI.SetActive(false);
    }

    private void Start()
    {
        //�C���[�W���擾
        fadealpha = panelfade.GetComponent<Image>();
        //�����x��1�ɂ��Ă���
        alpha = 1.0f;
        //�t�F�[�h�X�s�[�h
        fadeSpeed = 0.05f;

        //���߂̓t�F�[�h�C��
        fadein = true;
        fadeout = false;

        //���̃V�[���� title�����Ă���
        nextSceneName = "title";

        //���ʂ�0
        audioSource.volume = 0;
        //���ʂ̃t�F�[�h�̑��x    �ő剹�ʂ����l�̃t�F�[�h�ɂ����鎞�ԂŊ���
        fadeSeconds = HoldVariable.BGMVolume / (alpha / fadeSpeed);
        Debug.Log(fadeSeconds);

        isSceneLoading = false;

        //���̃V�[������ title �܂��� selectScene �ł͂Ȃ��Ƃ�����������
        if (NowGameScene() != "title" && NowGameScene() != "selectScene")
        {
            //�~�j�}�b�v���쐬
            GameObject mapPrefab = (GameObject)Resources.Load("minMap");
            this.map = Instantiate(mapPrefab);

            //�{�^�����쐬
            GameObject buttonPrefab = (GameObject)Resources.Load("ButtonCanvas");
            this.button = Instantiate(buttonPrefab);

            //�X�e�[�^�X���쐬
            GameObject startasPrefab = (GameObject)Resources.Load("PlayerStatasCanvas");
            this.startas = Instantiate(startasPrefab);
        }

        Canvas(false);
    }

    /// <summary>
    /// UI�̕\����\����ύX����
    /// </summary>
    /// <param name="active">true:�\�� false:��\��</param>
    private void Canvas(bool active)
    {
        //���̃V�[������ title �܂��� selectScene �̎���UI�𐶐����Ă��Ȃ��̂ŏ������I���
        if (NowGameScene() == "title" || NowGameScene() == "selectScene")
        {
            return;
        }
        
        map.SetActive(active);
        button.SetActive(active);
        startas.SetActive(active);
    }

    private void FixedUpdate()
    {
        //�t�F�[�h�C��
        if (fadein)
        {
            FadeIn();
        }
        //�t�F�[�h�A�E�g
        else if (fadeout)
        {
            FadeOut();
        }
        //�E�B���h�E���[�h��ύX����
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    /// <summary>
    /// �񓯊��ǂݍ��݂�����
    /// </summary>
    public void StartLoad()
    {
        StartCoroutine(Load());
    }

    /// <summary>
    /// ���[�h�摜��\��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {
        //���[�h��ʂ�\������
        loadingUI.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        //�V�[�������[�h���Ă��Ȃ��Ƃ�
        if (!isSceneLoading)
        {
            isSceneLoading = true;
            //�V�[����񓯊��Ń��[�h����
            async = SceneManager.LoadSceneAsync(nextSceneName);
            Debug.Log(nextSceneName + "�V�[�������[�h");
        }

        //���[�h����������܂őҋ@����
        while (!async.isDone)
        {
            yield return null;
        }

        //���[�h��ʂ��\���ɂ���
        loadingUI.SetActive(false);
    }

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    private void FadeIn()
    {
        //�����x��ύX
        alpha -= fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        //�p�l���̕\����\����ύX
        ChangePanelEnabled();

        //���̃t�F�[�h(�傫������)�@���݂̃{�����[�����ő剹�ʂ����������Ƃ�
        if (audioSource.volume < HoldVariable.BGMVolume)
        {
            audioSource.volume += fadeSeconds;
        }

        if (alpha <= 0)
        {
            //���ʂ��ő剹�ʂɂ���
            audioSource.volume = HoldVariable.BGMVolume;
            Debug.Log($"���ʁF{audioSource.volume}");
            //�t�F�[�h�C�����I���
            fadein = false;
            //UI���\���ɂ���
            Canvas(true);
            return;
        }
    }

    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    private void FadeOut()
    {
        //�����x��ύX
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        //�p�l���̕\����\����ύX
        ChangePanelEnabled();

        //����������
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }

        if (alpha >= 1)
        {
            //���ʂ�0�ɂ���
            audioSource.volume = 0;
            
            //�t�F�[�h�A�E�g���I���
            fadeout = false;

            //�ڑ���f��
            offlineMode.Disconnect();

            //UI���\���ɂ���
            Canvas(false);

            //�V�[����ύX����
            StartLoad();
            return;
        }
    }

    /// <summary>
    /// �I�����C���ŃV�[����ύX����Ƃ��Ɏg�p
    /// </summary>
    /// <returns>true:�t�F�[�h�A�E�g���I����� false:�t�F�[�h�A�E�g���I����ĂȂ�</returns>
    public bool OnlineFadeOut()
    {
        //�����x��ύX
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        ChangePanelEnabled();

        //����������
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }
        if (alpha >= 1)
        {
            //���ʂ�0�ɂ���
            audioSource.volume = 0;
            
            //�t�F�[�h�A�E�g���I���
            fadeout = false;
           
            //UI���\���ɂ���
            Canvas(false);

            return true;
        }
        return false;
    }

    /// <summary>
    /// �Q�[�����I���Ƃ��Ɏg�p����
    /// </summary>
    /// <returns>true �I����Ă����v: fasle �I�������_��</returns>
    public bool GameEnd()
    {
        //���l�ύX
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        ChangePanelEnabled();

        //����������
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }

        if (alpha >= 1)
        {
            //�t�F�[�h�A�E�g���I���
            fadeout = false;

            //�ڑ���f��
            offlineMode.Disconnect();

            return true;
        }
        return false;
    }

    /// <summary>
    /// �t�F�[�h�p�p�l���̕\����\���̕ύX
    /// </summary>
    private void ChangePanelEnabled()
    {
        //���l��0�ȉ��̂Ƃ��͔�\��
        if (alpha <= 0)
        {
            //�\�����Ă��鎞�͔�\���ɕς���
            if(panelfade.activeSelf)
            {
                panelfade.SetActive(false);
            }
        }
        //���l��0�����傫���Ƃ��͕\��
        else
        {
            //��\���ɂ��Ă��鎞�͕\���ɕς���
            if(!panelfade.activeSelf)
            {
                panelfade.SetActive(true);
            }
        }
    }

    /// <summary>
    /// �C���v�b�g�V�X�e�����擾����
    /// </summary>
    /// <returns>PlayerInput</returns>
    public PlayerInput GetPlayerInput()
    {
        return input;
    }

    /// <summary>
    /// �V�[����ύX����
    /// </summary>
    /// <param name="name">�ύX�������V�[���̖��O</param>
    public void ChangeScene(string name)
    {
        //�t�F�[�h�A�E�g���Ă��鎞�̓V�[������ύX�����I������
        if (fadeout) return;

        Debug.Log(name +"�ɕύX");

        //�V�[������ύX
        this.nextSceneName = name;

        //�t�F�[�h�A�E�g���J�n����
        fadeout = true;
        //�����x��0�ɂ���
        alpha = 0;
    }

    /// <summary>
    /// �V�[����ύX���Ă��邩�ǂ���
    /// </summary>
    /// <returns>true �ύX���Ă���: false �ύX���Ă��Ȃ�</returns>
    public bool IsChangeScene()
    {
        //�t�F�[�h�A�E�g���n�߂��玟�̃V�[���ɕύX���邩��fadeout��Ԃ�
        return fadeout;
    }

    /// <summary>
    /// ���݂̃Q�[���V�[���̖��O
    /// </summary>
    /// <returns>�Q�[���V�[����</returns>
    public string NowGameScene()
    {
        return SceneManager.GetActiveScene().name;
    }

}
