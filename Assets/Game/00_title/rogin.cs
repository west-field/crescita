using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using TMPro;

/// <summary>
/// ���O�C��
/// </summary>
public class rogin : MonoBehaviour
{
    //�z�X�g
    const string hostUrl = @"http://saya-2003.moo.jp/AccountDB/";//@"http://localhost/Login/";//

    //��M�pURL

    //���O�C��URL
    const string loginUrl = @"loginAccount.php";

    //�쐬���邽�߂�URL
    const string crreateUrl = @"createAccountInfo.php";

    [SerializeField] private TextMeshProUGUI nextText;

    /// <summary>
    /// ���O�C���ł��đ��̃f�[�^���擾�ł������ǂ���
    /// </summary>
    private bool isLogin;

    /// <summary>
    /// ���O�C���ł�����
    /// </summary>
    private bool isLoginData;

    /// <summary>
    /// �f�[�^�̃��[�h�����邽��
    /// </summary>
    private ItemDataLoadOrSave itemData;
    /// <summary>
    /// �X�e�[�^�X�����[�h���邽��
    /// </summary>
    private StartasLoadOrSave startas;

    void Start()
    {
        isLogin = false;
        isLoginData = false;

        itemData = this.GetComponent<ItemDataLoadOrSave>();
        startas = this.GetComponent<StartasLoadOrSave>();
    }

    private void Update()
    {
        if (isLoginData)
        {
            if (itemData.IsLoad())
            {
                isLogin = true;
            }
        }
    }

    /// <summary>
    /// ���O�C������
    /// </summary>
    /// <param name="name">���O</param>
    /// <param name="pass">�p�X���[�h</param>
    /// <returns></returns>
    public bool Login(string name,string pass)
    {
        StartCoroutine(LoginRequest(hostUrl, loginUrl, name, pass));
        return isLogin;
    }

    /// <summary>
    /// �쐬����
    /// </summary>
    /// <param name="name">���O</param>
    /// <param name="pass">�p�X���[�h</param>
    /// <returns></returns>
    public bool Create(string name, string pass)
    {
        StartCoroutine(CreateRequest(hostUrl,crreateUrl, name, pass));
        return isLogin;
    }

    //Post�ʐM�@���O�C��
    IEnumerator LoginRequest(string host,string postUrl,string name,string pass)
    {
        //Post��URL�݂̂̒ʐM�ł͂Ȃ��AURL�ɑ΂���Form��n�����ƂŒʐM����
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("pass", pass);

        string url = host + postUrl;

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);

        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
        }
        else
        {
            //�ʐM����
            Debug.Log(postRequest.downloadHandler.text);
            var str = postRequest.downloadHandler.text;

            //�����Ă����e�L�X�g��'\n'�ŋ�؂�
            string[] splitText = str.Split('\n');

            nextText.text = splitText[0];
            if (splitText[0] == "�S�Ĉ�v")
            {
                Debug.Log("��v");
                HoldVariable.id = splitText[1];

                //�A�C�e���f�[�^���擾����
                itemData.GetItemData();
                //�X�e�[�^�X���擾����
                startas.GetStartasData();

                nextText.text = "���O�C������";
                
                isLoginData = true;
            }
        }
    }

    //Post�ʐM�@�A�J�E���g���쐬����
    IEnumerator CreateRequest(string host, string postUrl, string name, string pass)
    {
        //Post��URL�݂̂̒ʐM�ł͂Ȃ��AURL�ɑ΂���Form��n�����ƂŒʐM����
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("pass", pass);

        string url = host + postUrl;

        using UnityWebRequest postRequest = UnityWebRequest.Post(url, form);

        yield return postRequest.SendWebRequest();

        //�ʐM�G���[����
        if (postRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            //�ʐM���s�@�G���[���e��\��
            Debug.Log(postRequest.error);
        }
        else
        {
            //�ʐM�����@���X�|���X��\��
            Debug.Log(postRequest.downloadHandler.text);

            var str = postRequest.downloadHandler.text;
            string[] splitText = str.Split('\n');

            
            if (splitText[0] == "�쐬")
            {
                nextText.text = "�쐬";

                HoldVariable.id = splitText[1];

                ItemDataLoadOrSave itemData = GameObject.Find("Manager").GetComponent<ItemDataLoadOrSave>();
                itemData.GetItemData();
                StartasLoadOrSave startas = GameObject.Find("Manager").GetComponent<StartasLoadOrSave>();
                startas.GetStartasData();

                isLogin = true;
            }

            if (splitText[0] == "�������O������܂�")
            {
                nextText.text = "�������O������܂��B�ʂ̖��O�ɕύX���Ă�������";
                isLogin = false;
            }
        }
    }
}
