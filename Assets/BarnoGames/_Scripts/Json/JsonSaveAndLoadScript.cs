﻿using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class JsonSaveAndLoadScript : MonoBehaviour
{
    private static JsonSaveAndLoadScript SharedInstance;
    private const string FILENAME = "UserDataRunner.json";
    private StoredData prefs;

    private const bool IS_ENCRYPTED = false;
    private const string ENCRYPT_HASH = "Q!M+89/>d$p";

    private string _path;
    private string _jsonString;
    public bool DoesSaveFileExist
    {
        get
        {
            return File.Exists(_path);
        }
    }

    public bool HasLoaded { get; private set; }

    #region Unity Callbacks

    private void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(gameObject);
            SharedInstance = this;

            _path = Path.Combine(Application.persistentDataPath , FILENAME);

        }
        else if (SharedInstance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadFromDisk();
    }

    private void OnApplicationPause( bool pause )
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
                Debug.LogErrorFormat("Failed parsing save file. Resetting. Error: {0}" , e);
            }
        }

        getData();

        #endregion

        HasLoaded = true;
        Debug.Log("Loaded ---> Save File");
    }

    private void SaveToDisk()
    {
        setData();

        if (IS_ENCRYPTED)
        {
            _jsonString = JsonUtility.ToJson(prefs);
            string EncryptedJson = Encrypt(_jsonString);
            File.WriteAllText(_path , EncryptedJson);
        }
        else
        {
            _jsonString = JsonUtility.ToJson(prefs);
            File.WriteAllText(_path , _jsonString);
        }

        Debug.LogFormat("Saved ---> {0}" , _path);
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

    #region Private Helpers

    private void setData()
    {
        prefs.gameManagerSettings.VALUE_TEST = GameManager.SharedInstance.TEXT_VALUE;
    }

    private void getData()
    {
        GameManager.SharedInstance.TEXT_VALUE = prefs.gameManagerSettings.VALUE_TEST;
    }

    private void LoadIfFileDoesNotExist()
    {
        PlayerPrefs.DeleteAll();
        prefs = new StoredData();

        if (IS_ENCRYPTED)
        {
            _jsonString = JsonUtility.ToJson(prefs);
            string EncryptedJson = Encrypt(_jsonString);
            File.WriteAllText(_path , EncryptedJson);
        }
        else
        {
            _jsonString = JsonUtility.ToJson(prefs);
            File.WriteAllText(_path , _jsonString);
        }

        Debug.Log("Cretaed Save File");
    }

    private static string Encrypt( string input )
    {
        byte [] data = Encoding.UTF8.GetBytes(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte [] key = md5.ComputeHash(Encoding.UTF8.GetBytes(ENCRYPT_HASH));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key , Mode = CipherMode.ECB , Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateEncryptor();
                byte [] results = tr.TransformFinalBlock(data , 0 , data.Length);
                return Convert.ToBase64String(results , 0 , results.Length);
            }
        }
    }

    private static string Decrypt( string input )
    {
        byte [] data = Convert.FromBase64String(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte [] key = md5.ComputeHash(Encoding.UTF8.GetBytes(ENCRYPT_HASH));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider()
            {
                Key = key ,
                Mode = CipherMode.ECB ,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform tr = trip.CreateDecryptor();
                byte [] results = tr.TransformFinalBlock(data , 0 , data.Length);
                return Encoding.UTF8.GetString(results);
            }
        }
    }
    #endregion
}
