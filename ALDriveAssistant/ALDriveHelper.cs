using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace ALDriveAssistant
{
    public class ALDriveHelper
    {
        const string ClientID = "25dzX3vbYqktVxyX";

        const string QRCodeQueryUrl = "https://passport.aliyundrive.com/newlogin/qrcode/query.do?appName=aliyun_drive";
        const string QRCodeGenerateUrl = "https://passport.aliyundrive.com/newlogin/qrcode/generate.do?appName=aliyun_drive";

        const string AuthorizeUrl = "https://auth.aliyundrive.com/v2/oauth/authorize";
        const string TokenLoginUrl = "https://auth.aliyundrive.com/v2/oauth/token_login";

        const string UserGetUrl = "https://api.aliyundrive.com/v2/user/get";
        const string TokenGetUrl = "https://api.aliyundrive.com/token/get";
        const string TokenRefleshUrl = "https://api.aliyundrive.com/token/refresh";

        const string FileListGetUrl = "https://api.aliyundrive.com/adrive/v3/file/list";
        const string FileRenameUrl = "https://api.aliyundrive.com/v3/file/update";


        JObject data;
        string strTokenFileName;
        HttpClient httpClient = null;
        HttpClientHandler httpClientHandler = null;
        Dictionary<string, object> dict_Token = new Dictionary<string, object>();

        public Dictionary<string, object> SetToken { set { dict_Token = value; }  }


        public ALDriveHelper()
        {
            httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = new CookieContainer();

            httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");

            strTokenFileName = Path.Combine(Environment.CurrentDirectory, "Token.json");
        }

        ~ALDriveHelper()
        {
            if (httpClient != null) httpClient.Dispose();
            if (httpClientHandler != null) httpClientHandler.Dispose();
        }

        #region-- Authorize() ALDrive API：Authorize
        /// <summary>
        /// ALDrive API：Authorize
        /// </summary>
        public async Task<string> AuthorizeAsync()
        {
            try
            {
                string strAuthorizeUrl = QueryHelpers.AddQueryString(AuthorizeUrl, "login_type", "custom");
                strAuthorizeUrl = QueryHelpers.AddQueryString(strAuthorizeUrl, "response_type", "code");
                strAuthorizeUrl = QueryHelpers.AddQueryString(strAuthorizeUrl, "redirect_uri", "https://www.aliyundrive.com/sign/callback");
                strAuthorizeUrl = QueryHelpers.AddQueryString(strAuthorizeUrl, "client_id", ClientID);
                strAuthorizeUrl = QueryHelpers.AddQueryString(strAuthorizeUrl, "state", "{\"origin\":\"https://www.aliyundrive.com\"}");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, strAuthorizeUrl);

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region-- AutoLoginAsync() 据上次登录的token尝试自动登录，失败后尝试用refresh_token获取新的token登录
        /// <summary>
        /// 据上次登录的token尝试自动登录，失败后尝试用refresh_token获取新的token登录
        /// </summary>
        public async Task<bool> AutoLoginAsync()
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

        #region-- QRCodeContentGetAsync() ALDrive API：二维码内容获取
        /// <summary>
        /// ALDrive API：二维码内容获取
        /// </summary>
        public async Task<string> QRCodeContentGetAsync()
        {
            try
            {
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

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, QRCodeGenerateUrl);
                string strCodeContent = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

                Dictionary<string, object> dict_CodeContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(strCodeContent);
                if (dict_CodeContent == null || !dict_CodeContent.ContainsKey("hasError") || dict_CodeContent["hasError"].Equals(true)) return "";

                data = (dict_CodeContent["content"] as JObject)["data"] as JObject;

                return data["codeContent"].ToString();

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

        #region-- QRCodeQueryAsync() ALDrive API：二维码状态查询
        /// <summary>
        /// ALDrive API：二维码状态查询
        /// </summary>
        public async Task<string> QRCodeQueryAsync()
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes($"t={data["t"]}&ck={data["ck"]}");
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, QRCodeQueryUrl);
                requestMessage.Content = byteContent;

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

                /*
                 {"content":{"data":{"qrCodeStatus":"NEW","resultCode":100},"status":0,"success":true},"hasError":false}
                 {"content":{"data":{"qrCodeStatus":"SCANED","resultCode":100},"status":1,"success":true},"hasError":false}
                 {"content":{"data":{"qrCodeStatus":"CONFIRMED","resultCode":100},"status":2,"success":true},"hasError":false}
                 {"content":{"data":{"qrCodeStatus":"EXPIRED","resultCode":100},"status":-1,"success":false},"hasError":false}
                 */
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region-- TokenLoginAsync() ALDrive API：token登录
        /// <summary>
        /// ALDrive API：token登录
        /// </summary>
        public async Task<string> TokenLoginAsync(string strAccessToken) 
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"token\":\"" + strAccessToken + "\"}");
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, TokenLoginUrl);
                requestMessage.Content = byteContent;

                string strGoTo = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> dict_GoTo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strGoTo);
                if (dict_GoTo == null || !dict_GoTo.ContainsKey("goto")) throw new Exception("登录失败！");

                Uri uri = new Uri(dict_GoTo["goto"].ToString());
                string strCode = QueryHelpers.ParseQuery(uri.Query)["code"].ToString();

                string strToken = await TokenGetAsync(strCode).ConfigureAwait(false);
                return strToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region-- TokenGetAsync() ALDrive API：token获取
        /// <summary>
        /// ALDrive API：token获取
        /// </summary>
        async Task<string> TokenGetAsync(string strCode)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"code\":\"" + strCode + "\"}");
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, TokenGetUrl);
                requestMessage.Content = byteContent;

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region -- RefleshTokenAsync() ALDrive API：刷新token
        /// <summary>
        /// ALDrive API：刷新token
        /// </summary>
        async Task<bool> RefleshTokenAsync(string strRefleshToken)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"refresh_token\":\"" + strRefleshToken + "\"}");
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, TokenRefleshUrl);
                requestMessage.Content = byteContent;

                string strToken = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

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

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, UserGetUrl);
                requestMessage.Content = byteContent;
                requestMessage.Headers.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

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
        public async Task<string> GetFileListAsync(string strParentFileId = "root", string strNextMarker = "")
        {
            try
            {
                string requestContent = "{\"drive_id\":\"\",\"parent_file_id\":\"\",\"limit\":100,\"all\":false,\"url_expire_sec\":1600,\"image_thumbnail_process\":\"image/resize,w_400/format,jpeg\",\"image_url_process\":\"image/resize,w_1920/format,jpeg\",\"video_thumbnail_process\":\"video/snapshot,t_0,f_jpg,ar_auto,w_300\",\"fields\":\"*\",\"order_by\":\"name\",\"order_direction\":\"ASC\"}";

                Dictionary<string, object> dict_RequestContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestContent);
                dict_RequestContent["drive_id"] = dict_Token["default_drive_id"];
                dict_RequestContent["parent_file_id"] = strParentFileId;
                if (!string.IsNullOrEmpty(strNextMarker)) dict_RequestContent["marker"] = strNextMarker;

                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dict_RequestContent));
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, FileListGetUrl);
                requestMessage.Content = byteContent;
                requestMessage.Headers.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

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
        public async Task<string> FileRenameAsync(string strFileId, string strFileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strFileId)) return "请输入文件ID";

                if (string.IsNullOrWhiteSpace(strFileName)) return "请输入文件名";

                string requestContent = "{\"drive_id\":\"\",\"file_id\":\"\",\"name\":\"\",\"check_name_mode\":\"refuse\"}";

                Dictionary<string, object> dict_RequestContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestContent);
                dict_RequestContent["drive_id"] = dict_Token["default_drive_id"];
                dict_RequestContent["file_id"] = strFileId;
                dict_RequestContent["name"] = strFileName;

                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dict_RequestContent));
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, FileRenameUrl);
                requestMessage.Content = byteContent;
                requestMessage.Headers.Add("Authorization", $"Bearer {dict_Token["access_token"]}");

                return httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

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
    }
}
