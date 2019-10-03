using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour {

    [SerializeField]
    int positionX;
    [SerializeField]
    int positionY;
    public ColorTile colorTileID;
    Renderer rend;
    int boardID;
    bool bIsSelected;

    public bool IsSelected
    {
        get { return bIsSelected;}
        set { bIsSelected = value;}
    }

    public int BoardID
    {
        get { return boardID;}
        set { boardID = value;}
    }

    public int PositionX
    {
        get { return positionX;}
        set { positionX = value;}
    }

    public int PositionY
    {
        get { return positionY;}
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

    public void AddRandomColor()
    {
        colorTileID = TilesManager.RandomPickColorID();
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = TilesManager.ColorID[colorTileID];      
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
