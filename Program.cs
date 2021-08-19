// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.Text;
using Newtonsoft.Json;

const string UserGetUrl = "https://api.aliyundrive.com/v2/user/get";
const string FileListGetUrl = "https://api.aliyundrive.com/adrive/v3/file/list";

string strAccessToken = "";
while (string.IsNullOrWhiteSpace(strAccessToken))
{
    Console.WriteLine("请输入access_token:");
    strAccessToken = Console.ReadLine();
}

string strUserInfo = await GetUser().ConfigureAwait(false);
Dictionary<string, object> dict_UserInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strUserInfo);

string strDriveId = dict_UserInfo["default_drive_id"].ToString();
string strFileList = await GetFileList().ConfigureAwait(false);
Dictionary<string, object> dict_FileList = JsonConvert.DeserializeObject<Dictionary<string, object>>(strFileList);

Console.ReadLine();

#region-- GetUser() ALDrive API：获取用户信息
/// <summary>
/// ALDrive API：获取用户信息
/// </summary>
async Task<string> GetUser()
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

#region-- GetFileList() ALDrive API：返回文件列表
/// <summary>
/// ALDrive API：返回文件列表
/// </summary>
async Task<string> GetFileList(string strParentFileId = "root", string strNextMarker="")
{
    try
    {
        string requestContent = "{\"drive_id\":\"\",\"parent_file_id\":\"\",\"limit\":100,\"all\":false,\"url_expire_sec\":1600,\"image_thumbnail_process\":\"image/resize,w_400/format,jpeg\",\"image_url_process\":\"image/resize,w_1920/format,jpeg\",\"video_thumbnail_process\":\"video/snapshot,t_0,f_jpg,ar_auto,w_300\",\"fields\":\"*\",\"order_by\":\"updated_at\",\"order_direction\":\"DESC\"}";

        Dictionary<string, object> dict_RequestContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestContent);
        dict_RequestContent["drive_id"] = strDriveId;
        dict_RequestContent["parent_file_id"] = strParentFileId;
        if (!string.IsNullOrEmpty(strNextMarker)) dict_RequestContent["next_marker"] = strNextMarker;

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