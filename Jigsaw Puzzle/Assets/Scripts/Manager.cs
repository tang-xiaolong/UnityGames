using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Manager : MonoBehaviour
{

    //1、读取图片
    //2、输入切割大小
    //3、切割图片
    //4、打乱图片
    private static Texture imageSource;
    private List<Element> pictures;
    private int size = 0;

    //TODO:remove static attribute when game runing
    [MenuItem("QuickTool/PSDImport ...", false, 1)]
    public static void OpenFile()
    {
        string inputFile = EditorUtility.OpenFilePanel("Choose Image", Application.dataPath, "jpg");
        if (!string.IsNullOrEmpty(inputFile))
        {
            Debug.Log(inputFile);
            string fileName;
            CopyToResources(inputFile,out fileName);
            AssetDatabase.Refresh();
            ReadImage(fileName);
        }
        GC.Collect();
    }
    public static void CopyToResources(string path,out string fileName)
    {
        string[] res = path.Split('/');
        fileName = res[res.Length - 1];
        string resPath = Application.dataPath + "/Resources/"+fileName;
        File.Copy(path, resPath, true);
    }
    public static void ReadImage(string path)
    {
        Debug.Log(path);
        string fileName = path.Split('.')[0];
        imageSource = Resources.Load<Texture>(fileName);
        if (imageSource == null)
        {
            Debug.Log("null");
            return;
        }
        TextureImporter import = AssetImporter.GetAtPath("Resources/"+path) as TextureImporter;
        import.textureType = TextureImporterType.Sprite;
        import.spriteImportMode = SpriteImportMode.Multiple;
        //TODO:
    }
    public void GetCount()
    {

    }
    public void SliceImage(int count)
    {

    }
    public void RandomImage()
    {

    }
    public void SwapElement()
    {

    }
}
