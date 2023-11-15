using System.Collections;
using UnityEngine;

public class Cookie : MonoBehaviour
{
    Board board;

    Vector2 firstTouchPos; // 터치
    Vector2 finalTouchPos;

    public Vector2 MoveVec; // 이동한 방향


    [SerializeField]
    float swipeAngle;

    public int column;
    public int row;


    [SerializeField]
    int TargetX;
    [SerializeField]
    int TargetY;

    public bool isMatched;
    public bool isMovedByOther;

    
    Collider2D coll;


    public Animator anim;

    //-------------------------------------------------- Unity Callbacks
    private void Awake()
    {
        board = FindObjectOfType<Board>();
        
        coll = GetComponent<Collider2D>();

        anim = this.gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        ResetCookie();   
    }

    private void FixedUpdate()
    {
        if (board.state == Board.BoardState.MoveReady) // MoveReady 상태일 때만 터치 인식
        {
            coll.enabled = true;
        }
        else
        {
            coll.enabled = false;
        }
    }

    private void Update()
    {
        if (board.state == Board.BoardState.MoveReady || board.state == Board.BoardState.Move)
        {
            TargetX = column;
            TargetY = row;
        }      
    }

    //-------------------------------------------------- MouseTouch
    private void OnMouseDown()
    {
        if (board.state != Board.BoardState.MoveReady) return;

        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        if (board.state != Board.BoardState.MoveReady) return;

        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        board.state = Board.BoardState.Move;
        

        CalculateAngle();
    }

    //-------------------------------------------------- Invoke State
    void MoveToCheck()
    {
        StopCoroutine("CookieMove");
        board.state = Board.BoardState.Check;
        board.BoardCheck();
    }

    void ReturnToMoveReady()
    {
        StopCoroutine("CookieReturn");
        board.state = Board.BoardState.MoveReady;
    }

    void CookieReturn()
    {
        if (board.allCookies[column, row] != null)
        {
            board.ReturnCookie(column, row, -MoveVec);
        }
    }


    //-------------------------------------------------- Coroutine

    public IEnumerator CookieInitialized(int _column, int _row) // When Cookie Initialize
    {
        Vector2 TargetPos = new Vector2(_column, _row);

        while (Mathf.Abs(TargetPos.y - this.transform.position.y) > 0.3f)
        {
            this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }

        this.transform.position = TargetPos;

        column = _column;
        row = _row;

        anim.Play("CookieSet");
        
    }

