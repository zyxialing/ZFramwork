using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptCreater
{

    public static void CreatePanelClassName(string path,string name, UINodePanel uIPanel)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string adressPath = path.Replace(".prefab", "").Replace("Assets/Game/AssetDynamic/Prefab/UI/", "");
        string dir = path.Replace(".prefab", "").Replace("Assets/Game/AssetDynamic/Prefab/UI","Assets/Game/Scripts/UI").Replace(name,"");
        string voDir = dir.Replace("Assets/Game/Scripts/UI", "Assets/Game/Scripts/UIVO");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (!Directory.Exists(voDir))
        {
            Directory.CreateDirectory(voDir);
        }
        voDir = voDir + name + "VO.cs";
        dir = dir + name + ".cs";
        stringBuilder.Append("using System;\n");
        stringBuilder.Append("using System.Collections;\n");
        stringBuilder.Append("using System.Collections.Generic;\n");
        stringBuilder.Append("using UnityEngine;\n");
        stringBuilder.Append("using UnityEngine.UI;\n");
        stringBuilder.Append(string.Format("public partial class {0} : BasePanel\n", name));
        stringBuilder.Append("{\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("    public override void Init(params object[] args)\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("        base.Init(args);\n");
        stringBuilder.Append("        panelLayer = PanelLayer.Panel;\n");
        stringBuilder.Append($"       adressPath = \"{adressPath}\";\n");
        stringBuilder.Append("    }\n");
        stringBuilder.Append("    public override void OnShowing()\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("    }\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("    public override void OnShowed()\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("        RefreshPanel();\n");
        stringBuilder.Append("    }\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("    private void RefreshPanel()\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("    }\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("}");


        if (!File.Exists(dir))
        {
            File.WriteAllText(dir, stringBuilder.ToString());
            ZLogUtil.Log("创建"+name+"成功");
        }

        //VO
        stringBuilder.Clear();
        stringBuilder.Append(string.Format("public partial class {0} : BasePanel\n", name));
        stringBuilder.Append("{\n");
        for (int i = 0; i < uIPanel.nodes.Count; i++)
        {
            stringBuilder.Append("   private "+ uIPanel.nodes[i].type+" "+uIPanel.nodes[i].tag+";\n");
        }
        stringBuilder.Append("\n");
        stringBuilder.Append("   public override void AutoInit()\n");
        stringBuilder.Append("   {\n");
        for (int i = 0; i < uIPanel.nodes.Count; i++)
        {
            Transform curTrans = uIPanel.nodes[i].transform;
            string componentPath = "/" + curTrans.name;
            int count = 0;
            while (curTrans.parent.name != name)
            {
                componentPath = componentPath.Insert(0,("/"+ curTrans.parent.name));
                count++;
                curTrans = curTrans.parent;
                if (count > 100) {
                    ZLogUtil.LogError("层级超过100,有毛病吧");
                    return;
                }
            }
            componentPath = componentPath.Remove(0,1);
            stringBuilder.Append($"    this.{uIPanel.nodes[i].tag} = panel.transform.Find(\"{componentPath}\").GetComponent<{uIPanel.nodes[i].type}>();\n");
        }
        stringBuilder.Append("   }\n");
        stringBuilder.Append("}\n");

 


        File.WriteAllText(voDir, stringBuilder.ToString());
        stringBuilder.Clear();
        AssetDatabase.Refresh();
    }


    public static void CreateScrollerClassName(string path,string panelName,string scrollName,UINodeScoller uINodeScoller)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string cellName = uINodeScoller.cellView.name;
        if (uINodeScoller.cellView.GetComponent(uINodeScoller.cellView.name) == null)
        {
            ZLogUtil.LogError($"请先完成预制体{uINodeScoller.cellView.name}");
            return;
        }
        string dir = path.Replace(".prefab", "").Replace("Assets/Game/AssetDynamic/Prefab/UI", "Assets/Game/Scripts/UI").Replace(panelName, "");
        string voDir = dir.Replace("Assets/Game/Scripts/UI", "Assets/Game/Scripts/UIVO");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (!Directory.Exists(voDir))
        {
            Directory.CreateDirectory(voDir);
        }
        string scrollerDir = dir + "/Scoller/";
        if (!Directory.Exists(scrollerDir))
        {
            Directory.CreateDirectory(scrollerDir);
        }
        string scrollerScript = scrollerDir + scrollName + ".cs";

        stringBuilder.Append("using EnhancedUI.EnhancedScroller;\n");
        stringBuilder.Append("using UnityEngine;\n\n");
        stringBuilder.Append("[RequireComponent(typeof(EnhancedScroller))]\n");
        stringBuilder.Append($"public class {scrollName} : MonoBehaviour, IEnhancedScrollerDelegate\n");
        stringBuilder.Append("{\n");
        stringBuilder.Append("    private EnhancedScroller scroller;\n");
        stringBuilder.Append("    private RectTransform cellRectTransform;\n");
        stringBuilder.Append("    private RectTransform rectTransform;\n\n");
        stringBuilder.Append($"    public EnhancedScrollerCellView cellViewPrefab;\n\n");
        stringBuilder.Append("    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append($"       {cellName} prefab = scroller.GetCellView(cellViewPrefab) as {cellName};\n");
        stringBuilder.Append($"       prefab.AutoInit();\n");
        stringBuilder.Append($"       prefab.UpdateData(scroller,dataIndex,cellIndex);\n");
        stringBuilder.Append("        return prefab;\n");
        stringBuilder.Append("    }\n\n");
        stringBuilder.Append("    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("        return cellRectTransform.rect.height;\n");
        stringBuilder.Append("    }\n\n");
        stringBuilder.Append("    public int GetNumberOfCells(EnhancedScroller scroller)\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("        return 6;\n");
        stringBuilder.Append("    }\n\n");
        stringBuilder.Append("    void Start()\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("        scroller = GetComponent<EnhancedScroller>();\n");
        stringBuilder.Append("        rectTransform = GetComponent<RectTransform>();\n");
        stringBuilder.Append("        cellRectTransform = cellViewPrefab.GetComponent<RectTransform>();\n");
        stringBuilder.Append("        scroller.Delegate = this;\n");
        stringBuilder.Append("     }\n");
        stringBuilder.Append("}\n");

        if (!File.Exists(scrollerScript))
        {
            File.WriteAllText(scrollerScript, stringBuilder.ToString());
            stringBuilder.Clear();
            ZLogUtil.Log("创建" + scrollerScript + "成功");
        }

        AssetDatabase.Refresh();
    }

    public static void CreateScrollerCellClassName(string path, string panelName, string cellName, UINodeScollerCell uINodeScoller)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string dir = path.Replace(".prefab", "").Replace("Assets/Game/AssetDynamic/Prefab/UI", "Assets/Game/Scripts/UI").Replace(panelName, "");
        string voDir = "Assets/Game/Scripts/UIVO/CellVO/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (!Directory.Exists(voDir))
        {
            Directory.CreateDirectory(voDir);
        }

        string scrollerDir = dir + "/Scoller/";
        if (!Directory.Exists(scrollerDir))
        {
            Directory.CreateDirectory(scrollerDir);
        }
        string scrollerCell = scrollerDir + "Cell/";
        if (!Directory.Exists(scrollerCell))
        {
            Directory.CreateDirectory(scrollerCell);
        }
        string scrollerCellScript = scrollerCell + cellName + ".cs";

        stringBuilder.Append("using EnhancedUI.EnhancedScroller;\n");
        stringBuilder.Append("using UnityEngine.UI;\n\n");
        stringBuilder.Append($"public partial class {cellName} : EnhancedScrollerCellView\n");
        stringBuilder.Append("{\n");
        stringBuilder.Append("    public void UpdateData(EnhancedScroller scroller, int dataIndex, int cellIndex)\n");
        stringBuilder.Append("    {\n");
        stringBuilder.Append("    }\n");
        stringBuilder.Append("}");


        if (!File.Exists(scrollerCellScript))
        {
            File.WriteAllText(scrollerCellScript, stringBuilder.ToString());
            stringBuilder.Clear();
            ZLogUtil.Log("创建" + scrollerCellScript + "成功");
        }

        //VO
        string scrollerVOCellScript = voDir + cellName + "VO.cs";
        stringBuilder.Clear();
        stringBuilder.Append("using EnhancedUI.EnhancedScroller;\n\n");
        stringBuilder.Append(string.Format("public partial class {0} : EnhancedScrollerCellView\n", cellName));
        stringBuilder.Append("{\n");
        stringBuilder.Append("   private bool isFirstInit = false;\n");

        for (int i = 0; i < uINodeScoller.nodes.Count; i++)
        {
            stringBuilder.Append("   private " + uINodeScoller.nodes[i].type + " " + uINodeScoller.nodes[i].tag + ";\n");
        }
        stringBuilder.Append("\n");
        stringBuilder.Append("   public void AutoInit()\n");
        stringBuilder.Append("   {\n");
        stringBuilder.Append("        if (isFirstInit) return;\n");
        stringBuilder.Append("        isFirstInit = true;\n");
        for (int i = 0; i < uINodeScoller.nodes.Count; i++)
        {
            Transform curTrans = uINodeScoller.nodes[i].transform;
            string componentPath = "/" + curTrans.name;
            int count = 0;
            while (curTrans.parent.name != cellName)
            {
                componentPath = componentPath.Insert(0, ("/" + curTrans.parent.name));
                count++;
                curTrans = curTrans.parent;
                if (count > 100)
                {
                    ZLogUtil.LogError("层级超过100,有毛病吧");
                    return;
                }
            }
            componentPath = componentPath.Remove(0, 1);
            stringBuilder.Append($"    this.{uINodeScoller.nodes[i].tag} = transform.Find(\"{componentPath}\").GetComponent<{uINodeScoller.nodes[i].type}>();\n");
        }
        stringBuilder.Append("   }\n");
        stringBuilder.Append("}\n");

        File.WriteAllText(scrollerVOCellScript, stringBuilder.ToString());
        stringBuilder.Clear();
        AssetDatabase.Refresh();

        }

}
