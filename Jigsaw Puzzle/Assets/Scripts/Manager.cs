using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    //1、读取图片
    //2、输入切割大小
    //3、得到图片碎片，添加脚本，并删除最后一个的显示
    //4、打乱图片
    private Texture imageSource;
    private List<Element> pictures = new List<Element>();
    private int size = 4;
    public Material material;
    public GameObject go;
    private Element emptyElement;

    public void Start()
    {
        OpenFile();
        SliceImage(size);
    }
    //[MenuItem("Tools/OPen File", false, 1)]
    /// <summary>
    /// 打开文件
    /// </summary>
    public void OpenFile()
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
    /// <summary>
    /// 复制资源到Resources文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    public static void CopyToResources(string path,out string fileName)
    {
        string[] res = path.Split('/');
        fileName = res[res.Length - 1];
        string resPath = Application.dataPath + "/Resources/"+fileName;
        if(!path.Equals(resPath))
            File.Copy(path, resPath, true);
    }
    /// <summary>
    /// 读取图片
    /// </summary>
    /// <param name="path"></param>
    public void ReadImage(string path)
    {
        Debug.Log(path);
        string fileName = path.Split('.')[0];
        imageSource = Resources.Load<Texture>(fileName);
        
    }
    public void GetCount()
    {

    }
    public void SliceImage(int count)
    {
        //去较小的一个值，然后切割成三份
        // n * n
        int minSize = Mathf.Min(imageSource.width, imageSource.height);
        float delta = 1.0f / size;
        float offset = 0.05f;
        //取较小的一个，并均分为n块
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                //拿到第i块 https://connect.unity.com/p/bian-ji-qi-jiao-ben-zhi-zi-dong-hua-qie-ge-sprite
                GameObject temp = Instantiate(go);
                temp.name = i.ToString() + "  " + j.ToString();
                //Material m = Instantiate(material);
                Material m = new Material(material);
                temp.transform.position = new Vector3(i + offset * i,j + offset * j,0);
                temp.GetComponent<MeshRenderer>().material = m;
                Element element = temp.AddComponent<Element>();

                element.SetId((size - j) * size + i);
                element.gameObject.GetComponent<Button>().onClick.AddListener(element.OnClick);
                pictures.Add(element);

                m.SetTexture("_MainTex", imageSource);//设置图片
                m.SetTextureScale("_MainTex", new Vector2(delta, delta));//设置大小和偏移
                m.SetTextureOffset("_MainTex", new Vector2(delta * i, delta * j));
                //Debug.Log(m.GetTextureOffset("_MainTex")+"  "+ new Vector2(delta * i, delta * j));
            }
        }
        pictures[size - 1].GetComponent<MeshFilter>().mesh = null;
    }
    public void RandomImage()
    {
        //随机交换两张图片的位置，交换多次即可
    }
    public void SwapElement()
    {

    }
}
