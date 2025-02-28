using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WifiIPChanger;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
partial class Form1
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

    private ComboBox cmbInterfaces;
    private Label lblInterface;
    private Label lblCurrentIP;
    private GroupBox groupBox1;
    private RadioButton rbStatic;
    private RadioButton rbDHCP;
    private Label lblIPAddress;
    private TextBox txtIPAddress;
    private Label lblSubnetMask;
    private TextBox txtSubnetMask;
    private Label lblGateway;
    private TextBox txtGateway;
    private Button btnApply;
    private Label lblStatus;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel toolStripStatusLabel;
    private Button btnRefresh;
    private Button btnRandomIP;
    private Label lblSSID;
    private GroupBox gbMacAddress;
    private TextBox txtMacAddress;
    private Button btnShowMAC;
    private Button btnGenerateMAC;
    private Button btnApplyMAC;
    private GroupBox gbNetworkTools;
    private Button btnReleaseRenew;
    private Button btnFlushDNS;
    private System.Windows.Forms.Timer timer1;
    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new Container();
        timer1 = new System.Windows.Forms.Timer(components);
        cmbInterfaces = new ComboBox();
        lblInterface = new Label();
        lblCurrentIP = new Label();
        lblSSID = new Label();
        groupBox1 = new GroupBox();
        rbStatic = new RadioButton();
        rbDHCP = new RadioButton();
        lblIPAddress = new Label();
        txtIPAddress = new TextBox();
        lblSubnetMask = new Label();
        txtSubnetMask = new TextBox();
        lblGateway = new Label();
        txtGateway = new TextBox();
        btnApply = new Button();
        lblStatus = new Label();
        statusStrip = new StatusStrip();
        toolStripStatusLabel = new ToolStripStatusLabel();
        btnRefresh = new Button();
        btnRandomIP = new Button();
        gbMacAddress = new GroupBox();
        txtMacAddress = new TextBox();
        btnShowMAC = new Button();
        btnGenerateMAC = new Button();
        btnApplyMAC = new Button();
        gbNetworkTools = new GroupBox();
        btnReleaseRenew = new Button();
        btnFlushDNS = new Button();
        groupBox1.SuspendLayout();
        statusStrip.SuspendLayout();
        gbMacAddress.SuspendLayout();
        gbNetworkTools.SuspendLayout();
        SuspendLayout();
        // 
        // cmbInterfaces
        // 
        cmbInterfaces.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbInterfaces.FormattingEnabled = true;
        cmbInterfaces.Location = new Point(169, 30);
        cmbInterfaces.Margin = new Padding(3, 4, 3, 4);
        cmbInterfaces.Name = "cmbInterfaces";
        cmbInterfaces.Size = new Size(338, 28);
        cmbInterfaces.TabIndex = 0;
        // 
        // lblInterface
        // 
        lblInterface.AutoSize = true;
        lblInterface.Location = new Point(30, 33);
        lblInterface.Name = "lblInterface";
        lblInterface.Size = new Size(88, 20);
        lblInterface.TabIndex = 1;
        lblInterface.Text = "Ağ Arayüzü:";
        // 
        // lblCurrentIP
        // 
        lblCurrentIP.AutoSize = true;
        lblCurrentIP.Location = new Point(30, 70);
        lblCurrentIP.Name = "lblCurrentIP";
        lblCurrentIP.Size = new Size(168, 20);
        lblCurrentIP.TabIndex = 13;
        lblCurrentIP.Text = "Mevcut IP: Mevcut Değil";
        // 
        // lblSSID
        // 
        lblSSID.AutoSize = true;
        lblSSID.Location = new Point(30, 94);
        lblSSID.Name = "lblSSID";
        lblSSID.Size = new Size(119, 20);
        lblSSID.TabIndex = 14;
        lblSSID.Text = "SSID: Bağlı değil";
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(rbStatic);
        groupBox1.Controls.Add(rbDHCP);
        groupBox1.Location = new Point(30, 118);
        groupBox1.Margin = new Padding(3, 4, 3, 4);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(463, 87);
        groupBox1.TabIndex = 2;
        groupBox1.TabStop = false;
        groupBox1.Text = "IP Ayarı";
        // 
        // rbStatic
        // 
        rbStatic.AutoSize = true;
        rbStatic.Location = new Point(247, 37);
        rbStatic.Margin = new Padding(3, 4, 3, 4);
        rbStatic.Name = "rbStatic";
        rbStatic.Size = new Size(83, 24);
        rbStatic.TabIndex = 1;
        rbStatic.Text = "Statik IP";
        rbStatic.UseVisualStyleBackColor = true;
        // 
        // rbDHCP
        // 
        rbDHCP.AutoSize = true;
        rbDHCP.Checked = true;
        rbDHCP.Location = new Point(54, 37);
        rbDHCP.Margin = new Padding(3, 4, 3, 4);
        rbDHCP.Name = "rbDHCP";
        rbDHCP.Size = new Size(69, 24);
        rbDHCP.TabIndex = 0;
        rbDHCP.TabStop = true;
        rbDHCP.Text = "DHCP";
        rbDHCP.UseVisualStyleBackColor = true;
        rbDHCP.CheckedChanged += rbDHCP_CheckedChanged;
        // 
        // lblIPAddress
        // 
        lblIPAddress.AutoSize = true;
        lblIPAddress.Enabled = false;
        lblIPAddress.Location = new Point(30, 224);
        lblIPAddress.Name = "lblIPAddress";
        lblIPAddress.Size = new Size(70, 20);
        lblIPAddress.TabIndex = 3;
        lblIPAddress.Text = "IP Adresi:";
        // 
        // txtIPAddress
        // 
        txtIPAddress.Enabled = false;
        txtIPAddress.Location = new Point(183, 217);
        txtIPAddress.Margin = new Padding(3, 4, 3, 4);
        txtIPAddress.Name = "txtIPAddress";
        txtIPAddress.Size = new Size(310, 27);
        txtIPAddress.TabIndex = 4;
        // 
        // lblSubnetMask
        // 
        lblSubnetMask.AutoSize = true;
        lblSubnetMask.Enabled = false;
        lblSubnetMask.Location = new Point(30, 259);
        lblSubnetMask.Name = "lblSubnetMask";
        lblSubnetMask.Size = new Size(110, 20);
        lblSubnetMask.TabIndex = 5;
        lblSubnetMask.Text = "Alt Ağ Maskesi:";
        // 
        // txtSubnetMask
        // 
        txtSubnetMask.Enabled = false;
        txtSubnetMask.Location = new Point(183, 252);
        txtSubnetMask.Margin = new Padding(3, 4, 3, 4);
        txtSubnetMask.Name = "txtSubnetMask";
        txtSubnetMask.Size = new Size(310, 27);
        txtSubnetMask.TabIndex = 6;
        // 
        // lblGateway
        // 
        lblGateway.AutoSize = true;
        lblGateway.Enabled = false;
        lblGateway.Location = new Point(30, 291);
        lblGateway.Name = "lblGateway";
        lblGateway.Size = new Size(147, 20);
        lblGateway.TabIndex = 7;
        lblGateway.Text = "Varsayılan Ağ Geçidi:";
        // 
        // txtGateway
        // 
        txtGateway.Enabled = false;
        txtGateway.Location = new Point(183, 287);
        txtGateway.Margin = new Padding(3, 4, 3, 4);
        txtGateway.Name = "txtGateway";
        txtGateway.Size = new Size(309, 27);
        txtGateway.TabIndex = 8;
        // 
        // btnApply
        // 
        btnApply.Location = new Point(405, 322);
        btnApply.Margin = new Padding(3, 4, 3, 4);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(183, 29);
        btnApply.TabIndex = 9;
        btnApply.Text = "Ayarları Uygula";
        btnApply.UseVisualStyleBackColor = true;
        btnApply.Click += btnApply_Click;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location = new Point(30, 560);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(45, 20);
        lblStatus.TabIndex = 10;
        lblStatus.Text = "Hazır";
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
        statusStrip.Location = new Point(0, 594);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(600, 26);
        statusStrip.TabIndex = 11;
        statusStrip.Text = "statusStrip1";
        // 
        // toolStripStatusLabel
        // 
        toolStripStatusLabel.Name = "toolStripStatusLabel";
        toolStripStatusLabel.Size = new Size(44, 20);
        toolStripStatusLabel.Text = "Hazır";
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(358, 63);
        btnRefresh.Margin = new Padding(3, 4, 3, 4);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(149, 35);
        btnRefresh.TabIndex = 12;
        btnRefresh.Text = "Arayüzleri Yenile";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnRandomIP
        // 
        btnRandomIP.Enabled = false;
        btnRandomIP.Location = new Point(498, 217);
        btnRandomIP.Margin = new Padding(3, 4, 3, 4);
        btnRandomIP.Name = "btnRandomIP";
        btnRandomIP.Size = new Size(90, 27);
        btnRandomIP.TabIndex = 5;
        btnRandomIP.Text = "Rastgele";
        btnRandomIP.UseVisualStyleBackColor = true;
        btnRandomIP.Click += btnRandomIP_Click;
        // 
        // gbMacAddress
        // 
        gbMacAddress.Controls.Add(txtMacAddress);
        gbMacAddress.Controls.Add(btnShowMAC);
        gbMacAddress.Controls.Add(btnGenerateMAC);
        gbMacAddress.Controls.Add(btnApplyMAC);
        gbMacAddress.Location = new Point(30, 344);
        gbMacAddress.Margin = new Padding(3, 4, 3, 4);
        gbMacAddress.Name = "gbMacAddress";
        gbMacAddress.Size = new Size(463, 120);
        gbMacAddress.TabIndex = 15;
        gbMacAddress.TabStop = false;
        gbMacAddress.Text = "MAC Adresi Değiştir";
        // 
        // txtMacAddress
        // 
        txtMacAddress.Location = new Point(110, 30);
        txtMacAddress.Margin = new Padding(3, 4, 3, 4);
        txtMacAddress.Name = "txtMacAddress";
        txtMacAddress.Size = new Size(338, 27);
        txtMacAddress.TabIndex = 0;
        // 
        // btnShowMAC
        // 
        btnShowMAC.Location = new Point(27, 70);
        btnShowMAC.Margin = new Padding(3, 4, 3, 4);
        btnShowMAC.Name = "btnShowMAC";
        btnShowMAC.Size = new Size(135, 35);
        btnShowMAC.TabIndex = 1;
        btnShowMAC.Text = "Mevcut MAC Göster";
        btnShowMAC.UseVisualStyleBackColor = true;
        // 
        // btnGenerateMAC
        // 
        btnGenerateMAC.Location = new Point(168, 70);
        btnGenerateMAC.Margin = new Padding(3, 4, 3, 4);
        btnGenerateMAC.Name = "btnGenerateMAC";
        btnGenerateMAC.Size = new Size(135, 35);
        btnGenerateMAC.TabIndex = 2;
        btnGenerateMAC.Text = "Rastgele MAC Oluştur";
        btnGenerateMAC.UseVisualStyleBackColor = true;
        // 
        // btnApplyMAC
        // 
        btnApplyMAC.Location = new Point(322, 65);
        btnApplyMAC.Margin = new Padding(3, 4, 3, 4);
        btnApplyMAC.Name = "btnApplyMAC";
        btnApplyMAC.Size = new Size(135, 50);
        btnApplyMAC.TabIndex = 3;
        btnApplyMAC.Text = "MAC Adresini Uygula";
        btnApplyMAC.UseVisualStyleBackColor = true;
        btnApplyMAC.Click += btnApplyMAC_Click;
        // 
        // gbNetworkTools
        // 
        gbNetworkTools.Controls.Add(btnReleaseRenew);
        gbNetworkTools.Controls.Add(btnFlushDNS);
        gbNetworkTools.Location = new Point(30, 474);
        gbNetworkTools.Margin = new Padding(3, 4, 3, 4);
        gbNetworkTools.Name = "gbNetworkTools";
        gbNetworkTools.Size = new Size(463, 80);
        gbNetworkTools.TabIndex = 16;
        gbNetworkTools.TabStop = false;
        gbNetworkTools.Text = "Network Tools";
        // 
        // btnReleaseRenew
        // 
        btnReleaseRenew.Location = new Point(27, 30);
        btnReleaseRenew.Margin = new Padding(3, 4, 3, 4);
        btnReleaseRenew.Name = "btnReleaseRenew";
        btnReleaseRenew.Size = new Size(195, 35);
        btnReleaseRenew.TabIndex = 0;
        btnReleaseRenew.Text = "Release/Renew IP";
        btnReleaseRenew.UseVisualStyleBackColor = true;
        btnReleaseRenew.Click += btnReleaseRenew_Click;
        // 
        // btnFlushDNS
        // 
        btnFlushDNS.Location = new Point(240, 30);
        btnFlushDNS.Margin = new Padding(3, 4, 3, 4);
        btnFlushDNS.Name = "btnFlushDNS";
        btnFlushDNS.Size = new Size(195, 35);
        btnFlushDNS.TabIndex = 1;
        btnFlushDNS.Text = "Flush DNS";
        btnFlushDNS.UseVisualStyleBackColor = true;
        btnFlushDNS.Click += btnFlushDNS_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(600, 620);
        Controls.Add(statusStrip);
        Controls.Add(lblStatus);
        Controls.Add(btnApply);
        Controls.Add(txtGateway);
        Controls.Add(lblGateway);
        Controls.Add(txtSubnetMask);
        Controls.Add(lblSubnetMask);
        Controls.Add(txtIPAddress);
        Controls.Add(lblIPAddress);
        Controls.Add(groupBox1);
        Controls.Add(lblInterface);
        Controls.Add(cmbInterfaces);
        Controls.Add(btnRefresh);
        Controls.Add(btnRandomIP);
        Controls.Add(lblCurrentIP);
        Controls.Add(lblSSID);
        Controls.Add(gbMacAddress);
        Controls.Add(gbNetworkTools);
        Margin = new Padding(3, 4, 3, 4);
        Name = "Form1";
        Text = "WiFi IP Değiştirici";
        Load += Form1_Load;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        gbMacAddress.ResumeLayout(false);
        gbMacAddress.PerformLayout();
        gbNetworkTools.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}

#endregion

