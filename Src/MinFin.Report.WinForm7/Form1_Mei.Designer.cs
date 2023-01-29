namespace MinFin.Report.WinForm7//.Mei
{
  partial class Form1_Mei
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    System.ComponentModel.IContainer components = null;

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
    void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1_Mei));
      this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
      this.button1 = new System.Windows.Forms.Button();
      this.Vw_TaxLiqReport_MeiBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.findemoDataSet = new MinFin.Report.WinForm7.findemoDataSet();
      this.Vw_TaxLiqReport_MeiTableAdapter = new MinFin.Report.WinForm7.findemoDataSetTableAdapters.Vw_TaxLiqReport_MeiTableAdapter();
      ((System.ComponentModel.ISupportInitialize)(this.Vw_TaxLiqReport_MeiBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.findemoDataSet)).BeginInit();
      this.SuspendLayout();
      // 
      // reportViewer1
      // 
      this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
      reportDataSource1.Name = "DataSet1";
      reportDataSource1.Value = this.Vw_TaxLiqReport_MeiBindingSource;
      this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
      this.reportViewer1.LocalReport.ReportEmbeddedResource = "MinFin.Report.WinForm7.Report3_Mei.rdlc";
      this.reportViewer1.Location = new System.Drawing.Point(0, 0);
      this.reportViewer1.Name = "reportViewer1";
      this.reportViewer1.Size = new System.Drawing.Size(784, 1024);
      this.reportViewer1.TabIndex = 0;
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button1.Location = new System.Drawing.Point(919, 13);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "Exit";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // Vw_TaxLiqReport_MeiBindingSource
      // 
      this.Vw_TaxLiqReport_MeiBindingSource.DataMember = "Vw_TaxLiqReport_Mei";
      this.Vw_TaxLiqReport_MeiBindingSource.DataSource = this.findemoDataSet;
      // 
      // findemoDataSet
      // 
      this.findemoDataSet.DataSetName = "findemoDataSet";
      this.findemoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // Vw_TaxLiqReport_MeiTableAdapter
      // 
      this.Vw_TaxLiqReport_MeiTableAdapter.ClearBeforeFill = true;
      // 
      // Form1_Mei
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.button1;
      this.ClientSize = new System.Drawing.Size(784, 1024);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.reportViewer1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form1_Mei";
      this.Text = "MinFin - Report3_Mei";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.Vw_TaxLiqReport_MeiBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.findemoDataSet)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    System.Windows.Forms.BindingSource Vw_TaxLiqReport_MeiBindingSource;
    findemoDataSet findemoDataSet;
    findemoDataSetTableAdapters.Vw_TaxLiqReport_MeiTableAdapter Vw_TaxLiqReport_MeiTableAdapter;
    System.Windows.Forms.Button button1;
  }
}

