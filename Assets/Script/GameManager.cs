using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] n;
    public GameObject Quit;
    public Text Score, BestScore, Plus;

    bool wait, move, stop;
    int x, y, i, j, k, l, score;
    Vector3 firstPos, gap;
    GameObject[,] Square = new GameObject[4, 4]; // (4,4)의 배열을 만듦

    
    void Start()
    {
        Spawn();
        Spawn();
        BestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // 뒤로가기
            Application.Quit();

        if(stop)
        {
            return;
        }

        
        if(Input.GetMouseButtonDown(0) || (Input.touchCount) == 1 && Input.GetTouch(0).phase == TouchPhase.Began) // 컴터인지 모바일인지 확인하기
        {
            wait = true;
            firstPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
                       
        }

        if(Input.GetMouseButton(0) || (Input.touchCount) == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) // gap 값 만들기
        {
            gap = (Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position) - firstPos;

            if (gap.magnitude < 100) return;
            gap.Normalize(); // 방향만 표시
            
            if(wait)
            {
                wait = false;

                // 위
                if (gap.y > 0 && gap.x > -0.5f && gap.x < 0.5f)
                {
                    for (x = 0; x <= 3; x++)
                        for (y = 0; y <= 2; y++)
                            for (i = 3; i >= y + 1; i--)
                                MoveOrCombine(x, i - 1, x, i);
                }
                // 아래
                else if (gap.y < 0 && gap.x > -0.5f && gap.x < 0.5f)
                {
                    for (x = 0; x <= 3; x++)
                        for (y = 3; y >= 1; y--)
                            for (i = 0; i <= y - 1; i++)
                                MoveOrCombine(x, i + 1, x, i);
                }
                // 오른쪽
                else if (gap.x > 0 && gap.y > -0.5f && gap.y < 0.5f)
                {
                    for (y = 0; y <= 3; y++)
                        for (x = 0; x <= 2; x++)
                            for (i = 3; i >= x + 1; i--)
                                MoveOrCombine(i - 1, y, i, y);
                }
                // 왼쪽
                else if (gap.x < 0 && gap.y > -0.5f && gap.y < 0.5f)
                {
                    for (y = 0; y <= 3; y++)
                        for (x = 3; x >= 1; x--)
                            for (i = 0; i <= x - 1; i++)
                                MoveOrCombine(i + 1, y, i, y);
                }
                else
                    return;

                if(move)
                {
                    move = false;
                    Spawn();
                    k = 0;
                    l = 0;

                    // 점수 구현
                    if (score > 0)
                    {
                        Plus.text = "+" + score.ToString() + "    ";
                        Plus.GetComponent<Animator>().SetTrigger("PlusBack");
                        Plus.GetComponent<Animator>().SetTrigger("Plus");
                        Score.text = (int.Parse(Score.text) + score).ToString();

                        if(PlayerPrefs.GetInt("BestScore", 0) < int.Parse(Score.text))
                        {
                            PlayerPrefs.SetInt("BestScore", int.Parse(Score.text));
                        }

                        BestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
                        score = 0;
                    }

                    for(x =0; x <=3; x++)
                        for(y =0; y<=3; y++)
                        {
                            if (Square[x, y] == null)   // 모든 타일이 가득 차면 k가 0이 됨
                            {
                                k++;
                                continue;
                            }
                                
                            if (Square[x, y].tag == "Combine")
                            {
                                Square[x, y].tag = "Untagged";
                            }
                                
                        }
                    if(k==0) // 가로, 세로 같은 블럭이 없으면 0이 되어서 게임 오버
                    {
                        for(y=0; y<=3; y++)
                            for(x=0; x<=2; x++)
                            {
                                if(Square[x,y].name == Square[x+1,y].name)
                                {
                                    l++;
                                }
                            }

                        for(x=0; x<=3; x++)
                            for(y=0; y<=2; y++)
                                if(Square[x,y].name == Square[x, y+1].name)
                                {
                                    l++;
                                }

                        if (l == 0)
                        {
                            stop = true;
                            Quit.SetActive(true);
                            return;
                        }
                    }

                }
            }
        }
    }

    void MoveOrCombine(int x1, int y1, int x2, int y2) // [x1, y1] 이동전 좌표, [x2, y2] 이동 될 좌표
    {
        if(Square[x2, y2] == null && Square[x1, y1] != null) // 이동될 좌표가 비어있고, 이동 전 좌표에 오브젝트가 존재하면 이동
        {
            move = true;
            Square[x1, y1].GetComponent<Moving>().Move(x2, y2, false);
            Square[x2, y2] = Square[x1, y1];
            Square[x1, y1] = null;
        }

        //둘다 같은 수일때 결합
        if(Square[x1, y1] != null && Square[x2, y2] != null && Square[x1, y1].name == Square[x2, y2].name && Square[x1, y1].tag != "Combine" && Square[x2, y2].tag != "Combine")
        {
            move = true;
            for(j = 0; j <= 16; j++)
                if(Square[x2, y2].name == n[j].name + "(Clone)")
                {
                    break;
                }
            Square[x1, y1].GetComponent<Moving>().Move(x2, y2, true);
            Destroy(Square[x2, y2]);
            Square[x1, y1] = null;
            Square[x2, y2] = Instantiate(n[j + 1], new Vector3(1.2f * x2 - 1.8f, 1.2f * y2 - 1.8f, 0), Quaternion.identity);
            Square[x2, y2].tag = "Combine";
            Square[x2, y2].GetComponent<Animator>().SetTrigger("Combine");
            score += (int)Mathf.Pow(2, j + 2);
        }
    }

    void Spawn()
    {
        while(true)
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);

            if(Square[x, y] == null) // 만들어진 [x, y]에 프리팹이 있으면 GameObject이고 없으면 null값 반환
            {
                break;
            }
        }

        // (조건식)? (참일때의 값) : (거짓일 때의 값)
        Square[x, y] = Instantiate(Random.Range(0, int.Parse(Score.text) > 800 ? 4 : 8) > 0 ? n[0] : n[1], new Vector3(1.2f * x - 1.8f, 1.2f * y - 1.8f, 0), Quaternion.identity);
        // 랜덤으로 0에서 7의 수를 골라서 그 수가 1,2,3,4,5,6,7이면 n[0]을 스폰하고
        // 그 수가 0이면 n[1]이 스폰을 한다.

        Square[x, y].GetComponent<Animator>().SetTrigger("Spawn");

    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬의 인덱스 번호로 확인하기
    }
}
