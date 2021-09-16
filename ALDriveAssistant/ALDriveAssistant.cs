using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALDriveAssistant
{
    public partial class ALDriveAssistant : Form
    {
        string mstrNextMaker = "";
        ALDriveHelper alDriveHelper;
        List<Dictionary<string, string>> lstDirNav = new List<Dictionary<string, string>>() { new Dictionary<string, string>() { { "file_id", "root" }, { "name", "文件" } } };

        public ALDriveAssistant(ALDriveHelper alDriveHelper)
        {
            InitializeComponent();
            this.alDriveHelper = alDriveHelper;

            PaintDirNav();
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
                mstrNextMaker = dict_FileList["next_marker"].ToString();
                foreach (var item in lst_Files)
                {
                    var fileInfo = item.ToObject<Dictionary<string, object>>();
                    object objSize = null;
                    if (fileInfo.ContainsKey("size"))
                    {
                        long size = Convert.ToInt64(fileInfo["size"]);
                        objSize = size < 1024 ? $"{size}B" : (size < 1024 * 1024 ? $"{Math.Round(size / 1024.0, 2)}KB" : (size < 1024 * 1024 * 1024 ? $"{Math.Round(size / 1024.0 / 1024.0, 2)}MB" : $"{Math.Round(size / 1024.0 / 1024.0 / 1024.0, 2)}GB"));
                    }
                    object[] rowData = new object[] { null, fileInfo["file_id"], fileInfo["type"], fileInfo["name"], Convert.ToDateTime(fileInfo["updated_at"]).ToString("yyyy-MM-dd HH:mm"), objSize };
                    dataGrid__file_list.Rows[dataGrid__file_list.Rows.Add(rowData)].Tag = fileInfo;
                }

                lbl__total_count.Text = $"共{dataGrid__file_list.Rows.Count}项";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region-- PaintDirNav()
        void PaintDirNav()
        {
            try
            {
                bool blnLastItem = false;
                flow_panel__dir_nav.Controls.Clear();

                Font linkLabelFont = new Font("Microsoft YaHei UI", 9, FontStyle.Bold);
                for (int i = 0, count = lstDirNav.Count; i < count; i++)
                {
                    blnLastItem = i == count - 1;

                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.Tag = lstDirNav[i];
                    linkLabel.Font = linkLabelFont;
                    linkLabel.Text = lstDirNav[i]["name"];

                    linkLabel.Width = linkLabel.PreferredWidth;
                    linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    linkLabel.LinkColor = blnLastItem ? Color.Blue : Color.Gray;

                    linkLabel.Click += linkLabel_Click;
                    flow_panel__dir_nav.Controls.Add(linkLabel);
                    linkLabel.Location = new Point(linkLabel.Location.X - 20, linkLabel.Location.Y);

                    if (blnLastItem) return;

                    Label label = new Label();
                    label.Text = " < ";
                    label.Left = 10;
                    label.Width = label.PreferredWidth;

                    flow_panel__dir_nav.Controls.Add(label);
                    label.Location = new Point(label.Location.X - 20, label.Location.Y);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        void linkLabel_Click(object sender, EventArgs e)
        {
            try
            {
                var dirInfo = (sender as LinkLabel).Tag as Dictionary<string, string>;
                if (dirInfo["file_id"] == lstDirNav.Last()["file_id"]) return;

                bool blnRemain = true;
                dataGrid__file_list.Rows.Clear();
                lstDirNav = lstDirNav.Where(x =>
                {
                    if (x["file_id"] == dirInfo["file_id"] && blnRemain) blnRemain = false; return x["file_id"] == dirInfo["file_id"] || blnRemain;
                }).ToList();

                PaintDirNav();
                GetFileList(dirInfo["file_id"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGrid__file_list_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                dataGrid__file_list.Rows[e.RowIndex].Selected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGrid__file_list_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                var selectedRow = dataGrid__file_list.Rows[e.RowIndex];
                selectedRow.Selected = true;

                if (e.ColumnIndex < 0) return;

                if (selectedRow.Cells["type"].Value.Equals("folder"))
                {
                    Dictionary<string, string> dirInfo = new Dictionary<string, string>();
                    dirInfo["name"] = selectedRow.Cells["name"].Value.ToString();
                    dirInfo["file_id"] = selectedRow.Cells["file_id"].Value.ToString();

                    lstDirNav.Add(dirInfo);
                    dataGrid__file_list.Rows.Clear();

                    PaintDirNav();
                    GetFileList(dirInfo["file_id"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGrid__file_list_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mstrNextMaker)) return;
                if (dataGrid__file_list.VerticalScrollingOffset < dataGrid__file_list.PreferredSize.Height - dataGrid__file_list.Height - 50) return;

                GetFileList(lstDirNav.Last()["file_id"], mstrNextMaker);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn__preview_Click(object sender, EventArgs e)
        {
            try
            {
                string strSearchPattern = txt_box__search_pattern.Text.Trim();
                string strReplacedPattern=txt_box__replace_pattern.Text.Trim();

                if (string.IsNullOrWhiteSpace(strSearchPattern) || string.IsNullOrWhiteSpace(strReplacedPattern))
                {
                    MessageBox.Show("匹配模式和替换模式不能为空！", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                data_grid__rename_preview.Rows.Clear();
                List<string> lstRename = new List<string>();
                Regex regSearch = new Regex(strSearchPattern);

                foreach (DataGridViewRow item in dataGrid__file_list.Rows)
                {
                    lstRename.Clear();
                    lstRename.AddRange( regSearch.Replace(item.Cells["name"].Value.ToString(), strReplacedPattern).Split(' '));
                    lstRename[0] = lstRename[0].PadLeft(3, '0');

                    object[] rowData = new object[] { item.Cells["file_id"].Value, lstRename[0], item.Cells["name"].Value, string.Join(" ", lstRename).Trim() };
                    data_grid__rename_preview.Rows.Add(rowData);
                }

                data_grid__rename_preview.Visible = true;
                data_grid__rename_preview.Location = dataGrid__file_list.Location;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn__rename_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in data_grid__rename_preview.Rows)
                {
                    Task<string> taskFileRename = alDriveHelper.FileRenameAsync(item.Cells["unid"].Value.ToString(), item.Cells["new_name"].Value.ToString());
                    taskFileRename.Wait();

                    Dictionary<string, object> dict_FileInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(taskFileRename.Result);
                    bool blnRenameSuccess = dict_FileInfo != null && dict_FileInfo.ContainsKey("name");
                    txt_box__info.AppendText($"{item.Cells["org_name"].Value}重命名{(blnRenameSuccess ? "成功" : "失败")}：{(blnRenameSuccess ? item.Cells["new_name"].Value : taskFileRename.Result)}\r\n");

                    // 间隔0.5s发起下一个文件重命名请求
                    Thread.Sleep(500);
                }

                MessageBox.Show("重命名成功！", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                data_grid__rename_preview.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn__cancel_Click(object sender, EventArgs e)
        {
            try
            {
                data_grid__rename_preview.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
