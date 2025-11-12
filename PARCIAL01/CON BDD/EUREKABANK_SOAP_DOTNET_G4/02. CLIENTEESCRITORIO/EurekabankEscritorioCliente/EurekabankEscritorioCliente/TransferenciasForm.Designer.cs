namespace EurekabankEscritorioCliente
{
    partial class TransferenciasForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtCuentaOrigen;
        private System.Windows.Forms.TextBox txtCuentaDestino;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Button btnRealizarTransferencia;
        private System.Windows.Forms.Label lblCuentaOrigen;
        private System.Windows.Forms.Label lblCuentaDestino;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.ProgressBar progressBar;

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
            this.txtCuentaOrigen = new System.Windows.Forms.TextBox();
            this.txtCuentaDestino = new System.Windows.Forms.TextBox();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.btnRealizarTransferencia = new System.Windows.Forms.Button();
            this.lblCuentaOrigen = new System.Windows.Forms.Label();
            this.lblCuentaDestino = new System.Windows.Forms.Label();
            this.lblValor = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtCuentaOrigen
            // 
            this.txtCuentaOrigen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuentaOrigen.Location = new System.Drawing.Point(242, 49);
            this.txtCuentaOrigen.Name = "txtCuentaOrigen";
            this.txtCuentaOrigen.Size = new System.Drawing.Size(200, 26);
            this.txtCuentaOrigen.TabIndex = 0;
            this.txtCuentaOrigen.Text = "Cuenta Origen";
            this.txtCuentaOrigen.Enter += new System.EventHandler(this.txtCuentaOrigen_Enter);
            this.txtCuentaOrigen.Leave += new System.EventHandler(this.txtCuentaOrigen_Leave);
            // 
            // txtCuentaDestino
            // 
            this.txtCuentaDestino.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuentaDestino.Location = new System.Drawing.Point(242, 99);
            this.txtCuentaDestino.Name = "txtCuentaDestino";
            this.txtCuentaDestino.Size = new System.Drawing.Size(200, 26);
            this.txtCuentaDestino.TabIndex = 1;
            this.txtCuentaDestino.Text = "Cuenta Destino";
            this.txtCuentaDestino.Enter += new System.EventHandler(this.txtCuentaDestino_Enter);
            this.txtCuentaDestino.Leave += new System.EventHandler(this.txtCuentaDestino_Leave);
            // 
            // txtValor
            // 
            this.txtValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValor.Location = new System.Drawing.Point(242, 149);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(200, 26);
            this.txtValor.TabIndex = 2;
            this.txtValor.Text = "Valor";
            this.txtValor.Enter += new System.EventHandler(this.txtValor_Enter);
            this.txtValor.Leave += new System.EventHandler(this.txtValor_Leave);
            // 
            // btnRealizarTransferencia
            // 
            this.btnRealizarTransferencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnRealizarTransferencia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRealizarTransferencia.ForeColor = System.Drawing.Color.White;
            this.btnRealizarTransferencia.Location = new System.Drawing.Point(170, 222);
            this.btnRealizarTransferencia.Name = "btnRealizarTransferencia";
            this.btnRealizarTransferencia.Size = new System.Drawing.Size(200, 40);
            this.btnRealizarTransferencia.TabIndex = 3;
            this.btnRealizarTransferencia.Text = "Realizar Transferencia";
            this.btnRealizarTransferencia.UseVisualStyleBackColor = false;
            this.btnRealizarTransferencia.Click += new System.EventHandler(this.btnRealizarTransferencia_Click);
            // 
            // lblCuentaOrigen
            // 
            this.lblCuentaOrigen.AutoSize = true;
            this.lblCuentaOrigen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCuentaOrigen.Location = new System.Drawing.Point(97, 49);
            this.lblCuentaOrigen.Name = "lblCuentaOrigen";
            this.lblCuentaOrigen.Size = new System.Drawing.Size(130, 20);
            this.lblCuentaOrigen.TabIndex = 4;
            this.lblCuentaOrigen.Text = "Cuenta Origen:";
            // 
            // lblCuentaDestino
            // 
            this.lblCuentaDestino.AutoSize = true;
            this.lblCuentaDestino.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCuentaDestino.Location = new System.Drawing.Point(97, 99);
            this.lblCuentaDestino.Name = "lblCuentaDestino";
            this.lblCuentaDestino.Size = new System.Drawing.Size(139, 20);
            this.lblCuentaDestino.TabIndex = 5;
            this.lblCuentaDestino.Text = "Cuenta Destino:";
            // 
            // lblValor
            // 
            this.lblValor.AutoSize = true;
            this.lblValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValor.Location = new System.Drawing.Point(97, 149);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(56, 20);
            this.lblValor.TabIndex = 6;
            this.lblValor.Text = "Valor:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(64, 284);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            this.progressBar.Visible = false;
            // 
            // TransferenciasForm
            // 
            this.ClientSize = new System.Drawing.Size(529, 328);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.lblCuentaDestino);
            this.Controls.Add(this.lblCuentaOrigen);
            this.Controls.Add(this.btnRealizarTransferencia);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.txtCuentaDestino);
            this.Controls.Add(this.txtCuentaOrigen);
            this.Name = "TransferenciasForm";
            this.Text = "Transferencias";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
