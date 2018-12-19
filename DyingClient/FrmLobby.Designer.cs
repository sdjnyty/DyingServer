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
      this.btnSpectate = new System.Windows.Forms.Button();
      this.lblSpectators = new System.Windows.Forms.Label();
      this.lbxSpectators = new System.Windows.Forms.ListBox();
      this.lblChat = new System.Windows.Forms.Label();
      this.txxChat = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(298, 27);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(76, 25);
      this.btnLogin.TabIndex = 0;
      this.btnLogin.Text = "登录";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // rtb
      // 
      this.rtb.Location = new System.Drawing.Point(36, 269);
      this.rtb.Name = "rtb";
      this.rtb.ReadOnly = true;
      this.rtb.Size = new System.Drawing.Size(737, 168);
      this.rtb.TabIndex = 3;
      this.rtb.Text = "";
      // 
      // txtUserName
      // 
      this.txtUserName.Location = new System.Drawing.Point(111, 27);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(167, 20);
      this.txtUserName.TabIndex = 4;
      // 
      // lblUerName
      // 
      this.lblUerName.AutoSize = true;
      this.lblUerName.Location = new System.Drawing.Point(36, 30);
      this.lblUerName.Name = "lblUerName";
      this.lblUerName.Size = new System.Drawing.Size(55, 13);
      this.lblUerName.TabIndex = 8;
      this.lblUerName.Text = "用户名：";
      // 
      // lbxOnlineUsers
      // 
      this.lbxOnlineUsers.FormattingEnabled = true;
      this.lbxOnlineUsers.Location = new System.Drawing.Point(36, 115);
      this.lbxOnlineUsers.Name = "lbxOnlineUsers";
      this.lbxOnlineUsers.Size = new System.Drawing.Size(120, 95);
      this.lbxOnlineUsers.TabIndex = 9;
      // 
      // lblOnlineUsers
      // 
      this.lblOnlineUsers.AutoSize = true;
      this.lblOnlineUsers.Location = new System.Drawing.Point(36, 95);
      this.lblOnlineUsers.Name = "lblOnlineUsers";
      this.lblOnlineUsers.Size = new System.Drawing.Size(91, 13);
      this.lblOnlineUsers.TabIndex = 10;
      this.lblOnlineUsers.Text = "在线人员列表：";
      // 
      // btnLogout
      // 
      this.btnLogout.Location = new System.Drawing.Point(395, 27);
      this.btnLogout.Name = "btnLogout";
      this.btnLogout.Size = new System.Drawing.Size(75, 25);
      this.btnLogout.TabIndex = 11;
      this.btnLogout.Text = "登出";
      this.btnLogout.UseVisualStyleBackColor = true;
      this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
      // 
      // lblRooms
      // 
      this.lblRooms.AutoSize = true;
      this.lblRooms.Location = new System.Drawing.Point(182, 95);
      this.lblRooms.Name = "lblRooms";
      this.lblRooms.Size = new System.Drawing.Size(67, 13);
      this.lblRooms.TabIndex = 12;
      this.lblRooms.Text = "房间列表：";
      // 
      // lbxRooms
      // 
      this.lbxRooms.FormattingEnabled = true;
      this.lbxRooms.Location = new System.Drawing.Point(184, 115);
      this.lbxRooms.Name = "lbxRooms";
      this.lbxRooms.Size = new System.Drawing.Size(120, 95);
      this.lbxRooms.TabIndex = 13;
      // 
      // lblDebug
      // 
      this.lblDebug.AutoSize = true;
      this.lblDebug.Location = new System.Drawing.Point(38, 249);
      this.lblDebug.Name = "lblDebug";
      this.lblDebug.Size = new System.Drawing.Size(103, 13);
      this.lblDebug.TabIndex = 14;
      this.lblDebug.Text = "调试信息输出区：";
      // 
      // btnCreateRoom
      // 
      this.btnCreateRoom.Location = new System.Drawing.Point(310, 59);
      this.btnCreateRoom.Name = "btnCreateRoom";
      this.btnCreateRoom.Size = new System.Drawing.Size(75, 25);
      this.btnCreateRoom.TabIndex = 15;
      this.btnCreateRoom.Text = "建房";
      this.btnCreateRoom.UseVisualStyleBackColor = true;
      this.btnCreateRoom.Click += new System.EventHandler(this.btnCreateRoom_Click);
      // 
      // btnJoinRoom
      // 
      this.btnJoinRoom.Location = new System.Drawing.Point(310, 90);
      this.btnJoinRoom.Name = "btnJoinRoom";
      this.btnJoinRoom.Size = new System.Drawing.Size(75, 25);
      this.btnJoinRoom.TabIndex = 16;
      this.btnJoinRoom.Text = "加入房间";
      this.btnJoinRoom.UseVisualStyleBackColor = true;
      this.btnJoinRoom.Click += new System.EventHandler(this.btnJoinRoom_Click);
      // 
      // btnLeaveRoom
      // 
      this.btnLeaveRoom.Location = new System.Drawing.Point(310, 153);
      this.btnLeaveRoom.Name = "btnLeaveRoom";
      this.btnLeaveRoom.Size = new System.Drawing.Size(75, 25);
      this.btnLeaveRoom.TabIndex = 17;
      this.btnLeaveRoom.Text = "离开房间";
      this.btnLeaveRoom.UseVisualStyleBackColor = true;
      this.btnLeaveRoom.Click += new System.EventHandler(this.btnLeaveRoom_Click);
      // 
      // btnRun
      // 
      this.btnRun.Location = new System.Drawing.Point(657, 121);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(116, 56);
      this.btnRun.TabIndex = 18;
      this.btnRun.Text = "开始游戏";
      this.btnRun.UseVisualStyleBackColor = true;
      this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
      // 
      // btnCancelRoom
      // 
      this.btnCancelRoom.Location = new System.Drawing.Point(310, 185);
      this.btnCancelRoom.Name = "btnCancelRoom";
      this.btnCancelRoom.Size = new System.Drawing.Size(75, 25);
      this.btnCancelRoom.TabIndex = 19;
      this.btnCancelRoom.Text = "取消房间";
      this.btnCancelRoom.UseVisualStyleBackColor = true;
      this.btnCancelRoom.Click += new System.EventHandler(this.btnCancelRoom_Click);
      // 
      // lbxRoomUsers
      // 
      this.lbxRoomUsers.FormattingEnabled = true;
      this.lbxRoomUsers.Location = new System.Drawing.Point(395, 115);
      this.lbxRoomUsers.Name = "lbxRoomUsers";
      this.lbxRoomUsers.Size = new System.Drawing.Size(120, 95);
      this.lbxRoomUsers.TabIndex = 20;
      // 
      // lblRoomUsers
      // 
      this.lblRoomUsers.AutoSize = true;
      this.lblRoomUsers.Location = new System.Drawing.Point(395, 95);
      this.lblRoomUsers.Name = "lblRoomUsers";
      this.lblRoomUsers.Size = new System.Drawing.Size(103, 13);
      this.lblRoomUsers.TabIndex = 21;
      this.lblRoomUsers.Text = "房间内玩家列表：";
      // 
      // btnSpectate
      // 
      this.btnSpectate.Location = new System.Drawing.Point(310, 121);
      this.btnSpectate.Name = "btnSpectate";
      this.btnSpectate.Size = new System.Drawing.Size(75, 25);
      this.btnSpectate.TabIndex = 22;
      this.btnSpectate.Text = "观战房间";
      this.btnSpectate.UseVisualStyleBackColor = true;
      this.btnSpectate.Click += new System.EventHandler(this.btnSpectate_Click);
      // 
      // lblSpectators
      // 
      this.lblSpectators.AutoSize = true;
      this.lblSpectators.Location = new System.Drawing.Point(524, 94);
      this.lblSpectators.Name = "lblSpectators";
      this.lblSpectators.Size = new System.Drawing.Size(79, 13);
      this.lblSpectators.TabIndex = 23;
      this.lblSpectators.Text = "观战者列表：";
      // 
      // lbxSpectators
      // 
      this.lbxSpectators.FormattingEnabled = true;
      this.lbxSpectators.Location = new System.Drawing.Point(526, 115);
      this.lbxSpectators.Name = "lbxSpectators";
      this.lbxSpectators.Size = new System.Drawing.Size(120, 95);
      this.lbxSpectators.TabIndex = 24;
      // 
      // lblChat
      // 
      this.lblChat.AutoSize = true;
      this.lblChat.Location = new System.Drawing.Point(36, 218);
      this.lblChat.Name = "lblChat";
      this.lblChat.Size = new System.Drawing.Size(163, 13);
      this.lblChat.TabIndex = 25;
      this.lblChat.Text = "吹水聊天框（按回车发送）：";
      // 
      // txxChat
      // 
      this.txxChat.Location = new System.Drawing.Point(204, 218);
      this.txxChat.Name = "txxChat";
      this.txxChat.Size = new System.Drawing.Size(442, 20);
      this.txxChat.TabIndex = 26;
      this.txxChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txxChat_KeyDown);
      // 
      // FrmLobby
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.txxChat);
      this.Controls.Add(this.lblChat);
      this.Controls.Add(this.lbxSpectators);
      this.Controls.Add(this.lblSpectators);
      this.Controls.Add(this.btnSpectate);
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
      this.Text = "呆鹰帝国对战平台 版本 封建时代0.4";
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
    private System.Windows.Forms.Button btnSpectate;
    private System.Windows.Forms.Label lblSpectators;
    private System.Windows.Forms.ListBox lbxSpectators;
    private System.Windows.Forms.Label lblChat;
    private System.Windows.Forms.TextBox txxChat;
  }
}

