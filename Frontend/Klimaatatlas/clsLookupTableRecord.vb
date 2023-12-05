Imports Klimaatatlas.clsGeneralFunctions
Imports Microsoft.VisualBasic.ApplicationServices

Public Class clsLookupTableRecord

    Friend Value As Object
    Friend Penalty As Double
    Friend ResultText As String

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myValue As Object, myPenalty As Double, myResultText As String)
        Value = myValue
        Penalty = myPenalty
        ResultText = myResultText
    End Sub

End Class
