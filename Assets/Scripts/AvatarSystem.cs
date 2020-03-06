﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSystem : MonoBehaviour {
    //girl
    private GameObject girlTarget;//骨骼物体
    //girl所有的资源信息
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> girlData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
    private Transform[] girlHips;//girl骨骼信息
    private Transform girlTrans;
    Dictionary<string, SkinnedMeshRenderer> girlSMR = new Dictionary<string, SkinnedMeshRenderer>();//换装骨骼上的MeshReneder信息
    string[,] girlStr = new string[,] { { "eyes", "1" }, { "hair", "1" }, { "top", "1" }, { "pants", "1" }, { "shoes", "1" }, { "face", "1" } };

    //boy
    private GameObject boyTarget;//骨骼物体
    //boy所有的资源信息
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> boyData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
    private Transform[] boyHips;//boy骨骼信息
    private Transform boyTrans;
    Dictionary<string, SkinnedMeshRenderer> boySMR = new Dictionary<string, SkinnedMeshRenderer>();//换装骨骼上的MeshReneder信息
    string[,] boyStr = new string[,] { { "eyes", "1" }, { "hair", "1" }, { "top", "1" }, { "pants", "1" }, { "shoes", "1" }, { "face", "1" } };
    void Start() {
        InstantiateGirlAvatar();
        DataSave(girlTrans, girlTarget,girlData,girlSMR);
        InitAvatar(girlData ,girlHips,girlSMR,girlStr);

        InstantiateBoyAvatar();
        DataSave(boyTrans, boyTarget, boyData, boySMR);
        InitAvatar(boyData, boyHips, boySMR, boyStr);
    }

    void Update() {

    }
    //初始化Model资源和Target模板
    void InstantiateGirlAvatar() {
        GameObject source = Instantiate(Resources.Load<GameObject>("FemaleModel"));
        girlTrans = source.transform;
        source.SetActive(false);
        girlTarget = Instantiate(Resources.Load<GameObject>("FemaleTarget"));
        girlHips = girlTarget.GetComponentsInChildren<Transform>();//存储骨骼信息
    }
    void InstantiateBoyAvatar() {
        GameObject source = Instantiate(Resources.Load<GameObject>("MaleModel"));
        boyTrans = source.transform;
        source.SetActive(false);
        boyTarget = Instantiate(Resources.Load<GameObject>("MaleTarget"));
        boyHips = boyTarget.GetComponentsInChildren<Transform>();//存储骨骼信息
    }

    //存储人物模型的信息
    void DataSave(Transform trans, GameObject tar, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, Dictionary<string, SkinnedMeshRenderer> skm) {
        if (trans == null) {
            return;
        }
        //遍历所有子物体的SkinnedMeshRenderer组件添加到数组中存储
        SkinnedMeshRenderer[] parts = trans.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var part in parts) {
            string[] names = part.name.Split('-');
            if (!data.ContainsKey(names[0])) {
                //生成对应部位 且只生成一个
                GameObject partGo = new GameObject {
                    name = names[0]
                };
                partGo.transform.parent = tar.transform;
                //把骨骼target身上的SkinnedMeshReneder信息存储
                skm.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>());

                data.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            data[names[0]].Add(names[1], part);//存储所有的SkinnedMeshReneder信息
        }
    }

    void MeshReplace(Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, Transform[] hips, Dictionary<string, SkinnedMeshRenderer> smr, string part, string num) {
        SkinnedMeshRenderer skm = data[part][num];//部位资源
        //获取骨骼
        List<Transform> bones = new List<Transform>();
        foreach (Transform skmBone in skm.bones) {
            foreach (Transform bone in hips) {
                if (bone.name == skmBone.name) {
                    bones.Add(bone);
                    break;
                }
            }
        }
        //更换mesh
        smr[part].bones = bones.ToArray();//绑定骨骼
        smr[part].materials = skm.materials;
        smr[part].sharedMesh = skm.sharedMesh;
    }
    //初始化
    void InitAvatar(Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, Transform[] hips, Dictionary<string, SkinnedMeshRenderer> smr, string[,] obj) {
        int length = obj.GetLength(0);//获取行数
        for (int i = 0; i < length; i++) {
            MeshReplace(data, hips, smr, obj[i, 0], obj[i, 1]);
        }
    }
}
