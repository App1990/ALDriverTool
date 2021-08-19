// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.Text;
using ConsoleTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

const string UserGetUrl = "https://api.aliyundrive.com/v2/user/get";
const string FileListGetUrl = "https://api.aliyundrive.com/adrive/v3/file/list";
const string FileRenameUrl = "https://api.aliyundrive.com/v3/file/update";

string strAppPath = Environment.CurrentDirectory;
string strAppConfigFileName = Path.Combine(strAppPath, "app_config.ini");
string strAccessToken = File.Exists(strAppConfigFileName) ? File.ReadAllText(strAppConfigFileName) : "";

bool blnRefleshToken = false;
Dictionary<string, object> dict_UserInfo= new Dictionary<string, object>();

do
{
    if (string.IsNullOrWhiteSpace(strAccessToken))
    {
        blnRefleshToken = true;
        Console.WriteLine("请输入Access Token:");
        strAccessToken = Console.ReadLine();
    }

    string strUserInfo = await GetUserAsync().ConfigureAwait(false);
    dict_UserInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strUserInfo);

    if (dict_UserInfo == null || !dict_UserInfo.ContainsKey("default_drive_id"))
    {
        strAccessToken = "";
        Console.WriteLine($"AccessTokenInvalid：{strUserInfo}");
    }
    else if(blnRefleshToken)
        File.WriteAllText(strAppConfigFileName, strAccessToken);
}
while (string.IsNullOrWhiteSpace(strAccessToken));



bool blnExitTool = false;
string strDriveId = dict_UserInfo["default_drive_id"].ToString();
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
                ConsoleTable consoleTable  = new ConsoleTable();
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
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {strAccessToken}");

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
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {strAccessToken}");

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
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {strAccessToken}");

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