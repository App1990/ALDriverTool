using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALDriveAssistant
{
    public partial class Login : Form
    {
        Task<bool> taskCodeQuery;
        CancellationTokenSource tokenSource;
        ALDriveHelper alDriveHelper;

        public Login()
        {
            InitializeComponent();
            alDriveHelper = new ALDriveHelper();
            txt__qr_code_msg.TextAlign = HorizontalAlignment.Center;

            _Init();
        }

        #region-- _Init()
        void _Init()
        {
            try
            {
                Task<string> taskAuthorize = alDriveHelper.AuthorizeAsync();
                taskAuthorize.Wait();

                Task<bool> taskAutoLogin = alDriveHelper.AutoLoginAsync();
                taskAutoLogin.Wait();

                // 自动登录成功，加载网盘文件列表
                if (taskAutoLogin.Result)
                {
                    _LoadAssistant();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

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
                image = qrCode.GetGraphic(3);

                img__qr_code.Size = new Size(image.Width, image.Height);
                img__qr_code.Image = Image.FromHbitmap(image.GetHbitmap());
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
        bool QRCodeQuery()
        {
            try
            {
                while (true)
                {
                    if (tokenSource != null && tokenSource.Token.IsCancellationRequested) return false;

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

                        case "SCANED":
                            if (!txt__qr_code_msg.InvokeRequired)
                                txt__qr_code_msg.Text = "已扫描，等待确认...";
                            else
                                txt__qr_code_msg.Invoke(() => txt__qr_code_msg.Text = "已扫描，等待确认...");
                            break;

                        case "CONFIRMED":
                            if (!txt__qr_code_msg.InvokeRequired)
                                txt__qr_code_msg.Text = "已确认，登陆中...";
                            else
                                txt__qr_code_msg.Invoke(() => txt__qr_code_msg.Text = "已确认，登陆中...");

                            string bizExt = ((result["content"] as JObject)["data"] as JObject)["bizExt"].ToString();
                            byte[] bytes = Convert.FromBase64String(bizExt);
                            bizExt = Encoding.UTF8.GetString(bytes);

                            object loginResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(bizExt)["pds_login_result"];
                            Task<string> taskTokenLogin = alDriveHelper.TokenLoginAsync((loginResult as JObject)["accessToken"].ToString());

                            taskTokenLogin.Wait();
                            Dictionary<string, object> dict_Token = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskTokenLogin.Result);

                            if (dict_Token == null || !dict_Token.ContainsKey("access_token"))
                            {
                                if (!txt__qr_code_msg.InvokeRequired)
                                    txt__qr_code_msg.Text = "登录失败，请重新登录...";
                                else
                                    txt__qr_code_msg.Invoke(() => txt__qr_code_msg.Text = "登录失败，请重新登录...");

                                return false;
                            }

                            alDriveHelper.SetToken = dict_Token;
                            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Token.json"), taskTokenLogin.Result);

                            //_LoadAssistant();
                            return true;
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

        #region-- _LoadAssistant()
        void _LoadAssistant()
        {
            try
            {
                ALDriveAssistant assistant = new ALDriveAssistant(alDriveHelper);
                assistant.Owner = this;
                assistant.StartPosition = FormStartPosition.CenterScreen;

                Hide();
                assistant.ShowDialog();
                Application.ExitThread();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private void btn__login_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tab__login_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tab__login_type.SelectedIndex == 1)
                {
                    QRCodeGenerate();
                    tokenSource = new CancellationTokenSource();
                    TaskScheduler uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                    taskCodeQuery = Task.Run(QRCodeQuery, tokenSource.Token);
                    taskCodeQuery.ContinueWith(m =>
                    {
                        if (taskCodeQuery.Result) _LoadAssistant();
                    }, uiContext);
                }
                else
                {
                    if (taskCodeQuery != null && tokenSource != null) tokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
