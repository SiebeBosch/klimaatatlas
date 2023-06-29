Imports Klimaatatlas.clsGeneralFunctions

Public Class clsClassification

    Friend id As String
    Friend Classes As List(Of clsClassificationClass)

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myID As String)
        id = myID
        Classes = New List(Of clsClassificationClass)

    End Sub
End Class
