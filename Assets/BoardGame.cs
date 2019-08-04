using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoardGame: MonoBehaviour
{ 

   
    // public Tile baseTile;
    int offset = 6;
    int boardHeight;
    int boardWidth;
    int height =4;
    int width = 4;
    ColorTile[] colorTable;
    ColorTile[,] colorTable_20;
    public int Height
    {
        get { return height; }
        set { height = value;}
    }
    public int Width
    {
        get { return width; }
        set { width = value;}
    }

    GameObject PrefabTile;
    GameObject[,] PrefabOnBoard;
    List<Tile> GameTiles = new List<Tile>();

    
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

    void SwapHorizontal_CheckVertical(int idx, int limit, ColorTile baseColor, ref int counter, int idxRoot)
    {
        if (idx < 0 || idx >= colorTable.Length )
            return;

        if ((idxRoot / width != idx / width))
            return;

        if (limit <= 0)
            return;

        if (baseColor != colorTable[idx])
            return;

        counter++;
        limit--;
        Debug.Log("Vertical: " + idx + " Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        SwapHorizontal_CheckVertical(idx + 1, limit, baseColor, ref counter,idxRoot);
        SwapHorizontal_CheckVertical(idx - 1, limit, baseColor, ref counter, idxRoot);
    }

    void SwapHorizontal_CheckHorizontal(int idx, int limit, ColorTile baseColor, ref int counter)
    {
        if (idx < 0 || idx >= colorTable.Length)
            return;

        if (limit <= 0)
            return;

        if (baseColor != colorTable[idx])
            return;

        counter++;
       
        limit--;
        Debug.Log("Horizontal: " + idx + " Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        SwapHorizontal_CheckHorizontal(idx + width, limit, baseColor, ref counter);
        SwapHorizontal_CheckHorizontal(idx - width, limit, baseColor, ref counter);
    }

    public bool FindOppprtunityMatching()
    {
        colorTable = new ColorTile[width * height];
        // create color vector
        for (int i = 0; i < width * height; i++)
        {
            int x = i / width;
            int y = i % width;

            colorTable[i] = PrefabOnBoard[x, y].GetComponent<Tile>().colorTileID;
            PrefabOnBoard[x, y].transform.localScale = Vector3.one;
        }
        // vertical swap check
        for (int k = 1; k < colorTable.Length; k++)
        {

            int counterSHV0 = 0, counterSHH0 = 0, counterSHV1 = 0, counterSHH1 = 0;

            Debug.Log("=============ROUND: " + k + " ============");
            ColorTile colorTileTemp;

            // Heigh
            if (k % width != 0)
            {
                colorTileTemp = colorTable[k];
                colorTable[k] = colorTable[k - 1];
                colorTable[k - 1] = colorTileTemp;

                Debug.Log("----------------------------------");
                SwapHorizontal_CheckVertical(k, 3, colorTable[k], ref counterSHV0,k);
                SwapHorizontal_CheckHorizontal(k, 3, colorTable[k], ref counterSHH0);
                Debug.Log("----------------------------------");
                SwapHorizontal_CheckVertical(k - 1, 3, colorTable[k - 1], ref counterSHV1,k-1);
                SwapHorizontal_CheckHorizontal(k - 1, 3, colorTable[k - 1], ref counterSHH1);
                Debug.Log("SHV0: " + counterSHV0 + " SHH0: " + counterSHH0 + " SHV1: " + counterSHV1 + " SHH1: " + counterSHH1);
                colorTileTemp = colorTable[k];
                colorTable[k] = colorTable[k - 1];
                colorTable[k - 1] = colorTileTemp;

            }

            if (counterSHH0 >= 4 || counterSHV0 >= 4 || counterSHH1 >= 4 || counterSHV1 >= 4)
            {
                
                Debug.Log("-----------MATCH-------------");
                Debug.Log("-----------MATCH-------------");
                Debug.Log("-----------MATCH-------------");
                return true;
            }

        }

         Debug.Log("--------------------------------------------------------------------");
         Debug.Log("--------------Width-----------------------------Width---------------");
         Debug.Log("--------------------------------------------------------------------");
        //horizontal swap check
        for (int k = 0; k < colorTable.Length; k++)
        {
            int counterSVV0 = 0, counterSVH0 = 0, counterSVV1 = 0, counterSVH1 = 0;

            Debug.Log("=============ROUND: " + k + " ============");
            ColorTile colorTileTemp;

            if (k >= width )
            {
                colorTileTemp = colorTable[k - width];
                colorTable[k - width] = colorTable[k];
                colorTable[k] = colorTileTemp;

                Debug.Log("----------------------------------");

                SwapVertical_CheckVertical(k, 3, colorTable[k], ref counterSVV0,k);
                SwapVertical_CheckHorizontal(k, 3, colorTable[k], ref counterSVH0);
                Debug.Log("----------------------------------");
                SwapVertical_CheckVertical(k - width, 3, colorTable[k - width], ref counterSVV1, k - width);
                SwapVertical_CheckHorizontal(k - width, 3, colorTable[k - width], ref counterSVH1);

                Debug.Log("SVV0: " + counterSVV0 + " SHH0: " + counterSVH0 + " SVV1: " + counterSVV1 + " SVH1: " + counterSVH1);

                colorTileTemp = colorTable[k - width];
                colorTable[k - width] = colorTable[k];
                colorTable[k] = colorTileTemp;

            }

            if (counterSVH0 >= 4 || counterSVV0 >= 4 || counterSVH1 >= 4 || counterSVV1 >= 4)
            {
                // return true;
                Debug.Log("-----------MATCH-------------");
                Debug.Log("-----------MATCH-------------");
                Debug.Log("-----------MATCH-------------");
                return true;
            }
        }
        return false;
    }
    void SwapVertical_CheckVertical(int idx,int limit,ColorTile baseColor, ref int counter, int RootIdx)
    {
        if (idx >= colorTable.Length || idx<0 )
            return ;

        if (RootIdx / width != idx / width)
            return;
        if (limit <= 0)
            return ;

        if (baseColor != colorTable[idx])
            return;
    
        counter++;
       
        limit--;
        Debug.Log("Vertical: "+idx+" Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        SwapVertical_CheckVertical(idx + 1, limit, baseColor, ref counter, RootIdx);
        SwapVertical_CheckVertical(idx - 1, limit, baseColor, ref counter, RootIdx);

    }
    void SwapVertical_CheckHorizontal(int idx, int limit, ColorTile baseColor, ref int counter)
    {
        if (idx >= colorTable.Length || idx < 0 )
            return;

        if (limit <= 0)
            return;

        if (baseColor != colorTable[idx])
            return;

        counter++;

        limit--;
        Debug.Log("Horizontal: " + idx + " Color: " + colorTable[idx] + " x: " + idx / width + " y: " + idx % width);
        SwapVertical_CheckHorizontal(idx + width, limit, baseColor, ref counter);
        SwapVertical_CheckHorizontal(idx - width, limit, baseColor, ref counter);

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
       // PrefabOnBoard[x, y].gameObject.transform.position = getPrefabLocation(x, y);
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

    public  void  FillGap()
    {
        //int width = boardGame.Width;
        //int height = boardGame.Height;
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
                    //Debug.Log(arrayNoGaps[idx2].GetComponent<Tile>().colorTileID);
                    idx2++;
                }

            }

            //Debug.Log("-------------------");



            foreach (var item in arrayNoGaps)
            {
                //Debug.Log("item: " + item.GetComponent<Tile>().colorTileID + " x: " + item.GetComponent<Tile>().PositionX + " y: " + item.GetComponent<Tile>().PositionY);
            }

            for (int i = 0; i < height; i++)
            {

                AddPrefabToBoard(arrayNoGaps[i], x, i);
                arrayForMoveAnim[x, i] = arrayNoGaps[i];
                //Debug.Log(arrayForMoveAnim[x, i].GetComponent<Tile>().colorTileID );
                // StopCoroutine("MoveAnim");
                // StartCoroutine(MoveAnim(boardGame.GetTileFormCoord(x, i).gameObject, x, i, .5f));
            }

        }
    }


}
