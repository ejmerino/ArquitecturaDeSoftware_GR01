namespace EurekabankEscritorioCliente
{
    partial class MenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnCuentas;
        private System.Windows.Forms.Button btnMovimientos;
        private System.Windows.Forms.Button btnTransferencias;
        private System.Windows.Forms.Label lblBienvenido;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.PictureBox pictureBoxLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            this.btnCuentas = new System.Windows.Forms.Button();
            this.btnMovimientos = new System.Windows.Forms.Button();
            this.btnTransferencias = new System.Windows.Forms.Button();
            this.lblBienvenido = new System.Windows.Forms.Label();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCuentas
            // 
            this.btnCuentas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(153)))));
            this.btnCuentas.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCuentas.ForeColor = System.Drawing.Color.White;
            this.btnCuentas.Location = new System.Drawing.Point(30, 100);
            this.btnCuentas.Name = "btnCuentas";
            this.btnCuentas.Size = new System.Drawing.Size(220, 50);
            this.btnCuentas.TabIndex = 0;
            this.btnCuentas.Text = "Cuentas";
            this.btnCuentas.UseVisualStyleBackColor = false;
            this.btnCuentas.Click += new System.EventHandler(this.btnCuentas_Click);
            // 
            // btnMovimientos
            // 
            this.btnMovimientos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(153)))));
            this.btnMovimientos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnMovimientos.ForeColor = System.Drawing.Color.White;
            this.btnMovimientos.Location = new System.Drawing.Point(30, 170);
            this.btnMovimientos.Name = "btnMovimientos";
            this.btnMovimientos.Size = new System.Drawing.Size(220, 50);
            this.btnMovimientos.TabIndex = 1;
            this.btnMovimientos.Text = "Movimientos";
            this.btnMovimientos.UseVisualStyleBackColor = false;
            this.btnMovimientos.Click += new System.EventHandler(this.btnMovimientos_Click);
            // 
            // btnTransferencias
            // 
            this.btnTransferencias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(153)))));
            this.btnTransferencias.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnTransferencias.ForeColor = System.Drawing.Color.White;
            this.btnTransferencias.Location = new System.Drawing.Point(30, 240);
            this.btnTransferencias.Name = "btnTransferencias";
            this.btnTransferencias.Size = new System.Drawing.Size(220, 50);
            this.btnTransferencias.TabIndex = 2;
            this.btnTransferencias.Text = "Realizar Transferencias";
            this.btnTransferencias.UseVisualStyleBackColor = false;
            this.btnTransferencias.Click += new System.EventHandler(this.btnTransferencias_Click);
            // 
            // lblBienvenido
            // 
            this.lblBienvenido.AutoSize = true;
            this.lblBienvenido.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblBienvenido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(153)))));
            this.lblBienvenido.Location = new System.Drawing.Point(148, 34);
            this.lblBienvenido.Name = "lblBienvenido";
            this.lblBienvenido.Size = new System.Drawing.Size(213, 30);
            this.lblBienvenido.TabIndex = 3;
            this.lblBienvenido.Text = "Bienvenido Usuario";
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCerrarSesion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCerrarSesion.ForeColor = System.Drawing.Color.White;
            this.btnCerrarSesion.Location = new System.Drawing.Point(280, 318);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(220, 50);
            this.btnCerrarSesion.TabIndex = 4;
            this.btnCerrarSesion.Text = "Cerrar Sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(270, 100);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(240, 200);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 5;
            this.pictureBoxLogo.TabStop = false;
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(540, 380);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.btnCerrarSesion);
            this.Controls.Add(this.lblBienvenido);
            this.Controls.Add(this.btnTransferencias);
            this.Controls.Add(this.btnMovimientos);
            this.Controls.Add(this.btnCuentas);
            this.Name = "MenuForm";
            this.Text = "Menú Principal";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
