using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ミニマップの拡大縮小
/// </summary>
public class miniMapScaling : MonoBehaviour
{
    //カメラを取得する
    private GameObject mainCamera;

    private MainManager mainManager;
    /// <summary>
    /// アクション
    /// </summary>
    private InputAction map;

    /// <summary>
    /// 拡大しているか
    /// </summary>
    private bool isExpansion;

    /// <summary>
    /// 拡大縮小するオブジェクト
    /// </summary>
    [SerializeField] private Transform miniMapObj;
    /// <summary>
    /// 初めの位置、拡大値を取得しておく
    /// </summary>
    private Vector3 startPos;
    [SerializeField] private Transform expansionPos;

    /// <summary>
    /// 拡大率を設定
    /// </summary>
    private float scale;

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();
        map = mainManager.GetPlayerInput().actions["map"];

        isExpansion = false;

        startPos = miniMapObj.position;
        scale = 2.5f;

        var obj = GameObject.FindGameObjectsWithTag("Player");
        //取得したプレイヤーを見る
        foreach (var ob in obj)
        {
            //ステータスがnullでないとき
            if (ob.GetComponent<PlayerStartas>() != null)
            {
                //取得する
                mainCamera = ob;
                Debug.Log("minimap あった");
                break;
            }
            Debug.Log("minimap ない");
        }
    }

    private void Update()
    {
        //押している間マップを拡大する
        if (map.IsPressed())
        {
            if(!isExpansion)
            {
                Debug.Log("マップ拡大");
                isExpansion = true;
                miniMapObj.position = expansionPos.position;
                miniMapObj.localScale = new Vector3(scale, scale, scale);
            }
        }
        else if (isExpansion)
        {
            Debug.Log("マップ縮小");
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
            //Debug.Log("ない");
            var obj = GameObject.FindGameObjectsWithTag("Player");

            //取得したプレイヤーを見る
            foreach (var ob in obj)
            {
                //ステータスがnullでないとき
                if (ob.GetComponent<PlayerStartas>() != null)
                {
                    //取得する
                    mainCamera = ob;
                    Debug.Log("あった");
                    break;
                }
                Debug.Log("ない");
            }
        }
    }
}
