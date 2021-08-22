
namespace ALDriveAssistant
{
    partial class ALDriveAssistant
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGrid__file_list = new System.Windows.Forms.DataGridView();
            this.icon = new System.Windows.Forms.DataGridViewImageColumn();
            this.file_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updated_at = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flow_panel__dir_nav = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl__total_count = new System.Windows.Forms.Label();
            this.txt_box__search_pattern = new System.Windows.Forms.TextBox();
            this.txt_box__replace_pattern = new System.Windows.Forms.TextBox();
            this.btn__preview = new System.Windows.Forms.Button();
            this.btn__rename = new System.Windows.Forms.Button();
            this.data_grid__rename_preview = new System.Windows.Forms.DataGridView();
            this.unid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.org_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.new_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txt_box__info = new System.Windows.Forms.TextBox();
            this.btn__cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid__file_list)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid__rename_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid__file_list
            // 
            this.dataGrid__file_list.AllowUserToAddRows = false;
            this.dataGrid__file_list.AllowUserToDeleteRows = false;
            this.dataGrid__file_list.AllowUserToResizeRows = false;
            this.dataGrid__file_list.BackgroundColor = System.Drawing.Color.White;
            this.dataGrid__file_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid__file_list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.icon,
            this.file_id,
            this.type,
            this.name,
            this.updated_at,
            this.size});
            this.dataGrid__file_list.Location = new System.Drawing.Point(170, 96);
            this.dataGrid__file_list.Name = "dataGrid__file_list";
            this.dataGrid__file_list.ReadOnly = true;
            this.dataGrid__file_list.RowTemplate.Height = 25;
            this.dataGrid__file_list.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGrid__file_list.Size = new System.Drawing.Size(863, 440);
            this.dataGrid__file_list.TabIndex = 1;
            this.dataGrid__file_list.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid__file_list_CellClick);
            this.dataGrid__file_list.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid__file_list_CellDoubleClick);
            this.dataGrid__file_list.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGrid__file_list_Scroll);
            // 
            // icon
            // 
            this.icon.HeaderText = "";
            this.icon.MinimumWidth = 50;
            this.icon.Name = "icon";
            this.icon.ReadOnly = true;
            // 
            // file_id
            // 
            this.file_id.HeaderText = "ID";
            this.file_id.Name = "file_id";
            this.file_id.ReadOnly = true;
            this.file_id.Visible = false;
            // 
            // type
            // 
            this.type.HeaderText = "Type";
            this.type.Name = "type";
            this.type.ReadOnly = true;
            this.type.Visible = false;
            // 
            // name
            // 
            this.name.HeaderText = "名称";
            this.name.MinimumWidth = 250;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 500;
            // 
            // updated_at
            // 
            this.updated_at.HeaderText = "修改时间";
            this.updated_at.MinimumWidth = 60;
            this.updated_at.Name = "updated_at";
            this.updated_at.ReadOnly = true;
            this.updated_at.Width = 120;
            // 
            // size
            // 
            this.size.HeaderText = "大小";
            this.size.MinimumWidth = 50;
            this.size.Name = "size";
            this.size.ReadOnly = true;
            // 
            // flow_panel__dir_nav
            // 
            this.flow_panel__dir_nav.AutoSize = true;
            this.flow_panel__dir_nav.Location = new System.Drawing.Point(170, 28);
            this.flow_panel__dir_nav.MaximumSize = new System.Drawing.Size(860, 0);
            this.flow_panel__dir_nav.MinimumSize = new System.Drawing.Size(20, 24);
            this.flow_panel__dir_nav.Name = "flow_panel__dir_nav";
            this.flow_panel__dir_nav.Size = new System.Drawing.Size(124, 24);
            this.flow_panel__dir_nav.TabIndex = 5;
            // 
            // lbl__total_count
            // 
            this.lbl__total_count.AutoSize = true;
            this.lbl__total_count.Location = new System.Drawing.Point(170, 65);
            this.lbl__total_count.Name = "lbl__total_count";
            this.lbl__total_count.Size = new System.Drawing.Size(39, 17);
            this.lbl__total_count.TabIndex = 6;
            this.lbl__total_count.Text = "共0项";
            // 
            // txt_box__search_pattern
            // 
            this.txt_box__search_pattern.Location = new System.Drawing.Point(244, 62);
            this.txt_box__search_pattern.Name = "txt_box__search_pattern";
            this.txt_box__search_pattern.PlaceholderText = "匹配模式";
            this.txt_box__search_pattern.Size = new System.Drawing.Size(160, 23);
            this.txt_box__search_pattern.TabIndex = 8;
            // 
            // txt_box__replace_pattern
            // 
            this.txt_box__replace_pattern.Location = new System.Drawing.Point(419, 62);
            this.txt_box__replace_pattern.Name = "txt_box__replace_pattern";
            this.txt_box__replace_pattern.PlaceholderText = "替换模式";
            this.txt_box__replace_pattern.Size = new System.Drawing.Size(160, 23);
            this.txt_box__replace_pattern.TabIndex = 9;
            // 
            // btn__preview
            // 
            this.btn__preview.Location = new System.Drawing.Point(602, 62);
            this.btn__preview.Name = "btn__preview";
            this.btn__preview.Size = new System.Drawing.Size(60, 23);
            this.btn__preview.TabIndex = 10;
            this.btn__preview.Text = "预览";
            this.btn__preview.UseVisualStyleBackColor = true;
            this.btn__preview.Click += new System.EventHandler(this.btn__preview_Click);
            // 
            // btn__rename
            // 
            this.btn__rename.Location = new System.Drawing.Point(677, 62);
            this.btn__rename.Name = "btn__rename";
            this.btn__rename.Size = new System.Drawing.Size(60, 23);
            this.btn__rename.TabIndex = 11;
            this.btn__rename.Text = "重命名";
            this.btn__rename.UseVisualStyleBackColor = true;
            this.btn__rename.Click += new System.EventHandler(this.btn__rename_Click);
            // 
            // data_grid__rename_preview
            // 
            this.data_grid__rename_preview.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.data_grid__rename_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.data_grid__rename_preview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.unid,
            this.file_no,
            this.org_name,
            this.new_name});
            this.data_grid__rename_preview.Location = new System.Drawing.Point(170, 96);
            this.data_grid__rename_preview.Name = "data_grid__rename_preview";
            this.data_grid__rename_preview.RowTemplate.Height = 25;
            this.data_grid__rename_preview.Size = new System.Drawing.Size(863, 440);
            this.data_grid__rename_preview.TabIndex = 12;
            this.data_grid__rename_preview.Visible = false;
            // 
            // unid
            // 
            this.unid.HeaderText = "ID";
            this.unid.Name = "unid";
            this.unid.ReadOnly = true;
            this.unid.Visible = false;
            // 
            // file_no
            // 
            this.file_no.HeaderText = "No.";
            this.file_no.Name = "file_no";
            this.file_no.ReadOnly = true;
            this.file_no.Width = 60;
            // 
            // org_name
            // 
            this.org_name.HeaderText = "原名称";
            this.org_name.Name = "org_name";
            this.org_name.ReadOnly = true;
            this.org_name.Width = 460;
            // 
            // new_name
            // 
            this.new_name.HeaderText = "重命名";
            this.new_name.Name = "new_name";
            this.new_name.ReadOnly = true;
            this.new_name.Width = 300;
            // 
            // txt_box__info
            // 
            this.txt_box__info.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txt_box__info.ForeColor = System.Drawing.SystemColors.Window;
            this.txt_box__info.Location = new System.Drawing.Point(170, 542);
            this.txt_box__info.Multiline = true;
            this.txt_box__info.Name = "txt_box__info";
            this.txt_box__info.ReadOnly = true;
            this.txt_box__info.Size = new System.Drawing.Size(863, 207);
            this.txt_box__info.TabIndex = 13;
            // 
            // btn__cancel
            // 
            this.btn__cancel.Location = new System.Drawing.Point(754, 62);
            this.btn__cancel.Name = "btn__cancel";
            this.btn__cancel.Size = new System.Drawing.Size(60, 23);
            this.btn__cancel.TabIndex = 14;
            this.btn__cancel.Text = "取消";
            this.btn__cancel.UseVisualStyleBackColor = true;
            this.btn__cancel.Click += new System.EventHandler(this.btn__cancel_Click);
            // 
            // ALDriveAssistant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.btn__cancel);
            this.Controls.Add(this.txt_box__info);
            this.Controls.Add(this.data_grid__rename_preview);
            this.Controls.Add(this.btn__rename);
            this.Controls.Add(this.btn__preview);
            this.Controls.Add(this.txt_box__replace_pattern);
            this.Controls.Add(this.txt_box__search_pattern);
            this.Controls.Add(this.lbl__total_count);
            this.Controls.Add(this.flow_panel__dir_nav);
            this.Controls.Add(this.dataGrid__file_list);
            this.Name = "ALDriveAssistant";
            this.Text = "ALDriveAssistant";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid__file_list)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid__rename_preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGrid__file_list;
        private System.Windows.Forms.DataGridViewImageColumn icon;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn updated_at;
        private System.Windows.Forms.DataGridViewTextBoxColumn size;
        private System.Windows.Forms.FlowLayoutPanel flow_panel__dir_nav;
        private System.Windows.Forms.Label lbl__total_count;
        private System.Windows.Forms.TextBox txt_box__search_pattern;
        private System.Windows.Forms.TextBox txt_box__replace_pattern;
        private System.Windows.Forms.Button btn__preview;
        private System.Windows.Forms.Button btn__rename;
        private System.Windows.Forms.DataGridView data_grid__rename_preview;
        private System.Windows.Forms.TextBox txt_box__info;
        private System.Windows.Forms.DataGridViewTextBoxColumn unid;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn org_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn new_name;
        private System.Windows.Forms.Button btn__cancel;
    }
}

