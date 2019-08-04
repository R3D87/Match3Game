using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour {

    

    // Use this for initialization
    [SerializeField]
    int positionX;
    [SerializeField]
    int positionY;
    int boardID;
    bool bIsSelected;
   public ColorTile colorTileID;

    public bool BIsSelected
    {
        get { return bIsSelected; }
        set { bIsSelected = value; }

    }

    public int BoardID
    {
        get { return boardID; }
        set { boardID = value; }
    }

    public int PositionX
    {
        get { return positionX; }
        set { positionX = value;
            
            
                                }
    }

    public int PositionY
    {
        get { return positionY; }
        set { positionY = value;}
    }

    public void convertPositionToBoardID(int width)
    {
        BoardID = PositionX + width * PositionY;
        
    }

    public void convertBoardIDToPosition(int width)
    {
        PositionX = BoardID % width;
        PositionY = BoardID / width;
    }

    Renderer rend;

    public void AddRandomColor()
    {
        colorTileID = TilesManager.RandomPickColorID();
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = TilesManager.ColorID[colorTileID];
        //Debug.Log(colorTileID);
    }

    public void DestroyObject()
    {
        Destroy(gameObject,0.2f);
    }

    private void OnDestroy()
    {
        Debug.Log("I Died");
    }

}
