//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//namespace SplatterSystem
//{
//    public class PaintBrush : MonoBehaviour
//    {
//        public Texture2D BrushTexture;
//        public Shader PaintShader;
//        [Range(0, 10)]
//        public int PaintRate = 2;
//        [Range(0f, 1f)]
//        public float BrushRadius = 0.5f;
        
//        [HideInInspector]
//        public float FiringRange = 10f;

//        public Material PaintMaterial = null;

//        private int BrushTexProp = Shader.PropertyToID("_BrushTexture");
//        private int PaintColorProp = Shader.PropertyToID("_PaintColor");
//        private int PaintRadiusProp = Shader.PropertyToID("_PaintRadius");


//        public PaintBrush(Color paintColor)
//        {
//            PaintMaterial = new Material(PaintShader);
//            PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
//            PaintMaterial.SetTexture(BrushTexProp, BrushTexture);
//            PaintMaterial.SetColor(PaintColorProp, paintColor);
//        }

//        //public PaintBrush(PaintColorType paintColor)
//        //{
//        //    m_PaintMaterial = new Material(PaintShader);
//        //    m_PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
//        //    m_PaintMaterial.SetTexture(BrushTexProp, BrushTexture);
//        //    m_PaintMaterial.SetColor(PaintColorProp, PaintBrushManager.GetBrushColor(paintColor));
//        //}
//    };
//}