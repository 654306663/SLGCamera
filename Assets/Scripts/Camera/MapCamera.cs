// =======================================================
// 文件：MapCamera.cs
// 描述：地图相机
// 作者：网虫虫
// 创建时间：2021/9/3 12:12:11
// =======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class MapCamera : MonoBehaviour
    {
        public static MapCamera Instance { private set; get; }
        public new Camera camera { private set; get; }
        private float screenRate;
        private Queue<Action> stableCallbacks = new Queue<Action>();   //相机稳定状态回调
        private bool isStable = true;   //相机是否稳定状态 未移动缩放

        void Awake()
        {
            Instance = this;
            camera = GetComponentInChildren<Camera>();
            screenRate = Screen.width / (float)Screen.height;
            cameraPositionTemp = transform.position;
            if (camera.orthographic)
                cameraZoomTemp = camera.orthographicSize;
            else
                cameraZoomTemp = camera.transform.localPosition.z;

            SetAreaBounds();
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            if (camera.orthographic)
                Zoom_Orthographic();
            else
                Zoom_Perspective();

            isStable = true;

            DoMove();
            if (camera.orthographic)
                DoZoom_Orthographic();
            else
                DoZoom_Perspective();

            if (isStable)
            {
                if (stableCallbacks.Count > 0)
                {
                    int length = stableCallbacks.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Action action = stableCallbacks.Dequeue();
                        action?.Invoke();
                        i--;
                        length--;
                    }
                }
            }
        }

        /// <summary>
        /// 等待相机静默状态回调
        /// </summary>
        /// <param name="quietCallback"></param>
        public void WaitStableState(Action stableCallback)
        {
            this.stableCallbacks.Enqueue(stableCallback);
        }
    }
}