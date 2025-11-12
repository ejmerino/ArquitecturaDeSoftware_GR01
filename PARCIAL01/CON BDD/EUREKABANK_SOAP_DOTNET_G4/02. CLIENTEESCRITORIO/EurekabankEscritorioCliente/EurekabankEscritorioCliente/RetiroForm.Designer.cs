namespace EurekabankEscritorioCliente
{
    partial class CuentasForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.TextBox txtNumeroCuenta;
        private System.Windows.Forms.DataGridView dgvCuentas;

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
            this.btnBuscar = new System.Windows.Forms.Button();
            this.txtNumeroCuenta = new System.Windows.Forms.TextBox();
            this.dgvCuentas = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).BeginInit();
            this.SuspendLayout();

            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.Color.FromArgb(0, 122, 204); // Azul oscuro
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.Location = new System.Drawing.Point(230, 20);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 0;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

            // 
            // txtNumeroCuenta
            // 
            this.txtNumeroCuenta.Location = new System.Drawing.Point(20, 20);
            this.txtNumeroCuenta.Name = "txtNumeroCuenta";
            this.txtNumeroCuenta.Size = new System.Drawing.Size(200, 20);
            this.txtNumeroCuenta.TabIndex = 1;

            // 
            // dgvCuentas
            // 
            this.dgvCuentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCuentas.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 122, 204); // Azul oscuro
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Bold);
            this.dgvCuentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCuentas.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 122, 204); // Color de selección
            this.dgvCuentas.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvCuentas.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245); // Color de fila alternante
            this.dgvCuentas.Location = new System.Drawing.Point(20, 60);
            this.dgvCuentas.Name = "dgvCuentas";
            this.dgvCuentas.RowHeadersVisible = false; // Ocultar encabezados de fila
            this.dgvCuentas.Size = new System.Drawing.Size(600, 300);
            this.dgvCuentas.TabIndex = 2;

            // 
            // CuentasForm
            // 
            this.ClientSize = new System.Drawing.Size(640, 380);
            this.Controls.Add(this.dgvCuentas);
            this.Controls.Add(this.txtNumeroCuenta);
            this.Controls.Add(this.btnBuscar);
            this.Name = "CuentasForm";
            this.Text = "Consulta de Cuentas";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
