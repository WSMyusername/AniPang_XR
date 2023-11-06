using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
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

    void InvokeMoveReady()
    {
        state = BoardState.MoveReady;
    }


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

        Invoke("InvokeMoveReady", 2f);
    }

    void SetCookie(int _column, int _row, GameObject _cookie)
    {
        _cookie.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        StartCoroutine(_cookie.GetComponent<Cookie>().CookieFall(_column, _row));
    }

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

        if (Dir.y == 0)
        {
            if (allCookies[col + (int)Dir.x, row] != null)
            {
                allCookies[col + (int)Dir.x, row].GetComponent<Cookie>().MoveVec = -Dir;
                StartCoroutine(allCookies[col + (int)Dir.x, row].GetComponent<Cookie>().CookieMove(-Dir));

                temp = allCookies[col + (int)Dir.x, row];
                allCookies[col + (int)Dir.x, row] = allCookies[col, row];
                allCookies[col, row] = temp;
            }
        }
        else if (Dir.x == 0)
        {
            if (allCookies[col, row + (int)Dir.y] != null)
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
        else if (Dir.x == 0 && Dir.y != 0)
        {
            if (allCookies[col, row - (int)Dir.y] != null)
            {
                StartCoroutine(allCookies[col, row - (int)Dir.y].GetComponent<Cookie>().CookieReturn(Dir));
            }
        }
    }


}
