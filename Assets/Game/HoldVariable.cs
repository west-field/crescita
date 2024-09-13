using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldVariable : MonoBehaviour
{
    public static Vector3 playerPosision { get; set; }//�v���C���[�̈ʒu
    public static Quaternion playerRotate { get; set; }//�v���C���[�̉�]

    public static string playerName { get; set; }//�v���C���[�̃j�b�N�l�[��

    public static float BGMVolume { get; set; }//�T�E���h�̉���
    public static float SEVolume { get; set; }//�T�E���h�̉���

    public static List<ItemData> itemDataList = new List<ItemData>();

    [SerializeField] private ItemDataBase itemDataBase;
    public static string id;

    public static bool isDeathBoss;

    /*�U���́A�h��́A���x��*/
    public struct Startas
    {
        public int maxHp;//���g�̍ő�HP
        public int attack;//�U����
        public int defense;//�h���
        public int weponAttack;//����
        public int armorDefense;//�h��
        public int level;//���x��
        public int levelPoint;//���x���|�C���g
    };
    public static Startas startas;

    private void Awake()
    {
        //���ʂ𔼕���
        BGMVolume = 0.5f;
        SEVolume = 0.5f;
    }

    void Start()
    {
        playerPosision = Vector3.zero;

        playerRotate = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        foreach (var item in itemDataBase.GetItemLists())
        {
            ItemData itemdata = new ItemData(item.Id, 0);
            itemDataList.Add(itemdata);
        }
        isDeathBoss = false;
    }
}
