Imports System.Reflection

Public Class frmAbout
    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Retrieve the version information
        Dim version As Version = Assembly.GetExecutingAssembly().GetName().Version
        ' Format the version information as needed
        Dim versionText As String = String.Format("Version {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision)
        ' Set the version text to the label
        lblVersion.Text = versionText
    End Sub
End Class