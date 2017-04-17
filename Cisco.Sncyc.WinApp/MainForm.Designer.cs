namespace Cisco.Sncyc.WinApp
{
    partial class MainForm
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
            this.pnlScan = new System.Windows.Forms.Panel();
            this.lblScan = new System.Windows.Forms.Label();
            this.txtScan = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.lblScanCount = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblItemQty = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.pnlScan.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlScan
            // 
            this.pnlScan.Controls.Add(this.lblScan);
            this.pnlScan.Controls.Add(this.txtScan);
            this.pnlScan.Location = new System.Drawing.Point(12, 108);
            this.pnlScan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlScan.Name = "pnlScan";
            this.pnlScan.Size = new System.Drawing.Size(449, 46);
            this.pnlScan.TabIndex = 2;
            this.pnlScan.Visible = false;
            // 
            // lblScan
            // 
            this.lblScan.AutoSize = true;
            this.lblScan.ForeColor = System.Drawing.Color.White;
            this.lblScan.Location = new System.Drawing.Point(3, 9);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(91, 25);
            this.lblScan.TabIndex = 3;
            this.lblScan.Text = "Scan >>";
            this.lblScan.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtScan
            // 
            this.txtScan.Location = new System.Drawing.Point(157, 6);
            this.txtScan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtScan.Name = "txtScan";
            this.txtScan.Size = new System.Drawing.Size(165, 31);
            this.txtScan.TabIndex = 2;
            this.txtScan.TextChanged += new System.EventHandler(this.txtScan_TextChanged);
            this.txtScan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtScan_KeyPress);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(18, 156);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(57, 25);
            this.lblError.TabIndex = 9;
            this.lblError.Text = "error";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblError.Visible = false;
            // 
            // lblScanCount
            // 
            this.lblScanCount.AutoSize = true;
            this.lblScanCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblScanCount.Location = new System.Drawing.Point(385, 20);
            this.lblScanCount.Name = "lblScanCount";
            this.lblScanCount.Size = new System.Drawing.Size(0, 25);
            this.lblScanCount.TabIndex = 10;
            this.lblScanCount.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.panel1.Controls.Add(this.lblItemQty);
            this.panel1.Controls.Add(this.lblProduct);
            this.panel1.Controls.Add(this.lblLocation);
            this.panel1.Controls.Add(this.lblCustomer);
            this.panel1.Location = new System.Drawing.Point(12, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(449, 76);
            this.panel1.TabIndex = 11;
            // 
            // lblItemQty
            // 
            this.lblItemQty.AutoSize = true;
            this.lblItemQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(225)))));
            this.lblItemQty.Location = new System.Drawing.Point(273, 39);
            this.lblItemQty.Name = "lblItemQty";
            this.lblItemQty.Size = new System.Drawing.Size(45, 25);
            this.lblItemQty.TabIndex = 11;
            this.lblItemQty.Text = "Qty";
            this.lblItemQty.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblItemQty.Visible = false;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(6, 39);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(86, 25);
            this.lblProduct.TabIndex = 10;
            this.lblProduct.Text = "Product";
            this.lblProduct.Visible = false;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.ForeColor = System.Drawing.Color.Black;
            this.lblLocation.Location = new System.Drawing.Point(273, 3);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(94, 25);
            this.lblLocation.TabIndex = 9;
            this.lblLocation.Text = "Location";
            this.lblLocation.Visible = false;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.ForeColor = System.Drawing.Color.Black;
            this.lblCustomer.Location = new System.Drawing.Point(6, 3);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(56, 25);
            this.lblCustomer.TabIndex = 8;
            this.lblCustomer.Text = "Cust";
            this.lblCustomer.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(473, 375);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblScanCount);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.pnlScan);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Cisco SN Cyc";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pnlScan.ResumeLayout(false);
            this.pnlScan.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlScan;
        private System.Windows.Forms.Label lblScan;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblScanCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Label lblItemQty;
    }
}

