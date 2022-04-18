// =======================================================
// 文件：MapCamera_Zoom.cs
// 描述：相机缩放
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
        private float cameraZoomTemp;
        private float defaultZoom = -30;
        private Touch oldTouch1; //上次触摸点1(手指1)
        private Touch oldTouch2; //上次触摸点2(手指2)
        [Header("相机缩放参数，Lerp时间")]
        [Space(20)]
        [SerializeField] private float cameraZoomLerpTime = 10f;
        [Header("相机缩放参数，系数(透视相机)")]
        [SerializeField] private float zoomCo_Perspective = 0.2f; //缩放系数
        [Header("相机缩放参数，系数(正交相机)")]
        [SerializeField] private float zoomCo_Orthographic = 0.2f; //缩放系数
        private bool canZoom = false;
        internal Bounds screenAreaBounds = new Bounds();    //屏幕视野对应的地图区域
        private void Zoom_Orthographic()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            cameraZoomTemp -= Input.mouseScrollDelta.y * 80f * zoomCo_Orthographic;
#else
            if (Input.touchCount <= 1) {
                return;
            }
    
            Touch touch1 = Input.GetTouch (0);
            Touch touch2 = Input.GetTouch (1);
            if (touch2.phase == TouchPhase.Began) {
                canZoom = !UITools.IsPointerOverUIObject();
                oldTouch2 = touch2;
                oldTouch1 = touch1;
                return;
            }
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
                if (canZoom == false)
                    return;
                float oldDistance = Vector2.Distance(oldTouch2.position, oldTouch1.position);
                float curDistance = Vector2.Distance(touch2.position, touch1.position);
                float delta = (curDistance - oldDistance) * zoomCo_Orthographic;
                cameraZoomTemp -= delta;
    
                oldTouch1 = touch1;
                oldTouch2 = touch2;
            }
#endif
        }
        private void Zoom_Perspective()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            cameraZoomTemp += Input.mouseScrollDelta.y * 80f * zoomCo_Perspective;
#else
            if (Input.touchCount <= 1) {
                return;
            }
    
            Touch touch1 = Input.GetTouch (0);
            Touch touch2 = Input.GetTouch (1);
            if (touch2.phase == TouchPhase.Began) {
                canZoom = !UITools.IsPointerOverUIObject();
                oldTouch2 = touch2;
                oldTouch1 = touch1;
                return;
            }
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
                if (canZoom == false)
                    return;
                float oldDistance = Vector2.Distance(oldTouch2.position, oldTouch1.position);
                float curDistance = Vector2.Distance(touch2.position, touch1.position);
                float delta = (curDistance - oldDistance) * zoomCo_Perspective;
                cameraZoomTemp += delta;
    
                oldTouch1 = touch1;
                oldTouch2 = touch2;
            }
#endif
        }

        private void DoZoom_Orthographic()
        {
            if (Mathf.Abs(cameraZoomTemp - camera.orthographicSize) > 0.05f)
            {
                isStable = false;
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraZoomTemp, cameraZoomLerpTime * Time.deltaTime);
                SetAreaBounds();
            }
        }

        private void DoZoom_Perspective()
        {
            if (Mathf.Abs(cameraZoomTemp - camera.transform.localPosition.z) > 0.05f)
            {
                isStable = false;
                camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, new Vector3(0, 0, cameraZoomTemp), cameraZoomLerpTime * Time.deltaTime);
                SetAreaBounds();
            }
        }

        /// <summary>
        /// 设置范围bounds
        /// </summary>
        private void SetAreaBounds()
        {
            Vector2 lu = new Vector2(0, Screen.height);
            Vector2 ru = new Vector2(Screen.width, Screen.height);
            Vector2 lb = new Vector2(0, 0);
            Vector2 luPosition = Vector2.zero;
            Vector2 ruPosition = Vector2.zero;
            Vector2 lbPosition = Vector2.zero;
            {
                Ray ray = camera.ScreenPointToRay(lu);
                RaycastHit rc;
                if (Physics.Raycast(ray, out rc, 10000f, 1 << LayerMask.NameToLayer("Plane")))
                    luPosition = rc.point;
            }
            {
                Ray ray = camera.ScreenPointToRay(ru);
                RaycastHit rc;
                if (Physics.Raycast(ray, out rc, 10000f, 1 << LayerMask.NameToLayer("Plane")))
                    ruPosition = rc.point;
            }
            {
                Ray ray = camera.ScreenPointToRay(lb);
                RaycastHit rc;
                if (Physics.Raycast(ray, out rc, 10000f, 1 << LayerMask.NameToLayer("Plane")))
                    lbPosition = rc.point;
            }
            screenAreaBounds.center = new Vector3(luPosition.x + (ruPosition.x - luPosition.x) / 2, lbPosition.y + (luPosition.y - lbPosition.y) / 2);
            screenAreaBounds.size = new Vector3(ruPosition.x - luPosition.x, luPosition.y - lbPosition.y);
        }

        /// <summary>
        /// 设置缩放尺寸
        /// </summary>
        /// <param name="size">缩放尺寸</param>
        /// <param name="lerp">是否缓动</param>
        /// <param name="triggerArea">是否触发加载区域</param>
        public void SetZoomSize(float size = -1, bool lerp = true, bool triggerArea = true)
        {
            cameraZoomTemp = size == -1 ? defaultZoom : size;
            if (lerp == false)
            {
                if (camera.orthographic)
                    camera.orthographicSize = cameraZoomTemp;
                else
                    camera.transform.localPosition = new Vector3(0, 0, cameraZoomTemp);
            }
        }

        /// <summary>
        /// 获取缩放尺寸
        /// </summary>
        /// <returns></returns>
        public float GetZoomSize()
        {
            return cameraZoomTemp;
        }
    }

}