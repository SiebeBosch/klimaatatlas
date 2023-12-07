Imports Klimaatatlas.clsGeneralFunctions

Public Class clsLookupTable

    Friend id As String
    Friend Records As List(Of clsLookupTableRecord)

    Private Setup As clsKlimaatatlas
    Public Sub New(ByRef mySetup As clsKlimaatatlas, myID As String)
        id = myID
        Records = New List(Of clsLookupTableRecord)

    End Sub
    Public Function lookupValue(FeatureValue As Object, ByRef Penalty As Double, ByRef ResultText As String) As Boolean
        Try
            For Each LookupTableRecord As clsLookupTableRecord In Records
                If LookupTableRecord.Value = FeatureValue Then
                    Penalty = LookupTableRecord.Penalty
                    ResultText = LookupTableRecord.ResultText
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function lookupValue of class clsClassification: " & ex.Message)
            Return False
        End Try
    End Function


End Class
