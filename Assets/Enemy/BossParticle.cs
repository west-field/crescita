using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボス用エフェクト
/// </summary>
public class BossParticle : MonoBehaviour
{
    [SerializeField]
    private GameObject particleObj;//パーティクルオブジェクト
    private ParticleSystem particle;//パーティクル
    [SerializeField]
    private GameObject hitParticleObj;//パーティクルオブジェクト
    private ParticleSystem hitParticle;//攻撃を受けたときのパーティクル

    private void Start()
    {
        particle = particleObj.GetComponent<ParticleSystem>();
        hitParticle = hitParticleObj.GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        var count = transform.childCount;
        //子オブジェクトが0のとき自分を削除する
        if (count <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ヒットエフェクト表示位置を変更,回転させてエフェクトを発生させる
    /// </summary>
    /// <param name="hitPosition">当たった位置</param>
    /// <param name="rot">回転</param>
    public void HitParticle(Vector3 hitPosition,Vector3 rot)
    {
        hitParticle.transform.position = hitPosition;
        hitParticle.transform.rotation = Quaternion.LookRotation(rot);

        //エフェクトを発生させる
        hitParticle.Play();
    }

    /// <summary>
    /// 死ぬときのエフェクト
    /// </summary>
    public void ParticlePlay(Vector3 pos)
    {
        Destroy(hitParticleObj);
        particle.transform.position = pos;
        //エフェクトを発生させる
        particle.Play();
    }
}
