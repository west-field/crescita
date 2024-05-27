using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �{�X�p�G�t�F�N�g
/// </summary>
public class BossParticle : MonoBehaviour
{
    [SerializeField]
    private GameObject particleObj;//�p�[�e�B�N���I�u�W�F�N�g
    private ParticleSystem particle;//�p�[�e�B�N��
    [SerializeField]
    private GameObject hitParticleObj;//�p�[�e�B�N���I�u�W�F�N�g
    private ParticleSystem hitParticle;//�U�����󂯂��Ƃ��̃p�[�e�B�N��

    private void Start()
    {
        particle = particleObj.GetComponent<ParticleSystem>();
        hitParticle = hitParticleObj.GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        var count = transform.childCount;
        //�q�I�u�W�F�N�g��0�̂Ƃ��������폜����
        if (count <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// �q�b�g�G�t�F�N�g�\���ʒu��ύX,��]�����ăG�t�F�N�g�𔭐�������
    /// </summary>
    /// <param name="hitPosition">���������ʒu</param>
    /// <param name="rot">��]</param>
    public void HitParticle(Vector3 hitPosition,Vector3 rot)
    {
        hitParticle.transform.position = hitPosition;
        hitParticle.transform.rotation = Quaternion.LookRotation(rot);

        //�G�t�F�N�g�𔭐�������
        hitParticle.Play();
    }

    /// <summary>
    /// ���ʂƂ��̃G�t�F�N�g
    /// </summary>
    public void ParticlePlay(Vector3 pos)
    {
        Destroy(hitParticleObj);
        particle.transform.position = pos;
        //�G�t�F�N�g�𔭐�������
        particle.Play();
    }
}
