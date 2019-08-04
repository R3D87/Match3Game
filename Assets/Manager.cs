using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




public class Manager : MonoBehaviour
{
    public delegate void CoroutineDelegate();
 
    CoroutineDelegate OnFinishCorutineInTile;
    CoroutineDelegate OnFinishCoroutine;

    public int RequiredAmounOfMatchedElements = 3;
    public int offset;
    public GameObject TemplateTile;

    bool isCoroutineProgressing = true;
    GameObject left;
    GameObject right;
    BoardGame boardGame;
    

 
    LayerMask mask;
    Tile currentSelectedTile = null;
    Tile previousSelectedTile = null;
    List<Tile> MatchedList = new List<Tile>();

    private void Awake()
    {
        mask = LayerMask.GetMask("Tile");
    }

    private void Start()
    {
        boardGame = new BoardGame(TemplateTile, offset);
        boardGame.CreateBoard();
        RemoveAllMatechesTiles();
    }

    bool FindMatedTiles()
    {    
        MatchedList = MatchTileCheckAll();

        if (MatchedList.Count != 0)
            return true;
        else
            return false;
    }

    IEnumerator ieRemoveAllMatechesTiles()
    { 
        bool Conitnue;   
        do
        {
            Conitnue = false;
            while (FindMatedTiles())
            {
                DestroyMatchedTile();
                yield return new WaitForSeconds(0.5f);
                boardGame.FillGap();

                ExecuteMove();
                yield return new WaitForSeconds(1f);
            }

            while (!boardGame.FindOppprtunityMatching())
            {
                boardGame.Shuffle();
                yield return new WaitForSeconds(1f);

                Conitnue = true;

                StartCoroutine(ieAutoShuffle());
                yield return new WaitForSeconds(1f);
               
            }

        } while (Conitnue);
       
        Conitnue = false;
        yield return null; 
    }

    IEnumerator ieAutoShuffle()
    {
        ExecuteMove();
        Debug.Log("Animation Shuffle");
        yield return null;
    }



    void RemoveAllMatechesTiles()
    {
        StartCoroutine(ieRemoveAllMatechesTiles());
    }
 
    void SkipAnimation ()
    {
        for (int x = 0; x < boardGame.Width; x++)
        {
            for (int y = 0; y < boardGame.Height; y++)
            {
                boardGame.SetPrefabProperties(x,y,true);
            }
        }
    }
    private void OnMouseDown()
    {
        Debug.Log("MouseDown");
    }
    void ClickTile(Tile tile)
    {

        previousSelectedTile = currentSelectedTile;
        if (previousSelectedTile != null)
            previousSelectedTile.BIsSelected = false;

        Debug.Log("Tile clicked: " + tile.colorTileID + " IsSelected: " + tile.BIsSelected+ " PosX: "+ tile.PositionX+ " PosY: "+tile.PositionY+ " Location: "+ tile.gameObject.transform.position);
        Debug.Log("Color: " + tile.colorTileID + " X: " + tile.PositionX + " Y: " + tile.PositionY);
        if (tile.BIsSelected == false)
        {
            currentSelectedTile = tile;
            SelectTile(currentSelectedTile);
        }
        else
        {
            DiselectTile(currentSelectedTile);
            currentSelectedTile = null;
        }

        if (isAbleToSwapTile())
        {
            
            Debug.Log("cur:" + currentSelectedTile.colorTileID);
            Debug.Log("prev: " + previousSelectedTile.colorTileID);

            if (SwapTile())
            {
               
                ExecuteSwap();
                
            }
            else
            {
               
              
                ExecuteUndoSwap();
            }
        }
        else if (previousSelectedTile != null)
        {
            DiselectTile(currentSelectedTile);
            currentSelectedTile = null;
            DiselectTile(previousSelectedTile);
            previousSelectedTile = null;
        }

    }

    bool isAbleToSwapTile()
    {

        if (currentSelectedTile != null && previousSelectedTile != null)
            if (isNeighborTile() == true)
            {
                return true;
            }

        return false;
    }


