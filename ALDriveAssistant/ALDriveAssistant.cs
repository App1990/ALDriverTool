using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using QRCoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALDriveAssistant
{
    public partial class ALDriveAssistant : Form
    {
        ALDriveHelper alDriveHelper;

        public ALDriveAssistant(ALDriveHelper alDriveHelper)
        {
            InitializeComponent();
            this.alDriveHelper = alDriveHelper;

            GetFileList();
        }

        #region-- GetFileList()
        void GetFileList(string strParentFileID = "root", string strNextMarker = "")
        {
            try
            {
                Task<string> taskGetFileList = alDriveHelper.GetFileListAsync(strParentFileID, strNextMarker);

                taskGetFileList.Wait();
                Dictionary<string, object> dict_FileList = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskGetFileList.Result);
                if (dict_FileList == null || !dict_FileList.ContainsKey("items") || (dict_FileList["items"] as JArray).Count == 0)
                {
                    dataGrid__file_list.Rows.Clear();

                    return;
                }

                JArray lst_Files = dict_FileList["items"] as JArray;
                foreach (var item in lst_Files)
                {
                    var fileInfo = item.ToObject<Dictionary<string, object>>();
                    object[] rowData = new object[] { null, fileInfo["file_id"], fileInfo["name"], Convert.ToDateTime(fileInfo["updated_at"]).ToString("yyyy-MM-dd HH:mm"), fileInfo.ContainsKey("size") ? fileInfo["size"] : "" };
                    dataGrid__file_list.Rows.Add(rowData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
