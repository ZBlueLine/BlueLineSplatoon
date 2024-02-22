using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SplatterSystem
{
    [Serializable]
    public class PaintBrush
    {
        public Texture2D BrushTexture;
        public Shader PaintShader;
        [Range(0f, 1f)]
        public float BrushRadius;
        [Range(0, 30)]
        public float FiringRange;
        [HideInInspector]
        public Material PaintFirstColorMaterial;
        [HideInInspector]
        public Material PaintSecondColorMaterial;
        [HideInInspector]
        public Color FirstColor;
        [HideInInspector]
        public Color SecondColor;

        int BrushTexProp = Shader.PropertyToID("_BrushTexture");
        int PaintColorProp = Shader.PropertyToID("_PaintColor");
        int PaintRadiusProp = Shader.PropertyToID("_PaintRadius");
        int HitCenter = Shader.PropertyToID("_HitCenter");
        public void init(Color firstColor, Color secondColor)
        {
            FirstColor = firstColor;
            SecondColor = secondColor;


            PaintFirstColorMaterial = new Material(PaintShader);
            PaintFirstColorMaterial.SetFloat(PaintRadiusProp, BrushRadius);
            PaintFirstColorMaterial.SetTexture(BrushTexProp, BrushTexture);
            PaintFirstColorMaterial.SetColor(PaintColorProp, FirstColor);

            PaintSecondColorMaterial = new Material(PaintShader);
            PaintSecondColorMaterial.SetFloat(PaintRadiusProp, BrushRadius);
            PaintSecondColorMaterial.SetTexture(BrushTexProp, BrushTexture);
            PaintSecondColorMaterial.SetColor(PaintColorProp, SecondColor);
        }
        public void UpdateBrushInfo()
        {
            if (PaintFirstColorMaterial == null) return;
            PaintFirstColorMaterial.SetFloat(PaintRadiusProp, BrushRadius);
            PaintFirstColorMaterial.SetTexture(BrushTexProp, BrushTexture);
            PaintFirstColorMaterial.SetColor(PaintColorProp, FirstColor);

            PaintSecondColorMaterial.SetFloat(PaintRadiusProp, BrushRadius);
            PaintSecondColorMaterial.SetTexture(BrushTexProp, BrushTexture);
            PaintSecondColorMaterial.SetColor(PaintColorProp, SecondColor);
        }
        public void Paint(Vector3 pos, Vector3 dir)
        {
            RaycastHit hitInfo;
            Physics.Raycast(pos, dir, out hitInfo, FiringRange);
            Debug.DrawRay(pos, dir * FiringRange, Color.red, 0.1f);
            if (!PaintBrushManager.CheckPaintObject(hitInfo))
            {
                return;
            }
            PaintFirstColorMaterial.SetVector(HitCenter, hitInfo.lightmapCoord);
        }
    };
    public enum PaintColorType
    {
        FirstColor,
        SecondColor
    }
    public enum BrushType
    {
        Point,
        Line
    };

    public class PaintBrushManager : MonoBehaviour
    {
        public static PaintBrushManager Instance { get; private set; }
        public RenderTexture PaintRT = null;
        public RenderTexture PaintRenderRT = null;
        public Color FirstColor = Color.red;
        public Color SecondColor = Color.green;
        public int PaintRTRes = 512;
        public PaintBrush CustomPaintBrush;

        private static List<int> mColliderInstanceIDList = new List<int>();
        private static RenderTextureDescriptor mPaintRTDescriptor;
        private static CommandBuffer m_Cmd;
        public static CommandBuffer Cmd
        {
            get { return m_Cmd; }
        }

        private int mFirstPaintColorProp = Shader.PropertyToID("_FirstPaintColor");
        private int mSecondPaintColorProp = Shader.PropertyToID("_SecondPaintColor");
        protected int PaintColorProp = Shader.PropertyToID("_PaintColor");
        private int mPaintColorRTProp = Shader.PropertyToID("_PaintRT");
        private int mPaintRTResolutionProp = Shader.PropertyToID("_PaintRTResolution");


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            m_Cmd = new CommandBuffer();
            mPaintRTDescriptor = new RenderTextureDescriptor(PaintRTRes, PaintRTRes, RenderTextureFormat.RGB111110Float, 0, 0);
            PaintRT = new RenderTexture(mPaintRTDescriptor);
            PaintRT.filterMode = FilterMode.Bilinear;
            PaintRT.name = "PaintRTfront";
            PaintRT.useMipMap = true;

            PaintRenderRT = new RenderTexture(mPaintRTDescriptor);
            PaintRenderRT.filterMode = FilterMode.Bilinear;
            PaintRenderRT.name = "PaintRenderRT";
            PaintRenderRT.useMipMap = true;

            Shader.SetGlobalTexture(mPaintColorRTProp, PaintRenderRT);
            Shader.SetGlobalColor(mFirstPaintColorProp, FirstColor);
            Shader.SetGlobalColor(mSecondPaintColorProp, SecondColor);
            Shader.SetGlobalVector(mPaintRTResolutionProp, new Vector2(PaintRTRes, PaintRTRes));
            CustomPaintBrush.init(Color.red, Color.green);
        }
        private void OnValidate()
        {
            CustomPaintBrush.UpdateBrushInfo();
        }

        public void Paint(Vector3 pos, Vector3 dir, PaintColorType colorType)
        {
            CustomPaintBrush.Paint(pos, dir);
            PaintBrushManager.Cmd.name = "BrushPaint";
            PaintBrushManager.Cmd.Blit(null, PaintRT, CustomPaintBrush.PaintFirstColorMaterial);
            PaintBrushManager.Cmd.CopyTexture(PaintRT, PaintRenderRT);
            Graphics.ExecuteCommandBuffer(PaintBrushManager.Cmd);
            PaintBrushManager.Cmd.Clear();
        }

        public static bool CheckPaintObject(in RaycastHit hitInfo)
        {
            if (hitInfo.collider == null)
                return false;
            if (!mColliderInstanceIDList.Contains(hitInfo.colliderInstanceID))
            {
                mColliderInstanceIDList.Add(hitInfo.colliderInstanceID);
                hitInfo.transform.GetComponent<Renderer>().material.EnableKeyword("_PAINTED");
            }
            return true;
        }
    }
}