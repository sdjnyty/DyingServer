namespace DyingClient
{
  partial class Form1
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
      this.btnConnect = new System.Windows.Forms.Button();
      this.btnDisconnect = new System.Windows.Forms.Button();
      this.rtb = new System.Windows.Forms.RichTextBox();
      this.txt = new System.Windows.Forms.TextBox();
      this.btnRunHost = new System.Windows.Forms.Button();
      this.btnRunClient = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnConnect
      // 
      this.btnConnect.Location = new System.Drawing.Point(36, 28);
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(75, 23);
      this.btnConnect.TabIndex = 0;
      this.btnConnect.Text = "Connect";
      this.btnConnect.UseVisualStyleBackColor = true;
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // btnDisconnect
      // 
      this.btnDisconnect.Location = new System.Drawing.Point(136, 28);
      this.btnDisconnect.Name = "btnDisconnect";
      this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
      this.btnDisconnect.TabIndex = 2;
      this.btnDisconnect.Text = "Disconnect";
      this.btnDisconnect.UseVisualStyleBackColor = true;
      this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
      // 
      // rtb
      // 
      this.rtb.Location = new System.Drawing.Point(36, 117);
      this.rtb.Name = "rtb";
      this.rtb.Size = new System.Drawing.Size(717, 298);
      this.rtb.TabIndex = 3;
      this.rtb.Text = "";
      // 
      // txt
      // 
      this.txt.Location = new System.Drawing.Point(234, 28);
      this.txt.Name = "txt";
      this.txt.Size = new System.Drawing.Size(100, 20);
      this.txt.TabIndex = 4;
      // 
      // btnRunHost
      // 
      this.btnRunHost.Location = new System.Drawing.Point(136, 73);
      this.btnRunHost.Name = "btnRunHost";
      this.btnRunHost.Size = new System.Drawing.Size(75, 23);
      this.btnRunHost.TabIndex = 6;
      this.btnRunHost.Text = "Run Host";
      this.btnRunHost.UseVisualStyleBackColor = true;
      this.btnRunHost.Click += new System.EventHandler(this.btnRunHost_Click);
      // 
      // btnRunClient
      // 
      this.btnRunClient.Location = new System.Drawing.Point(234, 73);
      this.btnRunClient.Name = "btnRunClient";
      this.btnRunClient.Size = new System.Drawing.Size(75, 23);
      this.btnRunClient.TabIndex = 7;
      this.btnRunClient.Text = "Run Client";
      this.btnRunClient.UseVisualStyleBackColor = true;
      this.btnRunClient.Click += new System.EventHandler(this.btnRunClient_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.btnRunClient);
      this.Controls.Add(this.btnRunHost);
      this.Controls.Add(this.txt);
      this.Controls.Add(this.rtb);
      this.Controls.Add(this.btnDisconnect);
      this.Controls.Add(this.btnConnect);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.Button btnDisconnect;
    private System.Windows.Forms.RichTextBox rtb;
    private System.Windows.Forms.TextBox txt;
    private System.Windows.Forms.Button btnRunHost;
    private System.Windows.Forms.Button btnRunClient;
  }
}

