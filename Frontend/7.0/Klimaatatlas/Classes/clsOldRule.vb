Imports Klimaatatlas.clsGeneralFunctions
Public Class clsOldRule
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
    Friend JoinMethod As enmJoinMethod          'which method to use to join our dataset with the features dataset

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myOrder As Integer)
        Subset = New Dictionary(Of enmFieldType, List(Of String))
        Filter = New clsFilter
        Rating = New clsRating
        Scenarios = New List(Of String)
        Setup = mySetup
        Order = myOrder
    End Sub


    Public Function Execute() As Boolean

        'how to deal with this dataset depends on the join method
        If JoinMethod = enmJoinMethod.feature_centerpoint_in_polygon AndAlso Not InputDataset.ID = Setup.featuresDataset.ID Then
            'we're dealing with a polygon shapefile for which we need point-in-poly
            If Not InputDataset.OpenAndPrepareShapefile() Then Throw New Exception($"Error opening shapefile {InputDataset.path}")
        ElseIf Not InputDataset.readingCompleted Then
            InputDataset.readToDictionary()
        End If

        'only process those scenarios that are assigned to this rule
        For Each ScenarioName As String In Scenarios
            If Setup.Scenarios.ContainsKey(ScenarioName.Trim.ToUpper) Then
                'add two fields to our dataset if not yet present
                Dim scenario As clsScenario = Setup.Scenarios.Item(ScenarioName.Trim.ToUpper)
                Dim penaltyfield As String = Setup.GetPenaltyFieldName(Order, scenario.Name)
                Dim CommentField As String = Setup.GetCommentFieldName(Order, scenario.Name)

                If Not Setup.featuresDataset.Fields.ContainsKey(penaltyfield.Trim.ToUpper) Then Setup.featuresDataset.GetAddField(penaltyfield, enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL)  '  .Fields.Add(PenaltyField.Trim.ToUpper, New clsSQLiteField(PenaltyField, enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
                If Not Setup.featuresDataset.Fields.ContainsKey(CommentField.Trim.ToUpper) Then Setup.featuresDataset.GetAddField(CommentField, enmFieldType.comment, clsSQLiteField.enmSQLiteDataType.SQLITETEXT)

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
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(penaltyfield.Trim.ToUpper).fieldIdx, featureidx) = Rating.Penalty
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(CommentField.Trim.ToUpper).fieldIdx, featureidx) = Rating.resultText
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(Setup.GetRatingFieldName(scenario.Name).Trim.ToUpper).fieldIdx, featureidx) -= Rating.Penalty
                        Next

                    Case enmRatingMethod.classification
                        'in case of a classification we'll retrieve the value from our datasource and compare it with the appropriate classification table
                        Dim myField As clsSQLiteField = InputDataset.getFieldByType(Rating.FieldType)
                        Dim classification As clsClassification = Setup.Classifications.Item(Rating.classificationId.Trim.ToUpper)
                        For Each featureidx As Integer In FeatureIdxList
                            Dim featureValue As Object = InputDataset.Values(myField.fieldIdx, featureidx)

                            'look up our value in the classification table
                            Dim Penalty As Double, ResultText As String = String.Empty
                            classification.lookupValue(featureValue, Penalty, ResultText)

                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(penaltyfield.Trim.ToUpper).fieldIdx, featureidx) = Penalty
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(CommentField.Trim.ToUpper).fieldIdx, featureidx) = ResultText
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(Setup.GetRatingFieldName(scenario.Name).Trim.ToUpper).fieldIdx, featureidx) -= Penalty
                        Next

                    Case enmRatingMethod.lookup_table
                        'in case of a lookup-table we'll retrieve the value from our datasource and lookup its corresponding value from the table
                        Dim myField As clsSQLiteField = InputDataset.getFieldByType(Rating.FieldType)
                        Dim lookuptable As clsLookupTable = Setup.Lookuptables.Item(Rating.lookuptableId.Trim.ToUpper)

                        For Each FeatureDatasetfeatureidx As Integer In FeatureIdxList

                            'get our value from the inputDataset
                            Dim featureValue As Object = InputDataset.getValue(myField.fieldIdx, FeatureDatasetfeatureidx, JoinMethod)

                            'in case a data transformation is in order, apply it here
                            If Rating.ApplyDataTransformation Then featureValue = ApplyDataTransformation(featureValue, Rating.transformation_function)

                            'look up our value in the classification table
                            Dim Penalty As Double, ResultText As String = String.Empty
                            lookuptable.lookupValue(featureValue, Penalty, ResultText)

                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(penaltyfield.Trim.ToUpper).fieldIdx, FeatureDatasetfeatureidx) = Penalty
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(CommentField.Trim.ToUpper).fieldIdx, FeatureDatasetfeatureidx) = ResultText
                            Setup.featuresDataset.Values(Setup.featuresDataset.Fields.Item(Setup.GetRatingFieldName(scenario.Name).Trim.ToUpper).fieldIdx, FeatureDatasetfeatureidx) -= Penalty
                        Next
                End Select
            End If
        Next

        'how to deal with this dataset depends on the join method
        If JoinMethod = enmJoinMethod.feature_centerpoint_in_polygon AndAlso Not InputDataset.ID = Setup.featuresDataset.ID Then
            'we're dealing with a polygon shapefile for which we need point-in-poly
            InputDataset.CloseShapefileAndEndPointInShapefile()
        End If


        Return True

    End Function

    Public Function ApplyDataTransformation(featureValue As Object, transformation_function As enmTransformationFunction) As Object
        Select Case transformation_function
            Case Is = enmTransformationFunction.bod2kleizandveen
                Return Setup.Bod2KleiZandVeen(featureValue)
        End Select
    End Function

    Public Function ApplySubsetCriteria() As List(Of Integer)
        'this function returns a list of featureIdx values for all features that pass the filter and subset criteria

        'let's start by adding all features index numbers from our features dataset
        Dim IndexList As List(Of Integer) = Setup.featuresDataset.getFeatureIndexList


        'now let's walk through this rule's dataset and remove all features from our features list that do not meet the subset criteria
        For Each FieldType As enmFieldType In Subset.Keys
            Dim Field As clsSQLiteField = InputDataset.getFieldByType(FieldType)
            Dim ValuesList As List(Of String) = Subset.Item(FieldType)
            For Each featureidx As Integer In IndexList
                'if this feature does not contain any of the values specified in the subset, removed it from the list
                If Not ValuesList.Contains(InputDataset.Values(Field.fieldIdx, featureidx)) Then IndexList.Remove(featureidx)
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
