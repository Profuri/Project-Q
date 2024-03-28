using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

public class FileDataHandler
{
    private string _directoryPath = "";
    private string _fileName = "";

    private bool _isEncrypt;
    private bool _isBase64;

    private CryotoModule _cryptoModule;

    public FileDataHandler(string directoryPath, string fileName, bool isEncrypt, bool isBase64 = false)
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
        _isEncrypt = isEncrypt;
        _isBase64 = isBase64;
        _cryptoModule = new CryotoModule();
    }

    public void Save(SaveData data)
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);

        try
        {
            Directory.CreateDirectory(_directoryPath);
            string dataToStore = JsonUtility.ToJson(data, true); //예쁘게 출력됨.

            if (_isEncrypt)
            {
                dataToStore = _cryptoModule.AESEncrypt256(dataToStore);
                //dataToStore = EncyptAndDecrpytData(dataToStore);
            }
                                            
            if (_isBase64)
            {
                dataToStore = Base64Process(dataToStore, true);
            }
            using (FileStream writeStream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(writeStream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error on trying to save data to file: {fullPath} \n {ex.Message}");
        }
    }

    public SaveData Load()
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);
        SaveData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream readStream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(readStream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (_isEncrypt)
                {
                    dataToLoad = EncyptAndDecrpytData(dataToLoad);
                }
                if (_isBase64)
                {
                    dataToLoad = _cryptoModule.Decrypt(dataToLoad);
                    //dataToLoad = Base64Process(dataToLoad, false);
                }
                loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on trying to load data to file: {fullPath} \n {ex.Message} ");
            }
        }

        if(loadedData == null)
        {
            return new SaveData();
        }
        return loadedData;
    }

    public void DeleteSaveData()
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);
        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on trying to delete file {fullPath} \n {ex.Message}");
            }
        }


    }
    private string _codeWord = "leagueoflegendraven";

    private string EncyptAndDecrpytData(string data)
    {
        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append((char)(data[i] ^ _codeWord[i % _codeWord.Length]));
        }
        return sBuilder.ToString();
    }

    //인코딩 => 표현함 디코딩 => 원상 복구
    private string Base64Process(string data, bool encoding)
    {
        if (encoding)
        {
            byte[] dataByteArr = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(dataByteArr); //base64 문자열로 나타낸다.
        }
        else
        {
            byte[] dataByteArr = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(dataByteArr);
        }

    }
}
