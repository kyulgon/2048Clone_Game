using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] n;

    bool wait;
    int x, y;
    Vector3 firstPos, gap;
    GameObject[,] Square = new GameObject[4, 4]; // (4,4)의 배열을 만듦

    
    void Start()
    {
        Spawn();
        Spawn();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

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
                    Debug.Log("위");
                }
                // 아래
                else if (gap.y < 0 && gap.x > -0.5f && gap.x < 0.5f)
                {
                    Debug.Log("아래");
                }
                // 오른쪽
                else if (gap.x > 0 && gap.y > -0.5f && gap.y < 0.5f)
                {
                    Debug.Log("오른쪽");
                }
                // 왼쪽
                else if (gap.x < 0 && gap.y > -0.5f && gap.y < 0.5f)
                {
                    Debug.Log("왼쪽");
                }
                else
                    return;
            }
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
        Square[x, y] = Instantiate(Random.Range(0, 8) > 0 ? n[0] : n[1], new Vector3(1.2f * x - 1.8f, 1.2f * y - 1.8f, 0), Quaternion.identity);
        // 랜덤으로 0에서 7의 수를 골라서 그 수가 1,2,3,4,5,6,7이면 n[0]을 스폰하고
        // 그 수가 0이면 n[1]이 스폰을 한다.

        Square[x, y].GetComponent<Animator>().SetTrigger("Spawn");

    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬의 인덱스 번호로 확인하기
    }
}
