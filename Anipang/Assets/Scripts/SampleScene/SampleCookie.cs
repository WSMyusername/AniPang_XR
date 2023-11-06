
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SampleCookie : MonoBehaviour //매치 체크, 
{
    SampleBoard board;
    GameObject otherCookie;

    public int column;
    public int row;

    public int TargetX;
    public int TargetY;

    public float swipeAngle = 0;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;

    public bool isMatched = false;

    public bool isMoving = false;

    public bool isReGenerated = false;

    //int[] SearchX = { 0, 1, 0, -1, 0, 2, 0, -2 };
    //int[] SearchY = { 1, 0, -1, 0, 2, 0, -2, 0 };

    private void Start()
    {
        board = FindObjectOfType<SampleBoard>();

        TargetX = (int)this.gameObject.transform.position.x;
        TargetY = (int)this.gameObject.transform.position.y;

        column = TargetX;
        row = TargetY;
    }

    private void Update()
    {
        if (isReGenerated == false)
        {
            TargetX = column;
            TargetY = row;

            CookieObjMove();

            if (isMoving == false)
            {
                MatchCheck();
            }

            if (row > 0 && board.allCookies[column, row - 1] == null)
            {
                isMoving = true;
                StartCoroutine(CookieFall());
            }
        }
    }

    void MoveRight()
    {
        otherCookie = board.allCookies[column + 1, row];
        otherCookie.GetComponent<SampleCookie>().MoveByOther("right");
        column += 1;

    }
    void MoveLeft()
    {


        otherCookie = board.allCookies[column - 1, row];
        otherCookie.GetComponent<SampleCookie>().MoveByOther("left");
        column += -1;

    }
    void MoveUp()
    {

        otherCookie = board.allCookies[column, row + 1];
        otherCookie.GetComponent<SampleCookie>().MoveByOther("up");
        row += 1;

    }
    void MoveDown()
    {

        otherCookie = board.allCookies[column, row - 1];
        otherCookie.GetComponent<SampleCookie>().MoveByOther("down");
        row += -1;

    }



    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isMoving = true;
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

        CookieMove();
    }

    void CookieMove()
    {

        if ((-45 < swipeAngle && swipeAngle <= 45) && swipeAngle != 0 && column + 1 < board.width && !(board.allCookies[column + 1, row] == null)) // 1. 오른쪽
        {

            MoveRight();

        }
        else if ((45 < swipeAngle && swipeAngle <= 135) && row + 1 < board.height && !(board.allCookies[column, row + 1] == null)) // 2. 위쪽
        {
            MoveUp();

        }
        else if ((135 < swipeAngle || swipeAngle <= -135) && column > 0 && !(board.allCookies[column - 1, row] == null)) // 3. 왼쪽
        {
            MoveLeft();

        }
        else if ((-135 < swipeAngle && swipeAngle <= -45) && row > 0 && !(board.allCookies[column, row - 1] == null)) // 4. 아래쪽
        {

            MoveDown();

        }

        otherCookie = null;
        swipeAngle = 0;
    }


    void CookieObjMove()
    {
        if (Mathf.Abs(TargetX - transform.position.x) > 0.1f) // X move
        {
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
        }
        else
        {
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = tempPosition;
            board.allCookies[column, row] = this.gameObject;
            isMoving = false;
        }


        if (Mathf.Abs(TargetY - transform.position.y) > 0.1f) // Y move
        {
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = tempPosition;
            board.allCookies[column, row] = this.gameObject;
            isMoving = false;
        }



    }
    void MatchCheck()
    {
        if (((column - 1) >= 0 && (column + 1) <= board.width - 1) && board.allCookies[column + 1, row] != null && board.allCookies[column - 1, row] != null) // 가로 체크
        {
            if (board.allCookies[column + 1, row].CompareTag(board.allCookies[column, row].tag) && board.allCookies[column - 1, row].CompareTag(board.allCookies[column, row].tag)) // 가로 3매치
            {
                isMatched = true;

                if (!board.VecList.Contains(new Vector2(column + 1, row)))
                {
                    board.allCookies[column + 1, row].GetComponent<SampleCookie>().MatchByOther(column + 1, row);
                }
                if (!board.VecList.Contains(new Vector2(column - 1, row)))
                {
                    board.allCookies[column - 1, row].GetComponent<SampleCookie>().MatchByOther(column - 1, row);
                }


                if (!((column + 2) > board.width - 1) && board.allCookies[column + 2, row] != null) // 가로 3 이상 매치
                {
                    if (board.allCookies[column + 2, row].CompareTag(board.allCookies[column + 1, row].tag) && board.allCookies[column + 1, row].CompareTag(board.allCookies[column, row].tag))
                    {
                        if (!board.VecList.Contains(new Vector2(column + 2, row)))
                        {
                            board.allCookies[column + 2, row].GetComponent<SampleCookie>().MatchByOther(column + 2, row);
                        }
                    }
                    isMatched = true;
                }
                if (!((column - 2) < 0) && board.allCookies[column - 2, row] != null)
                {
                    if (board.allCookies[column - 2, row].CompareTag(board.allCookies[column - 1, row].tag) && board.allCookies[column - 1, row].CompareTag(board.allCookies[column, row].tag))
                    {
                        if (!board.VecList.Contains(new Vector2(column - 2, row)))
                        {
                            board.allCookies[column - 2, row].GetComponent<SampleCookie>().MatchByOther(column - 2, row);
                        }
                    }
                    isMatched = true;
                }
            }
        }

        if (((row - 1) >= 0 && (row + 1) <= board.height - 1) && board.allCookies[column, row + 1] != null && board.allCookies[column, row - 1] != null) // 세로 체크
        {
            if (board.allCookies[column, row + 1].CompareTag(board.allCookies[column, row].tag) && board.allCookies[column, row - 1].CompareTag(board.allCookies[column, row].tag)) // 세로 3매치
            {
                isMatched = true;

                if (!board.VecList.Contains(new Vector2(column, row + 1)))
                {
                    board.allCookies[column, row + 1].GetComponent<SampleCookie>().MatchByOther(column, row + 1);
                }
                if (!board.VecList.Contains(new Vector2(column, row - 1)))
                {
                    board.allCookies[column, row - 1].GetComponent<SampleCookie>().MatchByOther(column, row - 1);
                }


                if (!((row + 2) > board.height - 1) && board.allCookies[column, row + 2] != null) // 세로 4 이상 매치
                {
                    if (board.allCookies[column, row + 2].CompareTag(board.allCookies[column, row + 1].tag) && board.allCookies[column, row + 1].CompareTag(board.allCookies[column, row].tag))
                    {
                        if (!board.VecList.Contains(new Vector2(column, row + 2)))
                        {
                            board.allCookies[column, row + 2].GetComponent<SampleCookie>().MatchByOther(column, row + 2);
                        }
                    }
                    isMatched = true;
                }
                if (!((row - 2) < 0) && board.allCookies[column, row - 2] != null)
                {
                    if (board.allCookies[column, row - 2].CompareTag(board.allCookies[column, row - 1].tag) && board.allCookies[column, row - 1].CompareTag(board.allCookies[column, row].tag))
                    {
                        if (!board.VecList.Contains(new Vector2(column, row - 2)))
                        {
                            board.allCookies[column, row - 2].GetComponent<SampleCookie>().MatchByOther(column, row - 2);
                        }
                    }
                    isMatched = true;
                }

            }
        }


        if (isMatched == true)
        {
            if (!board.VecList.Contains(new Vector2(column, row)))
            {
                board.VecList.Add(new Vector2(column, row));
                isMatched = false;
            }
        }

    }


    IEnumerator CookieFall()
    {
        Debug.Log("CookieFall");

        if (board.allCookies[column, row] != null)
        {
            row -= 1;

            board.allCookies[column, row] = this.gameObject;
            board.allCookies[column, row + 1] = null;


            yield return new WaitForSeconds(0.5f);

            isMoving = false;
        }
    }

    public void MoveByOther(string dir)
    {
        switch (dir)
        {
            case "right":
                column -= 1;
                board.allCookies[column, (int)this.transform.position.y] = this.gameObject;
                break;

            case "left":
                column += 1;
                board.allCookies[column, (int)this.transform.position.y] = this.gameObject;
                break;

            case "down":
                row += 1;
                board.allCookies[(int)this.transform.position.x, row] = this.gameObject;
                break;

            case "up":
                row -= 1;
                board.allCookies[(int)this.transform.position.x, row] = this.gameObject;
                break;
        }
    }

    public void MatchByOther(int _column, int _row)
    {
        if (this.gameObject == board.allCookies[_column, _row] && board.allCookies[_column, _row] != null)
        {
            isMatched = true;
            board.VecList.Add(new Vector2(_column, _row));
        }
    }

    public void ReGenerated(int _x)
    {
        isReGenerated = true;
        this.gameObject.transform.position = new Vector2(_x, 10);
        StartCoroutine(SetBottom(_x));
    }

    IEnumerator SetBottom(int _x)
    {
        yield return new WaitForSeconds(0.3f);


        for (int i = 0; i < board.height; i++)
        {
            if (board.allCookies[_x, i] == null)
            {
                column = _x;
                row = i;

                break;
            }
            else continue;
        }


        if (Mathf.Abs(row - this.transform.position.y) > 0.1f)
        {
            Vector2 temp = new Vector2(column, transform.position.y);
            this.transform.position = Vector2.Lerp(transform.position, temp, 0.1f);
        }
        else
        {
            Vector2 temp = new Vector2(column, transform.position.y);
            transform.position = temp;
            board.allCookies[column, row] = this.gameObject;
            Debug.Log("Coroutine comfirm");

            isReGenerated = false;
            isMoving = true;
        }
    }


}
