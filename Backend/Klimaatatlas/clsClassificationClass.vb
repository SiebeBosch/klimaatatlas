Imports Klimaatatlas.clsGeneralFunctions

Public Class clsClassificationClass

    Friend Lower As Double
    Friend Upper As Double
    Friend Penalty As Double
    Friend ResultText As String

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myLower As Double, myUpper As Double, myPenalty As Double, myResultText As String)
        Lower = myLower
        Upper = myUpper
        Penalty = myPenalty
        ResultText = myResultText
    End Sub

End Class
