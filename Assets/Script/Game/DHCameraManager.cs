/******************************************************************************
 * 
 *   이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
 * 
 *   소스 코드에 대한 모든 권리는 (주) 앱크로스에 있습니다.
 *   Copyright 2012 (c) Appcross All Rights Reserved.
 * 
 *   E-Mail : admin@appcross.co.kr
 * 
 ******************************************************************************/

using UnityEngine;
using System.Collections;

using DHTimeSingle = System.Single;
using UnityEngine.EventSystems;

//----------------------------------------------------------------------------*/
public class DHCameraManager : MonoBehaviour
{
    //----------------------------------------------------------------------------*/

    public enum State
    {

        DoNothing,

        MoveWithDragging,

        FollowTarget,

        OnForcedMove,
    }


    private const float DRAGGING_SPEED_DELTA_X = 2.0f;
    /* 카메라를 드래그로 이동할 때 속도 변화량(Z축)
     * */
    private const float DRAGGING_SPEED_DELTA_Z = 2.0f;

    /* 카메라의 시야 확대 / 축소할 때 속도 변화량

     * */
    private const float ZOOM_SPEED = 15.0f;

    /* 카메라 객체 이름

     * */
    private const string GAMEOBJECTNAME_CAMERA = "Camera";

    //----------------------------------------------------------------------------*/
    /* 싱글톤 인스턴스 획득

    //----------------------------------------------------------------------------*/
    public static DHCameraManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    //----------------------------------------------------------------------------*/
    public static void SetBlockTouchForSkill(bool Value)
    {
        if (singleton != null)
        {
            singleton.blockMoveEventForSkillTouch = Value;
        }
    }

    //----------------------------------------------------------------------------*/
    public static void SetFollowingTarget(UnityEntity Target)
    {
        if (singleton != null)
        {
            singleton.followingTarget = Target;
        }
    }

    //----------------------------------------------------------------------------*/
    public static void SetCameraType(State CameraState)
    {
        if (singleton != null)
        {
            singleton.currentState = CameraState;
        }
    }

    //----------------------------------------------------------------------------*/
    public static void ViewCameraToTargetCharacter()
    {
        
        //Debug.Log("ViewCameraToTargetCharacter");
        if (singleton.followingTarget == null)
        {
            return;
        }

        if (singleton.followingTarget.GetRenderingTransform == null)
        {
            return;
        }




        Vector3 NowPos = singleton.followingTarget.GetRenderingTransform.position;

        //Debug.Log(" ViewCameraToTargetCharacter position:" + NowPos);
        NowPos.y = singleton.transformOnMap.position.y;
        NowPos.z -= 6;

        //    protected Vector2 m_CameraMoveOffset;

        //protected Vector2 m_MouseStartPos;
        //protected Vector2 m_MouseEndPos;
        NowPos.x -= (singleton.m_TempCameraMoveOffset.x + singleton.m_CameraMoveOffset.x)* singleton.m_MoveSpeed;
        NowPos.z -= (singleton.m_TempCameraMoveOffset.y + singleton.m_CameraMoveOffset.y) * singleton.m_MoveSpeed;

        singleton.transformOnMap.position = NowPos;
    }

    //----------------------------------------------------------------------------*/
    public static void SetCameraToPosition(Vector3 Position)
    {
        if (singleton != null)
        {
            singleton.transformOnMap.position = Position;
        }
    }



    //----------------------------------------------------------------------------*/
    /* 피가 없을때 맞으면 뜨는 이펙트

    //----------------------------------------------------------------------------*/
    public void ShowIllEffect()
    {
        SendMessage("OnCustomTriggerWithValue",
                    "ShowBloodScreen",
                    SendMessageOptions.DontRequireReceiver);
    }

    //----------------------------------------------------------------------------*/
    public Transform TransformOnMap
    {
        get
        {
            return transformOnMap;
        }
    }

    //----------------------------------------------------------------------------*/
    public State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    //----------------------------------------------------------------------------*/
    private void Awake()
    {
        singleton = this;

        transformOnMap = transform;
    }

    //----------------------------------------------------------------------------*/
    private void Start()
    {
        /* 카메라 객체 찾아서 등록하기

         * */
        //Transform CameraTransform = transformOnMap.FindChild(GAMEOBJECTNAME_CAMERA);
        //attachedCamera = CameraTransform.GetComponent<Camera>();

        started = true;
    }

    //----------------------------------------------------------------------------*/
    private bool Initialize()
    {
        if (!started)
        {
            return false;
        }



        initialized = true;

        SetCameraType(State.FollowTarget);

        //DHUnitEntity MyCharacter = DHScene_GamePlay.Singleton.myCharacter;
        //DHCameraManager.SetFollowingTarget(MyCharacter);

        return true;
    }

    //----------------------------------------------------------------------------*/
    private void OnDestroy()
    {
        singleton = null;
    }



    //回到角色中心
    public void Move2UnitCenter()
    {
        m_CameraMoveOffset = new Vector2(0, 0);
    }
    public static float MaxMoveHeight = 6;
    public static float MaxMoveWidth = 9;

