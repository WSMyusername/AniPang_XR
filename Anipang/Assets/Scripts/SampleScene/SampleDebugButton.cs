using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class SampleDebugButton : MonoBehaviour
{
    SampleBoard board;

    public TMP_InputField inputX;
    public TMP_InputField inputY;

    int X;
    int Y;


    private void Start()
    {
        board = FindObjectOfType<SampleBoard>();
    }
    public void Btn_GetBoardData()
    {
        if(board.allCookies[X, Y] != null)
        {
            Debug.Log($"{X},{Y} : {board.allCookies[X, Y].name}, {board.allCookies[X, Y].tag}");
        }
        else
        {
            Debug.Log($"{X},{Y} : null");
        }        
    }

    public void Btn_PooledObjs()
    {
        for (int i = 0; i < board.pooledObjs.Count; i++)
        {
            Debug.Log($"{i}번째 오브젝트 : {board.pooledObjs[i].tag}");
        }
    }

    public void OnInputXFin()
    {
        X = int.Parse(inputX.text);
    }

    public void OnInputYFin()
    {
        Y = int.Parse(inputY.text);
    }

    public void Btn_OnRestart()
    {
        SceneManager.LoadScene(0);
    }

}
