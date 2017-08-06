using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapSkinAsset",menuName ="Game/MapSkin")]
public class MapSkinAsset : ScriptableObject
{
    public string skinName;
    public float snapInterval = 0.7f;

    public GameObject topLeftWall;
    public GameObject topWall;
    public GameObject topRightWall;
    public GameObject leftWall;
    public GameObject innerTopLeftWall;
    public GameObject innerTopRightWall;
    public GameObject innerBottomLeftWall;
    public GameObject innerBottomRightWall;
    public GameObject rightWall;
    public GameObject bottomLeftWall;
    public GameObject bottomWall;
    public GameObject bottomRightWall;
    public GameObject floor;
}
