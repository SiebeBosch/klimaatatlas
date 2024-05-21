Imports System.Reflection

Public Class frmAbout
    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Retrieve the version information
        Dim version As Version = Assembly.GetExecutingAssembly().GetName().Version
        ' Format the version information as needed
        Dim versionText As String = String.Format("Version {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision)
        ' Set the version text to the label
        lblVersion.Text = versionText

        lblLink.Text = "broncode"
        ' Define the link area (start at 0th character, length of 9 characters)
        lblLink.Links.Add(0, 9, "https://github.com/SiebeBosch/klimaatatlas")
    End Sub

    Private Sub lblLink_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblLink.LinkClicked
        ' Retrieve the URL from the LinkData property and open it in the default browser
        Dim target As String = CType(e.Link.LinkData, String)
        Try
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(target) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("Unable to open the link.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
