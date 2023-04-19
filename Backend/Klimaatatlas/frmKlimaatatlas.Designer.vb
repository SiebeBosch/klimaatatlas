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
        Me.btnExecute = New System.Windows.Forms.Button()
        Me.prProgress = New System.Windows.Forms.ProgressBar()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnConfigFile = New System.Windows.Forms.Button()
        Me.txtConfigFile = New System.Windows.Forms.TextBox()
        Me.lblConfigFile = New System.Windows.Forms.Label()
        Me.btnDatabase = New System.Windows.Forms.Button()
        Me.lblDatabase = New System.Windows.Forms.Label()
        Me.txtDatabase = New System.Windows.Forms.TextBox()
        Me.dlgOpenFile = New System.Windows.Forms.OpenFileDialog()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GISToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpatialJoinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExecute
        '
        Me.btnExecute.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExecute.Location = New System.Drawing.Point(970, 650)
        Me.btnExecute.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(156, 80)
        Me.btnExecute.TabIndex = 0
        Me.btnExecute.Text = "Execute"
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'prProgress
        '
        Me.prProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prProgress.Location = New System.Drawing.Point(17, 692)
        Me.prProgress.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.prProgress.Name = "prProgress"
        Me.prProgress.Size = New System.Drawing.Size(929, 38)
        Me.prProgress.TabIndex = 1
        '
        'lblProgress
        '
        Me.lblProgress.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Location = New System.Drawing.Point(17, 650)
        Me.lblProgress.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(81, 25)
        Me.lblProgress.TabIndex = 2
        Me.lblProgress.Text = "Progress"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnConfigFile)
        Me.GroupBox1.Controls.Add(Me.txtConfigFile)
        Me.GroupBox1.Controls.Add(Me.lblConfigFile)
        Me.GroupBox1.Controls.Add(Me.btnDatabase)
        Me.GroupBox1.Controls.Add(Me.lblDatabase)
        Me.GroupBox1.Controls.Add(Me.txtDatabase)
        Me.GroupBox1.Location = New System.Drawing.Point(17, 58)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(1109, 582)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Settings"
        '
        'btnConfigFile
        '
        Me.btnConfigFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConfigFile.Location = New System.Drawing.Point(1066, 95)
        Me.btnConfigFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnConfigFile.Name = "btnConfigFile"
        Me.btnConfigFile.Size = New System.Drawing.Size(34, 40)
        Me.btnConfigFile.TabIndex = 5
        Me.btnConfigFile.Text = ".."
        Me.btnConfigFile.UseVisualStyleBackColor = True
        '
        'txtConfigFile
        '
        Me.txtConfigFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtConfigFile.Location = New System.Drawing.Point(151, 95)
        Me.txtConfigFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtConfigFile.Name = "txtConfigFile"
        Me.txtConfigFile.Size = New System.Drawing.Size(904, 31)
        Me.txtConfigFile.TabIndex = 4
        '
        'lblConfigFile
        '
        Me.lblConfigFile.AutoSize = True
        Me.lblConfigFile.Location = New System.Drawing.Point(16, 100)
        Me.lblConfigFile.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblConfigFile.Name = "lblConfigFile"
        Me.lblConfigFile.Size = New System.Drawing.Size(97, 25)
        Me.lblConfigFile.TabIndex = 3
        Me.lblConfigFile.Text = "Config file:"
        '
        'btnDatabase
        '
        Me.btnDatabase.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDatabase.Location = New System.Drawing.Point(1066, 45)
        Me.btnDatabase.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnDatabase.Name = "btnDatabase"
        Me.btnDatabase.Size = New System.Drawing.Size(34, 40)
        Me.btnDatabase.TabIndex = 2
        Me.btnDatabase.Text = ".."
        Me.btnDatabase.UseVisualStyleBackColor = True
        '
        'lblDatabase
        '
        Me.lblDatabase.AutoSize = True
        Me.lblDatabase.Location = New System.Drawing.Point(16, 50)
        Me.lblDatabase.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblDatabase.Name = "lblDatabase"
        Me.lblDatabase.Size = New System.Drawing.Size(118, 25)
        Me.lblDatabase.TabIndex = 1
        Me.lblDatabase.Text = "Database file:"
        '
        'txtDatabase
        '
        Me.txtDatabase.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDatabase.Location = New System.Drawing.Point(151, 45)
        Me.txtDatabase.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtDatabase.Name = "txtDatabase"
        Me.txtDatabase.Size = New System.Drawing.Size(904, 31)
        Me.txtDatabase.TabIndex = 0
        '
        'dlgOpenFile
        '
        Me.dlgOpenFile.FileName = "dlgOpenFile"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1143, 33)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GISToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(69, 29)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'GISToolStripMenuItem
        '
        Me.GISToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SpatialJoinToolStripMenuItem})
        Me.GISToolStripMenuItem.Name = "GISToolStripMenuItem"
        Me.GISToolStripMenuItem.Size = New System.Drawing.Size(270, 34)
        Me.GISToolStripMenuItem.Text = "GIS"
        '
        'SpatialJoinToolStripMenuItem
        '
        Me.SpatialJoinToolStripMenuItem.Name = "SpatialJoinToolStripMenuItem"
        Me.SpatialJoinToolStripMenuItem.Size = New System.Drawing.Size(270, 34)
        Me.SpatialJoinToolStripMenuItem.Text = "Spatial Join"
        '
        'frmKlimaatatlas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1143, 750)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.prProgress)
        Me.Controls.Add(Me.btnExecute)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmKlimaatatlas"
        Me.Text = "Klimaatatlas"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
End Class
