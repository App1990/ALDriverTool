
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
            this.img__qr_code = new System.Windows.Forms.PictureBox();
            this.dataGrid__file_list = new System.Windows.Forms.DataGridView();
            this.file_type = new System.Windows.Forms.DataGridViewImageColumn();
            this.file_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.update_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.img__qr_code)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid__file_list)).BeginInit();
            this.SuspendLayout();
            // 
            // img__qr_code
            // 
            this.img__qr_code.Location = new System.Drawing.Point(351, 115);
            this.img__qr_code.Name = "img__qr_code";
            this.img__qr_code.Size = new System.Drawing.Size(200, 200);
            this.img__qr_code.TabIndex = 0;
            this.img__qr_code.TabStop = false;
            this.img__qr_code.Visible = false;
            // 
            // dataGrid__file_list
            // 
            this.dataGrid__file_list.AllowUserToAddRows = false;
            this.dataGrid__file_list.AllowUserToDeleteRows = false;
            this.dataGrid__file_list.AllowUserToResizeRows = false;
            this.dataGrid__file_list.BackgroundColor = System.Drawing.Color.White;
            this.dataGrid__file_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid__file_list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.file_type,
            this.file_id,
            this.file_name,
            this.update_date,
            this.file_size});
            this.dataGrid__file_list.Location = new System.Drawing.Point(12, 12);
            this.dataGrid__file_list.Name = "dataGrid__file_list";
            this.dataGrid__file_list.ReadOnly = true;
            this.dataGrid__file_list.RowTemplate.Height = 25;
            this.dataGrid__file_list.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGrid__file_list.Size = new System.Drawing.Size(863, 440);
            this.dataGrid__file_list.TabIndex = 1;
            this.dataGrid__file_list.Visible = false;
            // 
            // file_type
            // 
            this.file_type.HeaderText = "";
            this.file_type.MinimumWidth = 50;
            this.file_type.Name = "file_type";
            this.file_type.ReadOnly = true;
            // 
            // file_id
            // 
            this.file_id.HeaderText = "ID";
            this.file_id.Name = "file_id";
            this.file_id.ReadOnly = true;
            this.file_id.Visible = false;
            // 
            // file_name
            // 
            this.file_name.HeaderText = "名称";
            this.file_name.MinimumWidth = 250;
            this.file_name.Name = "file_name";
            this.file_name.ReadOnly = true;
            this.file_name.Width = 500;
            // 
            // update_date
            // 
            this.update_date.HeaderText = "修改时间";
            this.update_date.MinimumWidth = 60;
            this.update_date.Name = "update_date";
            this.update_date.ReadOnly = true;
            this.update_date.Width = 120;
            // 
            // file_size
            // 
            this.file_size.HeaderText = "大小";
            this.file_size.MinimumWidth = 50;
            this.file_size.Name = "file_size";
            this.file_size.ReadOnly = true;
            // 
            // ALDriveAssistant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 464);
            this.Controls.Add(this.img__qr_code);
            this.Controls.Add(this.dataGrid__file_list);
            this.Name = "ALDriveAssistant";
            this.Text = "ALDriveAssistant";
            ((System.ComponentModel.ISupportInitialize)(this.img__qr_code)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid__file_list)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox img__qr_code;
        private System.Windows.Forms.DataGridView dataGrid__file_list;
        private System.Windows.Forms.DataGridViewImageColumn file_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn update_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_size;
    }
}

