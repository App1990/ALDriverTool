
namespace ALDriveAssistant
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab__login_type = new System.Windows.Forms.TabControl();
            this.tabPage__password_login = new System.Windows.Forms.TabPage();
            this.btn__login = new System.Windows.Forms.Button();
            this.txt__password = new System.Windows.Forms.TextBox();
            this.txt__account = new System.Windows.Forms.TextBox();
            this.tabPage__qr_code_login = new System.Windows.Forms.TabPage();
            this.txt__qr_code_msg = new System.Windows.Forms.TextBox();
            this.img__qr_code = new System.Windows.Forms.PictureBox();
            this.tab__login_type.SuspendLayout();
            this.tabPage__password_login.SuspendLayout();
            this.tabPage__qr_code_login.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img__qr_code)).BeginInit();
            this.SuspendLayout();
            // 
            // tab__login_type
            // 
            this.tab__login_type.Controls.Add(this.tabPage__password_login);
            this.tab__login_type.Controls.Add(this.tabPage__qr_code_login);
            this.tab__login_type.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tab__login_type.Location = new System.Drawing.Point(21, 21);
            this.tab__login_type.Name = "tab__login_type";
            this.tab__login_type.SelectedIndex = 0;
            this.tab__login_type.Size = new System.Drawing.Size(260, 260);
            this.tab__login_type.TabIndex = 0;
            this.tab__login_type.SelectedIndexChanged += new System.EventHandler(this.tab__login_type_SelectedIndexChanged);
            // 
            // tabPage__password_login
            // 
            this.tabPage__password_login.Controls.Add(this.btn__login);
            this.tabPage__password_login.Controls.Add(this.txt__password);
            this.tabPage__password_login.Controls.Add(this.txt__account);
            this.tabPage__password_login.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tabPage__password_login.Location = new System.Drawing.Point(4, 28);
            this.tabPage__password_login.Name = "tabPage__password_login";
            this.tabPage__password_login.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage__password_login.Size = new System.Drawing.Size(252, 228);
            this.tabPage__password_login.TabIndex = 0;
            this.tabPage__password_login.Text = "账号登录";
            this.tabPage__password_login.UseVisualStyleBackColor = true;
            // 
            // btn__login
            // 
            this.btn__login.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btn__login.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btn__login.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn__login.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn__login.Location = new System.Drawing.Point(21, 118);
            this.btn__login.Name = "btn__login";
            this.btn__login.Size = new System.Drawing.Size(205, 32);
            this.btn__login.TabIndex = 2;
            this.btn__login.Text = "登录";
            this.btn__login.UseVisualStyleBackColor = false;
            this.btn__login.Click += new System.EventHandler(this.btn__login_Click);
            // 
            // txt__password
            // 
            this.txt__password.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txt__password.Location = new System.Drawing.Point(22, 66);
            this.txt__password.Name = "txt__password";
            this.txt__password.PasswordChar = '*';
            this.txt__password.PlaceholderText = "登陆密码";
            this.txt__password.Size = new System.Drawing.Size(204, 23);
            this.txt__password.TabIndex = 1;
            // 
            // txt__account
            // 
            this.txt__account.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txt__account.Location = new System.Drawing.Point(22, 21);
            this.txt__account.Name = "txt__account";
            this.txt__account.PlaceholderText = "请输入手机号码";
            this.txt__account.Size = new System.Drawing.Size(204, 23);
            this.txt__account.TabIndex = 0;
            // 
            // tabPage__qr_code_login
            // 
            this.tabPage__qr_code_login.Controls.Add(this.txt__qr_code_msg);
            this.tabPage__qr_code_login.Controls.Add(this.img__qr_code);
            this.tabPage__qr_code_login.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tabPage__qr_code_login.Location = new System.Drawing.Point(4, 28);
            this.tabPage__qr_code_login.Name = "tabPage__qr_code_login";
            this.tabPage__qr_code_login.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage__qr_code_login.Size = new System.Drawing.Size(252, 228);
            this.tabPage__qr_code_login.TabIndex = 1;
            this.tabPage__qr_code_login.Text = "扫码登陆";
            this.tabPage__qr_code_login.UseVisualStyleBackColor = true;
            // 
            // txt__qr_code_msg
            // 
            this.txt__qr_code_msg.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txt__qr_code_msg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt__qr_code_msg.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txt__qr_code_msg.Location = new System.Drawing.Point(6, 183);
            this.txt__qr_code_msg.Name = "txt__qr_code_msg";
            this.txt__qr_code_msg.ReadOnly = true;
            this.txt__qr_code_msg.Size = new System.Drawing.Size(240, 16);
            this.txt__qr_code_msg.TabIndex = 1;
            this.txt__qr_code_msg.Text = "请用阿里云盘 App 扫码";
            // 
            // img__qr_code
            // 
            this.img__qr_code.Location = new System.Drawing.Point(46, 17);
            this.img__qr_code.Name = "img__qr_code";
            this.img__qr_code.Size = new System.Drawing.Size(160, 160);
            this.img__qr_code.TabIndex = 0;
            this.img__qr_code.TabStop = false;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 302);
            this.Controls.Add(this.tab__login_type);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "Login";
            this.Text = "Login";
            this.tab__login_type.ResumeLayout(false);
            this.tabPage__password_login.ResumeLayout(false);
            this.tabPage__password_login.PerformLayout();
            this.tabPage__qr_code_login.ResumeLayout(false);
            this.tabPage__qr_code_login.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img__qr_code)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab__login_type;
        private System.Windows.Forms.TabPage tabPage__password_login;
        private System.Windows.Forms.TabPage tabPage__qr_code_login;
        private System.Windows.Forms.TextBox txt__password;
        private System.Windows.Forms.TextBox txt__account;
        private System.Windows.Forms.Button btn__login;
        private System.Windows.Forms.PictureBox img__qr_code;
        private System.Windows.Forms.TextBox txt__qr_code_msg;
    }
}