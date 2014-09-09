using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class HumanBoneList : MonoBehaviour
{
    [MenuItem("Mecanim/PrintBoneName")]
    public static void DoPrintBoneName()
    {
        string[] boneName = HumanTrait.BoneName;
        for (int i = 0; i < HumanTrait.BoneCount; ++i)
        {
            Debug.Log(boneName[i] + " :"+i);
        }
    }

    [MenuItem("Mecanim/PrintMuscleName")]
    public static void DoPrintMuscleName()
    {
        string[] boneName = HumanTrait.MuscleName;
        for (int i = 0; i < HumanTrait.MuscleCount; ++i)
        {
            Debug.Log(boneName[i]+ " :"+i);
        }
    }

    [MenuItem("Mecanim/PrintRequiredBone")]
    public static void DoPrintRequiredBone()
    {
        string[] boneName = HumanTrait.BoneName;
        for (int i = 0; i < HumanTrait.BoneCount; ++i)
        {
            if (HumanTrait.RequiredBone(i))
                Debug.Log(boneName[i] + " " + i);
        }
    }

    [MenuItem("Mecanim/PrintAvatarAccessibleBone")]
    public static void DoPrintAvatarAccessibleBone()
    {
        Animator anim = Component.FindObjectOfType(typeof(Animator)) as Animator;

        string[] boneName = HumanTrait.BoneName;
        for (int i = 0; i < HumanTrait.BoneCount; ++i)
        {
            if (anim.GetBoneTransform((HumanBodyBones)i))
                Debug.Log(boneName[i]+ " :"+i);
        }
    }    
}