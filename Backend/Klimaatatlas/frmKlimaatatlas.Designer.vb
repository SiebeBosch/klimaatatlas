<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmKlimaatatlas
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(frmKlimaatatlas))
        btnExecute = New Button()
        prProgress = New ProgressBar()
        lblProgress = New Label()
        GroupBox1 = New GroupBox()
        Label1 = New Label()
        btnResultsFile = New Button()
        txtResultsFile = New TextBox()
        btnConfigFile = New Button()
        txtConfigFile = New TextBox()
        lblConfigFile = New Label()
        btnDatabase = New Button()
        lblDatabase = New Label()
        txtDatabase = New TextBox()
        dlgOpenFile = New OpenFileDialog()
        MenuStrip1 = New MenuStrip()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        GISToolStripMenuItem = New ToolStripMenuItem()
        SpatialJoinToolStripMenuItem = New ToolStripMenuItem()
        dlgSaveFile = New SaveFileDialog()
        GroupBox1.SuspendLayout()
        MenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnExecute
        ' 
        btnExecute.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnExecute.Location = New Point(679, 390)
        btnExecute.Name = "btnExecute"
        btnExecute.Size = New Size(109, 48)
        btnExecute.TabIndex = 0
        btnExecute.Text = "Execute"
        btnExecute.UseVisualStyleBackColor = True
        ' 
        ' prProgress
        ' 
        prProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        prProgress.Location = New Point(12, 415)
        prProgress.Name = "prProgress"
        prProgress.Size = New Size(650, 23)
        prProgress.TabIndex = 1
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(12, 390)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(52, 15)
        lblProgress.TabIndex = 2
        lblProgress.Text = "Progress"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(btnResultsFile)
        GroupBox1.Controls.Add(txtResultsFile)
        GroupBox1.Controls.Add(btnConfigFile)
        GroupBox1.Controls.Add(txtConfigFile)
        GroupBox1.Controls.Add(lblConfigFile)
        GroupBox1.Controls.Add(btnDatabase)
        GroupBox1.Controls.Add(lblDatabase)
        GroupBox1.Controls.Add(txtDatabase)
        GroupBox1.Location = New Point(12, 35)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(776, 349)
        GroupBox1.TabIndex = 3
        GroupBox1.TabStop = False
        GroupBox1.Text = "Settings"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(11, 91)
        Label1.Name = "Label1"
        Label1.Size = New Size(66, 15)
        Label1.TabIndex = 8
        Label1.Text = "Results file:"
        ' 
        ' btnResultsFile
        ' 
        btnResultsFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnResultsFile.Location = New Point(744, 86)
        btnResultsFile.Name = "btnResultsFile"
        btnResultsFile.Size = New Size(24, 24)
        btnResultsFile.TabIndex = 7
        btnResultsFile.Text = ".."
        btnResultsFile.UseVisualStyleBackColor = True
        ' 
        ' txtResultsFile
        ' 
        txtResultsFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtResultsFile.Location = New Point(106, 89)
        txtResultsFile.Name = "txtResultsFile"
        txtResultsFile.Size = New Size(634, 23)
        txtResultsFile.TabIndex = 6
        ' 
        ' btnConfigFile
        ' 
        btnConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnConfigFile.Location = New Point(746, 57)
        btnConfigFile.Name = "btnConfigFile"
        btnConfigFile.Size = New Size(24, 24)
        btnConfigFile.TabIndex = 5
        btnConfigFile.Text = ".."
        btnConfigFile.UseVisualStyleBackColor = True
        ' 
        ' txtConfigFile
        ' 
        txtConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtConfigFile.Location = New Point(106, 57)
        txtConfigFile.Name = "txtConfigFile"
        txtConfigFile.Size = New Size(634, 23)
        txtConfigFile.TabIndex = 4
        ' 
        ' lblConfigFile
        ' 
        lblConfigFile.AutoSize = True
        lblConfigFile.Location = New Point(11, 60)
        lblConfigFile.Name = "lblConfigFile"
        lblConfigFile.Size = New Size(65, 15)
        lblConfigFile.TabIndex = 3
        lblConfigFile.Text = "Config file:"
        ' 
        ' btnDatabase
        ' 
        btnDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnDatabase.Location = New Point(746, 27)
        btnDatabase.Name = "btnDatabase"
        btnDatabase.Size = New Size(24, 24)
        btnDatabase.TabIndex = 2
        btnDatabase.Text = ".."
        btnDatabase.UseVisualStyleBackColor = True
        ' 
        ' lblDatabase
        ' 
        lblDatabase.AutoSize = True
        lblDatabase.Location = New Point(11, 30)
        lblDatabase.Name = "lblDatabase"
        lblDatabase.Size = New Size(77, 15)
        lblDatabase.TabIndex = 1
        lblDatabase.Text = "Database file:"
        ' 
        ' txtDatabase
        ' 
        txtDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtDatabase.Location = New Point(106, 27)
        txtDatabase.Name = "txtDatabase"
        txtDatabase.Size = New Size(634, 23)
        txtDatabase.TabIndex = 0
        ' 
        ' dlgOpenFile
        ' 
        dlgOpenFile.FileName = "dlgOpenFile"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(24, 24)
        MenuStrip1.Items.AddRange(New ToolStripItem() {ToolsToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(4, 1, 0, 1)
        MenuStrip1.Size = New Size(800, 24)
        MenuStrip1.TabIndex = 4
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GISToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(46, 22)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' GISToolStripMenuItem
        ' 
        GISToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {SpatialJoinToolStripMenuItem})
        GISToolStripMenuItem.Name = "GISToolStripMenuItem"
        GISToolStripMenuItem.Size = New Size(91, 22)
        GISToolStripMenuItem.Text = "GIS"
        ' 
        ' SpatialJoinToolStripMenuItem
        ' 
        SpatialJoinToolStripMenuItem.Name = "SpatialJoinToolStripMenuItem"
        SpatialJoinToolStripMenuItem.Size = New Size(133, 22)
        SpatialJoinToolStripMenuItem.Text = "Spatial Join"
        ' 
        ' frmKlimaatatlas
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(GroupBox1)
        Controls.Add(lblProgress)
        Controls.Add(prProgress)
        Controls.Add(btnExecute)
        Controls.Add(MenuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "frmKlimaatatlas"
        Text = "Klimaatatlas"
        WindowState = FormWindowState.Maximized
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnExecute As Button
    Friend WithEvents prProgress As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnConfigFile As Button
    Friend WithEvents txtConfigFile As TextBox
    Friend WithEvents lblConfigFile As Label
    Friend WithEvents btnDatabase As Button
    Friend WithEvents lblDatabase As Label
    Friend WithEvents txtDatabase As TextBox
    Friend WithEvents dlgOpenFile As OpenFileDialog
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GISToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SpatialJoinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label1 As Label
    Friend WithEvents btnResultsFile As Button
    Friend WithEvents txtResultsFile As TextBox
    Friend WithEvents dlgSaveFile As SaveFileDialog
End Class
