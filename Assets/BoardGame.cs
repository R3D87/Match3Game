using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoardGame: MonoBehaviour
{
    bool isDebug = false;
    int offset = 6;
    int boardHeight;
    int boardWidth;
    int height =4;
    int width = 4;
    int NumberTileMakeMatched = 3;

    GameObject PrefabTile;
    GameObject[,] PrefabOnBoard;
    List<Tile> GameTiles = new List<Tile>();
    ColorTile[] colorTable;
  
    public int Height
    {
        get { return height; }
        set { height = value; }
    }
    public int Width
    {
        get { return width; }
        set { width = value; }
    }

    public BoardGame(GameObject templateTile, int templateOffset)
    {
        PrefabTile = templateTile;
        offset = templateOffset;
        PrefabOnBoard = new GameObject[width, height];
    }

    public void Shuffle()
    {
        int lenghtArray = Height * Width;
        for (int i = lenghtArray-1; i >0 ; i--)
        {
            int x0 = i / Width;
            int y0 = i % Width;

            int r = Random.Range(0, i + 1);
            int x1 = r / Width;
            int y1 = r % Width;

            GameObject tempObject = PrefabOnBoard[x0, y0];

            PrefabOnBoard[x0, y0] = PrefabOnBoard[x1, y1];
            SetPrefabProperties(x0, y0,false);

            PrefabOnBoard[x1, y1] = tempObject;
            SetPrefabProperties(x1, y1, false);
        }
    }

    void  MakeColorVectorBaseOnBoardGame()
    {
        colorTable = new ColorTile[width * height];
        for (int i = 0; i < width * height; i++)
        {
            int x = i / width;
            int y = i % width;

            colorTable[i] = PrefabOnBoard[x, y].GetComponent<Tile>().colorTileID;
        }  
    }

    void SwapColorTile(int index, int delta)
    {
        ColorTile colorTileTemp;
        colorTileTemp = colorTable[index];
        colorTable[index] = colorTable[index - delta];
        colorTable[index - delta] = colorTileTemp;
    }

    public bool FindOpportunityMatching()
    {
        MakeColorVectorBaseOnBoardGame();
        for (int k = 0; k < colorTable.Length; k++)
        { 
            int counterSHV0 = 0, counterSHH0 = 0, counterSHV1 = 0, counterSHH1 = 0;
            if(isDebug)
                Debug.Log("=============ROUND: " + k + " ============");

            if (k!=0 && k % width != 0)
            {
                SwapColorTile(k, 1);
                CheckVertical(k, NumberTileMakeMatched, colorTable[k], ref counterSHV0,k);
                CheckHorizontal(k, NumberTileMakeMatched, colorTable[k], ref counterSHH0);

                CheckVertical(k - 1, NumberTileMakeMatched, colorTable[k - 1], ref counterSHV1,k-1);
                CheckHorizontal(k - 1, NumberTileMakeMatched, colorTable[k - 1], ref counterSHH1);

                if (isDebug)
                    Debug.Log("SHV0: " + counterSHV0 + " SHH0: " + counterSHH0 + " SHV1: " + counterSHV1 + " SHH1: " + counterSHH1);

                SwapColorTile(k, 1);
            }

            if (counterSHH0 >= 4 || counterSHV0 >= 4 || counterSHH1 >= 4 || counterSHV1 >= 4)
            {
                if(isDebug)
                    Debug.Log("-----------MATCH-------------");
                return true;
            }

            int counterSVV0 = 0, counterSVH0 = 0, counterSVV1 = 0, counterSVH1 = 0;
            if (k >= width )
            {
                SwapColorTile(k, width);

                CheckVertical(k, NumberTileMakeMatched, colorTable[k], ref counterSVV0,k);
                CheckHorizontal(k, NumberTileMakeMatched, colorTable[k], ref counterSVH0);
                CheckVertical(k - width, NumberTileMakeMatched, colorTable[k - width], ref counterSVV1, k - width);
                CheckHorizontal(k - width, NumberTileMakeMatched, colorTable[k - width], ref counterSVH1);

                if (isDebug)
                    Debug.Log("SVV0: " + counterSVV0 + " SHH0: " + counterSVH0 + " SVV1: " + counterSVV1 + " SVH1: " + counterSVH1);

                SwapColorTile(k, width);
            }

            if (counterSVH0 >= 4 || counterSVV0 >= 4 || counterSVH1 >= 4 || counterSVV1 >= 4)
            {
                if (isDebug)
                    Debug.Log("-----------MATCH-------------");
                return true;
            }
        }
        return false;
    }

    void CheckVertical(int idx, int limit, ColorTile baseColor, ref int counter, int idxRoot)
    {
        if (idx < 0 || idx >= colorTable.Length)
            return;

        if ((idxRoot / width != idx / width))
            return;

        if (limit <= 0)
            return;

        if (baseColor != colorTable[idx])
            return;

        counter++;
        limit--;
        if (isDebug)
            Debug.Log("Vertical: " + idx + " Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        CheckVertical(idx + 1, limit, baseColor, ref counter, idxRoot);
        CheckVertical(idx - 1, limit, baseColor, ref counter, idxRoot);
    }

    void CheckHorizontal(int idx, int limit, ColorTile baseColor, ref int counter)
    {
        if (idx >= colorTable.Length || idx < 0)
            return;

        if (limit <= 0)
            return;

        if (baseColor != colorTable[idx])
            return;

        counter++;
        limit--;
        if (isDebug)
            Debug.Log("Horizontal: " + idx + " Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        CheckHorizontal(idx + width, limit, baseColor, ref counter);
        CheckHorizontal(idx - width, limit, baseColor, ref counter);
    }

    public void SetPrefabProperties(int xi, int yi, bool setTransformPosition)
    {
        if(setTransformPosition)
            PrefabOnBoard[xi, yi].gameObject.transform.position = getPrefabLocation(xi, yi);

        PrefabOnBoard[xi, yi].GetComponent<Tile>().PositionX = xi;
        PrefabOnBoard[xi, yi].GetComponent<Tile>().PositionY = yi;
    }

    public ColorTile GetColorTileFormCoord(int x, int y)
    {
        GameObject singleTile = PrefabOnBoard[x, y];
        return singleTile.GetComponent<Tile>().colorTileID;
    }

    public Tile GetTileFormCoord(int x,int y)
    {
        Tile singleGameObjectTile = PrefabOnBoard[x, y].GetComponent<Tile>(); 
        return singleGameObjectTile;
    }

    public bool IsTileDestroyed(int x, int y)
    {    
        return PrefabOnBoard[x, y] == null;
    }

    public void RemoveTile(int x, int y)
    {
       
        PrefabOnBoard[x, y].AddComponent<Tile>().DestroyObject();
        PrefabOnBoard[x, y] = null;
    }

    public void AddPrefabToBoard(GameObject newGameObject, int x,int y)
    {
        PrefabOnBoard[x, y] = newGameObject;
    }

    public void SwapTileSync(Tile currentTile,Tile previousTile)
    {
        int x1 = currentTile.PositionX;
        int y1 = currentTile.PositionY;

        int x2 = previousTile.PositionX;
        int y2 = previousTile.PositionY;

        GameObject TempObject = PrefabOnBoard[x1, y1];
        PrefabOnBoard[x1, y1] = PrefabOnBoard[x2, y2];
        SetPrefabProperties(x1, y1,false);

        PrefabOnBoard[x2, y2] = TempObject;
        SetPrefabProperties(x2, y2,false);
    }
   
    public Vector3 getPrefabLocation(int x, int y)
    {
        return new Vector3(((offset * x - (width * offset / 2f) + 0.5f * offset)), (offset * y - (height * offset / 2f) + 0.5f * offset), 0);
    }

    public GameObject InitializeTilePrefab(int x, int y)
    {
        Vector3 position = getPrefabLocation(x, y);

        GameObject TilePrefab = Instantiate(PrefabTile, position, Quaternion.identity);

        TilePrefab.AddComponent<Tile>();
        TilePrefab.GetComponent<Tile>().PositionX = x;
        TilePrefab.GetComponent<Tile>().PositionY = y;
        TilePrefab.GetComponent<Tile>().AddRandomColor();

        TilePrefab.GetComponent<Tile>().convertPositionToBoardID(Width);
        PrefabOnBoard[x, y] = TilePrefab;
        return TilePrefab;
    }

    public void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PrefabOnBoard[x, y]= InitializeTilePrefab( x,y);
            }
        }
    }

    public void FillGap()
    {
        GameObject[] arrayWithGaps;
        GameObject[] arrayNoGaps;
        GameObject[,] arrayForMoveAnim = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            int countGaps = 0;
            arrayWithGaps = new GameObject[height];
            arrayNoGaps = new GameObject[height];

            for (int y = 0; y < height; y++)
            {
                if (!IsTileDestroyed(x, y))
                {
                    arrayWithGaps[y] = GetTileFormCoord(x, y).gameObject;
                }
                else
                {
                    arrayWithGaps[y] = null;
                    countGaps++;
                }
            }

            int idx1 = 0;
            int idx2 = height - countGaps;

            for (int y = 0; y < height; y++)
            {
                if (arrayWithGaps[y] != null)
                {
                    arrayNoGaps[idx1] = arrayWithGaps[y];
                    arrayNoGaps[idx1].GetComponent<Tile>().PositionY = idx1;
                    idx1++;
                }
                else
                {
                    arrayNoGaps[idx2] = InitializeTilePrefab(x, idx2);
                    Vector3 position = getPrefabLocation(x, idx2 + countGaps);
                    GetTileFormCoord(x, idx2).gameObject.transform.position = position;
                    idx2++;
                }

            }

            for (int i = 0; i < height; i++)
            {
                AddPrefabToBoard(arrayNoGaps[i], x, i);
                arrayForMoveAnim[x, i] = arrayNoGaps[i];
            }
        }
    }
}
