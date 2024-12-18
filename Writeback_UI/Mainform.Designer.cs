namespace Writeback_UI
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestERP = new System.Windows.Forms.Button();
            this.btnTestRun = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtERPCompany = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtERPPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtERPUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboERPType = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtAPIKEY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAPIURL = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgSettings = new System.Windows.Forms.DataGridView();
            this.KeyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeyValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblMessage = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSettings)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnTestERP);
            this.groupBox1.Controls.Add(this.btnTestRun);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(820, 439);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // btnTestERP
            // 
            this.btnTestERP.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnTestERP.Location = new System.Drawing.Point(668, 192);
            this.btnTestERP.Name = "btnTestERP";
            this.btnTestERP.Size = new System.Drawing.Size(129, 41);
            this.btnTestERP.TabIndex = 5;
            this.btnTestERP.Text = "Test ERP";
            this.btnTestERP.UseVisualStyleBackColor = true;
            this.btnTestERP.Click += new System.EventHandler(this.btnTestERP_Click);
            // 
            // btnTestRun
            // 
            this.btnTestRun.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnTestRun.Location = new System.Drawing.Point(668, 364);
            this.btnTestRun.Name = "btnTestRun";
            this.btnTestRun.Size = new System.Drawing.Size(129, 41);
            this.btnTestRun.TabIndex = 4;
            this.btnTestRun.Text = "Test Run";
            this.btnTestRun.UseVisualStyleBackColor = true;
            this.btnTestRun.Click += new System.EventHandler(this.btnTestRun_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtERPCompany);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtERPPassword);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtERPUser);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.cboERPType);
            this.groupBox4.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox4.Location = new System.Drawing.Point(7, 181);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(633, 252);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ERP Settings";
            // 
            // txtERPCompany
            // 
            this.txtERPCompany.Location = new System.Drawing.Point(12, 92);
            this.txtERPCompany.Name = "txtERPCompany";
            this.txtERPCompany.Size = new System.Drawing.Size(564, 22);
            this.txtERPCompany.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 16);
            this.label7.TabIndex = 9;
            this.label7.Text = "Company";
            // 
            // txtERPPassword
            // 
            this.txtERPPassword.Location = new System.Drawing.Point(12, 183);
            this.txtERPPassword.Name = "txtERPPassword";
            this.txtERPPassword.PasswordChar = '*';
            this.txtERPPassword.Size = new System.Drawing.Size(564, 22);
            this.txtERPPassword.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password";
            // 
            // txtERPUser
            // 
            this.txtERPUser.Location = new System.Drawing.Point(12, 136);
            this.txtERPUser.Name = "txtERPUser";
            this.txtERPUser.Size = new System.Drawing.Size(564, 22);
            this.txtERPUser.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "UserName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type of ERP";
            // 
            // cboERPType
            // 
            this.cboERPType.FormattingEnabled = true;
            this.cboERPType.Items.AddRange(new object[] {
            "SAGE"});
            this.cboERPType.Location = new System.Drawing.Point(12, 44);
            this.cboERPType.Name = "cboERPType";
            this.cboERPType.Size = new System.Drawing.Size(276, 24);
            this.cboERPType.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtAPIKEY);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtAPIURL);
            this.groupBox3.Controls.Add(this.Label1);
            this.groupBox3.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox3.Location = new System.Drawing.Point(7, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(633, 154);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ezyCollect Settings";
            // 
            // txtAPIKEY
            // 
            this.txtAPIKEY.Location = new System.Drawing.Point(12, 87);
            this.txtAPIKEY.Name = "txtAPIKEY";
            this.txtAPIKEY.PasswordChar = '*';
            this.txtAPIKEY.Size = new System.Drawing.Size(564, 22);
            this.txtAPIKEY.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "APIKey";
            // 
            // txtAPIURL
            // 
            this.txtAPIURL.Location = new System.Drawing.Point(12, 40);
            this.txtAPIURL.Name = "txtAPIURL";
            this.txtAPIURL.Size = new System.Drawing.Size(564, 22);
            this.txtAPIURL.TabIndex = 2;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(9, 21);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(34, 16);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "URL";
            // 
            // btnTest
            // 
            this.btnTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnTest.Location = new System.Drawing.Point(668, 108);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(129, 41);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test API";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSave.Location = new System.Drawing.Point(668, 275);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(129, 41);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgSettings);
            this.groupBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox2.Location = new System.Drawing.Point(12, 457);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(820, 194);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // dgSettings
            // 
            this.dgSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyName,
            this.KeyValue});
            this.dgSettings.Location = new System.Drawing.Point(11, 24);
            this.dgSettings.Name = "dgSettings";
            this.dgSettings.RowHeadersWidth = 51;
            this.dgSettings.RowTemplate.Height = 24;
            this.dgSettings.Size = new System.Drawing.Size(796, 158);
            this.dgSettings.TabIndex = 0;
            // 
            // KeyName
            // 
            this.KeyName.DataPropertyName = "KeyName";
            this.KeyName.HeaderText = "KeyName";
            this.KeyName.MinimumWidth = 6;
            this.KeyName.Name = "KeyName";
            this.KeyName.Width = 200;
            // 
            // KeyValue
            // 
            this.KeyValue.DataPropertyName = "KeyValue";
            this.KeyValue.HeaderText = "KeyValue";
            this.KeyValue.MinimumWidth = 6;
            this.KeyValue.Name = "KeyValue";
            this.KeyValue.Width = 325;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.ForeColor = System.Drawing.Color.Green;
            this.lblMessage.Location = new System.Drawing.Point(16, 615);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 16);
            this.lblMessage.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 724);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(844, 26);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 20);
            this.toolStripStatusLabel1.Text = "V 1.0.2";
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 750);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Mainform";
            this.Text = "ezyCollect Write-Back Sync Configuration";
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSettings)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtERPCompany;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtERPPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtERPUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboERPType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtAPIKEY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAPIURL;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.DataGridView dgSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeyValue;
        private System.Windows.Forms.Button btnTestRun;
        private System.Windows.Forms.Button btnTestERP;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

