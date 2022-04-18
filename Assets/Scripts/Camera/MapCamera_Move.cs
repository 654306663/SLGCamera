// =======================================================
// 文件：MapCamera_Move.cs
// 描述：相机移动
// 作者：王冲
// 创建时间：2021/11/11 9:12:27
// =======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class MapCamera : MonoBehaviour
    {
        private Vector3 cameraPositionTemp;
        [Header("相机移动参数，Lerp时间")]
        [Space(20)]
        [SerializeField] private float cameraMoveLerpTime = 10f;
        private float cameraMoveLerpTimeTemp = 10f;
        [Header("相机移动参数，系数")]
        [SerializeField] private float moveCo = 1f;
        [Header("相机移动参数，惯性")]
        [SerializeField] private float inertia = 8f;
        private float screenCo;
        private Vector3 oldMousePos;
        private bool canMove = true;
        private Vector3 prevPositionTemp;
        private int currentFingerId = -1;
        private void Move()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                // canMove = !UITools.IsPointerOverUIObject();
                oldMousePos = Input.mousePosition;
                screenCo = (screenAreaBounds.max.y - screenAreaBounds.min.y) / Screen.height;
                prevPositionTemp = cameraPositionTemp;
            }
            else if (Input.GetMouseButton(0))
            {
                if (canMove == false)
                    return;
                prevPositionTemp = cameraPositionTemp;
                Vector3 deltaPosition = Input.mousePosition - oldMousePos;
                Vector3 delta = new Vector2(deltaPosition.x, deltaPosition.y) * screenCo * moveCo;
                cameraPositionTemp -= new Vector3(delta.x, delta.y, 0);
                oldMousePos = Input.mousePosition;
                cameraMoveLerpTimeTemp = 15;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (canMove == false)
                    return;
                cameraMoveLerpTimeTemp = cameraMoveLerpTime;
                cameraPositionTemp = TransformUtil.GetBetweenPointForPercent(prevPositionTemp, cameraPositionTemp, inertia);
            }
#else
            if (Input.touchCount != 1)
            {
                return;
            }

            Touch touch1 = Input.GetTouch(0);
            
            if (touch1.phase == TouchPhase.Began)
            {
                canMove = !UITools.IsPointerOverUIObject();
                screenCo = (screenAreaBounds.max.y - screenAreaBounds.min.y) / GameGlobal.Instance.screenHeight;
                prevPositionTemp = cameraPositionTemp;
                currentFingerId = touch1.fingerId;
            }
            else if (touch1.fingerId == currentFingerId && touch1.phase == TouchPhase.Moved)
            {
                if (canMove == false)
                    return;
                prevPositionTemp = cameraPositionTemp;
                Vector3 delta = touch1.deltaPosition * screenCo * moveCo;
                cameraPositionTemp -= new Vector3(delta.x, delta.y, 0);
                cameraMoveLerpTimeTemp = 15;
            }
            else if (touch1.fingerId == currentFingerId && ( touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled))
            {
                if (canMove == false)
                    return;
                cameraMoveLerpTimeTemp = cameraMoveLerpTime;
                cameraPositionTemp = TransformUtil.GetBetweenPointForPercent(prevPositionTemp, cameraPositionTemp, inertia);
            }
#endif
        }

        private void DoMove()
        {
            if (TransformUtil.GetDistance(cameraPositionTemp, transform.position) > 0.05f)
            {
                isStable = false;
                transform.position = Vector3.Lerp(transform.position, cameraPositionTemp, cameraMoveLerpTimeTemp * Time.deltaTime);
            }
        }


        /// <summary>
        /// 设置移动位置
        /// </summary>
        /// <param name="position">目标点</param>
        /// <param name="lerp">是否缓动</param>
        /// <param name="triggerArea">是否触发区域变化</param>
        public void SetMovePosition(Vector2 position, bool lerp = false, bool triggerArea = true)
        {
            cameraPositionTemp.Set(position.x, position.y, cameraPositionTemp.z);
            if (lerp == false)
            {
                transform.position = cameraPositionTemp;
            }
        }

        /// <summary>
        /// 叠加移动位置
        /// </summary>
        /// <param name="position">目标点</param>
        /// <param name="lerp">是否缓动</param>
        public void AddMovePosition(Vector2 position, bool lerp = true)
        {
            cameraPositionTemp += new Vector3(position.x, position.y, 0);
            if (lerp == false)
            {
                transform.position = cameraPositionTemp;
            }
        }

        public void EnableMove(bool _canMove)
        {
            canMove = _canMove;
        }
    }

}