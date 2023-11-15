using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    public GameObject[] SampleCookies;
    public GameObject[] SampleBombCookies;

    public List<GameObject> Pool;

    Board board;

    [SerializeField]
    public readonly int MaxPoolSize = 100;

    private void Start()
    {
        Pool = new List<GameObject>();
        board = FindObjectOfType<Board>();


        CookiePooling();
    }

    public void CookiePooling()
    {
        for (int i = 0; i < MaxPoolSize; i++)
        {
            int ran = Random.Range(0, SampleCookies.Length);

            GameObject Cookie = Instantiate(SampleCookies[ran], this.transform.position, Quaternion.identity) as GameObject;

            Cookie.transform.parent = this.transform;
            Cookie.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
            Cookie.SetActive(false);

            Pool.Add(Cookie);
        }
    }
    public GameObject GetPooledCookie()
    {
        GameObject temp = Pool[0];

        Pool.Remove(Pool[0]);

        return temp;
    }

    public GameObject CheckPooledCookie()
    {
        GameObject temp = Pool[0];

        return temp;
    }

    public bool GetCheckedCookie(int _column, int _row, GameObject _cookie) // 처음 보드 생성시 매치되지 않도록 하는 함수
    {
        if (_column > 1 && _row > 1) // 2 ~ width-1, 2 ~ height - 1
        {
            if ((board.allCookies[_column - 1, _row].CompareTag(_cookie.tag) && board.allCookies[_column - 2, _row].CompareTag(_cookie.tag)) ||
                (board.allCookies[_column, _row - 1].CompareTag(_cookie.tag) && board.allCookies[_column, _row - 2].CompareTag(_cookie.tag)))
            {
                return true;
            }
        }

        if (_column <= 1 && _row > 1) // 0 ~ 1, 2 ~ height - 1
        {
            if (board.allCookies[_column, _row - 1].CompareTag(_cookie.tag) && board.allCookies[_column, _row - 2].CompareTag(_cookie.tag))
            {
                return true;
            }
        }

        if (_column > 1 && _row <= 1) // 2 ~ width - 1, 0 ~ 1
        {
            if (board.allCookies[_column - 1, _row].CompareTag(_cookie.tag) && board.allCookies[_column - 2, _row].CompareTag(_cookie.tag))
            {
                return true;
            }
        }

        return false;
    }

    public void ReturnToPool(GameObject returnCookie)
    {
        returnCookie.transform.parent = this.transform;
        returnCookie.GetComponent<Cookie>().column = -1;
        returnCookie.GetComponent<Cookie>().row = -1;
        returnCookie.SetActive(false);

        Pool.Add(returnCookie);

    }

    public void ShufflePool()
    {
        int ran = Random.Range(0, Pool.Count);

        while (Pool[ran].CompareTag(Pool[0].tag))
        {
            ran = Random.Range(0, Pool.Count);
        }

        GameObject temp = Pool[0];
        Pool[0] = Pool[ran];
        Pool[ran] = temp;
    }
}
