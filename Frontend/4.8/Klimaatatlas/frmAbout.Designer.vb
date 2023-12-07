<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
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
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(frmAbout))
        lblVersion = New Label()
        SuspendLayout()
        ' 
        ' lblVersion
        ' 
        lblVersion.AutoSize = True
        lblVersion.Location = New Point(350, 164)
        lblVersion.Name = "lblVersion"
        lblVersion.Size = New Size(45, 15)
        lblVersion.TabIndex = 0
        lblVersion.Text = "Version"
        ' 
        ' frmAbout
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(lblVersion)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "frmAbout"
        Text = "About"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblVersion As Label
End Class
