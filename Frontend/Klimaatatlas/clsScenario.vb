Imports Klimaatatlas.clsGeneralFunctions
Imports MapWinGIS
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Class clsScenario
    Private Setup As clsKlimaatatlas

    Public Name As String
    Dim RatingField As String       'the name of the rating field for this scenario
    Dim CommentField As String      'the name of the comment field for this scenario

    Public Sub New(ByRef myKlimaatatlas As clsKlimaatatlas, myName As String)
        Setup = myKlimaatatlas
        Name = myName
        Dim myRatingField As String = "RAT_" & myName
        Dim myCommentField As String = "TXT_" & myName
    End Sub

    Public Function getRatingField() As String
        Return RatingField
    End Function

    Public Function getCommentField() As String
        Return CommentField
    End Function

End Class
