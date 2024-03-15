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
        Label3 = New Label()
        btnDatabase = New Button()
        cmbDatabaseTable = New ComboBox()
        Label1 = New Label()
        txtDatabase = New TextBox()
        dlgOpenFile = New OpenFileDialog()
        GroupBox2 = New GroupBox()
        Label4 = New Label()
        btnGeopackage = New Button()
        cmbGeopackageLayer = New ComboBox()
        txtPolygonSf = New TextBox()
        chkInterpolationInsidePolygons = New CheckBox()
        Label2 = New Label()
        txtGeopackage = New TextBox()
        btnExecute = New Button()
        dlgFolder = New FolderBrowserDialog()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Label3)
        GroupBox1.Controls.Add(btnDatabase)
        GroupBox1.Controls.Add(cmbDatabaseTable)
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(txtDatabase)
        GroupBox1.Location = New Point(11, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(1157, 125)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Data source"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(6, 28)
        Label3.Name = "Label3"
        Label3.Size = New Size(75, 20)
        Label3.TabIndex = 10
        Label3.Text = "Database:"
        ' 
        ' btnDatabase
        ' 
        btnDatabase.Location = New Point(851, 24)
        btnDatabase.Name = "btnDatabase"
        btnDatabase.Size = New Size(29, 29)
        btnDatabase.TabIndex = 9
        btnDatabase.Text = ".."
        btnDatabase.UseVisualStyleBackColor = True
        ' 
        ' cmbDatabaseTable
        ' 
        cmbDatabaseTable.FormattingEnabled = True
        cmbDatabaseTable.Location = New Point(939, 25)
        cmbDatabaseTable.Margin = New Padding(3, 4, 3, 4)
        cmbDatabaseTable.Name = "cmbDatabaseTable"
        cmbDatabaseTable.Size = New Size(210, 28)
        cmbDatabaseTable.TabIndex = 8
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
        ' txtDatabase
        ' 
        txtDatabase.Location = New Point(139, 25)
        txtDatabase.Name = "txtDatabase"
        txtDatabase.Size = New Size(706, 27)
        txtDatabase.TabIndex = 1
        ' 
        ' dlgOpenFile
        ' 
        dlgOpenFile.FileName = "dlgOpenFile"
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(Label4)
        GroupBox2.Controls.Add(btnGeopackage)
        GroupBox2.Controls.Add(cmbGeopackageLayer)
        GroupBox2.Controls.Add(txtPolygonSf)
        GroupBox2.Controls.Add(chkInterpolationInsidePolygons)
        GroupBox2.Controls.Add(Label2)
        GroupBox2.Controls.Add(txtGeopackage)
        GroupBox2.Location = New Point(11, 143)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(1157, 233)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Data target"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(17, 40)
        Label4.Name = "Label4"
        Label4.Size = New Size(95, 20)
        Label4.TabIndex = 11
        Label4.Text = "Geopackage:"
        ' 
        ' btnGeopackage
        ' 
        btnGeopackage.Location = New Point(851, 36)
        btnGeopackage.Name = "btnGeopackage"
        btnGeopackage.Size = New Size(29, 29)
        btnGeopackage.TabIndex = 10
        btnGeopackage.Text = ".."
        btnGeopackage.UseVisualStyleBackColor = True
        ' 
        ' cmbGeopackageLayer
        ' 
        cmbGeopackageLayer.FormattingEnabled = True
        cmbGeopackageLayer.Location = New Point(939, 35)
        cmbGeopackageLayer.Margin = New Padding(3, 4, 3, 4)
        cmbGeopackageLayer.Name = "cmbGeopackageLayer"
        cmbGeopackageLayer.Size = New Size(210, 28)
        cmbGeopackageLayer.TabIndex = 7
        ' 
        ' txtPolygonSf
        ' 
        txtPolygonSf.Location = New Point(405, 80)
        txtPolygonSf.Name = "txtPolygonSf"
        txtPolygonSf.Size = New Size(744, 27)
        txtPolygonSf.TabIndex = 6
        ' 
        ' chkInterpolationInsidePolygons
        ' 
        chkInterpolationInsidePolygons.AutoSize = True
        chkInterpolationInsidePolygons.Location = New Point(139, 83)
        chkInterpolationInsidePolygons.Margin = New Padding(3, 4, 3, 4)
        chkInterpolationInsidePolygons.Name = "chkInterpolationInsidePolygons"
        chkInterpolationInsidePolygons.Size = New Size(261, 24)
        chkInterpolationInsidePolygons.TabIndex = 5
        chkInterpolationInsidePolygons.Text = "Interpolation within polygons only:"
        chkInterpolationInsidePolygons.UseVisualStyleBackColor = True
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
        ' txtGeopackage
        ' 
        txtGeopackage.Location = New Point(139, 36)
        txtGeopackage.Name = "txtGeopackage"
        txtGeopackage.Size = New Size(706, 27)
        txtGeopackage.TabIndex = 4
        ' 
        ' btnExecute
        ' 
        btnExecute.Location = New Point(1066, 394)
        btnExecute.Name = "btnExecute"
        btnExecute.Size = New Size(94, 45)
        btnExecute.TabIndex = 2
        btnExecute.Text = "Execute"
        btnExecute.UseVisualStyleBackColor = True
        ' 
        ' frmSpatialInterpolation
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1181, 451)
        Controls.Add(btnExecute)
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
    Friend WithEvents Label1 As Label
    Friend WithEvents txtTableName As TextBox
    Friend WithEvents dlgOpenFile As OpenFileDialog
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtGeopackage As TextBox
    Friend WithEvents txtPolygonSf As TextBox
    Friend WithEvents chkInterpolationInsidePolygons As CheckBox
    Friend WithEvents cmbGeopackageLayer As ComboBox
    Friend WithEvents cmbDatabaseTable As ComboBox
    Friend WithEvents btnGeopackage As Button
    Friend WithEvents btnDatabase As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnExecute As Button
    Friend WithEvents dlgFolder As FolderBrowserDialog
End Class
