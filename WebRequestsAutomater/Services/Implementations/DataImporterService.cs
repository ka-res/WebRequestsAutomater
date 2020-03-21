using System;
using System.Collections.Generic;
using System.IO;
using WebRequestsAutomater.Common;
using WebRequestsAutomater.Services.Interfaces;

namespace WebRequestsAutomater.Services.Implementations
{
    public class DataImporterService : IDataImporterService
    {
        public readonly string usersFilePath;
        public readonly string uniquesFilePath;

        public DataImporterService()
        {
            usersFilePath = GetFilePath(GetPathMode.Users);
            uniquesFilePath = GetFilePath(GetPathMode.Uniques);
        }
        
        public string GetFilePath(GetPathMode mode)
        {
            switch (mode)
            {
                case GetPathMode.Users:
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Constants.UsersFileName);
                case GetPathMode.Uniques:
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Constants.UniquesFileName);
                default:
                    return null;
            }
        }

        public bool CheckFileExistance()
        {
            var isFilePresent = File.Exists(usersFilePath);

            return isFilePresent;
        }

        public Dictionary<string, string> ImportData(out string lastUsername)
        {
            var tmpDictionary = new Dictionary<string, string>();
            var isFilePresent = CheckFileExistance();
            if (isFilePresent)
            {
                try
                {
                    using (var fileStream = new FileStream(usersFilePath, FileMode.Open))
                    {
                        using (var streamReader = new StreamReader(fileStream))
                        {
                            lastUsername = string.Empty;

                            while (!streamReader.EndOfStream)
                            {
                                var line = streamReader.ReadLine();
                                if (line == string.Empty || line.StartsWith("#"))
                                {
                                    continue;
                                }

                                var data = line.Split(';');
                                var user = data[0];
                                var password = data[1];
                                tmpDictionary.Add(user, password);
                                lastUsername = user;
                            }
                        }
                    }
                    
                    return tmpDictionary;
                }
                catch (Exception e)
                {
                    lastUsername = null;
                    return null;
                }
            }

            lastUsername = null;
            return null;
        }
    }
}
