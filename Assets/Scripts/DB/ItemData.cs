using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TEMARI.DB
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ItemData")]
    public class ItemData : ScriptableObject
    {
        /// <summary> アイテムリスト </summary>
        public IReadOnlyCollection<Item> ItemList => _itemList;
        [SerializeField] private List<Item> _itemList = new();


        public void InitList()
        {
            _itemList = new List<Item>
            {
                new Item(ItemType.Item, "アイテム1", "アイテム1\n説明文", 1), new Item(ItemType.Item, "アイテム2", "アイテム2\n説明文", 2),
                new Item(ItemType.Item, "アイテム3", "アイテム3\n説明文", 3), new Item(ItemType.Item, "アイテム4", "アイテム4\n説明文", 4)
            };
        }

        /// <summary>
        /// アイテム使用
        /// </summary>
        /// <param name="name"></param>
        public void UseItem(string name)
        {
            var item = FindItemByName(name);
            if(item.Type == ItemType.Item)
            {
                if(item.Possession > 0)
                {
                    item.Possession -= 1;
                    if (item.Possession == 0)
                    {

                    }
                }
            }
            else if(item.Type == ItemType.Gear)
            {
                ;
            }
        }

        /// <summary>
        /// アイテム名からリスト検索
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Item FindItemByName(string name)
        {
            return _itemList.FirstOrDefault(x =>  x.Name == name);
        }
    }

    /// <summary>
    /// アイテムのタイプ
    /// </summary>
    public enum ItemType
    {
        Item = 0,
        Gear
    }

    /// <summary>
    /// アイテムクラス
    /// </summary>
    [System.Serializable]
    public class Item
    {
        [SerializeField] private ItemType _type;
        /// <summary> アイテムタイプ </summary>
        public ItemType Type 
        { 
            get {  return _type; }
            private set {  _type = value; }
        }

        [SerializeField] private string _name;
        /// <summary> アイテム名 </summary>
        public string Name 
        {
            get { return _name; }
            private set { _name = value; }
        }

        [SerializeField] private string _description;
        /// <summary> アイテムの説明 </summary>
        public string Description 
        {
            get { return _description; } 
            private set { _description = value; }
        }

        [SerializeField] private int _possession;
        /// <summary> 所持数 </summary>
        public int Possession
        {
            get { return _possession; }
            set { _possession = Mathf.Clamp(value, 0, MaxPossession); }
        }

        /// <summary> 最大所持数 </summary>
        public static readonly int MaxPossession = 99;

        public Item(ItemType type, string name, string description, int possession)
        {
            _type = type;
            _name = name;
            _description = description;
            _possession = possession;
        }

        public override string ToString()
        {
            return $"type:{_type}, name:{_name}, description:{_description}, possession:{_possession}";
        }
    }
}