    bool isNeighborTile()
    {
        bool bIsNeighbor = false;

        int x1 = currentSelectedTile.PositionX;
        int y1 = currentSelectedTile.PositionY;

        int x2 = previousSelectedTile.PositionX;
        int y2 = previousSelectedTile.PositionY;

        int deltaX = Mathf.Abs(x1 - x2);
        int deltaY = Mathf.Abs(y1 - y2);

        if (deltaX == 1 && deltaY == 0 || deltaX == 0 && deltaY == 1)
        {
            bIsNeighbor = true;
        }

        return bIsNeighbor;
    }

    void ExecuteSwap()
    {

        OnFinishCoroutine += RemoveAllMatechesTiles;
        StartCoroutine(SwapAnim(GetGameObjectForTile(currentSelectedTile), GetGameObjectForTile(previousSelectedTile), 0.5f, OnFinishCoroutine));
        OnFinishCoroutine -= RemoveAllMatechesTiles;
       
        previousSelectedTile = null;
        currentSelectedTile = null;

      
    }
    void ExecuteUndoSwap()
    {
        DiselectTile(currentSelectedTile);
        DiselectTile(previousSelectedTile);
        previousSelectedTile = null;
        currentSelectedTile = null;
    }

    bool SwapTile()
    {
        bool bCanSwap = false; 

        int currentPositonX = currentSelectedTile.PositionX;
        int currentPositonY = currentSelectedTile.PositionY;

        int previousPositonX = previousSelectedTile.PositionX;
        int previousPositonY = previousSelectedTile.PositionY;

        boardGame.SwapTileSync(currentSelectedTile,previousSelectedTile);

        MatchedList = MatchTileCheckNeighbours(currentPositonX, currentPositonY);
        MatchedList.InsertRange(MatchedList.Count, MatchTileCheckNeighbours(previousPositonX, previousPositonY));

        if (MatchedList.Count != 0)
        {
            bCanSwap = true;

        }
        else
        {

            boardGame.SwapTileSync(currentSelectedTile, previousSelectedTile);
            bCanSwap = false;


        }


        Debug.Log("After Swap:");
        Debug.Log("CurrentTile: " + currentSelectedTile.colorTileID + " X: " + currentSelectedTile.PositionX + " Y: " + currentSelectedTile.PositionY);
        Debug.Log("PreviousTile: " + previousSelectedTile.colorTileID + " X: " + previousSelectedTile.PositionX + " Y: " + previousSelectedTile.PositionY);

        Debug.Log("GameBoard Data:");
        Debug.Log("PreviousTile: " + boardGame.GetTileFormCoord(previousPositonX, previousPositonY).GetComponent<Tile>().colorTileID +
            " X: " + boardGame.GetTileFormCoord(previousPositonX, previousPositonY).GetComponent<Tile>().PositionX +
            " Y: " + boardGame.GetTileFormCoord(previousPositonX, previousPositonY).GetComponent<Tile>().PositionY);
        Debug.Log("CurrentTile: " + boardGame.GetTileFormCoord(currentPositonX, currentPositonY).GetComponent<Tile>().colorTileID +
          " X: " + boardGame.GetTileFormCoord(currentPositonX, currentPositonY).GetComponent<Tile>().PositionX +
           " Y: " + boardGame.GetTileFormCoord(currentPositonX, currentPositonY).GetComponent<Tile>().PositionY);


        return bCanSwap;
    }

    void SelectTile(Tile tile)
    {

        tile.BIsSelected = true;
        SelectedTileAnim(tile);
    }



    GameObject GetGameObjectForTile(Tile tile)
    {
        GameObject TileObject = tile.gameObject;
        return TileObject;
    }
    void DiselectTile(Tile tile)
    {
        DeselectedTileAnim(tile);

        tile.BIsSelected = false;

    }
    void SelectedTileAnim(Tile tile)
    {
        
        GameObject TileObject = GetGameObjectForTile(tile);
        Vector3 scaleFinal = Vector3.one * 1.5f;

        StartCoroutine(ScaleAnim(TileObject,scaleFinal, 0.2f, OnFinishCoroutine));
    }


