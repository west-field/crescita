using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������npc
/// </summary>
public class npcMove : MonoBehaviour
{
    /// <summary>
    /// �A�j���[�^�[���g���ăA�j���[�V������ύX����
    /// </summary>
    private Animator animator;

    /// <summary>
    /// �ڕW�n�_
    /// </summary>
    [SerializeField] private Transform[] points;
    /// <summary>
    /// �ŏ��̖ړI�n
    /// </summary>
    private int destPoint = -1;

    /// <summary>
    /// �ړ����x
    /// </summary>
    private float speed;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        
        speed = 3.5f;

        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        // �n�_���Ȃɂ��ݒ肳��Ă��Ȃ��Ƃ��ɕԂ��܂�
        if (points.Length == 0)
            return;

        // �z����̎��̈ʒu��ڕW�n�_�ɐݒ肵�A
        // �K�v�Ȃ�Ώo���n�_�ɂ��ǂ�܂�
        destPoint = (destPoint + 1) % points.Length;

        // �G�[�W�F���g�����ݐݒ肳�ꂽ�ڕW�n�_�ɍs���悤�ɐݒ肵�܂�
        var diff = points[destPoint].position;
        diff.y = 0;

        this.transform.LookAt(points[destPoint].position);
    }

    private void FixedUpdate()
    {
        //�ړ�����
        var value = this.transform.forward;
        value.y = 0.0f;
        //�ړ��ʌv�Z
        var vel = value.normalized * Time.deltaTime * speed;
        //�ړ��ʂ𑫂�
        this.transform.position += vel;;

        //�^�[�Q�b�g�܂ł̋������v�Z
        var rot = points[destPoint].position - this.transform.position;

        //�傫����1��菬�����Ƃ�
        if (rot.magnitude < 1.0f)
        {
            //����������ύX����
            GotoNextPoint();
        }

        //�A�j���[�V�����̐؂�ւ�
        animator.SetBool("move", true);
    }
}
