// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using QRCoder;
using ConsoleTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

const string ClientID = "25dzX3vbYqktVxyX";

const string QRCodeQueryUrl = "https://passport.aliyundrive.com/newlogin/qrcode/query.do?appName=aliyun_drive";
const string QRCodeGenerateUrl = "https://passport.aliyundrive.com/newlogin/qrcode/generate.do?appName=aliyun_drive";

const string UserGetUrl = "https://api.aliyundrive.com/v2/user/get";
//const string TokenGetUrl = "https://api.aliyundrive.com/token/get";
const string TokenRefleshUrl = "https://api.aliyundrive.com/token/refresh";

const string FileListGetUrl = "https://api.aliyundrive.com/adrive/v3/file/list";
const string FileRenameUrl = "https://api.aliyundrive.com/v3/file/update";


string strAppPath = Environment.CurrentDirectory;
string strTokenFileName = Path.Combine(strAppPath, "Token.ini");
Dictionary<string, object> dict_Token = new Dictionary<string, object>();


if (!await AutoLoginAsync().ConfigureAwait(false))
    Console.WriteLine("登录失败，请扫码登录：");

QRCodeLoginAsync();

Console.ReadLine();
return;

bool blnExitTool = false;
string strDriveId = dict_Token["default_drive_id"].ToString();
Dictionary<string, object> dict_FileList = new Dictionary<string, object>();

Console.WriteLine("请选择操作：");
while (!blnExitTool)
{
    Console.WriteLine("0=退出程序，1=获取文件列表，2=文件重命名");
    string strOperateType = Console.ReadLine();

    switch (strOperateType)
    {
        case "0": return;

        case "1":
            Console.WriteLine("请输入文件根目录ID（默认root）：");
            string strParentFileId = Console.ReadLine();
            strParentFileId = string.IsNullOrEmpty(strParentFileId) ? "root" : strParentFileId;

            Console.WriteLine("是否加载下一页文件列表（1=是，其他=否）：");
            bool blnNextPage = Console.ReadLine() == "1";
            string strNextMarker = blnNextPage ? (dict_FileList != null && dict_FileList.ContainsKey("next_marker") ? dict_FileList["next_marker"].ToString() : "") : "";

            if (blnNextPage && string.IsNullOrWhiteSpace(strNextMarker)) break;

            string strFileList = await GetFileListAsync(strParentFileId, strNextMarker).ConfigureAwait(false);
            dict_FileList = JsonConvert.DeserializeObject<Dictionary<string, object>>(strFileList);
            if (dict_FileList == null || !dict_FileList.ContainsKey("items") || (dict_FileList["items"] as JArray).Count == 0)
                Console.WriteLine($"未获取到任何文件：{strFileList}");
            else
            {
                ConsoleTable consoleTable = new ConsoleTable();
                consoleTable.Columns = new string[] { "文件ID", "文件名", "文件类型" };
                JArray lst_Files = dict_FileList["items"] as JArray;
                foreach (var item in lst_Files)
                {
                    var fileInfo = item.ToObject<Dictionary<string, object>>();
                    consoleTable.Rows.Add(new object[] { fileInfo["file_id"], fileInfo["name"], fileInfo["type"] });
                }

                consoleTable.Write(Format.Default);
            }

            break;

        case "2":
            Console.WriteLine("请输入文件ID：");
            string strFileId = Console.ReadLine();

            Console.WriteLine("请输入文件名称：");
            string strFileName = Console.ReadLine();

            string strFileInfo = await FileRenameAsync(strFileId, strFileName).ConfigureAwait(false);
            Dictionary<string, object> dict_FileInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strFileInfo);

            if (dict_FileInfo == null || !dict_FileInfo.ContainsKey("name"))
                Console.WriteLine($"文件重命名失败：{strFileInfo}");
            else
                Console.WriteLine("文件重命名成功");

            break;

        default:
            Console.WriteLine("未知操作类型，请重新输入：");
            break;
    }
}

Console.ReadLine();

