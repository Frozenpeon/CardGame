using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//Author : Thomas VERDIER

namespace Com.IsartDigital.F2P  {
    /// <summary>
    /// Tool used to save and load datas in a json file.
    /// </summary>
    public class JSonDataService : IDataService  {

        private const string AES_KEY = "v4UjQOovxhzxlrTj7GaO9V2ejPc3Cu+2XJ6o/6yfbf0=";
        private const string AES_IV = "JUnUxMRSrwMKBw9UVyZWFg==";
        public T LoadData<T>(string pRelativePath, bool pEncrypted)  {
            string path = Application.persistentDataPath + pRelativePath;

            if (!File.Exists(path))  {
                Debug.LogError("Cannot find the saved file");
                throw new FileNotFoundException("Couldn't fing the file at path : " + path);
            } 

            try {
                T data;
                if (pEncrypted) { 
                    data = ReadEncryptedData<T>(path);
                }
                else 
                    data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            } catch (Exception e)  {
                Debug.LogError("Error while trying to load data at : " + path);
                throw e;
            }
        }

        private T ReadEncryptedData<T>(string pPath) {
            byte[] fileBytes = File.ReadAllBytes(pPath);
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(AES_KEY);
            aesProvider.IV = Convert.FromBase64String(AES_IV);
            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            using MemoryStream stream = new MemoryStream(fileBytes);
            using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Read);
            using StreamReader reader = new StreamReader(cryptoStream);
            string result = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(result);
        }
        public bool SaveData<T>(string pRelativePath, T pData, bool pEncrypted) {
            string path = Application.persistentDataPath + pRelativePath;
            try {
                if (File.Exists(path))
                    File.Delete(path);
                FileStream stream = File.Create(path);
                if (pEncrypted) {
                    WriteEncryptedData(pData, stream);
                    stream.Close();
                }
                else  {
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(pData, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                }
                return true;
            } catch(Exception e)  {
                Debug.LogError("Error when trying to save data : " + e.Message);
                return false;
            }
        }

        private void WriteEncryptedData<T>(T pData,FileStream stream)  {
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(AES_KEY);
            aesProvider.IV = Convert.FromBase64String(AES_IV);
            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(stream,  cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pData)));
        }
    }
}
