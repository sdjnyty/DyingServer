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
      this.btnJoinRoom = new System.Windows.Forms.Button();
      this.btnLeaveRoom = new System.Windows.Forms.Button();
      this.btnRun = new System.Windows.Forms.Button();
      this.btnCancelRoom = new System.Windows.Forms.Button();
      this.lbxRoomUsers = new System.Windows.Forms.ListBox();
      this.lblRoomUsers = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(298, 25);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(76, 23);
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
      this.btnCreateRoom.Location = new System.Drawing.Point(310, 83);
      this.btnCreateRoom.Name = "btnCreateRoom";
      this.btnCreateRoom.Size = new System.Drawing.Size(75, 23);
      this.btnCreateRoom.TabIndex = 15;
      this.btnCreateRoom.Text = "建房";
      this.btnCreateRoom.UseVisualStyleBackColor = true;
      this.btnCreateRoom.Click += new System.EventHandler(this.btnCreateRoom_Click);
      // 
      // btnJoinRoom
      // 
      this.btnJoinRoom.Location = new System.Drawing.Point(310, 112);
      this.btnJoinRoom.Name = "btnJoinRoom";
      this.btnJoinRoom.Size = new System.Drawing.Size(75, 23);
      this.btnJoinRoom.TabIndex = 16;
      this.btnJoinRoom.Text = "加入房间";
      this.btnJoinRoom.UseVisualStyleBackColor = true;
      this.btnJoinRoom.Click += new System.EventHandler(this.btnJoinRoom_Click);
      // 
      // btnLeaveRoom
      // 
      this.btnLeaveRoom.Location = new System.Drawing.Point(310, 141);
      this.btnLeaveRoom.Name = "btnLeaveRoom";
      this.btnLeaveRoom.Size = new System.Drawing.Size(75, 23);
      this.btnLeaveRoom.TabIndex = 17;
      this.btnLeaveRoom.Text = "离开房间";
      this.btnLeaveRoom.UseVisualStyleBackColor = true;
      this.btnLeaveRoom.Click += new System.EventHandler(this.btnLeaveRoom_Click);
      // 
      // btnRun
      // 
      this.btnRun.Location = new System.Drawing.Point(537, 112);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(116, 52);
      this.btnRun.TabIndex = 18;
      this.btnRun.Text = "开始游戏";
      this.btnRun.UseVisualStyleBackColor = true;
      // 
      // btnCancelRoom
      // 
      this.btnCancelRoom.Location = new System.Drawing.Point(311, 171);
      this.btnCancelRoom.Name = "btnCancelRoom";
      this.btnCancelRoom.Size = new System.Drawing.Size(75, 23);
      this.btnCancelRoom.TabIndex = 19;
      this.btnCancelRoom.Text = "取消房间";
      this.btnCancelRoom.UseVisualStyleBackColor = true;
      this.btnCancelRoom.Click += new System.EventHandler(this.btnCancelRoom_Click);
      // 
      // lbxRoomUsers
      // 
      this.lbxRoomUsers.FormattingEnabled = true;
      this.lbxRoomUsers.ItemHeight = 12;
      this.lbxRoomUsers.Location = new System.Drawing.Point(395, 106);
      this.lbxRoomUsers.Name = "lbxRoomUsers";
      this.lbxRoomUsers.Size = new System.Drawing.Size(120, 88);
      this.lbxRoomUsers.TabIndex = 20;
      // 
      // lblRoomUsers
      // 
      this.lblRoomUsers.AutoSize = true;
      this.lblRoomUsers.Location = new System.Drawing.Point(395, 88);
      this.lblRoomUsers.Name = "lblRoomUsers";
      this.lblRoomUsers.Size = new System.Drawing.Size(101, 12);
      this.lblRoomUsers.TabIndex = 21;
      this.lblRoomUsers.Text = "房间内玩家列表：";
      // 
      // FrmLobby
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 415);
      this.Controls.Add(this.lblRoomUsers);
      this.Controls.Add(this.lbxRoomUsers);
      this.Controls.Add(this.btnCancelRoom);
      this.Controls.Add(this.btnRun);
      this.Controls.Add(this.btnLeaveRoom);
      this.Controls.Add(this.btnJoinRoom);
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
      this.Name = "FrmLobby";
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
    private System.Windows.Forms.Button btnJoinRoom;
    private System.Windows.Forms.Button btnLeaveRoom;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Button btnCancelRoom;
    private System.Windows.Forms.ListBox lbxRoomUsers;
    private System.Windows.Forms.Label lblRoomUsers;
  }
}

