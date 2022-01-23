using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    public Transform teleportCircleUI; //텔레포트를 표시할 UI
    LineRenderer lr; //선을 그릴 라인 렌더러
    Vector3 originScale = Vector3.one * 0.02f;
    public int lineSmooth = 40; //커브의 부드러운 정도
    public float curveLength = 50; //커브의 길이
    public float gravity = -60; //커브의 중력
    public float simulateTime = 0.02f; //곡선 시뮬레이션의 간격 및 시간
    List<Vector3> lines = new List<Vector3>(); //곡선을 이루는 점들을 기억할 리스트
    
    void Start()
    {
        //시작할 때 비활성화
        teleportCircleUI.gameObject.SetActive(false);
        //라인 렌더러 컴포넌트 얻어오기
        lr = GetComponent<LineRenderer>();
        //라인 렌더러의 선 너비 지정
        lr.startWidth = 0.0f;
        lr.endWidth = 0.2f;
    }

    void Update()
    {
        //왼쪽 컨트롤러이 One 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            //라인 렌더러 컴포넌트 활성화
            lr.enabled = true;
        }
        //왼족 컨트롤러의 One 버튼에서 손을 떼면
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            //라인 렌더러 비활성화
            lr.enabled = false;
            //텔레포트 UI가 활성화 돼있을 때
            if (teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                //텔레포트 UI 위치로 순간 이동
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;
            }
            //텔레포트 UI 비활성화
            teleportCircleUI.gameObject.SetActive(false);
        }
        //왼쪽 컨트롤러의 One 버튼을 누르고 있을 때
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            //주어진 길이 크기의 커브를 만들고 싶다
            MakeLines();
        }
    }
    //라인 렌더러를 이용해 점을 만들고 선을 그린다
    void MakeLines()
    {
        //리스트에 담긴 위치 정보들을 비워준다
        lines.RemoveRange(0, lines.Count);
        //선이 진행될 방향을 정한다
        Vector3 dir = ARAVRInput.LHandDirection * curveLength;
        //선이 그려질 위치의 초깃값을 설정한다
        Vector3 pos = ARAVRInput.LHandPosition;
        //최초 위치를 리스트에 담는다
        lines.Add(pos);

        //lineSmooth 개수만큼 반복한다
        for(int i = 0; i < lineSmooth; i++)
        {
            //현재 위치 기억
            Vector3 lastPos = pos;
            //중력을 적용한 속도 계산 v = v0 + at
            dir.y += gravity * simulateTime;
            //등속 운동으로 다음 위치 계산 p = p0 + vt
            pos += dir * simulateTime;
            //구한 위치를 등록
            lines.Add(pos);
        }
        //라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당
        lr.positionCount = lines.Count;
        //라인 렌더러에 구해진 점의 정보를 지정
        lr.SetPositions(lines.ToArray());
    }
}
