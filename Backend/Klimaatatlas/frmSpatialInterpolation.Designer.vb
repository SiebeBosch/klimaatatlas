<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSpatialInterpolation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(frmSpatialInterpolation))
        GroupBox1 = New GroupBox()
        radDatabase = New RadioButton()
        txtDatabase = New TextBox()
        txtTableName = New TextBox()
        Label1 = New Label()
        OpenFileDialog1 = New OpenFileDialog()
        GroupBox2 = New GroupBox()
        RadioButton1 = New RadioButton()
        txtGeopackage = New TextBox()
        Label2 = New Label()
        txtGpkgLayer = New TextBox()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(txtTableName)
        GroupBox1.Controls.Add(txtDatabase)
        GroupBox1.Controls.Add(radDatabase)
        GroupBox1.Location = New Point(12, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(1157, 125)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Data source"
        ' 
        ' radDatabase
        ' 
        radDatabase.AutoSize = True
        radDatabase.Location = New Point(16, 26)
        radDatabase.Name = "radDatabase"
        radDatabase.Size = New Size(93, 24)
        radDatabase.TabIndex = 0
        radDatabase.TabStop = True
        radDatabase.Text = "Database"
        radDatabase.UseVisualStyleBackColor = True
        ' 
        ' txtDatabase
        ' 
        txtDatabase.Location = New Point(140, 25)
        txtDatabase.Name = "txtDatabase"
        txtDatabase.Size = New Size(740, 27)
        txtDatabase.TabIndex = 1
        ' 
        ' txtTableName
        ' 
        txtTableName.Location = New Point(939, 25)
        txtTableName.Name = "txtTableName"
        txtTableName.Size = New Size(212, 27)
        txtTableName.TabIndex = 2
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(886, 28)
        Label1.Name = "Label1"
        Label1.Size = New Size(47, 20)
        Label1.TabIndex = 3
        Label1.Text = "Table:"
        ' 
        ' OpenFileDialog1
        ' 
        OpenFileDialog1.FileName = "dlgOpenFile"
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(txtGpkgLayer)
        GroupBox2.Controls.Add(Label2)
        GroupBox2.Controls.Add(txtGeopackage)
        GroupBox2.Controls.Add(RadioButton1)
        GroupBox2.Location = New Point(12, 143)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(1157, 125)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Data target"
        ' 
        ' RadioButton1
        ' 
        RadioButton1.AutoSize = True
        RadioButton1.Location = New Point(16, 36)
        RadioButton1.Name = "RadioButton1"
        RadioButton1.Size = New Size(113, 24)
        RadioButton1.TabIndex = 4
        RadioButton1.TabStop = True
        RadioButton1.Text = "Geopackage"
        RadioButton1.UseVisualStyleBackColor = True
        ' 
        ' txtGeopackage
        ' 
        txtGeopackage.Location = New Point(140, 36)
        txtGeopackage.Name = "txtGeopackage"
        txtGeopackage.Size = New Size(740, 27)
        txtGeopackage.TabIndex = 4
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(886, 40)
        Label2.Name = "Label2"
        Label2.Size = New Size(47, 20)
        Label2.TabIndex = 4
        Label2.Text = "Layer:"
        ' 
        ' txtGpkgLayer
        ' 
        txtGpkgLayer.Location = New Point(939, 35)
        txtGpkgLayer.Name = "txtGpkgLayer"
        txtGpkgLayer.Size = New Size(212, 27)
        txtGpkgLayer.TabIndex = 4
        ' 
        ' frmSpatialInterpolation
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1181, 450)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "frmSpatialInterpolation"
        Text = "Spatial interpolation"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents txtDatabase As TextBox
    Friend WithEvents radDatabase As RadioButton
    Friend WithEvents Label1 As Label
    Friend WithEvents txtTableName As TextBox
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents txtGpkgLayer As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtGeopackage As TextBox
    Friend WithEvents RadioButton1 As RadioButton
End Class
