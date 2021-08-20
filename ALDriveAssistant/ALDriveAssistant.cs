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

        public ALDriveAssistant()
        {
            InitializeComponent();
            alDriveHelper = new ALDriveHelper();

            _Init();
        }

        void _Init()
        {
            try
            {
                Task<string> taskAuthorize = alDriveHelper.AuthorizeAsync();
                taskAuthorize.Wait();

                Task<bool> taskAutoLogin = alDriveHelper.AutoLoginAsync();

                taskAutoLogin.Wait();
                if (taskAutoLogin.Result)
                {
                    LoginSuccess();
                    return;
                }

                QRCodeGenerate();
                Task.Run(QRCodeQuery);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region-- QRCodeGenerate()
        void QRCodeGenerate()
        {
            Bitmap image = null;
            QRCode qrCode = null;
            QRCodeData qrCodeData = null;
            QRCodeGenerator qrGenerator = null;
            try
            {
                Task<string> taskQRCodeContent = alDriveHelper.QRCodeContentGetAsync();
                taskQRCodeContent.Wait();

                if (string.IsNullOrWhiteSpace(taskQRCodeContent.Result)) throw new Exception("获取二维码失败！");

                qrGenerator = new QRCodeGenerator();
                qrCodeData = qrGenerator.CreateQrCode(taskQRCodeContent.Result, QRCodeGenerator.ECCLevel.M);
                qrCode = new QRCode(qrCodeData);
                image = qrCode.GetGraphic(4);

                img__qr_code.Size = new Size(image.Width, image.Height);
                img__qr_code.Image = Image.FromHbitmap(image.GetHbitmap());

                img__qr_code.Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (image != null) image.Dispose();
                if (qrCode != null) qrCode.Dispose();
                if (qrCodeData != null) qrCodeData.Dispose();
                if (qrGenerator != null) qrGenerator.Dispose();
            }
        }
        #endregion

        #region-- QRCodeQuery()
        void QRCodeQuery()
        {
            try
            {
                while (true)
                {
                    Task<string> taskQRCodeQuery = alDriveHelper.QRCodeQueryAsync();
                    taskQRCodeQuery.Wait();

                    string strQRCodeStatus = "";
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    if (!string.IsNullOrWhiteSpace(taskQRCodeQuery.Result))
                        result = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskQRCodeQuery.Result);

                    if (result != null && result.ContainsKey("hasError") && result["hasError"].Equals(false))
                        strQRCodeStatus = ((result["content"] as JObject)["data"] as JObject)["qrCodeStatus"].ToString();

                    switch (strQRCodeStatus)
                    {
                        case "EXPIRED":
                            QRCodeGenerate();
                            break;

                        case "CONFIRMED":
                            string bizExt = ((result["content"] as JObject)["data"] as JObject)["bizExt"].ToString();
                            byte[] bytes = Convert.FromBase64String(bizExt);
                            bizExt = Encoding.UTF8.GetString(bytes);

                            object loginResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(bizExt)["pds_login_result"];
                            Task<string> taskTokenLogin = alDriveHelper.TokenLoginAsync((loginResult as JObject)["accessToken"].ToString());

                            taskTokenLogin.Wait();
                            Dictionary<string, object> dict_Token = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskTokenLogin.Result);

                            if (dict_Token == null || !dict_Token.ContainsKey("access_token")) throw new Exception("登录失败");

                            alDriveHelper.SetToken = dict_Token;
                            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Token.json"), taskTokenLogin.Result);

                            LoginSuccess();
                            return;
                    }

                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region-- LoginSuccess()
        void LoginSuccess()
        {
            try
            {
                Task<string> taskGetFileList = alDriveHelper.GetFileListAsync();

                if (!img__qr_code.InvokeRequired)
                {
                    img__qr_code.Visible = false;
                    dataGrid__file_list.Visible = true;
                }
                else
                {
                    img__qr_code.Invoke(() => img__qr_code.Visible = false);
                    dataGrid__file_list.Invoke(() => dataGrid__file_list.Visible = true);
                }

                taskGetFileList.Wait();
                Dictionary<string, object> dict_FileList = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskGetFileList.Result);
                if (dict_FileList == null || !dict_FileList.ContainsKey("items") || (dict_FileList["items"] as JArray).Count == 0)
                {
                    if(!dataGrid__file_list.InvokeRequired)
                        dataGrid__file_list.Rows.Clear();
                    else
                        dataGrid__file_list.Invoke(() => dataGrid__file_list.Rows.Clear());

                    return;
                }

                JArray lst_Files = dict_FileList["items"] as JArray;
                foreach (var item in lst_Files)
                {
                    var fileInfo = item.ToObject<Dictionary<string, object>>();
                    object[] rowData = new object[] { null, fileInfo["file_id"], fileInfo["name"], Convert.ToDateTime(fileInfo["updated_at"]).ToString("yyyy-MM-dd HH:mm"), fileInfo.ContainsKey("size") ? fileInfo["size"] : "" };
                    if (!dataGrid__file_list.InvokeRequired)
                        dataGrid__file_list.Rows.Add(rowData);
                    else
                        dataGrid__file_list.Invoke(() => dataGrid__file_list.Rows.Add(rowData));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
