using System.Collections.Generic;
using WebRequestsAutomater.Common;

namespace WebRequestsAutomater.Services.Interfaces
{
    public interface IDataImporterService
    {
        string GetFilePath(GetPathMode mode);
        bool CheckFileExistance();
        Dictionary<string, string> ImportData(out string lastUsername);
    }
}
