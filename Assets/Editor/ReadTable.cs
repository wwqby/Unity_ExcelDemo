using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 游戏启动时运行的脚本
/// 读取excel数据到游戏数据中
/// </summary>
[InitializeOnLoad]//在编辑器启动时执行
public class Startup
{
    static Startup()
    {
        string path = Application.dataPath + "/Editor/关卡管理.xlsx";
        string assetName = "Level";
        //检查是否有数据，有数据就不需要重新读取
        LevelData LocalData = Resources.Load<LevelData>(assetName);
        if (LocalData != null)
        {
            Debug.Log("本地已有数据");
            return;
        }
        FileInfo file = new FileInfo(path);
        //反射创建实例
        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        //读取excel文件数据,using会在代码块结尾自动关闭文件
        using (ExcelPackage package = new ExcelPackage(file))
        {
            //打开指定的表
            ExcelWorksheet sheet = package.Workbook.Worksheets["僵尸"];
            //遍历每一行
            for (int i = sheet.Dimension.Start.Row + 2; i <= sheet.Dimension.End.Row; i++)
            {
                LevelItem levelItem = new LevelItem();
                Type type = typeof(LevelItem);
                //遍历每一列
                for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                {
                    //读取输出i行j列的数据
                    Debug.Log(string.Format("{0}行{1}列的数据是{2}", i, j, sheet.Cells[i, j].Value));
                    //反射设置数据，数据类型需要是public类型
                    FieldInfo variable = type.GetField(sheet.Cells[2, j].Value.ToString());
                    string value = sheet.Cells[i, j].Value.ToString();
                    variable?.SetValue(levelItem, Convert.ChangeType(value, variable.FieldType));
                }
                //添加读到的数据
                levelData.levelItems.Add(levelItem);
            }
        }
        //保存ScriptableObject为asset文件
        AssetDatabase.CreateAsset(levelData, "Assets/Resources/" + assetName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("数据读取完成");
    }
}
