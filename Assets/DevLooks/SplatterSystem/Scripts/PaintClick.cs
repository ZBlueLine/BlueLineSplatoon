using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SplatterSystem
{
    public class PaintClick : MonoBehaviour
    {
        public new Camera camera;

        private float mMousePaintCurrentTime = 0;
        bool mMouseDownState = false;
        public float RayCastInterval = 0.01f;

        void Update()
        {
            DoMouseRayCast();
        }
        public void DoMouseRayCast()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mMouseDownState = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mMouseDownState = false;
            }
            if (mMousePaintCurrentTime > RayCastInterval)
            {
                if (mMouseDownState)
                {
                    mMousePaintCurrentTime = 0;
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    PaintBrushManager.Instance.Paint(ray.origin, ray.direction, PaintColorType.FirstColor);
                }
            }
            mMousePaintCurrentTime += Time.deltaTime;
        }

        //protected override void InitBrush()
        //{
        //    base.InitBrush();
        //    PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
        //}

        //protected override void UpdateBrush()
        //{
        //    base.UpdateBrush();
        //    PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
        //}

        //public override void Paint()
        //{
        //    RaycastHit hitInfo;
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    Physics.Raycast(ray, out hitInfo, FiringRange);
        //    if (!PaintHelperInstance.CheckPaintObject(hitInfo))
        //    {
        //        return;
        //    }
        //    PaintMaterial.SetVector(HitCenter, hitInfo.lightmapCoord);
        //    Graphics.ExecuteCommandBuffer(Cmd);
        //}
    }
}