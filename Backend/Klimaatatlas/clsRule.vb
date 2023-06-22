Imports Klimaatatlas.clsGeneralFunctions
Public Class clsRule
    Friend Order As Integer
    Friend Apply As Boolean
    Friend Name As String
    Friend InputDataset As clsDataset
    Friend InputField As String
    Friend Subset As New Dictionary(Of enmFieldType, List(Of String)) 'key is fieldtype
    Friend Filter As New clsFilter
    Friend Rating As New clsRating
    Friend Origin As String
    Friend Scenarios As New List(Of String)

    Public Sub New()
        Subset = New Dictionary(Of enmFieldType, List(Of String))
        Filter = New clsFilter
        Rating = New clsRating
        Scenarios = New List(Of String)
    End Sub

    Public Function Execute() As Boolean
        'this function executes our rule


        'first let's check which type of rule we're dealing with
        Select Case Rating.Method
            Case enmRatingMethod.constant
                'in case of a constant we'll simply apply a constant value to all features that pass our filter and subset criteria


            Case enmRatingMethod.classification

        End Select

    End Function



End Class