    public IEnumerator CookieMove(Vector2 Dir) // When Cookie Move
    {
        Vector2 TargetPos = new Vector2(column, row) + Dir;

        if (board.allCookies[(int)TargetPos.x, (int)TargetPos.y] == null)
        {
            board.state = Board.BoardState.MoveReady;
            yield break;
        }

        if (Dir.y == 0)
        {
            while (Mathf.Abs(TargetPos.x - this.transform.position.x) > 0.1f)
            {
                this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (Dir.x == 0)
        {
            while (Mathf.Abs(TargetPos.y - this.transform.position.y) > 0.1f)
            {
                this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
                yield return new WaitForSeconds(0.02f);
            }
        }

        
        this.transform.position = TargetPos;

        column = (int)TargetPos.x;
        row = (int)TargetPos.y;

        board.allCookies[column, row] = this.gameObject;

        firstTouchPos = Vector2.zero;
        finalTouchPos = Vector2.zero;
        swipeAngle = 0;

        Invoke("MoveToCheck", 0.1f);
    }

    public IEnumerator CookieReturn(Vector2 Dir) // When Cookie Move
    {
        Vector2 TargetPos = new Vector2(column, row) + Dir;

        GameObject otherCookie = board.allCookies[column + (int)Dir.x, row + (int)Dir.y];

        if (otherCookie.GetComponent<Cookie>().isMatched == true) yield break;

        if (Dir.y == 0 && Dir.x !=0)
        {
            while (Mathf.Abs(TargetPos.x - this.transform.position.x) > 0.1f)
            {
                this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (Dir.x == 0 && Dir.y !=0)
        {
            while (Mathf.Abs(TargetPos.y - this.transform.position.y) > 0.1f)
            {
                this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
                yield return new WaitForSeconds(0.02f);
            }
        }

        this.transform.position = TargetPos;

        column = (int)TargetPos.x;
        row = (int)TargetPos.y;
        

        board.allCookies[column, row] = this.gameObject;

        ResetCookie();

    }

    public IEnumerator Co_CookieAutoFall(int _dirRow) // When Cookie Move
    {
        Vector2 TargetPos = new Vector2(column, _dirRow);
        int prevCol = column;
        int prevRow = row;

        while (Mathf.Abs(TargetPos.y - this.transform.position.y) > 0.1f)
        {
            this.transform.position = Vector2.Lerp((Vector2)this.transform.position, TargetPos, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }
        

        this.transform.position = TargetPos;

        column = (int)TargetPos.x;
        row = (int)TargetPos.y;

        board.allCookies[column, row] = this.gameObject;
        board.allCookies[prevCol, prevRow] = null;
    }




    //-------------------------------------------------- Move
    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;

        Vector2 Dir = GetWay();

        MoveVec = Dir;
        board.allCookieMove(column, row, Dir);
    }


    public Vector2 GetWay()
    {
        if ((-45 < swipeAngle && swipeAngle <= 45) && swipeAngle != 0 && column + 1 < board.width && !(board.allCookies[column + 1, row] == null)) // 1. 오른쪽
        {
            return new Vector2(1, 0);
        }
        else if ((45 < swipeAngle && swipeAngle <= 135) && row + 1 < board.height && !(board.allCookies[column, row + 1] == null)) // 2. 위쪽
        {
            return new Vector2(0, 1);
        }
        else if ((135 < swipeAngle || swipeAngle <= -135) && column > 0 && !(board.allCookies[column - 1, row] == null)) // 3. 왼쪽
        {
            return new Vector2(-1, 0);
        }
        else if ((-135 < swipeAngle && swipeAngle <= -45) && row > 0 && !(board.allCookies[column, row - 1] == null)) // 4. 아래쪽
        {
            return new Vector2(0, -1);
        }
        else
        {
            return new Vector2(0, 0);
        }
        
    }



    public void MatchCheck()
    {
        int _column = column;
        int _row = row;

        // allCookies[_column, _row] 가로 1칸 유효성 검사
        if ((0 < _column && _column < board.width - 1) && ( -1 <_row && _row < board.height) && board.allCookies[_column + 1, _row] != null && board.allCookies[_column - 1, _row] != null)
        {
            // allCookies[_column, _row] 가로 3매치 검사
            if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column + 1, _row].tag) && board.allCookies[_column, _row].CompareTag(board.allCookies[_column - 1, _row].tag))
            {
                
                board.allCookies[_column + 1, _row].GetComponent<Cookie>().isMatched = true;
                board.allCookies[_column - 1, _row].GetComponent<Cookie>().isMatched = true;

                
                if (!board.deleteVecList.Contains(new Vector2(_column + 1, _row)))
                {
                    board.deleteVecList.Add(new Vector2(_column + 1, _row));
                }
                if (!board.deleteVecList.Contains(new Vector2(_column - 1, _row)))
                {
                    board.deleteVecList.Add(new Vector2(_column - 1, _row));
                }


                // allCookies[_column, _row] 가로 2칸 유효성 검사
                if ((1 < _column && _column < board.width - 2) && board.allCookies[_column + 2, _row] != null)
                {
                    // allCookies[_column, _row] 가로 4매치이상 검사
                    if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column + 2, _row].tag))
                    {
                        if (!board.deleteVecList.Contains(new Vector2(_column + 2, _row)))
                        {
                            board.allCookies[_column + 2, _row].GetComponent<Cookie>().isMatched = true;
                            board.deleteVecList.Add(new Vector2(_column + 2, _row));
                        }

                    }
                }
                // allCookies[_column, _row] 가로 2칸 유효성 검사
                if ((1 < _column && _column < board.width - 2) && board.allCookies[_column - 2, _row] != null)
                {
                    // allCookies[_column, _row] 가로 4매치이상 검사
                    if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column - 2, _row].tag))
                    {
                        if (!board.deleteVecList.Contains(new Vector2(_column - 2, _row)))
                        {
                            board.allCookies[_column - 2, _row].GetComponent<Cookie>().isMatched = true;
                            board.deleteVecList.Add(new Vector2(_column - 2, _row));
                        }
                    }
                }

                this.isMatched = true;
                if (!board.deleteVecList.Contains(new Vector2(column, row)))
                {
                    board.deleteVecList.Add(new Vector2(column, row));
                }
            }
        }

        // 가로 체크 먼저 하고 세로 체크
        // allCookies[_column, _row] 세로 1칸 유효성 검사
        if ((0 < _row && _row < board.height - 1) && (-1 < _column && _column < board.width) && board.allCookies[_column, _row + 1] != null && board.allCookies[_column, _row - 1] != null)
        {
            // allCookies[_column, _row] 세로 3매치 검사
            if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column, _row + 1].tag) && board.allCookies[_column, _row].CompareTag(board.allCookies[_column, _row - 1].tag))
            {
                board.allCookies[_column, _row + 1].GetComponent<Cookie>().isMatched = true;
                board.allCookies[_column, _row - 1].GetComponent<Cookie>().isMatched = true;


                if (!board.deleteVecList.Contains(new Vector2(_column, _row + 1)))
                {
                    board.deleteVecList.Add(new Vector2(_column, _row + 1));
                }
                if (!board.deleteVecList.Contains(new Vector2(_column, _row - 1)))
                {
                    board.deleteVecList.Add(new Vector2(_column, _row - 1));
                }


                // allCookies[_column, _row] 세로 2칸 유효성 검사
                if ((1 < _row && _row < board.height - 2) && board.allCookies[_column, _row + 2] != null)
                {
                    // allCookies[_column, _row] 세로 4매치이상 검사
                    if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column, _row + 2].tag))
                    {
                        if (!board.deleteVecList.Contains(new Vector2(_column, _row + 2)))
                        {
                            board.allCookies[_column, _row + 2].GetComponent<Cookie>().isMatched = true;
                            board.deleteVecList.Add(new Vector2(_column, _row + 2));
                        }
                    }
                }
                // allCookies[_column, _row] 세로 2칸 유효성 검사
                if ((1 < _row && _row < board.height - 2) && board.allCookies[_column, _row - 2] != null)
                {
                    // allCookies[_column, _row] 세로 4매치이상 검사
                    if (board.allCookies[_column, _row].CompareTag(board.allCookies[_column, _row - 2].tag))
                    {
                        if (!board.deleteVecList.Contains(new Vector2(_column, _row - 2)))
                        {
                            board.allCookies[_column, _row - 2].GetComponent<Cookie>().isMatched = true;
                            board.deleteVecList.Add(new Vector2(_column, _row - 2));
                        }
                    }
                }

                this.isMatched = true;
                if (!board.deleteVecList.Contains(new Vector2(column, row)))
                {
                    board.deleteVecList.Add(new Vector2(column, row));
                }
            }
        }

        if (this.isMatched == true)
        {
            anim.SetTrigger("Pang");
        }
        else if (this.isMatched == false && MoveVec != Vector2.zero) 
        {
            Invoke("InvokeReturnCookie",0.5f);
        }
    }

    void InvokeReturnCookie()
    {
        board.ReturnCookie(column, row, MoveVec);
    }

    public void CookieAutoFall() // When Bottom Cookie Destroyed
    {
        int dirRow = 0;

        for (int i = 0; i < board.height; i++)
        {
            dirRow += 1;
            if (board.allCookies[column, i] == null) break;
        }

        Co_CookieAutoFall(dirRow);
    }

    private void ResetCookie()
    {
        MoveVec = Vector2.zero;
        firstTouchPos = Vector2.zero;
        finalTouchPos = Vector2.zero;
        swipeAngle = 0;
    }

}
