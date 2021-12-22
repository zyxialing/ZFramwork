using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(UINodeScoller))]
public class UIScrollerEditor : Editor
{
    UINodeScoller _target;

    SerializedProperty cellViewProperty;
    void OnEnable()
    {
        _target = target as UINodeScoller;
        cellViewProperty = serializedObject.FindProperty("cellView");
    }

    private Vector2 scrollView;
    public override void OnInspectorGUI()
    {

        scrollView = GUILayout.BeginScrollView(scrollView);

        if (GUILayout.Button("更新脚本"))
        {
            string panelName = "";
            UINodePanel uIPanel = Selection.activeGameObject.GetComponentInParent<UINodePanel>();
            UINodeScoller uINodeScoller = Selection.activeGameObject.GetComponent<UINodeScoller>();
            if (uIPanel == null)
            {
                ZLogUtil.LogError("没有UINodePanel");
            }
            string path = PathFinderEditor.GetPrefabAssetPath(uIPanel.gameObject, out panelName);
            ScriptCreater.CreateScrollerClassName(path,panelName, uINodeScoller.transform.name, uINodeScoller);
        }
       
        if (_target.GetComponent(Selection.activeGameObject.name) == null)
        {
            Type className = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(Selection.activeGameObject.name);
            if (className != null)
            {
                Selection.activeGameObject.AddComponent(className);
            }
        }

        //FindBack();

        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(cellViewProperty);
        serializedObject.ApplyModifiedProperties();

        GUILayout.EndScrollView();
    }

    public string strIpt = "";

    /// <summary>
    /// nodes节点名字 锁定节点Obj
    /// </summary>
    public void FindBack()
    {
        try
        {
            strIpt = GUILayout.TextField(strIpt);
            if (!string.IsNullOrEmpty(strIpt))
            {
                foreach (var item in _target.nodes)
                {
                    if (item.tag == strIpt && item.transform != null)
                    {
                        Selection.activeGameObject = item.transform.gameObject;
                    }
                }
                strIpt = "";
            }
        }
        catch
        {
            Debug.Log("又来了,但不影响");
        }
       
    }
}