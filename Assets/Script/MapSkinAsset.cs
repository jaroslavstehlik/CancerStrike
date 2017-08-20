using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapSkinAsset",menuName ="Game/MapSkin")]
public class MapSkinAsset : ScriptableObject
{
    public enum TileRuleType
    {
        WALL,
        FLOOR,
        BOTH
    }

    [System.Serializable]
    public struct TileRule
    {
        public TileRuleType topLeft;
        public TileRuleType top;
        public TileRuleType topRight;

        public TileRuleType left;
        public TileRuleType right;

        public TileRuleType bottomLeft;
        public TileRuleType bottom;
        public TileRuleType bottomRight;        
    }

    [System.Serializable]
    public class Tile
    {
        public string name;
        public TileRule[] rules;
        public GameObject prefab;
    }

    public string skinName;
    public float snapInterval = 0.7f;

    public Tile[] tiles;
    public GameObject[] floor;
}
