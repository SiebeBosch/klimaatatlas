<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Klimaatatlas
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Klimaatatlas))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtExcel = New System.Windows.Forms.TextBox()
        Me.btnExcel = New System.Windows.Forms.Button()
        Me.prProgress = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(149, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Excel configuratie"
        '
        'txtExcel
        '
        Me.txtExcel.Location = New System.Drawing.Point(167, 50)
        Me.txtExcel.Name = "txtExcel"
        Me.txtExcel.Size = New System.Drawing.Size(1459, 31)
        Me.txtExcel.TabIndex = 1
        '
        'btnExcel
        '
        Me.btnExcel.Location = New System.Drawing.Point(1642, 48)
        Me.btnExcel.Name = "btnExcel"
        Me.btnExcel.Size = New System.Drawing.Size(38, 34)
        Me.btnExcel.TabIndex = 2
        Me.btnExcel.Text = ".."
        Me.btnExcel.UseVisualStyleBackColor = True
        '
        'prProgress
        '
        Me.prProgress.Location = New System.Drawing.Point(12, 643)
        Me.prProgress.Name = "prProgress"
        Me.prProgress.Size = New System.Drawing.Size(1483, 34)
        Me.prProgress.TabIndex = 3
        '
        'Klimaatatlas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1692, 701)
        Me.Controls.Add(Me.prProgress)
        Me.Controls.Add(Me.btnExcel)
        Me.Controls.Add(Me.txtExcel)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Klimaatatlas"
        Me.Text = "Klimaatatlas"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents txtExcel As TextBox
    Friend WithEvents btnExcel As Button
    Friend WithEvents prProgress As ProgressBar
End Class
