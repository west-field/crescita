using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��ʂɕ\������X�e�[�^�X
/// </summary>
public class PlayerStatasCanvasScript : MonoBehaviour
{
    /// <summary>
    /// ���x���\���e�L�X�g
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelText;
    /// <summary>
    /// ���x���A�b�v�|�C���g�\���e�L�X�g
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelPointText;

    //��r�̂��ߎ����Ă���
    private int level, levelPoint;

    //Hp�o�[��ύX������
    /// <summary>
    /// HP�o�[
    /// </summary>
    [SerializeField] private Image hpBars;

    /// <summary>
    /// ���݂�hp���擾���邽��
    /// </summary>
    private PlayerStartas startas;

    void Start()
    {
        levelText.text = "";
        levelPointText.text = "";
        level = 0;
        levelPoint = 0;

        var obje = GameObject.FindGameObjectsWithTag("Player");
        //�擾�����v���C���[������
        foreach(var ob in obje)
        {
            //�X�e�[�^�X��null�łȂ��Ƃ�
            if(ob.GetComponent<PlayerStartas>() != null)
            {
                //�擾����
                startas = ob.GetComponent<PlayerStartas>();
                Debug.Log("playerStartas ������");
                break;
            }
            Debug.Log("playerStartas �Ȃ�");
        }

        ////���x���\���ƍ��̃��x�����Ⴄ��
        //if (level != HoldVariable.startas.level)
        //{
        //    level = HoldVariable.startas.level;
        //    //���̃��x����\������
        //    levelText.text = $"{HoldVariable.startas.level}";
        //}

        //if (levelPoint != HoldVariable.startas.levelPoint)
        //{
        //    levelPoint = HoldVariable.startas.levelPoint;
        //    //�o���l���v�Z����
        //    var point = ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100) - HoldVariable.startas.levelPoint;

        //    //���x���A�b�v�̂��߂̃|�C���g��\������
        //    levelPointText.text = $"����{point}";
        //}
        
        //���x����\��������
        level = startas.GetStartas().level;
        //���̃��x����\������
        levelText.text = $"{startas.GetStartas().level}";

        //�o���l��\��������
        levelPoint = startas.GetStartas().levelPoint;
        //�o���l���v�Z����
        var point = ((startas.GetStartas().level - 1) * 100 + startas.GetStartas().level * 100) - startas.GetStartas().levelPoint;
        //���x���A�b�v�̂��߂̃|�C���g��\������
        levelPointText.text = $"����{point}";

    }

    private void FixedUpdate()
    {
        if(startas != null)
        {
            //���x���\���ƍ��̃��x�����Ⴄ��
            if (level != startas.GetStartas().level)
            {
                level = startas.GetStartas().level;
                //���̃��x����\������
                levelText.text = $"{startas.GetStartas().level}";
            }

            if (levelPoint != startas.GetStartas().levelPoint)
            {
                levelPoint = startas.GetStartas().levelPoint;
                //�o���l���v�Z����
                var point = ((startas.GetStartas().level - 1) * 100 + startas.GetStartas().level * 100) - startas.GetStartas().levelPoint;

                //���x���A�b�v�̂��߂̃|�C���g��\������
                levelPointText.text = $"����{point}";
            }

            hpBars.fillAmount = (float)startas.GetNowHp() / (float)startas.GetStartas().maxHp;
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
                    startas = ob.GetComponent<PlayerStartas>();
                    Debug.Log("������");
                    break;
                }
                Debug.Log("�Ȃ�");
            }
        }
    }
}
