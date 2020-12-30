using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BarnoGames.Runner2020.Editor
{
#if UNITY_EDITOR
    //[InitializeOnLoad]
    public static class SoundFXEnumGenerator
    {
        [MenuItem("BarnoTools/Generate Sounds Enums")]
        //[InitializeOnLoadMethod]
        public static void GenerateEnumsFrom()
        {
            List<string> stringToEnumList = new List<string>();
            Debug.Log("Generating Enums from Sounds Found");
            string folderPath = "Assets/BarnoGames/Sounds/";

            string enumName = "Sounds";
            string filePathAndName = "Assets/BarnoGames/Sounds/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            if (!Directory.Exists(folderPath))
            {
                Debug.Log("Created Folder");
                AssetDatabase.Refresh();
            }
            else
            {
                foreach (string file in Directory.GetFiles("Assets/BarnoGames", "*.wav", SearchOption.AllDirectories))
                {
                    string str = file;
                    string filename = Path.GetFileName(str);

                    filename = GetUntilOrEmpty(filename, ".");

                    stringToEnumList.Add(filename);

                    // TODO:: MOVE TO SOUNDS FOLDER
                }

                foreach (string file in Directory.GetFiles("Assets/BarnoGames", "*.cs", SearchOption.AllDirectories))
                {
                    string str = file;
                    string filename = Path.GetFileName(str);

                    filename = GetUntilOrEmpty(filename, ".");

                    //Debug.Log(filename);
                    stringToEnumList.Add(filename);

                    // TODO:: MOVE TO SOUNDS FOLDER
                }

                if (stringToEnumList.Count != 0)
                {
                    using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
                    {
                        streamWriter.WriteLine("namespace BarnoGames.Runner2020");
                        streamWriter.WriteLine("{");
                        streamWriter.WriteLine("   internal enum " + enumName);
                        streamWriter.WriteLine("   {");

                        foreach (string item in stringToEnumList)
                        {
                            streamWriter.WriteLine("\t" + item + ",");
                        }
                        streamWriter.WriteLine("   }");
                        streamWriter.WriteLine("}");

                        streamWriter.Close();
                    }
                    AssetDatabase.Refresh();

                };
            }
        }

        public static string GetUntilOrEmpty(this string text, string stopAt = ".")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
    }
#endif
}