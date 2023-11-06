using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SampleBoard : MonoBehaviour // 타일 및 쿠키 생성, 통합 관리
{
    public GameObject[] Cookies;
    public GameObject BackgroundTile;

    public SampleTile[,] allTiles;
    public GameObject[,] allCookies;

    public GameObject poolField;

    public readonly int width = 5;
    public readonly int height = 7;

    public List<GameObject> pooledObjs;

    public List<Vector2> VecList;
    public List<int> EmptyList;


    private void Start()
    {
        allTiles = new SampleTile[width, height];
        allCookies = new GameObject[width, height];

        VecList = new List<Vector2>();
        EmptyList = new List<int>();

        pooledObjs = new List<GameObject>();


        CookiePooling();
        BoardInitialize();
    }

    private void Update()
    {

        if(VecList.Count > 0)
        {
            DestroyCookie();
        }      

        if(EmptyList.Count > 0)
        {
            CreateNewCookie();
        }


    }


    void BoardInitialize()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject background = Instantiate(BackgroundTile, new Vector2(i, j), Quaternion.identity) as GameObject;
                background.transform.parent = this.transform;
                background.name = "(" + (i) + ", " + (j) + ") Grid";


                GameObject cookie;

                while (PreMatchesCheck(i, j, CheckPooledObj()))
                {
                    ShuffleCookie();
                }

                cookie = GetPooledObj();


                cookie.transform.parent = this.transform;
                cookie.transform.position = new Vector2(i, j);
                cookie.transform.rotation = Quaternion.identity;
                cookie.name = "(" + (i) + ", " + (j) + ") Cookie";
                allCookies[i, j] = cookie;

                cookie.SetActive(true);

            }
        }
    }

    bool PreMatchesCheck(int _column, int _row, GameObject _cookie)
    {
        if (_column > 1 && _row > 1)
        {
            if ((allCookies[_column - 1,_row].CompareTag(_cookie.tag) && allCookies[_column - 2, _row].CompareTag(_cookie.tag)) || (allCookies[_column, _row - 1].CompareTag(_cookie.tag) && allCookies[_column, _row - 2].CompareTag(_cookie.tag)))
            {
                return true;
            }
        }

        if (_column <= 1 && _row > 1)
        {
            if ((allCookies[_column, _row - 1].CompareTag(_cookie.tag) && allCookies[_column, _row - 2].CompareTag(_cookie.tag)))
            {
                return true;
            }
        }

        if (_column > 1 && _row <= 1)
        {
            if((allCookies[_column - 1, _row].CompareTag(_cookie.tag) && allCookies[_column - 2, _row].CompareTag(_cookie.tag)))
            {
                return true;
            }
        }
        
        return false;
    }



    public void CookiePooling()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject _cookie = Instantiate(Cookies[UnityEngine.Random.Range(0, Cookies.Length)]) as GameObject;
            _cookie.transform.parent = poolField.transform;

            pooledObjs.Add(_cookie);
            pooledObjs[i].name = $"[{i}] : {_cookie.tag}";

            _cookie.SetActive(false);
        }
    }

    GameObject GetPooledObj()
    {
        GameObject tempObj = pooledObjs[0];

        pooledObjs.Remove(pooledObjs[0]);

        return tempObj;
    }

    GameObject CheckPooledObj()
    {
        GameObject tempObj = pooledObjs[0];

        return tempObj;
    }

    public void ReturnPooledObj(GameObject returnObj)
    {
        if (returnObj == null) return;

        returnObj.transform.position = poolField.transform.position;
        returnObj.transform.parent = poolField.transform;
        returnObj.SetActive(false);

        pooledObjs.Add(returnObj);
    }

    void ShuffleCookie()
    {
        int ran = UnityEngine.Random.Range(0, pooledObjs.Count);

        while (pooledObjs[0].CompareTag(pooledObjs[ran].tag))
        {
            ran = UnityEngine.Random.Range(0, pooledObjs.Count);
        }

        GameObject tempObj = pooledObjs[0];
        pooledObjs[0] = pooledObjs[ran];
        pooledObjs[ran] = tempObj;

    }

    public void DestroyCookie()
    {
        for (int i = 0; i < VecList.Count; i++)
        {
            int X = (int)VecList[i].x;
            int Y = (int)VecList[i].y;

            allCookies[X, Y].GetComponent<SampleCookie>().isMoving = false;
            allCookies[X, Y].GetComponent<SampleCookie>().isMatched = false;
            allCookies[X, Y].GetComponent<SampleCookie>().column = 100;
            allCookies[X, Y].GetComponent<SampleCookie>().row = 100;
            allCookies[X, Y].GetComponent<SampleCookie>().TargetX = 100;
            allCookies[X, Y].GetComponent<SampleCookie>().TargetY = 100;


            if (allCookies[X, Y] != null)
            {
                ReturnPooledObj(allCookies[X, Y]);
            }

            allCookies[X, Y] = null;
            EmptyList.Add(X);
            VecList.Remove(VecList[i]);
        }
        VecList.Clear();

        
    }

    public void CreateNewCookie()
    {
        for(int i = 0; i < EmptyList.Count; i++)
        {
            GameObject newCookie = GetPooledObj();

            newCookie.SetActive(true);
            newCookie.transform.parent = this.transform;
            newCookie.GetComponent<SampleCookie>().ReGenerated(EmptyList[i]);

            EmptyList.Remove(EmptyList[i]);
        }

    }


}


