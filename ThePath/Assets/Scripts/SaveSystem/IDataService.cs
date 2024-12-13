namespace Com.IsartDigital.F2P
{
    public interface IDataService 
    {
        public bool SaveData<T>(string pRelativePath, T pData, bool pEncrypted);

        public T LoadData<T>(string pRelativePath, bool pEncrypted);
    }
}
