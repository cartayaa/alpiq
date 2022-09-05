namespace WCrypt
{
    partial class WCrypt
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
            this.lblClave = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblTxtAEncriptar = new System.Windows.Forms.Label();
            this.txtAEncriptar = new System.Windows.Forms.TextBox();
            this.btnEncriptar = new System.Windows.Forms.Button();
            this.txtEncriptado = new System.Windows.Forms.TextBox();
            this.lblEncriptado = new System.Windows.Forms.Label();
            this.btnDesencriptar = new System.Windows.Forms.Button();
            this.txtTextoDesencriptado = new System.Windows.Forms.TextBox();
            this.lblTextoDesencriptado = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Location = new System.Drawing.Point(13, 13);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(110, 13);
            this.lblClave.TabIndex = 0;
            this.lblClave.Text = "Clave de encriptación";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(16, 30);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(213, 20);
            this.txtClave.TabIndex = 1;
            // 
            // lblTxtAEncriptar
            // 
            this.lblTxtAEncriptar.AutoSize = true;
            this.lblTxtAEncriptar.Location = new System.Drawing.Point(13, 57);
            this.lblTxtAEncriptar.Name = "lblTxtAEncriptar";
            this.lblTxtAEncriptar.Size = new System.Drawing.Size(102, 13);
            this.lblTxtAEncriptar.TabIndex = 2;
            this.lblTxtAEncriptar.Text = "Texto para encriptar";
            // 
            // txtAEncriptar
            // 
            this.txtAEncriptar.Location = new System.Drawing.Point(16, 73);
            this.txtAEncriptar.Name = "txtAEncriptar";
            this.txtAEncriptar.Size = new System.Drawing.Size(681, 20);
            this.txtAEncriptar.TabIndex = 3;
            this.txtAEncriptar.WordWrap = false;
            // 
            // btnEncriptar
            // 
            this.btnEncriptar.Location = new System.Drawing.Point(13, 109);
            this.btnEncriptar.Name = "btnEncriptar";
            this.btnEncriptar.Size = new System.Drawing.Size(124, 29);
            this.btnEncriptar.TabIndex = 4;
            this.btnEncriptar.Text = "Encriptar";
            this.btnEncriptar.UseVisualStyleBackColor = true;
            this.btnEncriptar.Click += new System.EventHandler(this.btnEncriptar_Click);
            // 
            // txtEncriptado
            // 
            this.txtEncriptado.Location = new System.Drawing.Point(16, 170);
            this.txtEncriptado.Name = "txtEncriptado";
            this.txtEncriptado.Size = new System.Drawing.Size(681, 20);
            this.txtEncriptado.TabIndex = 6;
            // 
            // lblEncriptado
            // 
            this.lblEncriptado.AutoSize = true;
            this.lblEncriptado.Location = new System.Drawing.Point(13, 154);
            this.lblEncriptado.Name = "lblEncriptado";
            this.lblEncriptado.Size = new System.Drawing.Size(87, 13);
            this.lblEncriptado.TabIndex = 5;
            this.lblEncriptado.Text = "Texto encriptado";
            // 
            // btnDesencriptar
            // 
            this.btnDesencriptar.Location = new System.Drawing.Point(13, 197);
            this.btnDesencriptar.Name = "btnDesencriptar";
            this.btnDesencriptar.Size = new System.Drawing.Size(124, 23);
            this.btnDesencriptar.TabIndex = 7;
            this.btnDesencriptar.Text = "Desencriptar";
            this.btnDesencriptar.UseVisualStyleBackColor = true;
            this.btnDesencriptar.Click += new System.EventHandler(this.btnDesencriptar_Click);
            // 
            // txtTextoDesencriptado
            // 
            this.txtTextoDesencriptado.Location = new System.Drawing.Point(13, 249);
            this.txtTextoDesencriptado.Name = "txtTextoDesencriptado";
            this.txtTextoDesencriptado.ReadOnly = true;
            this.txtTextoDesencriptado.Size = new System.Drawing.Size(681, 20);
            this.txtTextoDesencriptado.TabIndex = 9;
            // 
            // lblTextoDesencriptado
            // 
            this.lblTextoDesencriptado.AutoSize = true;
            this.lblTextoDesencriptado.Location = new System.Drawing.Point(10, 233);
            this.lblTextoDesencriptado.Name = "lblTextoDesencriptado";
            this.lblTextoDesencriptado.Size = new System.Drawing.Size(87, 13);
            this.lblTextoDesencriptado.TabIndex = 8;
            this.lblTextoDesencriptado.Text = "Texto encriptado";
            // 
            // WCrypt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 291);
            this.Controls.Add(this.txtTextoDesencriptado);
            this.Controls.Add(this.lblTextoDesencriptado);
            this.Controls.Add(this.btnDesencriptar);
            this.Controls.Add(this.txtEncriptado);
            this.Controls.Add(this.lblEncriptado);
            this.Controls.Add(this.btnEncriptar);
            this.Controls.Add(this.txtAEncriptar);
            this.Controls.Add(this.lblTxtAEncriptar);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.lblClave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "WCrypt";
            this.Text = "Encriptación de textos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblTxtAEncriptar;
        private System.Windows.Forms.TextBox txtAEncriptar;
        private System.Windows.Forms.Button btnEncriptar;
        private System.Windows.Forms.TextBox txtEncriptado;
        private System.Windows.Forms.Label lblEncriptado;
        private System.Windows.Forms.Button btnDesencriptar;
        private System.Windows.Forms.TextBox txtTextoDesencriptado;
        private System.Windows.Forms.Label lblTextoDesencriptado;
    }
}

