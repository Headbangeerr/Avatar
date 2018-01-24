using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSystem : MonoBehaviour {

    public static AvatarSystem _instance;

    private Transform girlSourceTrans;
    private GameObject grilTarget;//女孩模型骨架
    private string[,] modelInitStr= new string [,] {{"eyes","1"},{"face","1" },{"hair","1"},
        { "pants","1" },{ "shoes","1" },{ "top","1"} };//女孩各部分初始化

    Transform[] girlHips;//各个骨骼组成部分
    Dictionary<string,Dictionary<string,SkinnedMeshRenderer>> girlData= new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
    //用于存储女孩的资源信息
    Dictionary<string, SkinnedMeshRenderer> girlSmr = new Dictionary<string, SkinnedMeshRenderer>();
    //换装骨骼上的各个skinMeshRenderer的信息

    private Transform boySourceTrans;
    private GameObject boyTarget;//男孩模型骨架
 

    Transform[] boyHips;//各个骨骼组成部分
    Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> boyData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
    //用于存储男孩的资源信息
    Dictionary<string, SkinnedMeshRenderer> boySmr = new Dictionary<string, SkinnedMeshRenderer>();
    //换装骨骼上的各个skinMeshRenderer的信息

    private void Awake()
    {
        _instance = this;
    }
    public AvatarSystem GetInstance()
    {
        return _instance;
    }
    private void Start()
    {
        InstantiateSource();       
        SaveData(girlSourceTrans,girlData,girlSmr,grilTarget);
        SaveData(boySourceTrans, boyData, boySmr, boyTarget);
        InitAvatar();
        boyTarget.SetActive(false);
    }
    /// <summary>
    /// 加载模型资源
    /// </summary>
    void InstantiateSource()
    {
        //获取Resources目录下的资源
        GameObject go = Instantiate(Resources.Load("FemaleModel")) as GameObject;
        //获取模型中的各个组件以后再禁用
        girlSourceTrans = go.transform;
        go.SetActive(false);
        go = Instantiate(Resources.Load("MaleModel")) as GameObject;
        boySourceTrans = go.transform;
        go.SetActive(false);
        

        //生成并获取到模型骨骼
        grilTarget = Instantiate(Resources.Load("FemaleTarget")) as GameObject;
        girlHips = grilTarget.GetComponentsInChildren<Transform>();
        boyTarget = Instantiate(Resources.Load("MaleTarget")) as GameObject;
        boyHips = boyTarget.GetComponentsInChildren<Transform>();
    } 

    /// <summary>
    /// 存储原模型文件中所有的资源，并按照部分名称进行保存。在目标骨骼模型上生成各组成部分的空物体，用于以后换装存储指定的skm
    /// </summary>
    void SaveData(Transform srcTrans, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, 
        Dictionary<string, SkinnedMeshRenderer> modelSmr,GameObject target)
    {
        //女孩相关资源
        if (srcTrans == null) return;
        SkinnedMeshRenderer[] parts = srcTrans.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var part in parts)
        {
            //将各个组件的名字进行分割，名字的格式均为“eye-1”
            string[] names = part.name.Split('-');
            if (!data.ContainsKey(names[0]))
            {
                //在骨骼上生成各个组成部分，且只生成一个
                GameObject partGo = new GameObject();
                partGo.name = names[0];
                partGo.transform.parent = target.transform;
                modelSmr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>());//存储target骨骼上的skmr信息
                data.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            data[names[0]].Add(names[1], part);//存储所有的skmr信息
        }       
    }
    /// <summary>
    /// 通过传入组成部分名称，以及要换装的部分编号进行换装
    /// </summary>
    /// <param name="part"></param>
    /// <param name="num"></param>
    void ChangeMesh(string part, string num,Transform[] hips, 
        Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, Dictionary<string, SkinnedMeshRenderer> modelSkm)
    {
        SkinnedMeshRenderer skm = data[part][num];
        //Dictionary可以通过[]操作符通过键值获取对应的对象，这里的girlData中value值是也是一个字典，所以第一个[]获取到的还是一个字典对象，第二个[]是对第一个返回的字典对象的取值
        List<Transform> bones = new List<Transform>();
        foreach(Transform tran in skm.bones)
        {
            foreach (var bone in hips)
            {
                if(bone.name == tran.name)
                {
                    bones.Add(bone);
                    break;
                }
            }
        }
        //实现换装
        modelSkm[part].sharedMaterials = skm.sharedMaterials;
        modelSkm[part].bones = bones.ToArray();
        modelSkm[part].sharedMesh = skm.sharedMesh;
    }
    void InitAvatar() {
        int length = modelInitStr.GetLength(0);//获取二维数组的行数
        for (int i = 0; i < length; i++) {
            ChangeMesh(modelInitStr[i,0], modelInitStr[i,1],girlHips,girlData,girlSmr);
            ChangeMesh(modelInitStr[i, 0], modelInitStr[i, 1], boyHips, boyData, boySmr);
        }

    }
    
} 