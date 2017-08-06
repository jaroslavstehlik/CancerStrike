using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public struct Sector
    {
        public int x;
        public int y;

        public CELL topLeft;
        public CELL top;
        public CELL topRight;

        public CELL left;
        public CELL center;
        public CELL right;

        public CELL bottomLeft;
        public CELL bottom;
        public CELL bottomRight;

        public Sector(int x, int y)
        {
            this.x = x;
            this.y = y;

            this.topLeft = this.top = this.topRight = this.left = this.center = this.right = this.bottomLeft = this.bottom = this.bottomRight = CELL.EMPTY;
        }
    }

    [System.Serializable]
    public class TeamData
    {
        public Vector2[] spawnPositions;
    }

    public string mapName;
    public string skin;
    public TeamData[] teamData;

    public enum CELL
    {
        EMPTY,
        WALL,
        FLOOR    
    }

    public int rows;
    public int columns;
    public byte[] mainLayer;

    public MapData(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        mainLayer = new byte[rows * columns];
    }

    public void Fill(CELL cell)
    {
        for(int i = 0; i < mainLayer.Length; i++)
        {
            mainLayer[i] = (byte)cell;
        }
    }

    public bool Contains(int x, int y)
    {
        if (x < 0 || x >= columns) return false;
        if (y < 0 || y >= rows) return false;
        return true;
    }

    public Sector GetSector(int x, int y)
    {
        Sector sector = new Sector();

        if (Contains(x - 1, y + 1)) sector.topLeft = this[x - 1, y + 1];
        if (Contains(x, y + 1)) sector.top = this[x, y + 1];
        if (Contains(x + 1, y + 1)) sector.topRight = this[x + 1, y + 1];

        if (Contains(x - 1, y)) sector.left = this[x - 1, y];
        if (Contains(x, y)) sector.center = this[x, y];
        if (Contains(x + 1, y)) sector.right = this[x + 1, y];

        if (Contains(x - 1, y - 1)) sector.bottomLeft = this[x - 1, y - 1];
        if (Contains(x, y - 1)) sector.bottom = this[x, y - 1];
        if (Contains(x + 1, y - 1)) sector.bottomRight = this[x + 1, y - 1];

        return sector;
    }

    public CELL this[int x, int y]
    {
        get
        {
            return (CELL)mainLayer[y * rows + x];
        }
        set
        {
            mainLayer[y * rows + x] = (byte)value;
        }
    }
    
    public static string Path(string mapName)
    {
        return Application.persistentDataPath + "/" + mapName + ".map";
    }

    public static void Save(MapData mapData)
    {
        Debug.Log(Path(mapData.mapName));
        string json = JsonUtility.ToJson(mapData);
        System.IO.File.WriteAllText(Path(mapData.mapName), json);
    }

    public static MapData Load(string mapName)
    {
        string path = Path(mapName);
        if(System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            if(!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonUtility.FromJson<MapData>(json);
                }
                catch { }
            }
        }

        return null;
    }
}
