namespace DyingClient
{
  partial class FrmRoom
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
      this.btnStart = new System.Windows.Forms.Button();
      this.btnLeaveRoom = new System.Windows.Forms.Button();
      this.lblPlayers = new System.Windows.Forms.Label();
      this.lbxPlayers = new System.Windows.Forms.ListBox();
      this.lblHostName = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(13, 13);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(75, 23);
      this.btnStart.TabIndex = 0;
      this.btnStart.Text = "启动游戏";
      this.btnStart.UseVisualStyleBackColor = true;
      // 
      // btnLeaveRoom
      // 
      this.btnLeaveRoom.Location = new System.Drawing.Point(110, 13);
      this.btnLeaveRoom.Name = "btnLeaveRoom";
      this.btnLeaveRoom.Size = new System.Drawing.Size(75, 23);
      this.btnLeaveRoom.TabIndex = 1;
      this.btnLeaveRoom.Text = "离开房间";
      this.btnLeaveRoom.UseVisualStyleBackColor = true;
      // 
      // lblPlayers
      // 
      this.lblPlayers.AutoSize = true;
      this.lblPlayers.Location = new System.Drawing.Point(13, 43);
      this.lblPlayers.Name = "lblPlayers";
      this.lblPlayers.Size = new System.Drawing.Size(89, 12);
      this.lblPlayers.TabIndex = 2;
      this.lblPlayers.Text = "房间玩家列表：";
      // 
      // lbxPlayers
      // 
      this.lbxPlayers.FormattingEnabled = true;
      this.lbxPlayers.ItemHeight = 12;
      this.lbxPlayers.Location = new System.Drawing.Point(13, 59);
      this.lbxPlayers.Name = "lbxPlayers";
      this.lbxPlayers.Size = new System.Drawing.Size(120, 112);
      this.lbxPlayers.TabIndex = 3;
      // 
      // lblHostName
      // 
      this.lblHostName.AutoSize = true;
      this.lblHostName.Location = new System.Drawing.Point(13, 178);
      this.lblHostName.Name = "lblHostName";
      this.lblHostName.Size = new System.Drawing.Size(65, 12);
      this.lblHostName.TabIndex = 4;
      this.lblHostName.Text = "房间主机：";
      // 
      // FrmRoom
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(674, 415);
      this.Controls.Add(this.lblHostName);
      this.Controls.Add(this.lbxPlayers);
      this.Controls.Add(this.lblPlayers);
      this.Controls.Add(this.btnLeaveRoom);
      this.Controls.Add(this.btnStart);
      this.Name = "FrmRoom";
      this.Text = "FrmRoom";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnLeaveRoom;
    private System.Windows.Forms.Label lblPlayers;
    private System.Windows.Forms.ListBox lbxPlayers;
    internal System.Windows.Forms.Label lblHostName;
  }
}