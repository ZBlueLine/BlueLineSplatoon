//using System;
//using UnityEngine;
//using UnityEngine.Rendering;

//namespace SplatterSystem
//{
//    [Serializable]
//    public class PaintGun : PaintBrush
//    {

//        //public PaintGun()
//        //{
//        //    gameObject = new GameObject("PaintGun");
//        //    transform = gameObject.transform;
//        //}
//        //public override void Init()
//        //{
//        //    m_PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
//        //    m_PaintMaterial = new Material(PaintShader);
//        //    m_PaintMaterial.SetTexture(BrushTexProp, BrushTexture);
//        //    m_PaintMaterial.SetColor(PaintColorProp, PaintBrushManager.GetBrushColor(PaintColor));
//        //}
//        //public override void Paint()
//        //{
//        //    m_RayCastDir = transform.TransformDirection(m_RayCastDir);
//        //    RaycastHit hitInfo;
//        //    Physics.Raycast(transform.position, m_RayCastDir, out hitInfo, FiringRange);
//        //    Debug.DrawRay(transform.position, m_RayCastDir * FiringRange, Color.red, 0.1f);
//        //    var t = PaintHelper.Instance;
//        //    if (!PaintBrushManager.CheckPaintObject(hitInfo))
//        //    {
//        //        return;
//        //    }
//        //    m_PaintMaterial.SetVector(HitCenter, hitInfo.lightmapCoord);

//        //    PaintBrushManager.Cmd.name = "BrushPaint";
//        //    PaintBrushManager.Cmd.Blit(null, PaintBrushManager.PaintRT, m_PaintMaterial);
//        //    PaintBrushManager.Cmd.CopyTexture(PaintBrushManager.PaintRT, PaintBrushManager.PaintRenderRT);
//        //    Graphics.ExecuteCommandBuffer(PaintBrushManager.Cmd);
//        //    PaintBrushManager.Cmd.Clear();
//        //}
//    }
//}