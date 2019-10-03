using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorTile
{
    Blue = 0,
    Cyan =1,
    Green=2,
    Pink=3,
    Red=4,
    Yellow=5
};

public class TilesManager: MonoBehaviour
{

    public Material[] arrayMat;
    static public Dictionary<ColorTile, Material> ColorID = new Dictionary<ColorTile, Material>();

    Material FindMaterial(string matName)
    {
        Material tempMat = null;
        foreach (Material item in arrayMat)
        {
            if (item.name == matName)
            {
                tempMat = item;
                break;
            }

        }
        return tempMat;
    }

    private void Awake()
    {
        ColorID.Add(ColorTile.Blue, FindMaterial("Blue"));
        ColorID.Add(ColorTile.Cyan, FindMaterial("Cyan"));
        ColorID.Add(ColorTile.Green, FindMaterial("Green"));
        ColorID.Add(ColorTile.Pink, FindMaterial("Pink"));
        ColorID.Add(ColorTile.Red, FindMaterial("Red"));
        ColorID.Add(ColorTile.Yellow, FindMaterial("Yellow"));
    }

    static public ColorTile RandomPickColorID()
    {
        int max = ColorID.Count;
        int index = Random.Range(0, max);

        return (ColorTile)index;
    }

}
      
   