    //增加摄像机移动偏移
    public void AddCameraMoveOffset(Vector2 offset)
    {
        m_CameraMoveOffset += offset;
        m_TempCameraMoveOffset = new Vector2(0, 0);

        float x = (singleton.m_TempCameraMoveOffset.x + singleton.m_CameraMoveOffset.x) * singleton.m_MoveSpeed;
        float z = (singleton.m_TempCameraMoveOffset.y + singleton.m_CameraMoveOffset.y) * singleton.m_MoveSpeed;
        if (Mathf.Abs(x) >= MaxMoveWidth)
        {
            singleton.m_CameraMoveOffset.x = (MaxMoveWidth * (x / Mathf.Abs(x)) / singleton.m_MoveSpeed - singleton.m_TempCameraMoveOffset.x);
        }
        if (Mathf.Abs(z) >= MaxMoveHeight)
        {
            singleton.m_CameraMoveOffset.y = (MaxMoveHeight * (z / Mathf.Abs(z)) / singleton.m_MoveSpeed - singleton.m_TempCameraMoveOffset.y);
        }
    }
    //增加暂时摄像机移动偏移
    public void SetTempCameraMoveOffset(Vector2 offset)
    {
        m_TempCameraMoveOffset = offset;

        float x = (singleton.m_TempCameraMoveOffset.x + singleton.m_CameraMoveOffset.x) * singleton.m_MoveSpeed;
        float z = (singleton.m_TempCameraMoveOffset.y + singleton.m_CameraMoveOffset.y) * singleton.m_MoveSpeed;
        if( Mathf.Abs(x) >= MaxMoveWidth)
        {
            singleton.m_TempCameraMoveOffset.x = (MaxMoveWidth * (x/ Mathf.Abs(x)) / singleton.m_MoveSpeed - singleton.m_CameraMoveOffset.x);
        }
        if (Mathf.Abs(z) >= MaxMoveHeight)
        {
            singleton.m_TempCameraMoveOffset.y = (MaxMoveHeight * (z/Mathf.Abs(z)) / singleton.m_MoveSpeed - singleton.m_CameraMoveOffset.y);
        }

        //Debug.Log(" SetTempCameraMoveOffset:" + m_TempCameraMoveOffset);
    }

    protected Vector2 m_CameraMoveOffset;
    protected Vector2 m_TempCameraMoveOffset;
    
    protected float m_MoveSpeed = 0.02f;
    public void CameraMove()
    {

        //if (EventSystem.current.IsPointerOverGameObject())
        //{

        //    return;
        //}

        //if (Input.GetMouseButtonDown(0))
        ////按下鼠标左键  
        //{
        //    Vector3 mousePosition = Input.mousePosition;
        //    m_MouseStartPos = new Vector2(mousePosition.x, mousePosition.y);
        //    m_MouseEndPos = new Vector2(mousePosition.x, mousePosition.y);
        //}


        //if (Input.GetMouseButton(0))
        ////持续按下鼠标左键  
        //{
        //    Vector3 mousePosition = Input.mousePosition;
        //    m_MouseEndPos = new Vector2(mousePosition.x, mousePosition.y);
        //}
        //if (Input.GetMouseButtonUp(0))
        //{
        //    m_CameraMoveOffset += (m_MouseEndPos - m_MouseStartPos);
        //    m_MouseStartPos = new Vector2(0, 0);
        //    m_MouseEndPos = new Vector2(0, 0);
        //}
    }

    //----------------------------------------------------------------------------*/
    private void Update()
    {
        if (!initialized)
        {
            if (!Initialize())
            {
                return;
            }
        }
        else
        {
            CameraMove();
        }
    }

    //----------------------------------------------------------------------------*/
    private void LateUpdate()
    {
        switch (currentState)
        {
            case State.DoNothing:
                {

                }
                break;
            case State.FollowTarget:
                {
                    OnFollowTargetUpdate();
                }
                break;
            case State.MoveWithDragging:
                {

                }
                break;
            case State.OnForcedMove:
                {
                }
                break;
        }
    }

    //----------------------------------------------------------------------------*/
    /* State 처리들 모음

    //----------------------------------------------------------------------------*/
    private void OnFollowTargetUpdate()
    {
        ViewCameraToTargetCharacter();
    }



    public Vector3 CameraColiderSize()
    {
        return col.size;
    }

    //----------------------------------------------------------------------------*/
    /* 맴버 변수

    //----------------------------------------------------------------------------*/
    private static DHCameraManager singleton = null;

    /* 지형맵 상의 위치 변환 객체

     * */
    private Transform transformOnMap = null;
    /* 현재 상태

     * */
    private State currentState = State.MoveWithDragging;
    //private State beforeState = State.MoveWithDragging;
    /* 따라다니는 대상

     * */
    private UnityEntity followingTarget = null;

    private bool isWorldToScreenRatioCalculated = false;
    private Vector2 worldToScreenRatio = Vector2.zero;
    private bool blockMoveEventForSkillTouch = false;


    /// [ Camera 객체 관련 ]

    /* 카메라 오브젝트(transformOnMap을 부모로 둔다)
     * */
    //private Camera attachedCamera = null;
    /* 카메라 오브젝트가 위치할 수 있는 최소 거리 위치를 표시하는 객체

     * */
    private Transform cameraLocalCoordMin = null;
    /* 카메라 오브젝트가 위치할 수 있는 최대 거리 위치를 표시하는 객체

     * */
    private Transform cameraLocalCoordMax = null;


    /// [ 기타 ]

    /* Start() 함수의 성공 여부

     * */
    private bool started = false;
    /* 초기화 여부

     * */
    private bool initialized = false;

    private BoxCollider col = null;
}