    IEnumerator SwapAnim(GameObject gameObjectA, GameObject gameObjectB, float duration, CoroutineDelegate DelegateOnFinishCoroutine)
    {
        float progress = 0;
        Vector3 targetForB = gameObjectA.transform.position;
        Vector3 targetForA = gameObjectB.transform.position;
        while (progress <= duration)
        {
            progress = progress + Time.deltaTime;
            float percent = Mathf.Clamp01(progress / duration);

            gameObjectA.transform.position = Vector3.Lerp(targetForB, targetForA, percent);
            gameObjectB.transform.position = Vector3.Lerp(targetForA, targetForB, percent);
            yield return null;

        }
        DiselectTile(gameObjectA.GetComponent<Tile>());
        DiselectTile(gameObjectB.GetComponent<Tile>());

        if (DelegateOnFinishCoroutine != null)
        {
            DelegateOnFinishCoroutine();

        }


      



    }


    IEnumerator ScaleAnim(GameObject TileObject, Vector3 targetScale, float duretion, CoroutineDelegate DelegateOnFinishCoroutine)
    {
        isCoroutineProgressing = true;
        float progress = 0;
        while (progress <= duretion)
        {
            progress = progress + Time.deltaTime;
            float percent = Mathf.Clamp01(progress / duretion);
            TileObject.transform.localScale = Vector3.Lerp(TileObject.transform.localScale, targetScale, percent);
            yield return null;
        }

        if (DelegateOnFinishCoroutine != null)
        {
            DelegateOnFinishCoroutine();

        }
        isCoroutineProgressing = false;
    }
   

    void DeselectedTileAnim(Tile tile)
    {
        GameObject TileObject = GetGameObjectForTile(tile);
        Vector3 scaleFinal = Vector3.one;
    

        StartCoroutine(ScaleAnim(TileObject,scaleFinal, 0.2f, OnFinishCoroutine));
       
        
    }


    List<Tile> MatchTileCheckNeighbours(int x, int y)
    {
        HashSet<Tile> MatchSet = new HashSet<Tile>();
        //Debug.Log("x: " + x + " y: " + y);
        int x1, x2, x3, y1, y2, y3;
        ColorTile ColorTarget = boardGame.GetColorTileFormCoord(x, y);
        int[] Template = new int[] { -2, -1, 0 };

        for (int i = 0; i < 3; i++)
        {
            x1 = x + Template[0] + i;
            x2 = x + Template[1] + i;
            x3 = x + Template[2] + i;
            //Debug.Log("x1: " + x1 + " x2: " + x2 + " x3: " + x3);

            if (x1 >= 0 && x2 >= 0 && x3 >= 0 && boardGame.Width > x1 && boardGame.Width > x2 && boardGame.Width > x3)
            {
                if (ColorTarget == boardGame.GetColorTileFormCoord(x1, y) &&
                    ColorTarget == boardGame.GetColorTileFormCoord(x2, y) &&
                    ColorTarget == boardGame.GetColorTileFormCoord(x3, y))
                {
                    MatchSet.Add(boardGame.GetTileFormCoord(x1, y));
                    MatchSet.Add(boardGame.GetTileFormCoord(x2, y));
                    MatchSet.Add(boardGame.GetTileFormCoord(x3, y));
                }
            }

            y1 = y + Template[0] + i;
            y2 = y + Template[1] + i;
            y3 = y + Template[2] + i;
            //Debug.Log("y1: " + y1 + " y2: " + y2 + " y3: " + y3);
            if (y1 >= 0 && y >= 0 && y3 >= 0 && boardGame.Height > y1 && boardGame.Height > y2 && boardGame.Height > y3)
            {
                if (ColorTarget == boardGame.GetColorTileFormCoord(x, y1) &&
                    ColorTarget == boardGame.GetColorTileFormCoord(x, y2) &&
                    ColorTarget == boardGame.GetColorTileFormCoord(x, y3))
                {
                    MatchSet.Add(boardGame.GetTileFormCoord(x, y1));
                    MatchSet.Add(boardGame.GetTileFormCoord(x, y2));
                    MatchSet.Add(boardGame.GetTileFormCoord(x, y3));
                }
            }
        }
        foreach (var item in MatchSet)
        {
            //Debug.Log("item: " + item.PositionX + " " + item.PositionY);
        }
        return new List<Tile>(MatchSet);

    }

