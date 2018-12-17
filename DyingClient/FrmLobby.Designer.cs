namespace DyingClient
{
  partial class FrmLobby
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
      this.btnLogin = new System.Windows.Forms.Button();
      this.rtb = new System.Windows.Forms.RichTextBox();
      this.txtUserName = new System.Windows.Forms.TextBox();
      this.lblUerName = new System.Windows.Forms.Label();
      this.lbxOnlineUsers = new System.Windows.Forms.ListBox();
      this.lblOnlineUsers = new System.Windows.Forms.Label();
      this.btnLogout = new System.Windows.Forms.Button();
      this.lblRooms = new System.Windows.Forms.Label();
      this.lbxRooms = new System.Windows.Forms.ListBox();
      this.lblDebug = new System.Windows.Forms.Label();
      this.btnCreateRoom = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(298, 25);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(75, 21);
      this.btnLogin.TabIndex = 0;
      this.btnLogin.Text = "登录";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // rtb
      // 
      this.rtb.Location = new System.Drawing.Point(36, 248);
      this.rtb.Name = "rtb";
      this.rtb.ReadOnly = true;
      this.rtb.Size = new System.Drawing.Size(737, 155);
      this.rtb.TabIndex = 3;
      this.rtb.Text = "";
      // 
      // txtUserName
      // 
      this.txtUserName.Location = new System.Drawing.Point(111, 25);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(167, 21);
      this.txtUserName.TabIndex = 4;
      // 
      // lblUerName
      // 
      this.lblUerName.AutoSize = true;
      this.lblUerName.Location = new System.Drawing.Point(36, 28);
      this.lblUerName.Name = "lblUerName";
      this.lblUerName.Size = new System.Drawing.Size(53, 12);
      this.lblUerName.TabIndex = 8;
      this.lblUerName.Text = "用户名：";
      // 
      // lbxOnlineUsers
      // 
      this.lbxOnlineUsers.FormattingEnabled = true;
      this.lbxOnlineUsers.ItemHeight = 12;
      this.lbxOnlineUsers.Location = new System.Drawing.Point(36, 106);
      this.lbxOnlineUsers.Name = "lbxOnlineUsers";
      this.lbxOnlineUsers.Size = new System.Drawing.Size(120, 88);
      this.lbxOnlineUsers.TabIndex = 9;
      // 
      // lblOnlineUsers
      // 
      this.lblOnlineUsers.AutoSize = true;
      this.lblOnlineUsers.Location = new System.Drawing.Point(36, 88);
      this.lblOnlineUsers.Name = "lblOnlineUsers";
      this.lblOnlineUsers.Size = new System.Drawing.Size(89, 12);
      this.lblOnlineUsers.TabIndex = 10;
      this.lblOnlineUsers.Text = "在线人员列表：";
      // 
      // btnLogout
      // 
      this.btnLogout.Location = new System.Drawing.Point(395, 25);
      this.btnLogout.Name = "btnLogout";
      this.btnLogout.Size = new System.Drawing.Size(75, 23);
      this.btnLogout.TabIndex = 11;
      this.btnLogout.Text = "登出";
      this.btnLogout.UseVisualStyleBackColor = true;
      this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
      // 
      // lblRooms
      // 
      this.lblRooms.AutoSize = true;
      this.lblRooms.Location = new System.Drawing.Point(182, 88);
      this.lblRooms.Name = "lblRooms";
      this.lblRooms.Size = new System.Drawing.Size(65, 12);
      this.lblRooms.TabIndex = 12;
      this.lblRooms.Text = "房间列表：";
      // 
      // lbxRooms
      // 
      this.lbxRooms.FormattingEnabled = true;
      this.lbxRooms.ItemHeight = 12;
      this.lbxRooms.Location = new System.Drawing.Point(184, 106);
      this.lbxRooms.Name = "lbxRooms";
      this.lbxRooms.Size = new System.Drawing.Size(120, 88);
      this.lbxRooms.TabIndex = 13;
      // 
      // lblDebug
      // 
      this.lblDebug.AutoSize = true;
      this.lblDebug.Location = new System.Drawing.Point(38, 230);
      this.lblDebug.Name = "lblDebug";
      this.lblDebug.Size = new System.Drawing.Size(101, 12);
      this.lblDebug.TabIndex = 14;
      this.lblDebug.Text = "调试信息输出区：";
      // 
      // btnCreateRoom
      // 
      this.btnCreateRoom.Location = new System.Drawing.Point(487, 25);
      this.btnCreateRoom.Name = "btnCreateRoom";
      this.btnCreateRoom.Size = new System.Drawing.Size(75, 23);
      this.btnCreateRoom.TabIndex = 15;
      this.btnCreateRoom.Text = "建房";
      this.btnCreateRoom.UseVisualStyleBackColor = true;
      this.btnCreateRoom.Click += new System.EventHandler(this.btnCreateRoom_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 415);
      this.Controls.Add(this.btnCreateRoom);
      this.Controls.Add(this.lblDebug);
      this.Controls.Add(this.lbxRooms);
      this.Controls.Add(this.lblRooms);
      this.Controls.Add(this.btnLogout);
      this.Controls.Add(this.lblOnlineUsers);
      this.Controls.Add(this.lbxOnlineUsers);
      this.Controls.Add(this.lblUerName);
      this.Controls.Add(this.txtUserName);
      this.Controls.Add(this.rtb);
      this.Controls.Add(this.btnLogin);
      this.Name = "Form1";
      this.Text = "呆鹰帝国对战平台 版本 黑暗时代3";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.RichTextBox rtb;
    private System.Windows.Forms.TextBox txtUserName;
    private System.Windows.Forms.Label lblUerName;
    private System.Windows.Forms.ListBox lbxOnlineUsers;
    private System.Windows.Forms.Label lblOnlineUsers;
    private System.Windows.Forms.Button btnLogout;
    private System.Windows.Forms.Label lblRooms;
    private System.Windows.Forms.ListBox lbxRooms;
    private System.Windows.Forms.Label lblDebug;
    private System.Windows.Forms.Button btnCreateRoom;
  }
}

