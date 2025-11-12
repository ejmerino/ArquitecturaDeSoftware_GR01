namespace EurekabankEscritorioCliente
{
    partial class CuentasForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblCuenta;
        private System.Windows.Forms.TextBox txtCuenta;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown numMonto;
        private System.Windows.Forms.Button btnDepositar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblCuenta = new System.Windows.Forms.Label();
            this.txtCuenta = new System.Windows.Forms.TextBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.numMonto = new System.Windows.Forms.NumericUpDown();
            this.btnDepositar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(151, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Realizar Depósito";
            // 
            // lblCuenta
            // 
            this.lblCuenta.AutoSize = true;
            this.lblCuenta.Location = new System.Drawing.Point(22, 55);
            this.lblCuenta.Name = "lblCuenta";
            this.lblCuenta.Size = new System.Drawing.Size(101, 15);
            this.lblCuenta.TabIndex = 1;
            this.lblCuenta.Text = "Número de cuenta";
            // 
            // txtCuenta
            // 
            this.txtCuenta.Location = new System.Drawing.Point(25, 73);
            this.txtCuenta.Name = "txtCuenta";
            this.txtCuenta.Size = new System.Drawing.Size(270, 23);
            this.txtCuenta.TabIndex = 0;
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(22, 110);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(95, 15);
            this.lblMonto.TabIndex = 3;
            this.lblMonto.Text = "Monto (en USD)";
            // 
            // numMonto
            // 
            this.numMonto.DecimalPlaces = 2;
            this.numMonto.Location = new System.Drawing.Point(25, 128);
            this.numMonto.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numMonto.Minimum = new decimal(new int[] { 1, 0, 0, 131072 }); // 0.01
            this.numMonto.Name = "numMonto";
            this.numMonto.Size = new System.Drawing.Size(150, 23);
            this.numMonto.TabIndex = 1;
            this.numMonto.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnDepositar
            // 
            this.btnDepositar.Location = new System.Drawing.Point(25, 170);
            this.btnDepositar.Name = "btnDepositar";
            this.btnDepositar.Size = new System.Drawing.Size(120, 30);
            this.btnDepositar.TabIndex = 2;
            this.btnDepositar.Text = "Depositar";
            this.btnDepositar.UseVisualStyleBackColor = true;
            this.btnDepositar.Click += new System.EventHandler(this.btnDepositar_Click);
            // 
            // DepositoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 225);
            this.Controls.Add(this.btnDepositar);
            this.Controls.Add(this.numMonto);
            this.Controls.Add(this.lblMonto);
            this.Controls.Add(this.txtCuenta);
            this.Controls.Add(this.lblCuenta);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DepositoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Depósito";
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
