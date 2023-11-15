using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject[,] allCookies;

    public GameObject BGTile;

    public GameObject CookieMng;
    public GameObject PoolMng;

    public List<Vector2> deleteVecList;
    public List<int> EmptyLine;

    Pooling pool;

    public readonly int width = 5;
    public readonly int height = 7;

    public bool isChecking;

    public void DebugBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCookies[i,j] == null)
                {
                    Debug.Log($"{i},{j} : null");
                }
                else
                {
                    Debug.Log($"{i},{j} : {allCookies[i, j].tag}");
                }
            }
        }
    }


    public enum BoardState
    {
        Initialize,

        MoveReady,
        Move,

        Check,

        Delete,

        Fall,

        Fill,
    }

    public BoardState state;    


    private void Start()
    {
        allCookies = new GameObject[width, height];
        pool = FindObjectOfType<Pooling>();


        BoardInitialize();
    }

    //==================================================================== Initialize
    void BoardInitialize()
    {
        state = BoardState.Initialize;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject BG = Instantiate(BGTile, new Vector2(i, j), Quaternion.identity) as GameObject;
                BG.transform.parent = this.transform;

                GameObject newCookie;

                while (pool.GetCheckedCookie(i, j, pool.CheckPooledCookie()))
                {
                    pool.ShufflePool();
                }

                newCookie = pool.GetPooledCookie();
                

                newCookie.transform.parent = CookieMng.transform;
                newCookie.transform.position = new Vector2(i, j + 7);

                allCookies[i, j] = newCookie;
                SetCookie(i, j, newCookie);



                newCookie.SetActive(true);
            }
        }

        StartCoroutine(CheckInitialize());
    }

    void SetCookie(int _column, int _row, GameObject _cookie)
    {
        _cookie.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        StartCoroutine(_cookie.GetComponent<Cookie>().CookieInitialized(_column, _row));
    }

    IEnumerator CheckInitialize()
    {
        yield return new WaitForSeconds(2f);
        int boardCount = 0;

        while (boardCount == 35)
        {
            foreach (var item in allCookies)
            {
                if (item != null)
                {
                    boardCount++;
                }
            }
        }

        state = BoardState.MoveReady;
    }

    //==================================================================== Move

    public void allCookieMove(int col, int row, Vector2 Dir)
    {

        if (col < 0 || col > width - 1) return;
        if (row < 0 || row > height - 1) return;

        if (col == 0 && Dir == Vector2.left) return;
        if (col == width - 1 && Dir == Vector2.right) return;
        if (row == 0 && Dir == Vector2.down) return;
        if (row == height - 1 && Dir == Vector2.up) return;


        GameObject moveCookie = allCookies[col, row];
        GameObject temp;


        StartCoroutine(moveCookie.GetComponent<Cookie>().CookieMove(Dir));

        if(Dir.y == 0)
        {
            if(allCookies[col + (int)Dir.x, row] != null)
            {
                allCookies[col + (int)Dir.x, row].GetComponent<Cookie>().MoveVec = -Dir;
                StartCoroutine(allCookies[col + (int)Dir.x, row].GetComponent<Cookie>().CookieMove(-Dir));

                temp = allCookies[col + (int)Dir.x, row];
                allCookies[col + (int)Dir.x, row] = allCookies[col, row];
                allCookies[col, row] = temp;
            }
        }
        else if(Dir.x == 0)
        {
            if(allCookies[col, row + (int)Dir.y] != null)
            {
                allCookies[col, row + (int)Dir.y].GetComponent<Cookie>().MoveVec = -Dir;
                StartCoroutine(allCookies[col, row + (int)Dir.y].GetComponent<Cookie>().CookieMove(-Dir));

                temp = allCookies[col, row + (int)Dir.y];
                allCookies[col, row + (int)Dir.y] = allCookies[col, row];
                allCookies[col, row] = temp;
            }
        }
    }

    
    public void ReturnCookie(int col, int row, Vector2 Dir)
    {
        if (col < 0 || col > width - 1) return;
        if (row < 0 || row > height - 1) return;

        if (col == 0 && Dir == Vector2.left) return;
        if (col == width - 1 && Dir == Vector2.right) return;
        if (row == 0 && Dir == Vector2.down) return;
        if (row == height - 1 && Dir == Vector2.up) return;

        if (allCookies[col - (int)Dir.x, row - (int)Dir.y] == null) return;
        if (allCookies[col - (int)Dir.x, row - (int)Dir.y].GetComponent<Cookie>().isMatched == true) return;
        


        GameObject moveCookie = allCookies[col, row];


        StartCoroutine(moveCookie.GetComponent<Cookie>().CookieReturn(-Dir));

        if (Dir.y == 0 && Dir.x != 0)
        {
            if (allCookies[col - (int)Dir.x, row] != null)
            {
                StartCoroutine(allCookies[col - (int)Dir.x, row].GetComponent<Cookie>().CookieReturn(Dir));
            }
        }
        else if (Dir.x == 0 && Dir.y !=0)
        {
            if (allCookies[col, row - (int)Dir.y] != null)
            {
                StartCoroutine(allCookies[col, row - (int)Dir.y].GetComponent<Cookie>().CookieReturn(Dir));
            }
        }
    }

    public void BoardCheck()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCookies[i, j] == null) continue;
                allCookies[i, j].GetComponent<Cookie>().MatchCheck();
            }
        }
        

        if(deleteVecList.Count > 0)
        {
            state = BoardState.Delete;
            Invoke("DeleteCookie", 0.3f);
        }
    }

    public void DeleteCookie()
    {
        for (int i = 0; i < deleteVecList.Count; i++)
        {
            GameObject deleteCookie = allCookies[(int)deleteVecList[i].x, (int)deleteVecList[i].y];

            pool.ReturnToPool(deleteCookie);
            allCookies[(int)deleteVecList[i].x, (int)deleteVecList[i].y] = null;


            EmptyLine.Add((int)deleteVecList[i].x);
            deleteVecList.Remove(deleteVecList[i]);
        }

        state = BoardState.Fall;
        BoardFall();
    }

    public void BoardFall()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (allCookies[i, j] == null) continue;
                if (allCookies[i, j - 1] == null)
                {
                    allCookies[i, j].GetComponent<Cookie>().CookieAutoFall();
                }
            }
        }
    }

    public void BoardReFill()
    {

    }

}