    List<Tile> MatchTileCheckAll()
    {


        List<Tile> HotizontalTileMatch = new List<Tile>();
        List<Tile> VerticalTileMatch = new List<Tile>();
        List<Tile> TileMatch = new List<Tile>();

        for (int x = 0; x < boardGame.Width; x++)
        {
            for (int y = 0; y < boardGame.Height; y++)
            {
                int horizontalCounter = 0;
                int verticalCounter = 0;
                //Debug.Log("Color: " + boardGame.GetTileFormCoord(x, y).colorTileID + " x: " + x + " y: " + y);


                for (int k = 1; k <= 2; k++)
                {
                    int xNeighbour = x + k;


                    if (xNeighbour >= boardGame.Width)
                    {
                        break;
                    }
                    //Debug.Log("Color: " + boardGame.GetTileFormCoord(xNeighbour, y).colorTileID + " xNeighbour:" + xNeighbour + " y: " + y);
                    ColorTile targetColor = boardGame.GetColorTileFormCoord(x, y);

                    ColorTile neighbourHorizontalColor = boardGame.GetColorTileFormCoord(xNeighbour, y);


                    if (neighbourHorizontalColor == targetColor)
                    {
                        horizontalCounter++;
                    }


                }

                for (int k = 1; k <= 2; k++)
                {

                    int yNeighbour = y + k;
                    if (yNeighbour >= boardGame.Height)
                    {
                        break;
                    }
                    //Debug.Log("Color: " + boardGame.GetTileFormCoord(x, yNeighbour).colorTileID + "x: " + x + " yNeighbour: " + yNeighbour);
                    ColorTile targetColor = boardGame.GetColorTileFormCoord(x, y);


                    ColorTile neighbourVerticalColor = boardGame.GetColorTileFormCoord(x, yNeighbour);

                    if (neighbourVerticalColor == targetColor)
                    {
                        verticalCounter++;
                    }

                }
                //Debug.Log("-------------------------------------");
                if (verticalCounter == 2)
                {
                    if (!VerticalTileMatch.Contains(boardGame.GetTileFormCoord(x, y)))
                    {
                        VerticalTileMatch.Add(boardGame.GetTileFormCoord(x, y));
                        VerticalTileMatch.Add(boardGame.GetTileFormCoord(x, y + 1));
                        VerticalTileMatch.Add(boardGame.GetTileFormCoord(x, y + 2));
                    }
                }
                if (horizontalCounter == 2)
                {
                    if (!HotizontalTileMatch.Contains(boardGame.GetTileFormCoord(x, y)))
                    {
                        HotizontalTileMatch.Add(boardGame.GetTileFormCoord(x, y));
                        HotizontalTileMatch.Add(boardGame.GetTileFormCoord(x + 1, y));
                        HotizontalTileMatch.Add(boardGame.GetTileFormCoord(x + 2, y));
                    }
                }

            }
        }



        //Debug.Log("List of Matched items:\n\n");
        foreach (var item in HotizontalTileMatch)
        {
            //Debug.Log("Color Tile: " + item.colorTileID + " x: " + item.PositionX + " y: " + item.PositionY);
            TileMatch.Add(item);
        }
        foreach (var item in VerticalTileMatch)
        {
            //Debug.Log("Color Tile: " + item.colorTileID + " x: " + item.PositionX + " y: " + item.PositionY);
            if (!TileMatch.Contains(item))
                TileMatch.Add(item);
        }

        return TileMatch;


    }
    void executeGameBoardRepair()
    {
        boardGame.FillGap();
        ExecuteMove();
    }

    void FloodFill(int x, int y, ColorTile color, int Limit)
    {

        if (Limit == 0)
                    return;


        Limit--;


        FloodFill(x - 1, y, color, Limit);
        FloodFill(x + 1, y, color, Limit);
        FloodFill(x, y - 1, color, Limit);
        FloodFill(x, y + 1, color, Limit);
    }


