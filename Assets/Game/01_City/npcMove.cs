using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 町を歩くnpc
/// </summary>
public class npcMove : MonoBehaviour
{
    /// <summary>
    /// アニメーターを使ってアニメーションを変更する
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 目標地点
    /// </summary>
    [SerializeField] private Transform[] points;
    /// <summary>
    /// 最初の目的地
    /// </summary>
    private int destPoint = -1;

    /// <summary>
    /// 移動速度
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
        // 地点がなにも設定されていないときに返します
        if (points.Length == 0)
            return;

        // 配列内の次の位置を目標地点に設定し、
        // 必要ならば出発地点にもどります
        destPoint = (destPoint + 1) % points.Length;

        // エージェントが現在設定された目標地点に行くように設定します
        var diff = points[destPoint].position;
        diff.y = 0;

        this.transform.LookAt(points[destPoint].position);
    }

    private void FixedUpdate()
    {
        //移動方向
        var value = this.transform.forward;
        value.y = 0.0f;
        //移動量計算
        var vel = value.normalized * Time.deltaTime * speed;
        //移動量を足す
        this.transform.position += vel;;

        //ターゲットまでの距離を計算
        var rot = points[destPoint].position - this.transform.position;

        //大きさが1より小さいとき
        if (rot.magnitude < 1.0f)
        {
            //向く方向を変更する
            GotoNextPoint();
        }

        //アニメーションの切り替え
        animator.SetBool("move", true);
    }
}
