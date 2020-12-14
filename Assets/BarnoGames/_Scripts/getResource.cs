using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

namespace BarnoGames.Runner2020
{
    public class getResource : MonoBehaviour
    {
        public MovementSettings[] gjS;
        void Start()
        {

        }


        void Update()
        {

        }

        [ContextMenu("dsd")]
        public void dosomething()
        {
            gjS = Resources.LoadAll<MovementSettings>("Test");

            foreach (var t in gjS)
            {
                Debug.Log(t.name);
            }
        }



#if UNITY_EDITOR
        [MenuItem("BarnoTools/CustomAttribute")]
        public static void TestMe()
        {
            //List<GameObject> allOfRThem = new List<GameObject>();


            //List<GameObject> rootObjects = new List<GameObject>();
            //Scene scene = EditorSceneManager.GetActiveScene();
            //scene.GetRootGameObjects(rootObjects);
            MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();

            foreach (MonoBehaviour mono in allObjects)
            {
                FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < objectFields.Length; i++)
                {
                    CustomMine attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(CustomMine)) as CustomMine;
                    if (attribute != null)
                    {
                        Debug.Log(mono + "  =>> "  + objectFields[i].Name); // The name of the flagged variable.
                    }
                }
            }
            Debug.Log("================ Done");
        }


        //public class GenerateEnum
        //{
        [MenuItem("BarnoTools/GenerateEnum")]
        public static void Go()
        {
            Debug.Log("Not Implemented");
            //string enumName = "MyEnum";
            //string[] enumEntries = { "Foo", "Goo", "Hoo" };
            //string filePathAndName = "Assets/BarnoGames/_Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            //using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            //{
            //    streamWriter.WriteLine("public enum " + enumName);
            //    streamWriter.WriteLine("{");
            //    for (int i = 0; i < enumEntries.Length; i++)
            //    {
            //        streamWriter.WriteLine("\t" + enumEntries[i] + ",");
            //    }
            //    streamWriter.WriteLine("}");
            //}
            //AssetDatabase.Refresh();

            //var types = from t in Assembly.GetExecutingAssembly().GetTypes()
            //            where t.GetCustomAttributes<MySpeciallllAttribute>().Count() > 0
            //            select t;

            //foreach (var item in types)
            //{
            //    Debug.Log(item);
            //}

        }
        //}
#endif
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CustomMine : Attribute { }

    //[AttributeUsage(AttributeTargets.Class)]
    //public class MySpeciallllAttribute : Attribute
    //{
    //    //public MySpeciallllAttribute()
    //    //{
    //    //    Debug.Log("Hi");
    //    //}
    //}

}

