using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 草につけるスクリプト
/// </summary>
public class bushScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject bush;

    private bool isStart;

    private int count;//子オブジェクトがあるかどうかを数える

    private int rand;//ランダムにアイテムを選ぶ

    void Start()
    {
        isStart = false;
    }

    private void FixedUpdate()
    {
        count = transform.childCount;
        //子オブジェクトが0のとき自分を削除する
        if(count <= 0)
        {
            switch(rand)
            {
                case 0:
                    // CubeプレハブをGameObject型で取得
                    GameObject lotusLeaf = (GameObject)Resources.Load("lotusLeaf");
                    Instantiate(lotusLeaf, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + lotusLeaf.transform.position.y, this.gameObject.transform.position.z), lotusLeaf.transform.localRotation);

                    break;
                case 1:
                    // CubeプレハブをGameObject型で取得
                    GameObject scarabSwallowtail = (GameObject)Resources.Load("scarabSwallowtail");
                    Instantiate(scarabSwallowtail, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + scarabSwallowtail.transform.position.y, this.gameObject.transform.position.z), scarabSwallowtail.transform.localRotation);

                    break;
                case 2:
                    // CubeプレハブをGameObject型で取得
                    GameObject gold = (GameObject)Resources.Load("gold");
                    Instantiate(gold, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + gold.transform.position.y, this.gameObject.transform.position.z), gold.transform.localRotation);

                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }
    }

    public void RandomItem()
    {
        rand = Random.Range(0, 100) % 4;
    }

    //weapon

    private void OnTriggerEnter(Collider other)
    {
        if (isStart) return;
        //武器に当たった時
        if(other.gameObject.tag == "weapon")
        {
            RandomItem();
            particle.Play();
            isStart = true;
            if (PhotonNetwork.InRoom) 
            {
                //フォトンビューがあるかどうか
                if (this.gameObject.GetComponent<PhotonView>())
                {
                    Debug.Log("ブッシュ削除");
                    this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.transform.root.GetComponent<PhotonView>().Owner);
                    Destroy(bush);
                }
            }
            else
            {
                Destroy(bush);
            }
        }
    }
}
