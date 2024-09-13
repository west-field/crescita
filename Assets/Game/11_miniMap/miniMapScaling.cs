using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �~�j�}�b�v�̊g��k��
/// </summary>
public class miniMapScaling : MonoBehaviour
{
    //�J�������擾����
    private GameObject mainCamera;

    private MainManager mainManager;
    /// <summary>
    /// �A�N�V����
    /// </summary>
    private InputAction map;

    /// <summary>
    /// �g�債�Ă��邩
    /// </summary>
    private bool isExpansion;

    /// <summary>
    /// �g��k������I�u�W�F�N�g
    /// </summary>
    [SerializeField] private Transform miniMapObj;
    /// <summary>
    /// ���߂̈ʒu�A�g��l���擾���Ă���
    /// </summary>
    private Vector3 startPos;
    [SerializeField] private Transform expansionPos;

    /// <summary>
    /// �g�嗦��ݒ�
    /// </summary>
    private float scale;

    void Start()
    {
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        map = mainManager.GetPlayerInput().actions["map"];

        isExpansion = false;

        startPos = miniMapObj.position;
        scale = 2.5f;

        var obj = GameObject.FindGameObjectsWithTag("Player");
        //�擾�����v���C���[������
        foreach (var ob in obj)
        {
            //�X�e�[�^�X��null�łȂ��Ƃ�
            if (ob.GetComponent<PlayerStartas>() != null)
            {
                //�擾����
                mainCamera = ob;
                Debug.Log("minimap ������");
                break;
            }
            Debug.Log("minimap �Ȃ�");
        }
    }

    private void Update()
    {
        //�����Ă���ԃ}�b�v���g�傷��
        if (map.IsPressed())
        {
            if(!isExpansion)
            {
                Debug.Log("�}�b�v�g��");
                isExpansion = true;
                miniMapObj.position = expansionPos.position;
                miniMapObj.localScale = new Vector3(scale, scale, scale);
            }
        }
        else if (isExpansion)
        {
            Debug.Log("�}�b�v�k��");
            isExpansion = false;
            miniMapObj.position = startPos;
            miniMapObj.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        if (mainCamera != null)
        {
            this.transform.position = new Vector3(mainCamera.transform.position.x, this.transform.position.y, mainCamera.transform.position.z);
        }
        else
        {
            //Debug.Log("�Ȃ�");
            var obj = GameObject.FindGameObjectsWithTag("Player");

            //�擾�����v���C���[������
            foreach (var ob in obj)
            {
                //�X�e�[�^�X��null�łȂ��Ƃ�
                if (ob.GetComponent<PlayerStartas>() != null)
                {
                    //�擾����
                    mainCamera = ob;
                    Debug.Log("������");
                    break;
                }
                Debug.Log("�Ȃ�");
            }
        }
    }
}