#region-- AutoLoginAsync() 据上次登录的token尝试自动登录，失败后尝试用refresh_token获取新的token登录
/// <summary>
/// 据上次登录的token尝试自动登录，失败后尝试用refresh_token获取新的token登录
/// </summary>
async Task<bool> AutoLoginAsync()
{
    try
    {
        string strToken = File.Exists(strTokenFileName) ? File.ReadAllText(strTokenFileName) : "";

        if (string.IsNullOrWhiteSpace(strToken)) return false;

        dict_Token = JsonConvert.DeserializeObject<Dictionary<string, object>>(strToken);

        if (dict_Token == null || !dict_Token.ContainsKey("access_token")) return false;

        string strUserInfo = await GetUserAsync().ConfigureAwait(false);
        Dictionary<string, object> dict_UserInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strUserInfo);

        // 根据access_token获取用户信息失败，尝试刷新token
        if (dict_UserInfo == null || !dict_UserInfo.ContainsKey("user_id"))
            return await RefleshTokenAsync(dict_Token["refresh_token"].ToString()).ConfigureAwait(false);

        return true;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region-- QRCodeLoginAsync() ALDrive API：二维码登录
/// <summary>
/// ALDrive API：二维码登录
/// </summary>
async void QRCodeLoginAsync()
{
    try
    {
        string strCodeContent = "";
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            strCodeContent = await httpClient.GetStringAsync(QRCodeGenerateUrl).ConfigureAwait(false);
            /*
            {
            "content": {
                "data": {
                    "t": 1629444501720,
                    "codeContent": "https://passport.aliyundrive.com/qrcodeCheck.htm?lgToken=1829abe3b951f1bfd689740e2f77ce40d_0000000&_from=havana",
                    "ck": "16312a1540565cfd7782eb03cacc7e4a",
                    "resultCode": 100
                },
                "status": 0,
                "success": true
            },
            "hasError": false
            }
            */
        }

        Dictionary<string, object> dict_CodeContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(strCodeContent);
        if (dict_CodeContent == null || !dict_CodeContent.ContainsKey("hasError") || dict_CodeContent["hasError"].Equals(true)) return;

        object data = (dict_CodeContent["content"] as JObject).ToObject<Dictionary<string, object>>()["data"];
        string codeContent = (data as JObject).ToObject<Dictionary<string, object>>()["codeContent"].ToString();

        PrintQRCode(codeContent);

        /*bizExt = response.json()['content']['data']['bizExt']
        bizExt = base64.b64decode(bizExt).decode('gb18030')
        accessToken = json.loads(bizExt)['pds_login_result']['accessToken']*/
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region -- RefleshTokenAsync() ALDrive API：获取token
/// <summary>
/// ALDrive API：获取token
/// </summary>
async Task<bool> RefleshTokenAsync(string strRefleshToken)
{
    try
    {
        byte[] bytes = Encoding.UTF8.GetBytes("{\"refresh_token\":\"" + strRefleshToken + "\"}");
        var byteContent = new ByteArrayContent(bytes);
        byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

        string strToken = "";
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(TokenRefleshUrl, byteContent).ConfigureAwait(false);

            Stream stream = httpResponseMessage.Content.ReadAsStream();
            StreamReader streamReader = new StreamReader(stream);

            strToken = streamReader.ReadToEnd();
        }

        dict_Token = JsonConvert.DeserializeObject<Dictionary<string, object>>(strToken);

        if (dict_Token == null || !dict_Token.ContainsKey("access_token")) return false;

        File.WriteAllText(strTokenFileName, strToken);

        return true;
        /*
        {
        "default_sbox_drive_id": "2260661",
        "role": "user",
        "device_id": "4f0f773e0c784e0b9537669540504cde",
        "user_name": "185***699",
        "need_link": false,
        "expire_time": "2021-08-20T03:09:49Z",
        "pin_setup": true,
        "need_rp_verify": false,
        "avatar": "",
        "user_data": {
            "DingDingRobotUrl": "https://oapi.dingtalk.com/robot/send?access_token=0b4a936d0e98c08608cd99f693393c18fa905aa0868215485a28497501916fec",
            "EncourageDesc": "内测期间有效反馈前10名用户将获得终身免费会员",
            "FeedBackSwitch": true,
            "FollowingDesc": "34848372",
            "ding_ding_robot_url": "https://oapi.dingtalk.com/robot/send?access_token=0b4a936d0e98c08608cd99f693393c18fa905aa0868215485a28497501916fec",
            "encourage_desc": "内测期间有效反馈前10名用户将获得终身免费会员",
            "feed_back_switch": true,
            "following_desc": "34848372"
        },
        "token_type": "Bearer",
        "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1Y2I5OTA3Mzk2NDg0NzNkOGY1NWYzZGMyMWRlNmVjMyIsImN1c3RvbUpzb24iOiJ7XCJjbGllbnRJZFwiOlwiMjVkelgzdmJZcWt0Vnh5WFwiLFwiZG9tYWluSWRcIjpcImJqMjlcIixcInNjb3BlXCI6W1wiRFJJVkUuQUxMXCIsXCJTSEFSRS5BTExcIixcIkZJTEUuQUxMXCIsXCJVU0VSLkFMTFwiLFwiU1RPUkFHRS5BTExcIixcIlNUT1JBR0VGSUxFLkxJU1RcIixcIkJBVENIXCIsXCJPQVVUSC5BTExcIixcIklNQUdFLkFMTFwiLFwiSU5WSVRFLkFMTFwiLFwiQUNDT1VOVC5BTExcIl0sXCJyb2xlXCI6XCJ1c2VyXCIsXCJyZWZcIjpcImh0dHBzOi8vd3d3LmFsaXl1bmRyaXZlLmNvbS9cIn0iLCJleHAiOjE2Mjk0Mjg5ODksImlhdCI6MTYyOTQyMTcyOX0.stREutxWPT_UoUbu7NMrTLutfZxyO2iv4js9Z6yBT1cbUo5k97EL-01-K5I6khUJcb3gjpaFddq25lBVygxUhP_tOeGIKEO55Yx3OIEPC8g3UU9T4rn670tjwdnLEmgCQ9rgHl9bx6pdY5OUlOa7zgbFTBS99laXBXiWPOlhrOM",
        "default_drive_id": "2260660",
        "refresh_token": "1ea92d6a04c84adb86137c08ae738891",
        "is_first_login": false,
        "user_id": "5cb990739648473d8f55f3dc21de6ec3",
        "nick_name": "",
        "exist_link": [],
        "state": "",
        "expires_in": 7200,
        "status": "enabled"
        }
         */
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region-- GetUserAsync() ALDrive API：获取用户信息
/// <summary>
/// ALDrive API：获取用户信息
/// </summary>
async Task<string> GetUserAsync()
{
    try
    {
        byte[] bytes = Encoding.UTF8.GetBytes("{}");
        var byteContent = new ByteArrayContent(bytes);
        byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(UserGetUrl, byteContent).ConfigureAwait(false);

            Stream stream = httpResponseMessage.Content.ReadAsStream();
            StreamReader streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        /*
         {
          "domain_id": "bj29",
          "user_id": "5cb990739648473d8f55f3dc21de6ec3",
          "avatar": "",
          "created_at": 1615793390710,
          "updated_at": 1615793390782,
          "email": "",
          "nick_name": "",
          "phone": "18516141699",
          "role": "user",
          "status": "enabled",
          "user_name": "185***699",
          "description": "",
          "default_drive_id": "2260660",
          "user_data": {},
          "deny_change_password_by_self": false,
          "need_change_password_next_login": false
         }
         */
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region-- GetFileListAsync() ALDrive API：返回文件列表
/// <summary>
/// ALDrive API：返回文件列表
/// </summary>
async Task<string> GetFileListAsync(string strParentFileId = "root", string strNextMarker = "")
{
    try
    {
        string requestContent = "{\"drive_id\":\"\",\"parent_file_id\":\"\",\"limit\":100,\"all\":false,\"url_expire_sec\":1600,\"image_thumbnail_process\":\"image/resize,w_400/format,jpeg\",\"image_url_process\":\"image/resize,w_1920/format,jpeg\",\"video_thumbnail_process\":\"video/snapshot,t_0,f_jpg,ar_auto,w_300\",\"fields\":\"*\",\"order_by\":\"updated_at\",\"order_direction\":\"DESC\"}";

        Dictionary<string, object> dict_RequestContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestContent);
        dict_RequestContent["drive_id"] = strDriveId;
        dict_RequestContent["parent_file_id"] = strParentFileId;
        if (!string.IsNullOrEmpty(strNextMarker)) dict_RequestContent["marker"] = strNextMarker;

        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dict_RequestContent));
        var byteContent = new ByteArrayContent(bytes);
        byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(FileListGetUrl, byteContent).ConfigureAwait(false);

            Stream stream = httpResponseMessage.Content.ReadAsStream();
            StreamReader streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        /*
        {
          "items": [
            {
              "drive_id": "2260660",
              "domain_id": "bj29",
              "file_id": "6119346e339648896e204c91b8bc2b8a5d579113",
              "name": "来自分享",
              "type": "folder",
              "created_at": "2021-08-15T15:36:14.313Z",
              "updated_at": "2021-08-15T15:36:14.313Z",
              "hidden": false,
              "starred": false,
              "status": "available",
              "parent_file_id": "root",
              "encrypt_mode": "none"
            },
            {
              "drive_id": "2260660",
              "domain_id": "bj29",
              "file_id": "610e718091eefb1ef1314228bdec9ed3c8a7dd3f",
              "name": "音乐艺术",
              "type": "folder",
              "created_at": "2021-08-07T11:41:52.274Z",
              "updated_at": "2021-08-07T11:41:52.274Z",
              "hidden": false,
              "starred": false,
              "status": "available",
              "parent_file_id": "root",
              "encrypt_mode": "none"
            },
            ...
          ],
          "next_marker": ""
        }                 
         */
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region-- FileRenameAsync() ALDrive API：文件重命名
/// <summary>
/// ALDrive API：文件重命名
/// </summary>
async Task<string> FileRenameAsync(string strFileId, string strFileName)
{
    try
    {
        if (string.IsNullOrWhiteSpace(strFileId)) return "请输入文件ID";

        if (string.IsNullOrWhiteSpace(strFileName)) return "请输入文件名";

        string requestContent = "{\"drive_id\":\"\",\"file_id\":\"\",\"name\":\"\",\"check_name_mode\":\"refuse\"}";

        Dictionary<string, object> dict_RequestContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestContent);
        dict_RequestContent["drive_id"] = strDriveId;
        dict_RequestContent["file_id"] = strFileId;
        dict_RequestContent["name"] = strFileName;

        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dict_RequestContent));
        var byteContent = new ByteArrayContent(bytes);
        byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(FileRenameUrl, byteContent).ConfigureAwait(false);

            Stream stream = httpResponseMessage.Content.ReadAsStream();
            StreamReader streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        /*
         {
          "drive_id": "2260660",
          "domain_id": "bj29",
          "file_id": "611e2c6acb4125a668f34ca69c75cedca07adb00",
          "name": "GitHub Token.txt",
          "type": "file",
          "content_type": "application/oct-stream",
          "created_at": "2021-08-19T10:03:22.019Z",
          "updated_at": "2021-08-19T11:59:02.715Z",
          "file_extension": "txt",
          "hidden": false,
          "size": 40,
          "starred": false,
          "status": "available",
          "upload_id": "2DEB2F9F5F974E89A55456D8D100D6DB",
          "parent_file_id": "root",
          "crc64_hash": "5614914170780682618",
          "content_hash": "EEC92C5C180F7DAB6486419324C53E8CB1020878",
          "content_hash_name": "sha1",
          "download_url": "https://bj29.cn-beijing.data.alicloudccp.com/C5umo8Wv%2F2260660%2F611e2c6acb4125a668f34ca69c75cedca07adb00%2F611e2c6a515d414f72dd41489b61496b4c136d90?di=bj29&dr=2260660&f=611e2c6acb4125a668f34ca69c75cedca07adb00&response-content-disposition=attachment%3B%20filename%2A%3DUTF-8%27%27GitHub%2520Token.txt&u=5cb990739648473d8f55f3dc21de6ec3&x-oss-access-key-id=LTAIsE5mAn2F493Q&x-oss-additional-headers=referer&x-oss-expires=1629375242&x-oss-signature=y%2BIDOtbsjJ0AVoMok2XgonQK%2FYMSQ5P9f2UjPP1AN6c%3D&x-oss-signature-version=OSS2",
          "url": "https://bj29.cn-beijing.data.alicloudccp.com/C5umo8Wv%2F2260660%2F611e2c6acb4125a668f34ca69c75cedca07adb00%2F611e2c6a515d414f72dd41489b61496b4c136d90?di=bj29&dr=2260660&f=611e2c6acb4125a668f34ca69c75cedca07adb00&u=5cb990739648473d8f55f3dc21de6ec3&x-oss-access-key-id=LTAIsE5mAn2F493Q&x-oss-additional-headers=referer&x-oss-expires=1629375242&x-oss-signature=P0MIV74j7isdX2RnPct1DgJTYB8cXFBw8RgdgxA80ss%3D&x-oss-signature-version=OSS2",
          "category": "doc",
          "encrypt_mode": "none",
          "punish_flag": 0,
          "trashed": false
        }
         */
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
#endregion

#region-- PrintQRCode()
void PrintQRCode(string strCodeContent)
{
    Bitmap image = null;
    QRCode qrCode = null;
    QRCodeData qrCodeData = null;
    QRCodeGenerator qrGenerator = null;
    try
    {
        qrGenerator = new QRCodeGenerator();
        qrCodeData = qrGenerator.CreateQrCode(strCodeContent, QRCodeGenerator.ECCLevel.M);
        qrCode = new QRCode(qrCodeData);
        image = qrCode.GetGraphic(1);

        for (int y = 0; y < image.Height; ++y)
        {
            for (int x = 0; x < image.Width; ++x)
            {
                Console.BackgroundColor = image.GetPixel(x, y).B <= 180 ? ConsoleColor.White : ConsoleColor.Black;
                Console.ForegroundColor = image.GetPixel(x, y).B <= 180 ? ConsoleColor.White : ConsoleColor.Black;
                Console.Write("　");
                Console.ResetColor();
            }

            Console.WriteLine();
        }
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