    void ExecuteMove()
    {
        
        for (int x = 0; x < boardGame.Width; x++)
        {
            for (int y = 0; y < boardGame.Height; y++)
            {
               if(!boardGame.IsTileDestroyed(x,y))
               MoveTileAnimation(boardGame.GetTileFormCoord(x,y).gameObject, x, y);
            }
        }
    }

 
    public void DestroyMatchedTile()
    {
        foreach (Tile tile in MatchedList)
        {
            int xTemp = tile.PositionX;
            int yTemp = tile.PositionY;

           DestroyTileAnimation(xTemp, yTemp); 
        }
        MatchedList.Clear();
    }
    public void DestroyTileAnimation(int x, int y)
    {
       
            StartCoroutine(ExecuteDestroyTileAnimation(boardGame.GetTileFormCoord(x,y).gameObject, new Vector3(0, 0, 0), 0.3f, OnFinishCorutineInTile));
       

    }

 

    IEnumerator ExecuteDestroyTileAnimation(GameObject TileObject, Vector3 targetScale, float duretion, CoroutineDelegate DelegateOnFinishCoroutine)
    {
        // isCoroutineProgressing = true;
        float progress = 0;
        while (progress <= duretion)
        {
            progress = progress + Time.deltaTime;
            float percent = Mathf.Clamp01(progress / duretion);
            TileObject.transform.localScale = Vector3.Lerp(TileObject.transform.localScale, targetScale, percent);
            yield return null;
        }

        if (DelegateOnFinishCoroutine != null)
        {
            DelegateOnFinishCoroutine();

        }
        boardGame.RemoveTile(TileObject.GetComponent<Tile>().PositionX, TileObject.GetComponent<Tile>().PositionY);
        //isCoroutineProgressing = false;
    }
    public void MoveTileAnimation(GameObject PrefabTile, int x, int y)
    {
        Vector3 targetPosition = boardGame.getPrefabLocation(x, y);
        //if (targetPosition != gameObject.transform.position)
        {
            
            StartCoroutine(ExecuteMoveTileAnimation(PrefabTile, targetPosition, 1.5f, OnFinishCorutineInTile));
            
        }
    }

    IEnumerator ExecuteMoveTileAnimation(GameObject PrefabObject, Vector3 targetPosition, float duration, CoroutineDelegate DelegateOnFinishCoroutine)
    {
        float progress = 0;
        Vector3 startPosition = gameObject.transform.position;
        //yield return new WaitForSeconds(2f);
        while (progress <= duration)
        {
            progress = progress + Time.deltaTime;
            float percent = Mathf.Clamp01(progress / duration);

            PrefabObject.transform.position = Vector3.Lerp(PrefabObject.transform.position, targetPosition, percent);

            yield return null;

        }
        if (DelegateOnFinishCoroutine != null)
        {
            DelegateOnFinishCoroutine();

        }


    }


    private void Update()
    {
        bool bButtonUp = Input.GetMouseButtonUp(0);
        bool bButtonDown = Input.GetMouseButtonDown(0);
        bool bDoOnce = true;
        if (Input.GetKeyDown(KeyCode.Z))



        if (Input.GetKeyDown(KeyCode.Space))
        {
              DestroyMatchedTile();
            executeGameBoardRepair();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
           
            MatchedList = MatchTileCheckAll();


        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            boardGame.Shuffle();
            MatchedList = MatchTileCheckAll();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            MatchedList = MatchTileCheckAll();

        }
        if(Input.GetKeyDown(KeyCode.A))
        {
    //        boardGame.TestAnim();
        }

        if (bButtonDown && bDoOnce)
        {
            bDoOnce = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
               
            Debug.DrawRay(ray.origin, ray.direction, Color.red, int.MaxValue);
            RaycastHit hitInfo;
         
            if (Physics.Raycast(ray,out hitInfo))
            {
              //  //Debug.Log(mask.ToString());
                Physics.Raycast(ray, out hitInfo, int.MaxValue, mask);
                GameObject hitObject = hitInfo.collider.gameObject;
                Renderer renderer = hitObject.GetComponent<Renderer>();


              //  //Debug.Log(renderer.material.name);
                if (hitObject.GetComponent<Tile>() != null)
                {
                    ClickTile(hitObject.GetComponent<Tile>());
                    
                    ////Debug.Log(hitObject.GetComponent<Tile>().PositionX + " " + hitObject.GetComponent<Tile>().PositionY);
                }
            }
        }
        bDoOnce = bButtonUp;

        
    }

}
