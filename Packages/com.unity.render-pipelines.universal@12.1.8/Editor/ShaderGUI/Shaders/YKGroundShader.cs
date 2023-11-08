using System;
using System.Collections.Generic;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class YKGroundShader : BaseShaderGUI
    {
        static readonly string[] workflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));

        private LitGUI.LitProperties litProperties;
        private LitDetailGUI.LitProperties litDetailProperties;

        protected class Styles
        {
            public static readonly GUIContent InkInput = EditorGUIUtility.TrTextContent("Painting Input", "Controlling the rendering of ink");
            public static readonly GUIContent inkNormalMap = EditorGUIUtility.TrTextContent("Ink Normal Map", "");
            public static readonly GUIContent normalWeightText = EditorGUIUtility.TrTextContent("Normal Weight", "");
            public static readonly GUIContent normalTilingText = EditorGUIUtility.TrTextContent("Normal Tiling", "");
            public static readonly GUIContent edgeNormalWeightText = EditorGUIUtility.TrTextContent("Edge Normal Weight", "");
            public static readonly GUIContent edgeColor = EditorGUIUtility.TrTextContent("Edge Color", "");
        }
        

        protected enum GroundShaderExpandable
        {
            InkInput = 1 << 4,
        }

        private MaterialProperty normalMapProp;
        private MaterialProperty NormalWeightProp;
        private MaterialProperty normalTilingProp;
        private MaterialProperty edgeNormalWeightProp;
        private MaterialProperty edgeColorProp;

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(LitDetailGUI.Styles.detailInputs, Expandable.Details, _ => LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor));
            materialScopesList.RegisterHeaderScope(Styles.InkInput, (uint)GroundShaderExpandable.InkInput, DrawInkInput);
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            litDetailProperties = new LitDetailGUI.LitProperties(properties);

            normalMapProp = BaseShaderGUI.FindProperty("_NormalMap", properties, false);
            normalTilingProp = BaseShaderGUI.FindProperty("_NormalTilling", properties, false);
            NormalWeightProp = BaseShaderGUI.FindProperty("_NormalWeight", properties, false);
            edgeNormalWeightProp = BaseShaderGUI.FindProperty("_EdgeNormalWeight", properties, false);
            edgeColorProp = BaseShaderGUI.FindProperty("_EdgeColor", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            if (litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);

            base.DrawSurfaceOptions(material);
        }

        public void DrawInkInput(Material material)
        {
            materialEditor.TexturePropertySingleLine(Styles.inkNormalMap, normalMapProp);
            materialEditor.ShaderProperty(NormalWeightProp, Styles.normalWeightText);
            materialEditor.ShaderProperty(normalTilingProp, Styles.normalTilingText);
            materialEditor.ShaderProperty(edgeNormalWeightProp, Styles.edgeNormalWeightText);
            materialEditor.ShaderProperty(edgeColorProp, Styles.edgeColor);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
            }

            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
        }
    }
}
