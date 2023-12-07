Imports Klimaatatlas.clsGeneralFunctions

Public Class clsClassification

    Friend id As String
    Friend Classes As List(Of clsClassificationClass)

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myID As String)
        id = myID
        Classes = New List(Of clsClassificationClass)

    End Sub
    Public Function lookupValue(FeatureValue As Object, ByRef Penalty As Double, ByRef ResultText As String) As Boolean
        Try
            For Each ClassificationClass As clsClassificationClass In Classes
                If ClassificationClass.Lower <= FeatureValue AndAlso FeatureValue <= ClassificationClass.Upper Then
                    Penalty = ClassificationClass.Penalty
                    ResultText = ClassificationClass.ResultText
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
