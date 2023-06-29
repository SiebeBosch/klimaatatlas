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

    Dim PenaltyField As String 'field name for the results penalty
    Dim CommentField As String 'fielt name for the results comment

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myOrder As Integer)
        Subset = New Dictionary(Of enmFieldType, List(Of String))
        Filter = New clsFilter
        Rating = New clsRating
        Scenarios = New List(Of String)
        Setup = mySetup
        Order = myOrder

        PenaltyField = "PEN_" & Order
        CommentField = "TXT_" & Order

    End Sub

    Public Function Execute() As Boolean
        'this function executes our rule


        'add two fields to our dataset if not yet present
        If Not Setup.featuresDataset.Fields.ContainsKey(PenaltyField.Trim.ToUpper) Then Setup.featuresDataset.addField(PenaltyField, enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL)  '  .Fields.Add(PenaltyField.Trim.ToUpper, New clsSQLiteField(PenaltyField, enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
        If Not Setup.featuresDataset.Fields.ContainsKey(CommentField.Trim.ToUpper) Then Setup.featuresDataset.AddField(CommentField, enmFieldType.comment, clsSQLiteField.enmSQLiteDataType.SQLITETEXT)

        'first apply the subset criteria in order to keep only the features we need
        Dim FeatureIdxList As List(Of Integer) = ApplySubsetCriteria()

        'then apply the filter in order to reduce the number of features to which this rule applies even further
        ApplyFilter(FeatureIdxList)

        'finally, to the remaining features, apply our rule!
        'first let's check which type of rule we're dealing with
        Select Case Rating.Method
            Case enmRatingMethod.constant
                'in case of a constant we'll simply apply a constant penalty to all features that pass our filter and subset criteria
                For Each featureidx As Integer In FeatureIdxList
                    Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(PenaltyField.Trim.ToUpper).fieldIdx, featureidx) = Rating.Penalty
                    Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(CommentField.Trim.ToUpper).fieldIdx, featureidx) = Rating.resultText
                Next

            Case enmRatingMethod.classification
                'in case of a classification


                For i = 0 To Setup.featuresDataset.Features.Count - 1

                Next

                For Each Feature As clsSpatialFeature In Setup.featuresDataset.Features.Values

                Next


        End Select

    End Function

    Public Function ApplySubsetCriteria() As List(Of Integer)
        'this function returns a list of featureIdx values for all features that pass the filter and subset criteria

        'let's start by adding all features from our dataset
        Dim IndexList As List(Of Integer) = InputDataset.getFeatureIndexList

        'now let's walk through the dataset and remove all features that do not meet the subset criteria
        For Each FieldType As enmFieldType In Subset.Keys
            Dim Field As clsSQLiteField = InputDataset.getFieldByType(FieldType)
            Dim ValuesList As List(Of String) = Subset.Item(FieldType)
            For Each featureidx As Integer In IndexList
                'if this feature does not contain any of the values specified in the subset, removed it from the list
                If Not ValuesList.Contains(InputDataset.Values(Field.sourceFieldIdx, featureidx)) Then IndexList.Remove(featureidx)
            Next
        Next
        Return IndexList
    End Function

    Public Function ApplyFilter(ByRef featureindexList As List(Of Integer)) As Boolean
        'this function applies the rule's filter to the dataset and adjusts the list of feature indices accordingly
        Try
            Dim Field As clsSQLiteField = InputDataset.getFieldByType(Filter.fieldType)
            Select Case Filter.evaluation
                Case Is = "="
                    'checks if the value is equal to the filter value
                    featureindexList.RemoveAll(Function(featureidx) Not InputDataset.Values(Field.fieldIdx, featureidx) = Filter.value)
            End Select

            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function ApplyFilter: " & ex.Message)
            Return False
        End Try

    End Function

End Class
