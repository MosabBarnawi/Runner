using System.Security.Cryptography;
using System.Collections.Generic;
using BarnoGames.Runner2020;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

namespace BarnoGames.Utilities
{
    public enum SettingsMap { AutoGoIn, FirstLaunch }

    public class JsonSaveAndLoadScript : MonoSingleton<JsonSaveAndLoadScript>
    {
        private static JsonSaveAndLoadScript SharedInstance;
        private const string FILENAME = "UserDataRunner.json";
        private StoredData prefs;

        private bool IS_ENCRYPTED = false;
        private string ENCRYPT_HASH = "Q!M+89/>d$p";

        [SerializeField]
        private bool Save = true;

        private string _path;
        private string _jsonString;

        public bool DoesSaveFileExist
        {
            get
            {
                return File.Exists(_path);
            }
        }

        private bool HasLoaded
        {
            set
            {
                if (value == true)
                {
                    GameManager.SharedInstance.OnInitilization();
                }
            }
        }

        #region Unity Callbacks
        public override void Init()
        {
            base.Init();
            _path = Path.Combine(Application.persistentDataPath, FILENAME);
        }
        //private void Awake()
        //{
        //    if (SharedInstance == null)
        //    {
        //        SharedInstance = this;
        //        _path = Path.Combine(Application.persistentDataPath, FILENAME);
        //    }
        //    else if (SharedInstance != this)
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        private void Start()
        {
            LoadFromDisk();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveToDisk();
        }

        //private void OnApplicationFocus( bool focus )
        //{
        //    if (!focus) SaveToDisk();
        //}

        private void OnApplicationQuit()
        {
            SaveToDisk();
        }

        #endregion

        #region Private API

        private void LoadFromDisk()
        {
            if (Save)
            {
                #region IF SAVE FILE DOSE NOT EXIST
                if (!DoesSaveFileExist)
                {
                    LoadIfFileDoesNotExist();
                }
                #endregion

                #region IF SAVE FILE EXISTS

                if (DoesSaveFileExist)
                {
                    try
                    {
                        _jsonString = File.ReadAllText(_path);
                        if (IS_ENCRYPTED)
                        {
                            string DecryptedText = Decrypt(_jsonString);
                            prefs = JsonUtility.FromJson<StoredData>(DecryptedText);
                        }
                        else
                        {
                            prefs = JsonUtility.FromJson<StoredData>(_jsonString);
                        }
                    }

                    catch (Exception e)
                    {
                        DeleteData();
                        LoadIfFileDoesNotExist();
                        Debug.LogErrorFormat("Failed parsing save file. Resetting. Error: {0}", e);
                    }
                }

                getData();

                #endregion

                Debug.Log("Loaded ---> Save File");
                HasLoaded = true;
            }
            else
            {
                Debug.LogWarning("Saving Function is Disabled");
                HasLoaded = true;
            }
        }

        private void SaveToDisk()
        {
            if (Save)
            {
                setData();

                if (IS_ENCRYPTED)
                {
                    _jsonString = JsonUtility.ToJson(prefs);
                    string EncryptedJson = Encrypt(_jsonString);
                    File.WriteAllText(_path, EncryptedJson);
                }
                else
                {
                    _jsonString = JsonUtility.ToJson(prefs);
                    File.WriteAllText(_path, _jsonString);
                }

                Debug.LogFormat("Saved ---> {0}", _path);
            }
        }

        private void DeleteData()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
                PlayerPrefs.DeleteAll();
                Debug.Log("Deleted ---> Save File");
            }

            else return;
        }

        #endregion

        public void SetOptionsSettings()
        {
            prefs.generalSettings.AutoGoIn = OptionsManager.instance.isAutoGoIn;
        }

        private void GetOptionsSettings()
        {
            Dictionary<SettingsMap, object> hashMap = new Dictionary<SettingsMap, object>();

            hashMap.Add(SettingsMap.AutoGoIn, prefs.generalSettings.AutoGoIn);

            OptionsManager.instance.SetSetting(hashMap);
        }

        #region Private Helpers

        private void setData()
        {
            SetOptionsSettings();

            string saveLevel = GameManager.SharedInstance.GetCurrentScene();

            if (saveLevel != Level.EMPTY_LEVEL)
            {
                prefs.gameManagerSettings.SavedScene = GameManager.SharedInstance.GetCurrentScene();
            }

            //prefs.gameManagerSettings.VALUE_TEST = GameManager.SharedInstance.TEXT_VALUE;
        }

        private void getData()
        {
            GetOptionsSettings();
            GameManager.SharedInstance.SceneToLoad(prefs.gameManagerSettings.SavedScene);
            //GameManager.SharedInstance.LoadSpecificLevel(prefs.gameManagerSettings.SavedScene, null);
            //GameManager.SharedInstance.TEXT_VALUE = prefs.gameManagerSettings.VALUE_TEST;
        }

        private void LoadIfFileDoesNotExist()
        {
            PlayerPrefs.DeleteAll();
            prefs = new StoredData();

            if (IS_ENCRYPTED)
            {
                _jsonString = JsonUtility.ToJson(prefs);
                string EncryptedJson = Encrypt(_jsonString);
                File.WriteAllText(_path, EncryptedJson);
            }
            else
            {
                _jsonString = JsonUtility.ToJson(prefs);
                File.WriteAllText(_path, _jsonString);
            }

            Debug.Log("Cretaed Save File");
        }

        private string Encrypt(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(ENCRYPT_HASH));
                using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform tr = trip.CreateEncryptor();
                    byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        private string Decrypt(string input)
        {
            byte[] data = Convert.FromBase64String(input);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(ENCRYPT_HASH));
                using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider()
                {
                    Key = key,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
                {
                    ICryptoTransform tr = trip.CreateDecryptor();
                    byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                    return Encoding.UTF8.GetString(results);
                }
            }
        }
        #endregion
    }

}
