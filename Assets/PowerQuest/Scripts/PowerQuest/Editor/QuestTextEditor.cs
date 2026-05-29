using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PowerTools.Quest;
using PowerTools;
using PowerTools.QuestGui;
using UnityEditor.Experimental.SceneManagement;

namespace PowerTools.Quest
{

[CanEditMultipleObjects]
[CustomEditor(typeof(QuestText))]
public class QuestTextEditor : Editor 
{

	[SerializeField]TextMesh m_meshComponent = null;

	public void OnEnable()
	{
		// Set charater size to 10 if snapping
		QuestText component = (QuestText)target;
		if ( component != null && component.IsPixelStyle ) // (PowerQuestEditor.Snap && component.FontPixelStyle == QuestText.eFontPixelStyle.Auto) || component.FontPixelStyle == QuestText.eFontPixelStyle.Pixel || component.FontPixelStyle == QuestText.eFontPixelStyle.PixelAntiAliased)
		{
			TextMesh tm = component != null ? component.GetComponent<TextMesh>() : null;
			if ( tm != null && tm.characterSize == 1 )
			{
				tm.characterSize = 10;
				EditorUtility.SetDirty(tm);
			}
		}
	}

	public override void OnInspectorGUI()
	{
		// NB: Quest text duplicates a bunch of textmesh controls so they can be in one place.

		QuestText component = (QuestText)target;
		SerializedObject serializedObj = new SerializedObject(component);
		if ( m_meshComponent == null || m_meshComponent.gameObject != component.gameObject )
			m_meshComponent = component.transform.GetComponent<TextMesh>();

		// Make the quest text component the first in the list.. Have to do some hackery to ensure it's not an unstaged prefab
		Component[] list = component.GetComponents<Component>();
		if ( list[1] != component && list[1] is MeshRenderer && (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab || ( PrefabStageUtility.GetCurrentPrefabStage() != null && PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot == component.gameObject) ))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		
		EditorGUI.BeginChangeCheck();

		EditorGUILayout.LabelField("Text Properties", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_text"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_localize"));

		GUILayout.Space(10);		
		EditorGUILayout.LabelField("Font", EditorStyles.boldLabel);
		
		Font newFont = EditorGUILayout.ObjectField("Font", m_meshComponent.font, typeof(Font),false ) as Font;
		if ( newFont != m_meshComponent.font && newFont != null )
		{
			m_meshComponent.font = newFont;
			m_meshComponent.GetComponent<MeshRenderer>().material = newFont.material;
		}
		
		int fontSize = EditorGUILayout.IntField("Size", m_meshComponent.fontSize);		
		if ( m_meshComponent.fontSize != fontSize ) // have to only change if necessary or unity goes weird
			m_meshComponent.fontSize = fontSize;
		
		float lineSpacing = m_meshComponent.lineSpacing * Mathf.Max(m_meshComponent.fontSize,10);
		lineSpacing = EditorGUILayout.FloatField("Line spacing", lineSpacing);
		lineSpacing = lineSpacing / Mathf.Max(m_meshComponent.fontSize,10);
		if ( lineSpacing != m_meshComponent.lineSpacing)
			m_meshComponent.lineSpacing = lineSpacing;
		Color col = EditorGUILayout.ColorField("Color", m_meshComponent.color);
		if ( m_meshComponent.color != col )
			m_meshComponent.color = col;
		
		EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_outline"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_typeSpeed"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_shaderOverride"));
		//EditorGUILayout.PropertyField(serializedObj.FindProperty("m_setFiltering"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_fontPixelStyle"));
		
		GUILayout.Space(10);	
		EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel);

		TextAlignment align = (TextAlignment)EditorGUILayout.EnumPopup("Alignment", m_meshComponent.alignment );
		if ( align != m_meshComponent.alignment )
			m_meshComponent.alignment = align;
		TextAnchor anchor = (TextAnchor)EditorGUILayout.EnumPopup("Anchor", m_meshComponent.anchor );
		if ( anchor != m_meshComponent.anchor )
			m_meshComponent.anchor = anchor;
		
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_sortingLayer"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_orderInLayer"));
		
		GUILayout.Space(10);	
		EditorGUILayout.LabelField("Wrap/Truncate Settings", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapWidth"), new GUIContent("Width (Pixels)"));
		if ( component.WrapWidth > 0 )
		{
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_truncate"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapUniformLineWidth"));
		}
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_keepOnScreen"));
		bool keepOnScreen = serializedObj.FindProperty("m_keepOnScreen").boolValue == true;

		if ( keepOnScreen )
		{
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_screenPadding"));
		}

		if ( keepOnScreen || serializedObj.FindProperty("m_wrapUniformLineWidth").boolValue == true)
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapWidthMin"));
		
		GUILayout.Space(10);	
		EditorGUILayout.HelpBox("Note, some fields are duplicated in the TextMesh component below, you can edit them in either place just fine ;)", MessageType.Info);

		if ( EditorGUI.EndChangeCheck() )
		{
			component.SendMessage("EditorUpdate",SendMessageOptions.DontRequireReceiver);
			serializedObj.ApplyModifiedProperties();
			EditorUtility.SetDirty(m_meshComponent);
			EditorUtility.SetDirty(target);
		}

		
	}

}

}
