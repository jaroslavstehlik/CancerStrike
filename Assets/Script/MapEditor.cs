﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public string mapName = "desert";
    public Camera camera;
    public int rows = 8;
    public int collumns = 8;    
    public MapSkinAsset skin;
    public NavMeshSurface navMeshSurface;
    MapData mapData;

    public MapData.CELL brush;
    GameObject[] grid;
    int pointerId = -1;
    PointerEventData pointerEventData;
    
    private void Awake()
    {
        mapData = MapData.Load(mapName);
        if (mapData == null)
        {
            mapData = new MapData(rows, collumns);
            mapData.mapName = mapName;            
        }
        mapData.skin = skin.skinName;
        grid = new GameObject[rows * collumns];
        mapData.Fill(MapData.CELL.EMPTY);

        Refresh();
        navMeshSurface.BuildNavMesh();
    }

    private void OnEnable()
    {
        pointerId = -1;
    }

    private void Update()
    {
        if(pointerId != -1)
        {
            Place(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    private void OnDestroy()
    {
        MapData.Save(mapData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerId != -1) return;
#if UNITY_STANDALONE
        pointerId = 0;
#else
        pointerId = eventData.pointerId;
#endif
        pointerEventData = eventData;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
#if !UNITY_STANDALONE        
        if (pointerId != eventData.pointerId) return;
#endif

        pointerId = -1;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    private void Place(Vector2 position)
    {
        Vector2 localPosition = transform.InverseTransformPoint(camera.ScreenToWorldPoint(position));
        
        int x = Mathf.RoundToInt(localPosition.x / skin.snapInterval);
        int y = Mathf.RoundToInt(localPosition.y / skin.snapInterval);
        
        x = Mathf.Clamp(x, 0, rows - 1);
        y = Mathf.Clamp(y, 0, collumns - 1);

        PlaceFloor(x, y);

        Refresh();
    }

    void PlaceFloor(int x, int y)
    {
        mapData[x, y] = MapData.CELL.FLOOR;
        if(mapData.Contains(x - 1, y + 1) && mapData[x - 1, y + 1] != MapData.CELL.FLOOR) mapData[x - 1, y + 1] = MapData.CELL.WALL;
        if (mapData.Contains(x, y + 1) && mapData[x, y + 1] != MapData.CELL.FLOOR) mapData[x, y + 1] = MapData.CELL.WALL;
        if (mapData.Contains(x + 1, y + 1) && mapData[x + 1, y + 1] != MapData.CELL.FLOOR) mapData[x + 1, y + 1] = MapData.CELL.WALL;
        
        if (mapData.Contains(x - 1, y) && mapData[x - 1, y] != MapData.CELL.FLOOR) mapData[x - 1, y] = MapData.CELL.WALL;
        if (mapData.Contains(x, y) && mapData[x, y] != MapData.CELL.FLOOR) mapData[x, y] = MapData.CELL.WALL;
        if (mapData.Contains(x + 1, y) && mapData[x + 1, y] != MapData.CELL.FLOOR) mapData[x + 1, y] = MapData.CELL.WALL;

        if (mapData.Contains(x - 1, y - 1) && mapData[x - 1, y - 1] != MapData.CELL.FLOOR) mapData[x - 1, y - 1] = MapData.CELL.WALL;
        if (mapData.Contains(x, y - 1) && mapData[x, y - 1] != MapData.CELL.FLOOR) mapData[x, y - 1] = MapData.CELL.WALL;
        if (mapData.Contains(x + 1, y - 1) && mapData[x + 1, y - 1] != MapData.CELL.FLOOR) mapData[x + 1, y - 1] = MapData.CELL.WALL;        
    }
    
    void Refresh()
    {
        ClearMap();
        CreateMap();
    }

    void ClearMap()
    {
        for (int i = 0; i < grid.Length; i++)
        {

            if (grid[i] != null) Destroy(grid[i]);
        }
    }

    void CreateMap()
    {
        float offset = skin.snapInterval;
        byte[] mainLayer = mapData.mainLayer;
        for (int i = 0; i < mainLayer.Length; i++)
        {
            int x = i % collumns;
            int y = Mathf.FloorToInt(i / collumns);

            MapData.CELL cell = (MapData.CELL)mainLayer[i];
            GameObject prefab = GetSkinPrefab(cell, x, y);
            if(prefab != null)
            {
                grid[i] = Instantiate<GameObject>(prefab, transform);
                grid[i].transform.localPosition = new Vector3((float)x * offset, (float)y * offset, 0f);
            }            
        }
    }

    GameObject GetSkinPrefab(MapData.CELL cell, int x, int y)
    {
        switch(cell)
        {
            case MapData.CELL.FLOOR:
                return skin.floor;
            case MapData.CELL.WALL:
                return GetWallPrefab(x, y);
        }

        return null;
    }

    GameObject GetWallPrefab(int x, int y)
    {
        
        GameObject go = null;
        MapData.Sector sector = mapData.GetSector(x, y);
        
        if( sector.top != MapData.CELL.FLOOR &&
            sector.left != MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.WALL &&
            sector.bottomRight == MapData.CELL.FLOOR
            )
        {
            return skin.topLeftWall;
        }

        if(
            sector.top != MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.FLOOR
            )
        {
            return skin.topWall;
        }

        if (
            sector.top != MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right != MapData.CELL.FLOOR &&
            sector.bottom == MapData.CELL.WALL &&
            sector.bottomLeft == MapData.CELL.FLOOR
            )
        {
            return skin.topRightWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.left != MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.FLOOR &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.leftWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.EMPTY &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.rightWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.topRight == MapData.CELL.FLOOR &&
            sector.left != MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom != MapData.CELL.FLOOR
            )
        {
            return skin.bottomLeftWall;
        }

        if (
            sector.top == MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom != MapData.CELL.FLOOR
            )
        {
            return skin.bottomWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.topLeft == MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right != MapData.CELL.FLOOR &&
            sector.bottom != MapData.CELL.FLOOR
            )
        {
            return skin.bottomRightWall;
        }

        if (
            sector.top == MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.innerTopLeftWall;
        }

        if (
            sector.top == MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.bottomWall;
        }

        if (
            sector.top == MapData.CELL.FLOOR &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.FLOOR &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.innerTopRightWall;
        }
        
        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.rightWall;
        }
        
        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.FLOOR &&
            sector.bottom == MapData.CELL.WALL
            )
        {
            return skin.leftWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.FLOOR &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.FLOOR
            )
        {
            return skin.innerBottomLeftWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.WALL &&
            sector.bottom == MapData.CELL.FLOOR
            )
        {
            return skin.topWall;
        }

        if (
            sector.top == MapData.CELL.WALL &&
            sector.left == MapData.CELL.WALL &&
            sector.center == MapData.CELL.WALL &&
            sector.right == MapData.CELL.FLOOR &&
            sector.bottom == MapData.CELL.FLOOR
            )
        {
            return skin.innerBottomRightWall;
        }

        return go;
    }

    float Round(float number, float interval)
    {
        return (float)(Mathf.RoundToInt(number / interval)) * interval;
    }


    Vector3 RoundVector(Vector3 vector, float interval)
    {
        return new Vector3(Round(vector.x, interval), Round(vector.y, interval), Round(vector.z, interval));
    }    
}
