//using System.Collections.Generic;
//using System.Collections;
//using System.Reflection;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//using System.IO;
//using System;
//using UnityEditor.Callbacks;

//namespace BarnoGames.Runner2020
//{
//#if UNITY_EDITOR



//    //[UnityEditor.Callbacks.DidReloadScripts]
//    public class GenerateEnumFromGlobalJump
//    {
//        //[OnOpenAssetAttribute(1)]
//        //public static bool step1(int instanceID, int line)
//        //{
//        //    string name = EditorUtility.InstanceIDToObject(instanceID).name;
//        //    Debug.Log("Open Asset step: 1 (" + name + ")");
//        //    return false; // we did not handle the open
//        //}

//        //// step2 has an attribute with index 2, so will be called after step1
//        //[OnOpenAssetAttribute(2)]
//        //public static bool step2(int instanceID, int line)
//        //{
//        //    Debug.Log("Open Asset step: 2 (" + instanceID + ")");
//        //    return false; // we did not handle the open
//        //}




//        [MenuItem("Tools/GenerateEnum")]
//        public static void Go()
//        {
//            string enumName = "MyEnum";
//            string[] enumEntries = { "Fooooo", "Goo", "Hoo" };
//            string filePathAndName = "Assets/BarnoGames/_Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

//            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
//            {
//                streamWriter.WriteLine("public enum " + enumName);
//                streamWriter.WriteLine("{");
//                for (int i = 0; i < enumEntries.Length; i++)
//                {
//                    streamWriter.WriteLine("\t" + enumEntries[i] + ",");
//                }
//                streamWriter.WriteLine("}");
//            }
//            AssetDatabase.Refresh();

//            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
//                        where t.GetCustomAttributes<MySpeciallllAttribute>().Count() > 0
//                        select t;

//            foreach (var item in types)
//            {
//                Debug.Log(item);
//            }
//        }
//    }


//    [AttributeUsage(AttributeTargets.Class)]
//    internal class MySpeciallllAttribute : Attribute
//    {
//        //public MySpeciallllAttribute()
//        //{
//        //    Debug.Log("Hi");
//        //}
//    }
//#endif
//}