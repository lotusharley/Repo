using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace xTrace.Utils
{
    public class FileUtils
    {
        public static async Task<string> ReadTextNavFile(string sCycleInfo,string sFilename)
        {
            string sRtn = string.Empty;
            Windows.Storage.StorageFolder localfolder = await Package.Current.InstalledLocation.GetFolderAsync("Navdata\\" + sCycleInfo);
            Stream stm = await localfolder.OpenStreamForReadAsync(sFilename);
            StreamReader stmRdr = new StreamReader(stm);
            sRtn = stmRdr.ReadToEnd();
            return sRtn;
        }

        public static async Task<List<String>> ReturnCycleInfos()
        {

            List<String> rtn = new List<string>();

            Windows.Storage.StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Navdata\\");
            System.Collections.Generic.IReadOnlyList<Windows.Storage.StorageFolder> folders = await folder.GetFoldersAsync();
            foreach(Windows.Storage.StorageFolder f in folders)
            {
                rtn.Add(f.Name);
            }
            return rtn;
        }
    }
}
