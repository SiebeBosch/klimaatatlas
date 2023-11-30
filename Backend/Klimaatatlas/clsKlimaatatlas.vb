Imports System.Data.SQLite
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports NCalc
Imports MathNet.Symbolics
Imports MapWinGIS
Imports System.Globalization
Imports Klimaatatlas.clsGeneralFunctions
Imports System.Data.Entity.ModelConfiguration.Conventions
Imports FParsec
Imports Klimaatatlas.clsBenchmark

Public Class clsKlimaatatlas
    Private _connString As String
    Private _config As JObject

    Public Log As clsLog
    Public SQLiteCon As SQLiteConnection
    Public Generalfunctions As New clsGeneralFunctions(Me)

    'the connection to the geopackage
    Public GpkgCon As SQLiteConnection
    Public GpkgTable As String


    Public ProgressBar As ProgressBar
    Public ProgressLabel As System.Windows.Forms.Label

    Public featuresDataset As clsDataset                        'this is the dataset containing our features
    Public Scenarios As New Dictionary(Of String, clsScenario)
    'Public Benchmarks As New Dictionary(Of String, clsBenchmark)
    Public Datasets As New Dictionary(Of String, clsDataset)
    Public Classifications As New Dictionary(Of String, clsClassification)
    Public Lookuptables As New Dictionary(Of String, clsLookupTable)
    Public OldRules As New SortedDictionary(Of Integer, clsOldRule)
    Public Rules As New SortedDictionary(Of String, clsRule)

    Public Enum enmClassificationType
        Discrete = 0
        Continuous = 1
    End Enum


    Public Sub New()
        Log = New clsLog
    End Sub

    Public Sub SetProgressBar(ByRef pr As ProgressBar, ByRef lb As System.Windows.Forms.Label)
        ProgressBar = pr
        ProgressLabel = lb
    End Sub


    Public Sub ReadConfigurationFile(jsonPath As String)
        Dim configContent As String = File.ReadAllText(jsonPath)
        _config = JObject.Parse(configContent)
    End Sub

    Public Sub SetDatabaseConnection(path As String)
        _connString = Strings.Replace("Data Source=@;Version=3;", "@", path)
        SQLiteCon = New SQLite.SQLiteConnection(_connString)
    End Sub

    Public Function UpgradeDatabase()
        UpgradeWQTIMESERIESTable(10)
        UpgradeWQNonEquidistantTimeseriesTable(20)
        UpgradeWQDERIVEDSERIESTable(50)
        UpgradeWQINDICATORSTable(90)
        UpgradeMappingTable(95)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Database successfully upgraded.", 0, 100, True)
    End Function

    Public Sub UpgradeWQTIMESERIESTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Upgrading WQTIMESERIES table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("DATASOURCE", New clsSQLiteField("DATASOURCE", enmFieldType.origin, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", enmFieldType.scenario, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("SUBSTANCE", New clsSQLiteField("SUBSTANCE", enmFieldType.parameter_name, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATEANDTIME", New clsSQLiteField("DATEANDTIME", enmFieldType.datetime, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
        CreateOrUpdateSQLiteTable(SQLiteCon, "WQTIMESERIES", Fields)
    End Sub

    Public Sub UpgradeWQDERIVEDSERIESTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Upgrading WQDERIVEDSERIES table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", enmFieldType.scenario, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("SUBSTANCE", New clsSQLiteField("SUBSTANCE", enmFieldType.parameter_name, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATEANDTIME", New clsSQLiteField("DATEANDTIME", enmFieldType.datetime, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
        CreateOrUpdateSQLiteTable(SQLiteCon, "WQDERIVEDSERIES", Fields)
    End Sub

    Public Sub UpgradeWQNonEquidistantTimeseriesTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Upgrading WQNONEQUIDISTANTSERIES table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", enmFieldType.scenario, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("SUBSTANCE", New clsSQLiteField("SUBSTANCE", enmFieldType.parameter_name, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATEANDTIME", New clsSQLiteField("DATEANDTIME", enmFieldType.datetime, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
        CreateOrUpdateSQLiteTable(SQLiteCon, "WQNONEQUIDISTANTSERIES", Fields)
    End Sub

    Public Sub UpgradeWQINDICATORSTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Upgrading INDICATORS table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", enmFieldType.scenario, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("INDICATOR", New clsSQLiteField("INDICATOR", enmFieldType.parameter_name, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL))
        CreateOrUpdateSQLiteTable(SQLiteCon, "INDICATORS", Fields)
    End Sub
    Public Sub UpgradeMappingTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Upgrading Koppeltabel...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("CODE", New clsSQLiteField("CODE", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", enmFieldType.id, clsSQLiteField.enmSQLiteDataType.SQLITETEXT))
        CreateOrUpdateSQLiteTable(SQLiteCon, "KOPPELTABEL", Fields)
    End Sub

    Public Function SetGeoPackageConnection() As Boolean
        Try
            Generalfunctions.UpdateProgressBar("Setting database connection for geopackage...", 0, 10, True)

            Dim dataset As JObject = _config("features_dataset")
            Dim Path As String = dataset("path").ToString()         'PATH TO THE GEOPACKAGE
            GpkgTable = dataset("tablename").ToString()             'the table in our geopackage containing our features and attribute data

            ' Creating a connection string
            Dim connectionString As String = $"Data Source={Path};Version=3;"

            ' Initializing and opening the connection
            GpkgCon = New SQLiteConnection(connectionString)
            GpkgCon.Open()

            ' If the connection state is open, return true
            If GpkgCon.State = ConnectionState.Open Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            ' You might want to log or display the exception message to understand the nature of the error
            Console.WriteLine(ex.Message)
            Return False

        Finally
            Generalfunctions.UpdateProgressBar("Connection to geopackage successfully established.", 0, 10, True)
        End Try
    End Function


    Public Function readFeaturesDataset() As Boolean
        Try
            'this function reads the key dataset containing our features. This is also the dataset to which our results will be written
            featuresDataset = New clsDataset(Me)

            Dim dataset As JObject = _config("features_dataset")
            featuresDataset.Comment = dataset("_comment").ToString
            featuresDataset.ID = dataset("id").ToString
            featuresDataset.dataType = CType([Enum].Parse(GetType(enmDataType), dataset("data_type").ToString), enmDataType)
            featuresDataset.storageType = CType([Enum].Parse(GetType(enmStorageType), dataset("storage_type").ToString), enmStorageType)
            If featuresDataset.storageType = enmStorageType.geopackage Then featuresDataset.tablename = dataset("tablename".ToString)
            featuresDataset.path = dataset("path").ToString
            For Each field In dataset("fields")
                Dim fieldType As enmFieldType = CType([Enum].Parse(GetType(enmFieldType), field("fieldtype").ToString()), enmFieldType)
                Dim fieldName As String = field("fieldname").ToString()
                Dim fieldDataType As enmSQLiteDataType = CType([Enum].Parse(GetType(enmSQLiteDataType), field("datatype").ToString()), enmSQLiteDataType)
                Dim newField As New clsSQLiteField(fieldName, fieldType, fieldDataType)
                If Not featuresDataset.Fields.ContainsKey(fieldName.Trim.ToUpper) Then
                    newField.fieldIdx = featuresDataset.Fields.Count
                    featuresDataset.Fields.Add(fieldName.Trim.ToUpper, newField)
                End If
            Next

            'we will read this entire dataset to memory. This allows us to perform filtering, classify and assign penalties
            featuresDataset.readToDictionary()


            Return True
        Catch ex As Exception

            Return False
        End Try

    End Function


    Public Function PopulateScenarios() As Boolean
        Try
            Dim jsonArray As JArray = CType(_config("scenarios"), JArray)
            For Each item As JToken In jsonArray
                Dim myScenario As New clsScenario(Me, item.ToString)
                Scenarios.Add(myScenario.Name.Trim.ToUpper, myScenario)
            Next
            Return True
        Catch ex As Exception
            Log.AddError("Error In Function PopulateScenarios Of Class clsKlimaatatlas:  " & ex.Message)
            Return False
        End Try
    End Function

    'Public Function PopulateBenchmarks() As Boolean
    '    Try
    '        If _config("benchmarks") Is Nothing Then
    '            Throw New InvalidOperationException("The configuration does not contain a 'benchmarks' property.")
    '        End If

    '        Dim benchmarksArray As JArray = CType(_config("benchmarks"), JArray)
    '        For Each item As JObject In benchmarksArray
    '            Dim classification As enmClassificationType = If(item("classification").ToString().ToLower() = "discrete", enmClassificationType.Discrete, enmClassificationType.Continuous)
    '            Dim myBenchmark As New clsBenchmark(Me, item("name").ToString(), item("fieldname").ToString(), classification)

    '            If classification = enmClassificationType.Discrete Then
    '                Dim discreteClasses As New Dictionary(Of String, Double)
    '                For Each classItem As JObject In item("classes")
    '                    ' Accessing properties of classItem JObject and converting them to appropriate types
    '                    discreteClasses.Add(classItem("name").ToString(), classItem("value").ToObject(Of Double)())
    '                Next
    '                myBenchmark.SetDiscreteClasses(discreteClasses)
    '            ElseIf classification = enmClassificationType.Continuous Then
    '                Dim valuesRange = New SortedDictionary(Of Double, Double)
    '                For Each classItem As JObject In item("valuesRange")
    '                    ' Accessing properties of classItem JObject and converting them to appropriate types
    '                    valuesRange.Add(classItem("value").ToObject(Of Double)(), classItem("verdict").ToObject(Of Double)())
    '                Next
    '                myBenchmark.SetContinuousClasses(valuesRange)
    '            End If

    '            Benchmarks.Add(myBenchmark.Name.Trim.ToUpper(), myBenchmark)
    '        Next
    '        Return True
    '    Catch ex As Exception
    '        Log.AddError("Error in function PopulateBenchmarks of class clsKlimaatatlas: " & ex.Message)
    '        Return False
    '    End Try
    'End Function


    Public Function PopulateDatasets() As Boolean
        Try
            For Each dataset As JObject In _config("datasets")

                Dim myDataset As New clsDataset(Me)
                myDataset.Comment = dataset("_comment").ToString
                myDataset.ID = dataset("id").ToString
                myDataset.path = dataset("path").ToString
                myDataset.dataType = CType([Enum].Parse(GetType(enmDataType), dataset("data_type").ToString), enmDataType)
                myDataset.storageType = CType([Enum].Parse(GetType(enmStorageType), dataset("storage_type").ToString), enmStorageType)
                Select Case myDataset.storageType
                    Case enmStorageType.sqlite
                        myDataset.tablename = dataset("tablename")
                End Select

                'now walk through each field that is needed for our 
                For Each field In dataset("fields")
                    Dim fieldType As enmFieldType = CType([Enum].Parse(GetType(enmFieldType), field("fieldtype").ToString()), enmFieldType)
                    Dim fieldName As String = field("fieldname").ToString()
                    Dim DataType As enmSQLiteDataType

                    Select Case fieldType
                        Case enmFieldType.datavalue
                            DataType = enmSQLiteDataType.SQLITEREAL
                        Case enmFieldType.deadend
                            DataType = enmSQLiteDataType.SQLITETEXT
                        Case enmFieldType.depth
                            DataType = enmSQLiteDataType.SQLITEREAL
                        Case enmFieldType.featureidx
                            DataType = enmSQLiteDataType.SQLITEINT
                        Case enmFieldType.origin
                            DataType = enmSQLiteDataType.SQLITETEXT
                        Case enmFieldType.parameter_name
                            DataType = enmSQLiteDataType.SQLITETEXT
                        Case enmFieldType.id
                            DataType = enmSQLiteDataType.SQLITETEXT
                        Case enmFieldType.percentile
                            DataType = enmSQLiteDataType.SQLITEREAL
                        Case enmFieldType.scenario
                            DataType = enmSQLiteDataType.SQLITETEXT
                        Case enmFieldType.soiltype
                            DataType = enmSQLiteDataType.SQLITETEXT
                    End Select

                    Dim myField As New clsSQLiteField(fieldName, fieldType, DataType)
                    If Not myDataset.Fields.ContainsKey(fieldName.Trim.ToUpper) Then
                        myField.fieldIdx = myDataset.Fields.Count
                        myDataset.Fields.Add(fieldName.Trim.ToUpper, myField)
                    End If
                Next


                Datasets.Add(myDataset.ID.Trim.ToUpper, myDataset)

            Next
            Return True
        Catch ex As Exception
            Log.AddError("Error in function PopulateDatasets of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function PopulateLookuptables() As Boolean
        Try
            For Each lookuptable As JObject In _config("lookup_tables")
                'create a new instance of clsClassification
                Dim myLookupTable As New clsLookupTable(Me, lookuptable("id").ToString)

                myLookupTable.Records = New List(Of clsLookupTableRecord)

                For Each record In lookuptable("table")
                    Dim newRecord As New clsLookupTableRecord(Me, record("input"), Convert.ToDouble(record("penalty")), record("result_text").ToString)
                    myLookupTable.Records.Add(newRecord)
                Next

                Lookuptables.Add(myLookupTable.id.Trim.ToUpper, myLookupTable)
            Next
            Return True
        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessRules of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function PopulateClassifications() As Boolean
        Try
            For Each classification As JObject In _config("classifications")
                'create a new instance of clsClassification
                Dim myClassification As New clsClassification(Me, classification("id").ToString)
                myClassification.Classes = New List(Of clsClassificationClass)

                For Each classdef In classification("classes")
                    Dim newClass As New clsClassificationClass(Me, Convert.ToDouble(classdef("lbound")), Convert.ToDouble(classdef("ubound")), Convert.ToDouble(classdef("penalty")), classdef("result_text").ToString)
                    myClassification.Classes.Add(newClass)
                Next

                Classifications.Add(myClassification.id.Trim.ToUpper, myClassification)
            Next
            Return True
        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessRules of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function
    Public Function PopulateRules(ByRef cmbRekenRegels As ComboBox) As Boolean
        Try
            'clear the combobox containing the rules
            Rules.Clear()
            cmbRekenRegels.Items.Clear()

            Dim rulesArray As JArray = CType(_config("rules"), JArray)
            For Each item As JObject In rulesArray
                Dim myRule As New clsRule(Me)
                myRule.Name = item("name").ToString()

                ' Handle benchmarks
                Dim benchmarksArray As JArray = CType(item("benchmarks"), JArray)
                For Each benchmarkItem As JObject In benchmarksArray
                    Dim classification As enmClassificationType = If(benchmarkItem("classification").ToString().ToLower() = "discrete", enmClassificationType.Discrete, enmClassificationType.Continuous)

                    ' Process fieldnames per scenario
                    Dim fieldNamesPerScenario As New Dictionary(Of String, String)
                    Dim transformationPerScenario As New Dictionary(Of String, String)
                    For Each fieldnameItem As JObject In CType(benchmarkItem("fieldname"), JArray)
                        Dim scenario As String = fieldnameItem("scenario").ToString()
                        Dim field As String = fieldnameItem("field").ToString()
                        fieldNamesPerScenario.Add(scenario.Trim.ToUpper, field)

                        Dim transformation As String = If(fieldnameItem.ContainsKey("transformation"), fieldnameItem("transformation").ToString(), String.Empty)
                        transformationPerScenario.Add(scenario.Trim.ToUpper, transformation)
                    Next

                    'create the benchmark class instance
                    Dim myBenchmark As New clsBenchmark(Me, benchmarkItem("name").ToString(), fieldNamesPerScenario, transformationPerScenario, classification)

                    'process the classification of our benchmark
                    If classification = enmClassificationType.Discrete Then
                        Dim discreteClasses As New Dictionary(Of String, Double)
                        For Each classItem As JObject In benchmarkItem("classes")
                            discreteClasses.Add(classItem("name").ToString(), classItem("value").ToObject(Of Double)())
                        Next
                        myBenchmark.SetDiscreteClasses(discreteClasses)
                    ElseIf classification = enmClassificationType.Continuous Then
                        Dim valuesRange = New SortedDictionary(Of Double, Double)
                        For Each classItem As JObject In benchmarkItem("valuesRange")
                            valuesRange.Add(classItem("value").ToObject(Of Double)(), classItem("verdict").ToObject(Of Double)())
                        Next
                        myBenchmark.SetContinuousClasses(valuesRange)
                    End If

                    ' Add the created benchmark to the current rule
                    myRule.Benchmarks.Add(myBenchmark.Name.Trim.ToUpper(), myBenchmark)
                Next

                ' Handle equation components
                Dim factors As JArray = CType(item("components"), JArray)
                For Each factor As JObject In factors
                    Dim benchmark As String = factor("benchmark").ToString()
                    Dim weight As Double = Convert.ToDouble(factor("weight"))
                    Dim resultsField As String = factor("resultsfield").ToString()

                    Dim component As New clsEquationComponent(Me, myRule, benchmark, weight, resultsField)
                    myRule.EquationComponents.Add(component)
                Next

                Rules.Add(myRule.Name.Trim.ToUpper(), myRule)
                cmbRekenRegels.Items.Add(myRule.Name)
            Next

            Return True
        Catch ex As Exception
            Log.AddError("Error in function PopulateRules of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function


    Public Function PopulateOldRules() As Boolean
        Try
            SQLiteCon.Open()
            For Each rule As JObject In _config("rules")

                'create a new instance of clsRule
                Dim myRule As New clsOldRule(Me, Convert.ToInt16(rule("order").ToString))

                'set the general properties
                myRule.Apply = Convert.ToBoolean(rule("apply"))
                myRule.Name = rule("name").ToString

                myRule.InputDataset = GetDatasetObjectByID(rule("input_dataset").ToString)

                'if our input dataset is NOT the feature dataset we must have a join method specified
                If myRule.InputDataset.ID IsNot featuresDataset.ID Then
                    If rule.ContainsKey("join_method") Then
                        myRule.JoinMethod = CType([Enum].Parse(GetType(enmJoinMethod), rule("join_method").ToString()), enmJoinMethod)
                    Else
                        Log.AddError($"No data join method specified for rule {myRule.Name}")
                        myRule.JoinMethod = enmJoinMethod.none
                    End If
                Else
                    myRule.JoinMethod = enmJoinMethod.none
                End If

                'set the subset, if specified
                myRule.Subset = New Dictionary(Of enmFieldType, List(Of String))
                If rule.ContainsKey("subset") Then
                    For Each subset In rule("subset")
                        Dim fieldType As enmFieldType = CType([Enum].Parse(GetType(enmFieldType), subset("fieldtype").ToString()), enmFieldType)

                        ' Iterate over each value in the "values" array
                        Dim vals As JArray = subset.Value(Of JArray)("values")
                        Dim Values As New List(Of String)
                        For Each value As JToken In vals
                            ' Add the value to the list
                            Values.Add(value.Value(Of String))
                        Next
                        myRule.Subset.Add(fieldType, Values)
                    Next
                End If

                'set the filter, if specified
                myRule.Filter = New clsFilter()
                If rule.ContainsKey("filter") Then
                    Dim myFilter As JObject = rule("filter")
                    myRule.Filter.fieldType = CType([Enum].Parse(GetType(enmFieldType), myFilter("field_type").ToString()), enmFieldType)
                    myRule.Filter.evaluation = myFilter("evaluation")
                    myRule.Filter.value = myFilter("value")
                End If

                'set the rule's execution methodology
                myRule.Rating = New clsRating()
                Dim myRating As JObject = rule("rating")
                myRule.Rating.Method = CType([Enum].Parse(GetType(enmRatingMethod), myRating("method").ToString()), enmRatingMethod)

                'if this Rule involves a value transformation it is defined here
                If myRating.ContainsKey("transformation_function") Then
                    myRule.Rating.ApplyDataTransformation = True
                    myRule.Rating.transformation_function = CType([Enum].Parse(GetType(enmTransformationFunction), myRating("transformation_function").ToString()), enmTransformationFunction)
                End If

                Select Case myRule.Rating.Method
                    Case enmRatingMethod.constant
                        myRule.Rating.Penalty = Convert.ToDouble(myRating("penalty"))
                        If myRating.ContainsKey("result_text") Then
                            myRule.Rating.resultText = myRating("result_text")
                        End If
                    Case enmRatingMethod.classification
                        If myRating.ContainsKey("classification_id") Then
                            myRule.Rating.classificationId = myRating("classification_id")
                        End If
                    Case enmRatingMethod.lookup_table
                        If myRating.ContainsKey("table_id") Then
                            myRule.Rating.lookuptableId = myRating("table_id")
                        End If

                End Select

                If myRating.ContainsKey("field_type") Then
                    myRule.Rating.FieldType = CType([Enum].Parse(GetType(enmFieldType), myRating("field_type").ToString()), enmFieldType)
                End If

                'set the rule's origin
                myRule.Origin = rule("origin")

                'set the rule's scenarios
                Dim scens As JArray = rule.Value(Of JArray)("scenarios")
                For Each scen As JToken In scens
                    myRule.Scenarios.Add(scen.ToString)
                Next

                'add our rule to the list
                If Rules.ContainsKey(myRule.Order) Then Throw New Exception("Error: two or more rules with the same order number in the JSON configuration file. Please correct this before proceedng.")
                OldRules.Add(myRule.Order, myRule)

            Next
            SQLiteCon.Close()
            Return True
        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessRules of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try

    End Function

    Public Function ProcessRules() As Boolean
        'this function processes all rules we have just read and computes the penalties for each feature
        Try
            Generalfunctions.UpdateProgressBar("Processing rules...", 0, 10, True)

            'execute the rules one by one
            For Each myRule As clsRule In Rules.Values()
                Log.WriteToDiagnosticsFile("Processing rule " & myRule.Name)
                myRule.Execute()
                Log.WriteToDiagnosticsFile("Rule " & myRule.Name & " processed.")
            Next

            Generalfunctions.UpdateProgressBar("Rules successfully processed.", 0, 10, True)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function ExportResultsToShapefile(path As String) As Boolean
        Try

            Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Writing results to shapefile...", 0, 10, True)
            If System.IO.File.Exists(path) Then Me.Generalfunctions.DeleteShapeFile(path)

            Dim sf As New MapWinGIS.Shapefile
            sf.CreateNew(path, ShpfileType.SHP_POLYGON)
            sf.StartEditingShapes(True)

            'add all required fields
            For i = 0 To featuresDataset.Fields.Count - 1
                Dim myFieldType As MapWinGIS.FieldType
                Dim myFieldLength As Integer
                Dim myFieldPrecision As Integer
                Dim myFieldName As String = featuresDataset.Fields.Values(i).FieldName
                Select Case featuresDataset.Fields.Values(i).DataType
                    Case clsSQLiteField.enmSQLiteDataType.SQLITEINT
                        myFieldType = MapWinGIS.FieldType.INTEGER_FIELD
                        myFieldLength = 10
                        myFieldPrecision = 0
                    Case clsSQLiteField.enmSQLiteDataType.SQLITEREAL
                        myFieldType = MapWinGIS.FieldType.DOUBLE_FIELD
                        myFieldLength = 10
                        myFieldPrecision = 2
                    Case Else
                        myFieldType = MapWinGIS.FieldType.STRING_FIELD
                        myFieldLength = 100
                        myFieldPrecision = 0
                End Select
                sf.EditAddField(myFieldName, myFieldType, myFieldPrecision, myFieldLength)
            Next

            'add the features and write the values
            For i = 0 To featuresDataset.Features.Count - 1

                Dim newShape As New MapWinGIS.Shape
                newShape.ImportFromWKT(featuresDataset.Features(i).WKT)
                sf.EditAddShape(newShape)

                For j = 0 To featuresDataset.Fields.Count - 1
                    sf.EditCellValue(j, i, featuresDataset.Values(j, i))
                Next

            Next

            Cursor.Current = Cursors.WaitCursor
            sf.StopEditingShapes(True, True)
            Cursor.Current = Cursors.Default

            Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Operation complete.", 0, 10, True)

            'MsgBox("Operation complete.")

        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function ExportResultsToGeoPackage(path As String) As Boolean
        Try
            Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Writing results to GeoPackage...", 0, 10, True)

            ' Create or connect to a GeoPackage database
            Dim connectionString As String = $"Data Source={path};Version=3;"
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()

                ' Start a transaction
                Using transaction = connection.BeginTransaction()

                    ' Create geometry table
                    Using cmd As New SQLiteCommand($"CREATE TABLE IF NOT EXISTS geometries (id INTEGER PRIMARY KEY, geom BLOB);", connection)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' Add attributes columns and handle existing columns
                    For i = 0 To featuresDataset.Fields.Count - 1
                        Dim myFieldName As String = featuresDataset.Fields.Values(i).FieldName
                        Dim myFieldType As String
                        ' Determine the field type
                        Select Case featuresDataset.Fields.Values(i).DataType
                            Case clsSQLiteField.enmSQLiteDataType.SQLITEINT
                                myFieldType = "INTEGER"
                            Case clsSQLiteField.enmSQLiteDataType.SQLITEREAL
                                myFieldType = "REAL"
                            Case Else
                                myFieldType = "TEXT"
                        End Select

                        '--------------------------------------------------------------------------------------------------------------
                        ' Check if the column already exists. If not, then add it
                        Dim columnExists As Boolean = False

                        ' Check if the column already exists
                        Using cmd As New SQLiteCommand($"PRAGMA table_info(geometries);", connection)
                            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                                While reader.Read()
                                    If String.Equals(reader("name").ToString(), myFieldName, StringComparison.OrdinalIgnoreCase) Then
                                        columnExists = True
                                        Exit While
                                    End If
                                End While
                            End Using
                        End Using

                        ' If the column doesn't exist, then add it
                        If Not columnExists Then
                            Using cmd As New SQLiteCommand($"ALTER TABLE geometries ADD COLUMN {myFieldName} {myFieldType};", connection)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                        '--------------------------------------------------------------------------------------------------------------

                    Next

                    ' Insert geometries and attributes
                    For i = 0 To featuresDataset.Features.Count - 1

                        ' Convert geometry to WKB (Well-Known Binary)
                        Dim wkb As Byte() = featuresDataset.Features(i).WKB

                        ' Building the INSERT command text
                        Dim cmdText As String = "INSERT INTO geometries (geom"
                        Dim valuesText As String = $" VALUES (@geom"
                        For j = 0 To featuresDataset.Fields.Count - 1
                            cmdText &= $", {featuresDataset.Fields.Values(j).FieldName}"
                            valuesText &= $", @{featuresDataset.Fields.Values(j).FieldName}"
                        Next
                        cmdText &= ")"
                        valuesText &= ")"

                        Using cmd As New SQLiteCommand($"{cmdText} {valuesText};", connection, transaction)
                            cmd.Parameters.AddWithValue("@geom", wkb)
                            For j = 0 To featuresDataset.Fields.Count - 1
                                cmd.Parameters.AddWithValue($"@{featuresDataset.Fields.Values(j).FieldName}", featuresDataset.Values(j, i))
                            Next
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    ' Commit the transaction
                    transaction.Commit()
                End Using
            End Using

            Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Operation complete.", 0, 10, True)

        Catch ex As Exception
            ' Handle your exception here
            Return False
        End Try

        Return True
    End Function



    Public Sub ProcessPolygonToPointMapping(ByVal rule As JObject)
        Dim polygons As JObject = GetDatasetById(rule("input")("target_dataset").ToString())
        Dim points As JObject = GetDatasetById(rule("input")("source_dataset").ToString())
        Dim koppeltabel As JObject = GetDatasetById(rule("output")("dataset").ToString())

        Dim polygonsPath As String = polygons("path").ToString()

        Dim sf As New Shapefile()
        If Not sf.Open(polygonsPath) Then
            Console.WriteLine("Error opening polygons shapefile: " & sf.ErrorMsg(sf.LastErrorCode))
            Return
        End If

        If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

        'update the progress bar
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Reading point locations from database...", 0, 10, True)

        Dim idFieldName As String = GetFieldNameByType(points, "id")
        Dim xFieldName As String = GetFieldNameByType(points, "x")
        Dim yFieldName As String = GetFieldNameByType(points, "y")

        Dim cmd As New SQLiteCommand($"SELECT DISTINCT {idFieldName}, {xFieldName}, {yFieldName} FROM {points("tablename")}", SQLiteCon)
        Dim dt As New DataTable()

        Using da As New SQLiteDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Dim results As New Dictionary(Of Integer, (x As Double, y As Double))

        For Each row As DataRow In dt.Rows
            Dim locationId As Integer = Convert.ToInt32(row(idFieldName))
            Dim x As Double = Convert.ToDouble(row(xFieldName))
            Dim y As Double = Convert.ToDouble(row(yFieldName))
            results(locationId) = (x, y)
        Next

        Using koppelCmd As New SQLiteCommand($"DELETE FROM {koppeltabel("tablename")}", SQLiteCon)
            koppelCmd.ExecuteNonQuery()
        End Using

        'update the progress bar
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Mapping delwaq segments to water surface areas...", 0, 10, True)

        Using koppelTrans = SQLiteCon.BeginTransaction()
            Using koppelCmd As New SQLiteCommand(SQLiteCon)
                koppelCmd.Transaction = koppelTrans
                koppelCmd.CommandText = $"INSERT INTO {koppeltabel("tablename")} ({koppeltabel("fields")(0)("fieldname")}, {koppeltabel("fields")(1)("fieldname")}) VALUES (@targetId, @sourceId)"

                koppelCmd.Parameters.Add("@targetId", DbType.String)
                koppelCmd.Parameters.Add("@sourceId", DbType.String)

                For i As Integer = 0 To sf.NumShapes - 1

                    'update the progress bar
                    Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "", i + 1, sf.NumShapes)

                    Dim shape As Shape = sf.Shape(i)
                    Dim center As Point = shape.Center

                    Dim nearestId As Integer = -1
                    Dim minDist As Double = Double.MaxValue

                    For Each kvp As KeyValuePair(Of Integer, (x As Double, y As Double)) In results
                        Dim dist As Double = Math.Sqrt(Math.Pow(center.x - kvp.Value.x, 2) + Math.Pow(center.y - kvp.Value.y, 2))
                        If dist < minDist Then
                            minDist = dist
                            nearestId = kvp.Key
                        End If
                    Next

                    If nearestId <> -1 Then
                        koppelCmd.Parameters("@targetId").Value = sf.CellValue(sf.Table.FieldIndexByName(polygons("fields")(0)("fieldname").ToString()), i)
                        koppelCmd.Parameters("@sourceId").Value = nearestId
                        koppelCmd.ExecuteNonQuery()
                    End If
                Next
            End Using
            koppelTrans.Commit()
        End Using

        'update the progress bar
        Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Mapping complete.", 0, 10, True)


    End Sub

    'Private Function ProcessTimeseriesTransformation(rule As JObject) As Boolean
    '    Try
    '        Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Executing timeseries transformation """ & rule("name").ToString() & """...", 0, 10, True)

    '        If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

    '        Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
    '        Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
    '        Dim equation = rule("equation").ToString()
    '        Dim outputParameterName = rule("output")("parameter_name").ToString()

    '        ' Extract variables from equation
    '        Dim VariablesList As List(Of String) = ExtractVariablesFromEquation(equation)

    '        ' Create variables to be used in query
    '        Dim variablesString As String = String.Join(",", VariablesList.Select(Function(v) $"MAX(CASE WHEN {GetFieldNameByType(inputDataset, "parameter_name")} = '{v}' THEN {GetFieldNameByType(inputDataset, "parameter_value")} ELSE NULL END) AS {v}"))

    '        ' Get all unique combinations of scenario and location ID
    '        Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

    '        ' Determine if the input dataset has "data_type" of "non-equidistant_timeseries"
    '        Dim isNonEquidistant As Boolean = inputDataset("data_type").ToString() = "non-equidistant_timeseries"

    '        ' Process each unique combination
    '        For i = 0 To UniqueSeries.Count - 1
    '            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "", i + 1, UniqueSeries.Count)

    '            Dim Scenario As String = UniqueSeries(i).Values(0)
    '            Dim LocationID As String = UniqueSeries(i).Values(1)

    '            ' Remove old results for the given location ID and scenario
    '            DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)


    '            ' Create a sorted list of all unique DateTime values in the database table for the variables involved in the equation
    '            Dim uniqueDates As New List(Of DateTime)
    '            If isNonEquidistant Then
    '                Using cmd As New SQLiteCommand(SQLiteCon)
    '                    cmd.CommandText = $"SELECT DISTINCT {GetFieldNameByType(inputDataset, "date")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' AND {GetFieldNameByType(inputDataset, "parameter_name")} IN ('{String.Join("', '", VariablesList)}') ORDER BY {GetFieldNameByType(inputDataset, "date")};"
    '                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
    '                        While reader.Read()
    '                            uniqueDates.Add(DateTime.Parse(reader.GetString(0)))
    '                        End While
    '                    End Using
    '                End Using
    '            End If

    '            ' Get data for all variables in a single query
    '            Dim dt As New DataTable
    '            Using cmd As New SQLiteCommand(SQLiteCon)
    '                cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {variablesString} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' GROUP BY {GetFieldNameByType(inputDataset, "date")} ORDER BY {GetFieldNameByType(inputDataset, "date")};"
    '                Using reader As SQLiteDataReader = cmd.ExecuteReader()
    '                    dt.Load(reader)
    '                End Using
    '            End Using


    '            ' Process the data and create resultDataTable
    '            Dim resultDataTable As New DataTable
    '            resultDataTable.Columns.Add("Date", GetType(DateTime))
    '            resultDataTable.Columns.Add("Value", GetType(Double))

    '            ' Dictionary to store the last value for each variable
    '            Dim lastValues As New Dictionary(Of String, Double)


    '            ' Debugging lines to print uniqueDates and DataTable
    '            Debug.Print("Unique Dates:")
    '            For Each d As DateTime In uniqueDates
    '                Debug.Print(d)
    '            Next
    '            Debug.Print("DataTable:")
    '            For Each r As DataRow In dt.Rows
    '                Debug.Print(r(0).ToString())
    '                For Each Variable As String In VariablesList
    '                    Debug.Print(" " & Variable & ": " & r(Variable).ToString())
    '                Next
    '                Debug.Print("")
    '            Next


    '            If isNonEquidistant Then
    '                'non-equidistant data
    '                Dim dateFieldIndex As Integer = dt.Columns.IndexOf(GetFieldNameByType(inputDataset, "date"))

    '                For Each currentDate As DateTime In uniqueDates
    '                    Dim evaluatedEquation As String = equation

    '                    For Each Variable As String In VariablesList
    '                        ' Find the row for the current variable and DateTime


    '                        Dim row As DataRow = dt.AsEnumerable().LastOrDefault(Function(r) Not DBNull.Value.Equals(r(Variable)) AndAlso r(Variable).ToString() <> "" AndAlso DateTime.Parse(r.Field(Of String)(0)) <= currentDate)

    '                        ' If a row is found, update the last value for the variable
    '                        If row IsNot Nothing Then
    '                            lastValues(Variable) = row.Field(Of Double)(Variable)
    '                        ElseIf Not lastValues.ContainsKey(Variable) Then
    '                            ' If the variable is not in the lastValues dictionary, add it with a default value of 0
    '                            lastValues(Variable) = 0
    '                        End If

    '                        ' Update the equation with the actual variable value
    '                        evaluatedEquation = evaluatedEquation.Replace("[" & Variable & "]", lastValues(Variable).ToString())
    '                    Next

    '                    Dim result As Double

    '                    Generalfunctions.EvaluateSecondDegreePolynomeExpression(evaluatedEquation, result)

    '                    resultDataTable.Rows.Add(currentDate, result)
    '                Next
    '            Else
    '                'equidistant data
    '                For Each row As DataRow In dt.Rows
    '                    Dim dateValue As DateTime = row(0)
    '                    Dim evaluatedEquation As String = equation

    '                    For Each Variable As String In VariablesList
    '                        Dim variableValue As Double
    '                        If Not DBNull.Value.Equals(row(Variable)) Then
    '                            variableValue = row(Variable)
    '                        Else
    '                            variableValue = 0 ' Assign a default value if DBNull is encountered
    '                        End If
    '                        evaluatedEquation = evaluatedEquation.Replace("[" & Variable & "]", variableValue.ToString())
    '                    Next

    '                    Dim expression As New NCalc.Expression(evaluatedEquation)
    '                    Dim result As Double = Convert.ToDouble(expression.Evaluate())
    '                    resultDataTable.Rows.Add(dateValue, result)
    '                Next
    '            End If

    '            ' Save results to the output dataset
    '            Using transaction = SQLiteCon.BeginTransaction()
    '                For Each row As DataRow In resultDataTable.Rows
    '                    Dim dateValue As DateTime = row("Date")
    '                    Dim dataValue As Double = row("Value")
    '                    SaveResultsToOutputDataset(outputDataset, Scenario, LocationID, outputParameterName, resultDataTable)
    '                Next
    '                transaction.Commit()
    '            End Using

    '        Next
    '        Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

    '        SQLiteCon.Close()
    '        Return True

    '    Catch ex As Exception
    '        Me.Log.AddError("Error in function ProcessTimeseriesTransformation of class clsKlimaatatlas: " & ex.Message)
    '        Return False
    '    End Try
    'End Function

    'Private Function ProcessTimeseriesTransformation(rule As JObject) As Boolean
    '    Try

    '        If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

    '        Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
    '        Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
    '        Dim equation = rule("equation").ToString()
    '        Dim outputParameterName = rule("output")("parameter_name").ToString()

    '        'first thing to do is to distinguish all individual timeseries from the input dataset
    '        'each combination of origin, scenario, parameter_name and id we will place in a list
    '        Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

    '        'for each combination of scenario and location ID we will now execute our equation
    '        Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Computing new timeseries " & outputParameterName, 0, 10, True)
    '        For i = 0 To UniqueSeries.Count - 1
    '            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "", i + 1, UniqueSeries.Count)

    '            Dim Scenario As String = UniqueSeries(i).Values(0)
    '            Dim LocationID As String = UniqueSeries(i).Values(1)

    '            ' Remove old results for the given location ID and scenario
    '            DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

    '            'next we need to look up which parameters are needed from our dataset in order to compute our equation
    '            Dim VariablesList As List(Of String) = ExtractVariablesFromEquation(equation)

    '            'now for each variable we will need to extract its timeseries from the database and add it as an item to a dictionary
    '            Dim Variables As New Dictionary(Of String, DataTable)

    '            For Each Variable As String In VariablesList
    '                Dim dt As New DataTable
    '                Using cmd As New SQLiteCommand(SQLiteCon)
    '                    cmd.CommandText = $"Select {GetFieldNameByType(inputDataset, "date")}, {GetFieldNameByType(inputDataset, "id")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}';"
    '                    'cmd.CommandText = $"Select {inputDataset("date")}, {inputDataset("id")} FROM {inputDataset("tablename")} WHERE {inputDataset("scenario")} = '{Scenario}' AND {inputDataset("scenario")} = '{LocationID}';"
    '                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
    '                        dt.Load(reader)
    '                    End Using
    '                End Using
    '                Variables.Add(Variable, dt)
    '            Next

    '            'now that we have populated each variable with its corresponding timeseries we can evaluate our equation and calculate the result
    '            Dim resultDataTable As New DataTable
    '            resultDataTable.Columns.Add("Date", GetType(DateTime))
    '            resultDataTable.Columns.Add("Value", GetType(Double))

    '            For rowIdx As Integer = 0 To Variables.Values(0).Rows.Count - 1
    '                Dim dateValue As DateTime = Variables.Values(0).Rows(rowIdx).Item(0)
    '                Dim evaluatedEquation As String = equation

    '                For Each Variable As String In VariablesList
    '                    Dim variableValue As Double = Variables(Variable).Rows(rowIdx).Item(1)
    '                    evaluatedEquation = evaluatedEquation.Replace("[" & Variable & "]", variableValue.ToString())
    '                Next

    '                Dim expression As New Expression(evaluatedEquation)
    '                Dim result As Double = Convert.ToDouble(expression.Evaluate())
    '                resultDataTable.Rows.Add(dateValue, result)
    '            Next

    '            Using transaction = SQLiteCon.BeginTransaction()
    '                For Each row As DataRow In resultDataTable.Rows
    '                    Dim dateValue As DateTime = row("Date")
    '                    Dim dataValue As Double = row("Value")

    '                    Dim dateFieldName As String = GetFieldNameByType(outputDataset, "date")
    '                    Dim scenarioFieldName As String = GetFieldNameByType(outputDataset, "scenario")
    '                    Dim parameterNameFieldName As String = GetFieldNameByType(outputDataset, "parameter_name")
    '                    Dim idFieldName As String = GetFieldNameByType(outputDataset, "id")
    '                    Dim parameterValueFieldName As String = GetFieldNameByType(outputDataset, "parameter_value")

    '                    Using cmd As New SQLiteCommand(SQLiteCon)
    '                        cmd.Transaction = transaction
    '                        cmd.CommandText = $"INSERT INTO {outputDataset("tablename")} ({dateFieldName}, {scenarioFieldName}, {parameterNameFieldName}, {idFieldName}, {parameterValueFieldName}) VALUES (@dateValue, @scenario, @outputParameterName, @locationID, @dataValue);"

    '                        cmd.Parameters.AddWithValue("@dateValue", dateValue)
    '                        cmd.Parameters.AddWithValue("@scenario", Scenario)
    '                        cmd.Parameters.AddWithValue("@outputParameterName", outputParameterName)
    '                        cmd.Parameters.AddWithValue("@locationID", LocationID)
    '                        cmd.Parameters.AddWithValue("@dataValue", dataValue)

    '                        cmd.ExecuteNonQuery()
    '                    End Using
    '                Next

    '                transaction.Commit()
    '            End Using

    '        Next
    '        Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

    '        SQLiteCon.Close()
    '        Return True

    '    Catch ex As Exception
    '        Me.Log.AddError("Error in function ProcessTimeseriesTransformation of class clsKlimaatatlas: " & ex.Message)
    '        Return False
    '    End Try

    'End Function

    Private Function GetFieldNameByType(Dataset As JObject, fieldType As String) As String
        For Each field As JObject In Dataset("fields")
            If field("fieldtype").ToString() = fieldType Then
                Return field("fieldname").ToString()
            End If
        Next
        Return ""
    End Function
    Private Function ProcessTimeseriesFilter_thresholdExceedance(rule As JObject) As Boolean
        Try
            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Executing timeseries filter """ & rule("name").ToString() & """...", 0, 10, True)

            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim threshold = rule("filter")("args")(0).ToObject(Of Double)()
            Dim valueTrue = rule("filter")("value_true").ToObject(Of Double)()
            Dim valueFalse = rule("filter")("value_false").ToObject(Of Double)()
            Dim inputParameterName = rule("input")("parameter_name").ToString()
            Dim outputParameterName = rule("output")("parameter_name").ToString()

            ' Get all unique combinations of scenario and location ID
            Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

            ' Process each unique combination
            For i = 0 To UniqueSeries.Count - 1
                Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "", i + 1, UniqueSeries.Count)

                Dim Scenario As String = UniqueSeries(i).Values(0)
                Dim LocationID As String = UniqueSeries(i).Values(1)

                ' Remove old results for the given location ID and scenario
                DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                ' Get data for the input parameter
                Dim inputDataTable As DataTable = GetInputData(inputDataset, Scenario, LocationID, inputParameterName)

                ' Process the data and create resultDataTable
                Dim resultDataTable As DataTable = CalculateResultDataTable(inputDataTable, threshold, valueTrue, valueFalse)

                ' Save results to the output dataset
                SaveTimeseriesToOutputDataset(outputDataset, Scenario, LocationID, outputParameterName, resultDataTable)

            Next

            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessTimeseriesFilter of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function GetInputData(inputDataset As JObject, Scenario As String, LocationID As String, inputParameterName As String) As DataTable
        Dim dt As New DataTable
        Using cmd As New SQLiteCommand(SQLiteCon)
            cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {GetFieldNameByType(inputDataset, "parameter_value")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' AND {GetFieldNameByType(inputDataset, "parameter_name")} = '{inputParameterName}' ORDER BY {GetFieldNameByType(inputDataset, "date")};"
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                dt.Load(reader)
            End Using
        End Using
        Return dt
    End Function

    Private Function CalculateResultDataTable(inputDataTable As DataTable, threshold As Double, valueTrue As Double, valueFalse As Double) As DataTable
        Dim resultDataTable As New DataTable
        resultDataTable.Columns.Add("Date", GetType(DateTime))
        resultDataTable.Columns.Add("Value", GetType(Double))

        For Each row As DataRow In inputDataTable.Rows
            Dim dateValue = row(0)
            Dim dataValue = row(1)

            Dim result As Double = If(dataValue > threshold, valueTrue, valueFalse)
            resultDataTable.Rows.Add(dateValue, result)
        Next

        Return resultDataTable
    End Function

    Private Sub SaveTimeseriesToOutputDataset(outputDataset As JObject, Scenario As String, LocationID As String, outputParameterName As String, resultDataTable As DataTable)
        ' Determine the required database fieldnames
        Dim dateFieldName As String = GetFieldNameByType(outputDataset, "date")
        Dim scenarioFieldName As String = GetFieldNameByType(outputDataset, "scenario")
        Dim parameterNameFieldName As String = GetFieldNameByType(outputDataset, "parameter_name")
        Dim idFieldName As String = GetFieldNameByType(outputDataset, "id")
        Dim parameterValueFieldName As String = GetFieldNameByType(outputDataset, "parameter_value")

        Using transaction = SQLiteCon.BeginTransaction()
            Using cmd As New SQLiteCommand(SQLiteCon)
                cmd.Transaction = transaction

                Dim lastVal As Double = Nothing
                Dim insertData As Boolean

                For i = 0 To resultDataTable.Rows.Count - 1
                    Dim row As DataRow = resultDataTable.Rows(i)
                    Dim dateValue = row("Date")
                    Dim dataValue = row("Value")
                    insertData = False

                    Dim dataType As String = outputDataset("data_type").ToString()
                    If dataType = "equidistant_timeseries" Then
                        insertData = True
                    ElseIf dataType = "non-equidistant_timeseries" AndAlso (i = 0 OrElse dataValue <> lastVal) Then
                        insertData = True
                        lastVal = dataValue
                    End If

                    If insertData Then
                        cmd.CommandText = $"INSERT INTO {outputDataset("tablename")} ({dateFieldName}, {scenarioFieldName}, {parameterNameFieldName}, {idFieldName}, {parameterValueFieldName}) VALUES (@dateValue, @scenario, @outputParameterName, @locationID, @dataValue);"

                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@dateValue", dateValue)
                        cmd.Parameters.AddWithValue("@scenario", Scenario)
                        cmd.Parameters.AddWithValue("@outputParameterName", outputParameterName)
                        cmd.Parameters.AddWithValue("@locationID", LocationID)
                        cmd.Parameters.AddWithValue("@dataValue", dataValue)

                        cmd.ExecuteNonQuery()
                    End If
                Next
            End Using

            transaction.Commit()
        End Using
    End Sub

    Private Function ProcessTimeseriesFilter_hoursThresholdExceedance(rule As JObject) As Boolean
        Try
            'update the progress bar
            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Executing timeseries filter """ & rule("name").ToString() & """...", 0, 10, True)

            'open the SQLite database
            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            'read all relevant rule aspects from our Json object
            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim duration = rule("filter")("args")(0).ToObject(Of Double)()
            Dim threshold = rule("filter")("args")(1).ToObject(Of Double)()
            Dim valueTrue = rule("filter")("value_true").ToObject(Of Double)()
            Dim valueFalse = rule("filter")("value_false").ToObject(Of Double)()
            Dim inputParameterName = rule("input")("parameter_name").ToString()
            Dim outputParameterName = rule("output")("parameter_name").ToString()

            ' Get all unique combinations of scenario and location ID
            Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

            ' Process each unique combination
            For i = 0 To UniqueSeries.Count - 1
                Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "", i + 1, UniqueSeries.Count)

                'retrieve the scenario and location ID for this combination
                Dim Scenario As String = UniqueSeries(i).Values(0)
                Dim LocationID As String = UniqueSeries(i).Values(1)

                ' Remove old results for the given location ID and scenario
                DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                ' Get data for the input parameter
                Dim inputDataTable As DataTable = GetInputData(inputDataset, Scenario, LocationID, inputParameterName)

                ' Calculate the result data table
                Dim resultDataTable As DataTable = CalculateResultHoursDataTable(inputDataTable, threshold, duration, valueTrue, valueFalse)

                ' Save results to the output dataset
                SaveResultsToOutputDataset(outputDataset, Scenario, LocationID, outputParameterName, resultDataTable)
            Next
            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessTimeseriesFilter_hoursThresholdExceedance of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function CalculateResultHoursDataTable(inputDataTable As DataTable, threshold As Double, duration As Double, valueTrue As Double, valueFalse As Double) As DataTable
        Dim resultDataTable As New DataTable
        resultDataTable.Columns.Add("Date", GetType(DateTime))
        resultDataTable.Columns.Add("Value", GetType(Double))

        Dim BlockActive As Boolean = False
        Dim BlockStartIdx As Integer = 0

        ' Process the first timestep separately
        Dim dateValue As DateTime = inputDataTable.Rows(0)(0)
        Dim dataValue As Double = inputDataTable.Rows(0)(1)
        If dataValue > threshold Then
            BlockActive = True
            BlockStartIdx = 0
        Else
            BlockActive = False
        End If

        'loop through the remaining timesteps
        For rowIndex = 1 To inputDataTable.Rows.Count - 1
            dateValue = inputDataTable.Rows(rowIndex)(0)
            dataValue = inputDataTable.Rows(rowIndex)(1)
            If dataValue > threshold Then
                If Not BlockActive Then
                    'start a new block
                    BlockActive = True
                    BlockStartIdx = rowIndex
                Else
                    'we're in an ongoing block. it will be written at the end of the block
                End If
            Else
                If BlockActive Then
                    'the active block ends here, so determine whether it meets the duration threshold and write the outcome accordingly
                    BlockActive = False
                    Dim BlockStartDate As DateTime = inputDataTable.Rows(BlockStartIdx)(0)
                    Dim BlockEndDate As DateTime = inputDataTable.Rows(rowIndex)(0)
                    Dim BlockDuration As Double = (BlockEndDate - BlockStartDate).TotalHours
                    If BlockDuration >= duration Then
                        'block exceeds minimum duration so write the valueTrue
                        For j = BlockStartIdx To rowIndex
                            Dim blockDate As DateTime = inputDataTable.Rows(j)(0)
                            resultDataTable.Rows.Add(blockDate, valueTrue)
                        Next
                    Else
                        'block was too short so write the valueFalse
                        For j = BlockStartIdx To rowIndex
                            Dim blockDate As DateTime = inputDataTable.Rows(j)(0)
                            resultDataTable.Rows.Add(blockDate, valueFalse)
                        Next
                    End If
                Else
                    'just a regular data point below threshold
                    resultDataTable.Rows.Add(dateValue, valueFalse)
                End If
            End If
        Next

        Return resultDataTable
    End Function

    Private Sub SaveResultsToOutputDataset(outputDataset As JObject, Scenario As String, LocationID As String, outputParameterName As String, resultDataTable As DataTable)
        Using transaction = SQLiteCon.BeginTransaction()
            For Each row As DataRow In resultDataTable.Rows
                Dim dateValue As DateTime = row("Date")
                Dim dataValue As Double = row("Value")

                Dim dateFieldName As String = GetFieldNameByType(outputDataset, "date")
                Dim scenarioFieldName As String = GetFieldNameByType(outputDataset, "scenario")
                Dim parameterNameFieldName As String = GetFieldNameByType(outputDataset, "parameter_name")
                Dim idFieldName As String = GetFieldNameByType(outputDataset, "id")
                Dim parameterValueFieldName As String = GetFieldNameByType(outputDataset, "parameter_value")
                Using cmd As New SQLiteCommand(SQLiteCon)
                    cmd.Transaction = transaction
                    cmd.CommandText = $"INSERT INTO {outputDataset("tablename")} ({dateFieldName}, {scenarioFieldName}, {parameterNameFieldName}, {idFieldName}, {parameterValueFieldName}) VALUES (@dateValue, @scenario, @outputParameterName, @locationID, @dataValue);"

                    cmd.Parameters.AddWithValue("@dateValue", dateValue)
                    cmd.Parameters.AddWithValue("@scenario", Scenario)
                    cmd.Parameters.AddWithValue("@outputParameterName", outputParameterName)
                    cmd.Parameters.AddWithValue("@locationID", LocationID)
                    cmd.Parameters.AddWithValue("@dataValue", dataValue)

                    cmd.ExecuteNonQuery()
                End Using
            Next

            transaction.Commit()
        End Using
    End Sub

    Private Function processTimeseriesClassification(rule As JObject) As Boolean
        Try
            Dim i As Integer, j As Integer

            'update the progress bar
            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "Executing timeseries classification """ & rule("name").ToString() & """...", 0, 10, True)

            'open the SQLite database
            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            'read all relevant rule aspects from our Json object
            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim inputParameterName = rule("input")("parameter_name").ToString()
            Dim outputParameterName = rule("output")("parameter_name").ToString()
            'Dim filterType = rule("filter")("type").ToString()
            Dim filter As JObject = rule("filter")

            Select Case filter("type")
                Case "count_hours_per_year"

                    Dim filterValue = Convert.ToDouble(rule("filter")("value"))      'the value we're looking for in order to classify its duration
                    Dim classes As JArray = filter("classes")

                    ' Get all unique combinations of scenario and location ID
                    Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)


                    Dim scenarioFieldName As String = GetFieldNameByType(outputDataset, "scenario")
                    Dim parameterNameFieldName As String = GetFieldNameByType(outputDataset, "parameter_name")
                    Dim idFieldName As String = GetFieldNameByType(outputDataset, "id")
                    Dim parameterValueFieldName As String = GetFieldNameByType(outputDataset, "parameter_value")

                    ' Process each unique combination and calculate the duration our criterium is met
                    For i = 0 To UniqueSeries.Count - 1
                        Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "", i + 1, UniqueSeries.Count)

                        'retrieve the scenario and location ID for this combination
                        Dim Scenario As String = UniqueSeries(i).Values(0)
                        Dim LocationID As String = UniqueSeries(i).Values(1)

                        ' Remove old results for the given location ID and scenario
                        DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                        ' Get the timeseries for the input parameter and current scenario and location
                        Dim dt As New DataTable
                        Using cmd As New SQLiteCommand(SQLiteCon)
                            cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {GetFieldNameByType(inputDataset, "parameter_value")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' AND {GetFieldNameByType(inputDataset, "parameter_name")} = '{inputParameterName}' ORDER BY {GetFieldNameByType(inputDataset, "date")};"
                            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                                dt.Load(reader)
                            End Using
                        End Using

                        'walk through our datatable and sum up the total duration where our value equals the requested value
                        Dim TotalHours As Double = 0
                        Dim Span As TimeSpan
                        If dt.Rows(0)(1) = filterValue Then
                            Span = Convert.ToDateTime(dt.Rows(1)(0)).Subtract(dt.Rows(0)(0))
                            TotalHours = Span.TotalHours
                        End If

                        For j = 1 To dt.Rows.Count - 1
                            If dt.Rows(j)(1) = filterValue Then
                                Span = Convert.ToDateTime(dt.Rows(j)(0)).Subtract(dt.Rows(j - 1)(0))
                                TotalHours += Span.TotalHours
                            End If
                        Next

                        'convert our number of hours to its corresponding rating
                        Dim rating As Integer = GetRating(TotalHours, classes)

                        Using outputcmd As New SQLiteCommand(SQLiteCon)
                            outputcmd.CommandText = $"INSERT INTO {outputDataset("tablename")} ({scenarioFieldName}, {parameterNameFieldName}, {idFieldName}, {parameterValueFieldName}) VALUES (@scenario, @outputParameterName, @locationID, @dataValue);"
                            outputcmd.Parameters.AddWithValue("@scenario", Scenario)
                            outputcmd.Parameters.AddWithValue("@outputParameterName", outputParameterName)
                            outputcmd.Parameters.AddWithValue("@locationID", LocationID)
                            outputcmd.Parameters.AddWithValue("@dataValue", rating)
                            outputcmd.ExecuteNonQuery()
                        End Using

                    Next

            End Select


            Me.Generalfunctions.UpdateProgressBar(ProgressBar, ProgressLabel, "New classification " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function timeseriesClassification of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function GetRating(valueToClassify As Double, classes As JArray) As Integer
        Dim rating As Integer = 0

        For Each classItem As JObject In classes
            Dim fromValue As Double = CDbl(classItem("from"))
            Dim toValue As Double

            If classItem("to").Type = JTokenType.String AndAlso CStr(classItem("to")).ToLower() = "inf" Then
                toValue = Double.PositiveInfinity
            Else
                toValue = CDbl(classItem("to"))
            End If

            If valueToClassify >= fromValue AndAlso valueToClassify < toValue Then
                rating = CInt(classItem("rating"))
                Exit For
            End If
        Next

        Return rating
    End Function




    Private Function GetVariables(expression As String) As HashSet(Of String)
        Dim variables As New HashSet(Of String)()
        Dim regex As New Regex("\[([A-Za-z]+)\]")

        For Each match As Match In regex.Matches(expression)
            variables.Add(match.Groups(1).Value)
        Next

        Return variables
    End Function



    Public Function ExtractVariablesFromEquation(ByVal equation As String) As List(Of String)
        Dim variablePattern As String = "\[(.*?)\]"
        Dim variables As New List(Of String)
        Dim matches As MatchCollection = Regex.Matches(equation, variablePattern)

        For Each match As Match In matches
            variables.Add(match.Groups(1).Value)
        Next

        Return variables
    End Function

    'Private Function ParseEquation(equation As String, row As DataRow) As Double
    '    Dim e As New Expression(equation)

    '    ' Extract variables from the expression
    '    Dim variables = GetVariables(equation)

    '    ' Replace variable names with their respective values from the DataRow
    '    For Each variableName In variables
    '        If row.Table.Columns.Contains(variableName) Then
    '            Dim variableValue = Convert.ToDouble(row(variableName))
    '            e.Parameters(variableName) = variableValue
    '        Else
    '            Throw New ArgumentException($"Variable '{variableName}' not found in the input dataset.")
    '        End If
    '    Next

    '    ' Evaluate the expression
    '    Return Convert.ToDouble(e.Evaluate())
    'End Function

    Private Function GetDatasetObjectByID(id As String) As clsDataset
        If featuresDataset.ID.Trim.ToUpper = id.Trim.ToUpper Then
            Return featuresDataset
        ElseIf Datasets.ContainsKey(id.Trim.ToUpper) Then
            Return Datasets.Item(id.Trim.ToUpper)
        Else
            Return Nothing
        End If
    End Function
    Private Function GetDatasetById(id As String) As JObject
        For Each dataset As JObject In _config("datasets")
            If dataset("id").ToString() = id Then
                Return dataset
            End If
        Next

        Throw New ArgumentException($"Dataset with ID '{id}' not found.")
    End Function
    Private Sub DeleteOldResults(outputDataset As JObject, scenario As String, locationID As String, outputParameterName As String)
        Using cmd As New SQLiteCommand(SQLiteCon)
            Dim scenarioFieldName As String = GetFieldNameByType(outputDataset, "scenario")
            Dim idFieldName As String = GetFieldNameByType(outputDataset, "id")
            Dim parameterNameFieldName As String = GetFieldNameByType(outputDataset, "parameter_name")
            cmd.CommandText = $"DELETE FROM {outputDataset("tablename")} WHERE {scenarioFieldName} = @scenario AND {idFieldName} = @locationID AND {parameterNameFieldName} = @outputParameterName;"
            cmd.Parameters.AddWithValue("@scenario", scenario)
            cmd.Parameters.AddWithValue("@locationID", locationID)
            cmd.Parameters.AddWithValue("@outputParameterName", outputParameterName)
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Public Function GetScenarioLocationCombinations(jsonObj As JObject) As List(Of Dictionary(Of String, String))

        'this function returns all combinations of scenario and location ID
        'the reason we don't iterate through the parameters as well is that the parameters will be used in the equation
        Dim tableName As String = jsonObj("tablename").ToString()
        Dim fields As JArray = jsonObj("fields")

        Dim scenarioField, idField As JObject

        For Each field As JObject In fields
            Select Case field("fieldtype").ToString()
                Case "scenario"
                    scenarioField = field
                Case "id"
                    idField = field
            End Select
        Next

        Dim scenarios, ids As New List(Of String)

        scenarios = GetDistinctValues(SQLiteCon, tableName, scenarioField)
        ids = GetDistinctValues(SQLiteCon, tableName, idField)

        Dim combinations As New List(Of Dictionary(Of String, String))

        For Each scenario In scenarios
            For Each id In ids
                Dim combination As New Dictionary(Of String, String) From {
                    {"scenario", scenario},
                    {"id", id}
                }
                combinations.Add(combination)
            Next
        Next

        Return combinations
    End Function

    Private Function GetDistinctValues(connection As SQLiteConnection, tableName As String, field As JObject) As List(Of String)
        Dim fieldName As String = field("fieldname").ToString()
        Dim query As String = "SELECT DISTINCT " & fieldName & " FROM " & tableName

        If field.ContainsKey("selection") Then
            Dim selection As JArray = field("selection")
            query &= " WHERE " & fieldName & " IN (" & String.Join(",", selection.Select(Function(s) $"'{s}'")) & ")"
        End If

        Dim values As New List(Of String)

        Using command As New SQLiteCommand(query, connection)
            Using reader As SQLiteDataReader = command.ExecuteReader()
                While reader.Read()
                    values.Add(reader.GetString(0))
                End While
            End Using
        End Using

        Return values
    End Function


    Public Function GetPenaltyFieldName(RuleOrder As Integer, ScenarioName As String) As String
        Return "PEN_" & RuleOrder & "_" & ScenarioName
    End Function
    Public Function GetCommentFieldName(RuleOrder As Integer, ScenarioName As String) As String
        Return "TXT_" & RuleOrder & "_" & ScenarioName
    End Function

    Public Function GetRatingFieldName(ScenarioName As String) As String
        Return ScenarioName
    End Function

    Public Function SetAndInitializeRatingFields() As Boolean
        'creates fields in the featuresdataset for our results for the given scenarios, if not yet existing
        'for now the name of each field is the same as the name of the given scenario
        Try
            For Each Scenario As clsScenario In Scenarios.Values
                Dim ScenarioName As String = Scenario.Name
                'add two fields to our dataset if not yet present
                Dim RatingField As clsSQLiteField = Nothing
                If Not featuresDataset.Fields.ContainsKey(ScenarioName.Trim.ToUpper) Then
                    RatingField = featuresDataset.GetAddField(ScenarioName, enmFieldType.datavalue, clsSQLiteField.enmSQLiteDataType.SQLITEREAL)
                Else
                    RatingField = featuresDataset.Fields.Item(ScenarioName.Trim.ToUpper)
                End If

                'initialize our rating to a 10
                For featureidx = 0 To featuresDataset.Features.Count - 1
                    featuresDataset.Values(RatingField.fieldIdx, featureidx) = 100
                Next
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function Bod2Capsim(EERSTE_BOD As String) As String

        'converteert bodemtypes uit de Bodemkaart Nederland naar het corresponderende CAPSIM bodemnummer in SOBEK
        'Veengronden: code V
        'Moerige gronden: code W
        'Podzolgronden: code H en Y
        'BrikGronden: code B
        'Dikke eerdgronden: code EZ EL en EK
        'Kalkloze zandgronden: code Z
        'Kalkhoudende zandgronden: code Z...A
        'Kalkhoudende bijzonder lutumarme gronden: code S...A
        'Niet gerijpte minerale gronden: code MO-zeeklei, RO-rivierklei
        'Zeekleigronden: code M
        'Rivierkleigronden: code R
        'Oude rivierkleigronden: code KR
        'Leemgronden: code L
        'Mariene afzettingen ouder dan Pleistoceen: code MA, MK, MZ
        'Fluviatiele afzttingen ouder dan Pleistoceen: code FG, FK
        'Kalksteenverweringsgronden: code KM, KK, KS
        'Ondiepe keileemgronden: code KX
        'Overige oude kleigronden: code KT
        'Grindgronden: code G
        EERSTE_BOD = Generalfunctions.ParseString(EERSTE_BOD, "/")
        If EERSTE_BOD = "" Then Return 101
        If EERSTE_BOD = "|a GROEVE" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|b AFGRAV" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|c OPHOOG" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|d EGAL" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|e VERWERK" Then Return 113 'willekeurig bedenksel
        If EERSTE_BOD = "|f TERP" Then Return 116 'lichte klei, klopt wel enigszins met omgeving
        If EERSTE_BOD = "|g MOERAS" Then Return 101 'veengrond ligt voor de hand
        If EERSTE_BOD = "|g WATER" Then Return 101 'tsja
        If EERSTE_BOD = "|h BEBOUW" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|h DIJK" Then Return 119 'klei op zand aangenomen
        If EERSTE_BOD = "|i BOVLAND" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "|j MYNSTRT" Then Return 108 'podzol=leemarm grof zand
        If EERSTE_BOD = "AAKp" Then Return 119
        If EERSTE_BOD = "AAP" Then Return 105
        If EERSTE_BOD = "ABk" Then Return 119
        If EERSTE_BOD = "ABkt" Then Return 119
        If EERSTE_BOD = "ABl" Then Return 121
        If EERSTE_BOD = "ABv" Then Return 105
        If EERSTE_BOD = "ABvg" Then Return 105
        If EERSTE_BOD = "ABvt" Then Return 105
        If EERSTE_BOD = "ABvx" Then Return 105
        If EERSTE_BOD = "ABz" Then Return 113
        If EERSTE_BOD = "ABzt" Then Return 113
        If EERSTE_BOD = "AEk9" Then Return 116
        If EERSTE_BOD = "AEm5" Then Return 115
        If EERSTE_BOD = "AEm8" Then Return 116
        If EERSTE_BOD = "AEm9A" Then Return 116
        If EERSTE_BOD = "AEp6A" Then Return 116
        If EERSTE_BOD = "AEp7A" Then Return 116
        If EERSTE_BOD = "AFz" Then Return 113
        If EERSTE_BOD = "Aha" Then Return 121
        If EERSTE_BOD = "AHc" Then Return 121
        If EERSTE_BOD = "AHk" Then Return 121
        If EERSTE_BOD = "AHl" Then Return 121
        If EERSTE_BOD = "Ahs" Then Return 121
        If EERSTE_BOD = "AHt" Then Return 121
        If EERSTE_BOD = "AHv" Then Return 121
        If EERSTE_BOD = "AHz" Then Return 121
        If EERSTE_BOD = "AK" Then Return 119
        If EERSTE_BOD = "AKp" Then Return 119
        If EERSTE_BOD = "ALu" Then Return 116
        If EERSTE_BOD = "AM" Then Return 119
        If EERSTE_BOD = "AMm" Then Return 115
        If EERSTE_BOD = "AO" Then Return 119
        If EERSTE_BOD = "AOg" Then Return 119
        If EERSTE_BOD = "AOp" Then Return 119
        If EERSTE_BOD = "AOv" Then Return 119
        If EERSTE_BOD = "AP" Then Return 101
        If EERSTE_BOD = "App" Then Return 102
        If EERSTE_BOD = "AQ" Then Return 107
        If EERSTE_BOD = "AR" Then Return 119
        If EERSTE_BOD = "AS" Then Return 107
        If EERSTE_BOD = "aVc" Then Return 101
        If EERSTE_BOD = "AVk" Then Return 105
        If EERSTE_BOD = "AVo" Then Return 101
        If EERSTE_BOD = "aVp" Then Return 102
        If EERSTE_BOD = "aVpg" Then Return 102
        If EERSTE_BOD = "aVpx" Then Return 102
        If EERSTE_BOD = "aVs" Then Return 101
        If EERSTE_BOD = "aVz" Then Return 102
        If EERSTE_BOD = "aVzt" Then Return 102
        If EERSTE_BOD = "aVzx" Then Return 102
        If EERSTE_BOD = "AWg" Then Return 116
        If EERSTE_BOD = "AWo" Then Return 106
        If EERSTE_BOD = "AWv" Then Return 106
        If EERSTE_BOD = "AZ1" Then Return 114
        If EERSTE_BOD = "AZW0A" Then Return 107
        If EERSTE_BOD = "AZW0Al" Then Return 107
        If EERSTE_BOD = "AZW0Av" Then Return 107
        If EERSTE_BOD = "AZW1A" Then Return 119
        If EERSTE_BOD = "AZW1Ar" Then Return 119
        If EERSTE_BOD = "AZW1Aw" Then Return 119
        If EERSTE_BOD = "AZW5A" Then Return 119
        If EERSTE_BOD = "AZW6A" Then Return 119
        If EERSTE_BOD = "AZW6Al" Then Return 116
        If EERSTE_BOD = "AZW6Alv" Then Return 118
        If EERSTE_BOD = "AZW7Al" Then Return 116
        If EERSTE_BOD = "AZW7Alw" Then Return 116
        If EERSTE_BOD = "AZW7Alwp" Then Return 119
        If EERSTE_BOD = "AZW8A" Then Return 116
        If EERSTE_BOD = "AZW8Al" Then Return 116
        If EERSTE_BOD = "AZW8Alw" Then Return 116
        If EERSTE_BOD = "bEZ21" Then Return 112
        If EERSTE_BOD = "bEZ21g" Then Return 112
        If EERSTE_BOD = "bEZ21x" Then Return 112
        If EERSTE_BOD = "bEZ23" Then Return 112
        If EERSTE_BOD = "bEZ23g" Then Return 112
        If EERSTE_BOD = "bEZ23t" Then Return 112
        If EERSTE_BOD = "bEZ23x" Then Return 112
        If EERSTE_BOD = "bEZ30" Then Return 112
        If EERSTE_BOD = "bEZ30x" Then Return 112
        If EERSTE_BOD = "bgMn15C" Then Return 115
        If EERSTE_BOD = "bgMn25C" Then Return 115
        If EERSTE_BOD = "bgMn53C" Then Return 117
        If EERSTE_BOD = "BKd25" Then Return 115
        If EERSTE_BOD = "BKd25x" Then Return 115
        If EERSTE_BOD = "BKd26" Then Return 115
        If EERSTE_BOD = "BKh25" Then Return 115
        If EERSTE_BOD = "BKh25x" Then Return 115
        If EERSTE_BOD = "BKh26" Then Return 115
        If EERSTE_BOD = "BKh26x" Then Return 115
        If EERSTE_BOD = "BLb6" Then Return 121
        If EERSTE_BOD = "BLb6g" Then Return 121
        If EERSTE_BOD = "BLb6k" Then Return 121
        If EERSTE_BOD = "BLb6s" Then Return 121
        If EERSTE_BOD = "BLd5" Then Return 121
        If EERSTE_BOD = "BLd5g" Then Return 121
        If EERSTE_BOD = "BLd5t" Then Return 121
        If EERSTE_BOD = "BLd6" Then Return 121
        If EERSTE_BOD = "BLd6m" Then Return 121
        If EERSTE_BOD = "BLh5m" Then Return 121
        If EERSTE_BOD = "BLh6" Then Return 121
        If EERSTE_BOD = "BLh6g" Then Return 121
        If EERSTE_BOD = "BLh6m" Then Return 121
        If EERSTE_BOD = "BLh6s" Then Return 121
        If EERSTE_BOD = "BLn5m" Then Return 121
        If EERSTE_BOD = "BLn5t" Then Return 121
        If EERSTE_BOD = "BLn6" Then Return 121
        If EERSTE_BOD = "BLn6g" Then Return 121
        If EERSTE_BOD = "BLn6m" Then Return 121
        If EERSTE_BOD = "BLn6s" Then Return 121
        If EERSTE_BOD = "bMn15A" Then Return 115
        If EERSTE_BOD = "bMn15C" Then Return 115
        If EERSTE_BOD = "bMn25A" Then Return 115
        If EERSTE_BOD = "bMn25C" Then Return 115
        If EERSTE_BOD = "bMn35A" Then Return 116
        If EERSTE_BOD = "bMn45A" Then Return 117
        If EERSTE_BOD = "bMn56Cp" Then Return 119
        If EERSTE_BOD = "bMn85C" Then Return 116
        If EERSTE_BOD = "bMn86C" Then Return 117
        If EERSTE_BOD = "bRn46C" Then Return 117
        If EERSTE_BOD = "BZd23" Then Return 113
        If EERSTE_BOD = "BZd24" Then Return 113
        If EERSTE_BOD = "cHd21" Then Return 108
        If EERSTE_BOD = "cHd21g" Then Return 110
        If EERSTE_BOD = "cHd21x" Then Return 111
        If EERSTE_BOD = "cHd23" Then Return 113
        If EERSTE_BOD = "cHd23x" Then Return 111
        If EERSTE_BOD = "cHd30" Then Return 114
        If EERSTE_BOD = "cHn21" Then Return 109
        If EERSTE_BOD = "cHn21g" Then Return 110
        If EERSTE_BOD = "cHn21t" Then Return 111
        If EERSTE_BOD = "cHn21w" Then Return 111
        If EERSTE_BOD = "cHn21x" Then Return 111
        If EERSTE_BOD = "cHn23" Then Return 113
        If EERSTE_BOD = "cHn23g" Then Return 110
        If EERSTE_BOD = "cHn23t" Then Return 111
        If EERSTE_BOD = "cHn23wx" Then Return 111
        If EERSTE_BOD = "cHn23x" Then Return 111
        If EERSTE_BOD = "cHn30" Then Return 114
        If EERSTE_BOD = "cHn30g" Then Return 114
        If EERSTE_BOD = "cY21" Then Return 109
        If EERSTE_BOD = "cY21g" Then Return 110
        If EERSTE_BOD = "cY21x" Then Return 111
        If EERSTE_BOD = "cY23" Then Return 113
        If EERSTE_BOD = "cY23g" Then Return 113
        If EERSTE_BOD = "cY23x" Then Return 111
        If EERSTE_BOD = "cY30" Then Return 114
        If EERSTE_BOD = "cY30g" Then Return 114
        If EERSTE_BOD = "cZd21" Then Return 108
        If EERSTE_BOD = "cZd21g" Then Return 110
        If EERSTE_BOD = "cZd23" Then Return 113
        If EERSTE_BOD = "cZd30" Then Return 114
        If EERSTE_BOD = "dgMn58Cv" Then Return 117
        If EERSTE_BOD = "dgMn83C" Then Return 117
        If EERSTE_BOD = "dgMn88Cv" Then Return 117
        If EERSTE_BOD = "dhVb" Then Return 101
        If EERSTE_BOD = "dhVk" Then Return 106
        If EERSTE_BOD = "dhVr" Then Return 101
        If EERSTE_BOD = "dkVc" Then Return 103
        If EERSTE_BOD = "dMn86C" Then Return 117
        If EERSTE_BOD = "dMv41C" Then Return 118
        If EERSTE_BOD = "dMv61C" Then Return 118
        If EERSTE_BOD = "dpVc" Then Return 103
        If EERSTE_BOD = "dVc" Then Return 101
        If EERSTE_BOD = "dVd" Then Return 101
        If EERSTE_BOD = "dVk" Then Return 106
        If EERSTE_BOD = "dVr" Then Return 101
        If EERSTE_BOD = "dWo" Then Return 106
        If EERSTE_BOD = "dWol" Then Return 106
        If EERSTE_BOD = "EK19" Then Return 115
        If EERSTE_BOD = "EK19p" Then Return 119
        If EERSTE_BOD = "EK19x" Then Return 115
        If EERSTE_BOD = "EK76" Then Return 117
        If EERSTE_BOD = "EK79" Then Return 116
        If EERSTE_BOD = "EK79v" Then Return 116
        If EERSTE_BOD = "EK79w" Then Return 116
        If EERSTE_BOD = "EL5" Then Return 121
        If EERSTE_BOD = "eMn12Ap" Then Return 119
        If EERSTE_BOD = "eMn15A" Then Return 115
        If EERSTE_BOD = "eMn15Ap" Then Return 119
        If EERSTE_BOD = "eMn22A" Then Return 119
        If EERSTE_BOD = "eMn22Ap" Then Return 119
        If EERSTE_BOD = "eMn25A" Then Return 115
        If EERSTE_BOD = "eMn25Ap" Then Return 119
        If EERSTE_BOD = "eMn25Av" Then Return 118
        If EERSTE_BOD = "eMn35A" Then Return 116
        If EERSTE_BOD = "eMn35Ap" Then Return 119
        If EERSTE_BOD = "eMn35Av" Then Return 118
        If EERSTE_BOD = "eMn35Awp" Then Return 119
        If EERSTE_BOD = "eMn45A" Then Return 117
        If EERSTE_BOD = "eMn45Ap" Then Return 117
        If EERSTE_BOD = "eMn45Av" Then Return 118
        If EERSTE_BOD = "eMn52Cg" Then Return 119
        If EERSTE_BOD = "eMn52Cp" Then Return 119
        If EERSTE_BOD = "eMn52Cwp" Then Return 119
        If EERSTE_BOD = "eMn56Av" Then Return 118
        If EERSTE_BOD = "eMn82A" Then Return 119
        If EERSTE_BOD = "eMn82Ap" Then Return 119
        If EERSTE_BOD = "eMn82C" Then Return 119
        If EERSTE_BOD = "eMn82Cp" Then Return 119
        If EERSTE_BOD = "eMn86A" Then Return 117
        If EERSTE_BOD = "eMn86Av" Then Return 118
        If EERSTE_BOD = "eMn86C" Then Return 117
        If EERSTE_BOD = "eMn86Cv" Then Return 118
        If EERSTE_BOD = "eMn86Cw" Then Return 117
        If EERSTE_BOD = "eMo20A" Then Return 119
        If EERSTE_BOD = "eMo20Ap" Then Return 119
        If EERSTE_BOD = "eMo80A" Then Return 116
        If EERSTE_BOD = "eMo80Ap" Then Return 119
        If EERSTE_BOD = "eMo80C" Then Return 116
        If EERSTE_BOD = "eMo80Cv" Then Return 118
        If EERSTE_BOD = "eMOb72" Then Return 119
        If EERSTE_BOD = "eMOb75" Then Return 116
        If EERSTE_BOD = "eMOo05" Then Return 115
        If EERSTE_BOD = "eMv41C" Then Return 118
        If EERSTE_BOD = "eMv51A" Then Return 118
        If EERSTE_BOD = "eMv61C" Then Return 118
        If EERSTE_BOD = "eMv61Cp" Then Return 118
        If EERSTE_BOD = "eMv81A" Then Return 118
        If EERSTE_BOD = "eMv81Ap" Then Return 118
        If EERSTE_BOD = "epMn55A" Then Return 115
        If EERSTE_BOD = "epMn85A" Then Return 116
        If EERSTE_BOD = "epMo50" Then Return 115
        If EERSTE_BOD = "epMo80" Then Return 116
        If EERSTE_BOD = "epMv81" Then Return 118
        If EERSTE_BOD = "epRn56" Then Return 117
        If EERSTE_BOD = "epRn59" Then Return 119
        If EERSTE_BOD = "epRn86" Then Return 117
        If EERSTE_BOD = "eRn45A" Then Return 117
        If EERSTE_BOD = "eRn46A" Then Return 117
        If EERSTE_BOD = "eRn46Av" Then Return 118
        If EERSTE_BOD = "eRn47C" Then Return 117
        If EERSTE_BOD = "eRn52A" Then Return 119
        If EERSTE_BOD = "eRn66A" Then Return 117
        If EERSTE_BOD = "eRn66Av" Then Return 118
        If EERSTE_BOD = "eRn82A" Then Return 119
        If EERSTE_BOD = "eRn94C" Then Return 117
        If EERSTE_BOD = "eRn95A" Then Return 116
        If EERSTE_BOD = "eRn95Av" Then Return 118
        If EERSTE_BOD = "eRo40A" Then Return 117
        If EERSTE_BOD = "eRv01A" Then Return 118
        If EERSTE_BOD = "eRv01C" Then Return 118
        If EERSTE_BOD = "EZ50A" Then Return 107
        If EERSTE_BOD = "EZ50Av" Then Return 107
        If EERSTE_BOD = "EZg21" Then Return 112
        If EERSTE_BOD = "EZg21g" Then Return 112
        If EERSTE_BOD = "EZg21v" Then Return 112
        If EERSTE_BOD = "EZg21w" Then Return 112
        If EERSTE_BOD = "EZg23" Then Return 112
        If EERSTE_BOD = "EZg23g" Then Return 112
        If EERSTE_BOD = "EZg23t" Then Return 112
        If EERSTE_BOD = "EZg23tw" Then Return 112
        If EERSTE_BOD = "EZg23w" Then Return 112
        If EERSTE_BOD = "EZg23wg" Then Return 112
        If EERSTE_BOD = "EZg23wt" Then Return 112
        If EERSTE_BOD = "EZg30" Then Return 112
        If EERSTE_BOD = "EZg30g" Then Return 112
        If EERSTE_BOD = "EZg30v" Then Return 112
        If EERSTE_BOD = "fABk" Then Return 119
        If EERSTE_BOD = "fAFk" Then Return 119
        If EERSTE_BOD = "fAFz" Then Return 113
        If EERSTE_BOD = "faVc" Then Return 101
        If EERSTE_BOD = "faVz" Then Return 102
        If EERSTE_BOD = "faVzt" Then Return 102
        If EERSTE_BOD = "FG" Then Return 114
        If EERSTE_BOD = "fHn21" Then Return 109
        If EERSTE_BOD = "fhVc" Then Return 101
        If EERSTE_BOD = "fhVd" Then Return 101
        If EERSTE_BOD = "fhVz" Then Return 102
        If EERSTE_BOD = "fiVc" Then Return 105
        If EERSTE_BOD = "fiVz" Then Return 105
        If EERSTE_BOD = "fiWp" Then Return 105
        If EERSTE_BOD = "fiWz" Then Return 105
        If EERSTE_BOD = "FKk" Then Return 121
        If EERSTE_BOD = "fkpZg23" Then Return 119
        If EERSTE_BOD = "fkpZg23g" Then Return 120
        If EERSTE_BOD = "fkpZg23t" Then Return 119
        If EERSTE_BOD = "fKRn1" Then Return 119
        If EERSTE_BOD = "fKRn1g" Then Return 120
        If EERSTE_BOD = "fKRn2g" Then Return 120
        If EERSTE_BOD = "fKRn8" Then Return 119
        If EERSTE_BOD = "fKRn8g" Then Return 120
        If EERSTE_BOD = "fkVc" Then Return 103
        If EERSTE_BOD = "fkVs" Then Return 103
        If EERSTE_BOD = "fkVz" Then Return 104
        If EERSTE_BOD = "fkWz" Then Return 104
        If EERSTE_BOD = "fkWzg" Then Return 104
        If EERSTE_BOD = "fkZn21" Then Return 119
        If EERSTE_BOD = "fkZn23" Then Return 119
        If EERSTE_BOD = "fkZn23g" Then Return 120
        If EERSTE_BOD = "fkZn30" Then Return 120
        If EERSTE_BOD = "fMn56Cp" Then Return 119
        If EERSTE_BOD = "fMn56Cv" Then Return 118
        If EERSTE_BOD = "fpLn5" Then Return 121
        If EERSTE_BOD = "fpRn59" Then Return 119
        If EERSTE_BOD = "fpRn86" Then Return 117
        If EERSTE_BOD = "fpVc" Then Return 103
        If EERSTE_BOD = "fpVs" Then Return 103
        If EERSTE_BOD = "fpVz" Then Return 104
        If EERSTE_BOD = "fpZg21" Then Return 109
        If EERSTE_BOD = "fpZg21g" Then Return 110
        If EERSTE_BOD = "fpZg23" Then Return 113
        If EERSTE_BOD = "fpZg23g" Then Return 113
        If EERSTE_BOD = "fpZg23t" Then Return 111
        If EERSTE_BOD = "fpZg23x" Then Return 111
        If EERSTE_BOD = "fpZn21" Then Return 109
        If EERSTE_BOD = "fpZn23tg" Then Return 111
        If EERSTE_BOD = "fRn15C" Then Return 115
        If EERSTE_BOD = "fRn62C" Then Return 119
        If EERSTE_BOD = "fRn62Cg" Then Return 120
        If EERSTE_BOD = "fRn95C" Then Return 116
        If EERSTE_BOD = "fRo60C" Then Return 116
        If EERSTE_BOD = "fRv01C" Then Return 118
        If EERSTE_BOD = "fVc" Then Return 101
        If EERSTE_BOD = "fvWz" Then Return 102
        If EERSTE_BOD = "fvWzt" Then Return 102
        If EERSTE_BOD = "fvWztx" Then Return 102
        If EERSTE_BOD = "fVz" Then Return 102
        If EERSTE_BOD = "fZn21" Then Return 107
        If EERSTE_BOD = "fZn21g" Then Return 107
        If EERSTE_BOD = "fZn23" Then Return 113
        If EERSTE_BOD = "fZn23-F" Then Return 113
        If EERSTE_BOD = "fZn23g" Then Return 113
        If EERSTE_BOD = "fzVc" Then Return 105
        If EERSTE_BOD = "fzVz" Then Return 105
        If EERSTE_BOD = "fzVzt" Then Return 105
        If EERSTE_BOD = "fzWp" Then Return 105
        If EERSTE_BOD = "fzWz" Then Return 105
        If EERSTE_BOD = "fzWzt" Then Return 105
        If EERSTE_BOD = "gbEZ21" Then Return 112
        If EERSTE_BOD = "gbEZ30" Then Return 112
        If EERSTE_BOD = "gcHd30" Then Return 114
        If EERSTE_BOD = "gcHn21" Then Return 109
        If EERSTE_BOD = "gcHn30" Then Return 114
        If EERSTE_BOD = "gcY21" Then Return 109
        If EERSTE_BOD = "gcY23" Then Return 113
        If EERSTE_BOD = "gcY30" Then Return 114
        If EERSTE_BOD = "gcZd30" Then Return 114
        If EERSTE_BOD = "gHd21" Then Return 108
        If EERSTE_BOD = "gHd30" Then Return 114
        If EERSTE_BOD = "gHn21" Then Return 109
        If EERSTE_BOD = "gHn21t" Then Return 111
        If EERSTE_BOD = "gHn21x" Then Return 111
        If EERSTE_BOD = "gHn23" Then Return 113
        If EERSTE_BOD = "gHn23x" Then Return 111
        If EERSTE_BOD = "gHn30" Then Return 114
        If EERSTE_BOD = "gHn30t" Then Return 114
        If EERSTE_BOD = "gHn30x" Then Return 114
        If EERSTE_BOD = "gKRd1" Then Return 119
        If EERSTE_BOD = "gKRd7" Then Return 119
        If EERSTE_BOD = "gKRn1" Then Return 119
        If EERSTE_BOD = "gKRn2" Then Return 119
        If EERSTE_BOD = "gLd6" Then Return 121
        If EERSTE_BOD = "gLh6" Then Return 121
        If EERSTE_BOD = "gMK" Then Return 115
        If EERSTE_BOD = "gMn15C" Then Return 115
        If EERSTE_BOD = "gMn25C" Then Return 115
        If EERSTE_BOD = "gMn25Cv" Then Return 115
        If EERSTE_BOD = "gMn52C" Then Return 119
        If EERSTE_BOD = "gMn52Cp" Then Return 119
        If EERSTE_BOD = "gMn52Cw" Then Return 119
        If EERSTE_BOD = "gMn53C" Then Return 117
        If EERSTE_BOD = "gMn53Cp" Then Return 119
        If EERSTE_BOD = "gMn53Cpx" Then Return 119
        If EERSTE_BOD = "gMn53Cv" Then Return 118
        If EERSTE_BOD = "gMn53Cw" Then Return 117
        If EERSTE_BOD = "gMn53Cwp" Then Return 119
        If EERSTE_BOD = "gMn58C" Then Return 117
        If EERSTE_BOD = "gMn58Cv" Then Return 117
        If EERSTE_BOD = "nkZn50A" Then Return 119
        If EERSTE_BOD = "gMn82C" Then Return 119
        If EERSTE_BOD = "gMn83C" Then Return 117
        If EERSTE_BOD = "gMn83Cp" Then Return 117
        If EERSTE_BOD = "gMn83Cv" Then Return 118
        If EERSTE_BOD = "gMn83Cw" Then Return 117
        If EERSTE_BOD = "gMn83Cwp" Then Return 117
        If EERSTE_BOD = "gMn85C" Then Return 116
        If EERSTE_BOD = "gMn85Cv" Then Return 118
        If EERSTE_BOD = "gMn85Cwl" Then Return 116
        If EERSTE_BOD = "gMn88C" Then Return 117
        If EERSTE_BOD = "gMn88Cl" Then Return 117
        If EERSTE_BOD = "gMn88Clv" Then Return 118
        If EERSTE_BOD = "gMn88Cv" Then Return 118
        If EERSTE_BOD = "gMn88Cw" Then Return 117
        If EERSTE_BOD = "gpZg23x" Then Return 111
        If EERSTE_BOD = "gpZg30" Then Return 114
        If EERSTE_BOD = "gpZn21" Then Return 109
        If EERSTE_BOD = "gpZn21x" Then Return 111
        If EERSTE_BOD = "gpZn23x" Then Return 111
        If EERSTE_BOD = "gpZn30" Then Return 114
        If EERSTE_BOD = "gRd10A" Then Return 119
        If EERSTE_BOD = "gRn15A" Then Return 119
        If EERSTE_BOD = "gRn94Cv" Then Return 117
        If EERSTE_BOD = "gtZd30" Then Return 114
        If EERSTE_BOD = "gvWp" Then Return 102
        If EERSTE_BOD = "gY21" Then Return 109
        If EERSTE_BOD = "gY21g" Then Return 109
        If EERSTE_BOD = "gY23" Then Return 113
        If EERSTE_BOD = "gY30" Then Return 114
        If EERSTE_BOD = "gY30-F" Then Return 114
        If EERSTE_BOD = "gY30-G" Then Return 114
        If EERSTE_BOD = "gZb30" Then Return 114
        If EERSTE_BOD = "gZd21" Then Return 107
        If EERSTE_BOD = "gZd30" Then Return 114
        If EERSTE_BOD = "gzEZ21" Then Return 112
        If EERSTE_BOD = "gzEZ23" Then Return 112
        If EERSTE_BOD = "gzEZ30" Then Return 112
        If EERSTE_BOD = "gZn30" Then Return 114
        If EERSTE_BOD = "Hd21" Then Return 108
        If EERSTE_BOD = "Hd21g" Then Return 108
        If EERSTE_BOD = "Hd21x" Then Return 108
        If EERSTE_BOD = "Hd23" Then Return 113
        If EERSTE_BOD = "Hd23g" Then Return 110
        If EERSTE_BOD = "Hd23x" Then Return 111
        If EERSTE_BOD = "Hd30" Then Return 114
        If EERSTE_BOD = "Hd30g" Then Return 114
        If EERSTE_BOD = "hEV" Then Return 101
        If EERSTE_BOD = "Hn21" Then Return 109
        If EERSTE_BOD = "Hn21-F" Then Return 109
        If EERSTE_BOD = "Hn21g" Then Return 110
        If EERSTE_BOD = "Hn21gx" Then Return 110
        If EERSTE_BOD = "Hn21t" Then Return 111
        If EERSTE_BOD = "Hn21v" Then Return 109
        If EERSTE_BOD = "Hn21w" Then Return 109
        If EERSTE_BOD = "Hn21wg" Then Return 109
        If EERSTE_BOD = "Hn21x" Then Return 111
        If EERSTE_BOD = "Hn21x-F" Then Return 111
        If EERSTE_BOD = "Hn21xg" Then Return 111
        If EERSTE_BOD = "Hn23" Then Return 113
        If EERSTE_BOD = "Hn23-F" Then Return 113
        If EERSTE_BOD = "Hn23g" Then Return 110
        If EERSTE_BOD = "Hn23t" Then Return 111
        If EERSTE_BOD = "Hn23x" Then Return 111
        If EERSTE_BOD = "Hn23x-F" Then Return 111
        If EERSTE_BOD = "Hn23xg" Then Return 111
        If EERSTE_BOD = "Hn30" Then Return 114
        If EERSTE_BOD = "Hn30g" Then Return 114
        If EERSTE_BOD = "Hn30x" Then Return 114
        If EERSTE_BOD = "hRd10A" Then Return 119
        If EERSTE_BOD = "hRd10C" Then Return 119
        If EERSTE_BOD = "hRd90A" Then Return 116
        If EERSTE_BOD = "hVb" Then Return 101
        If EERSTE_BOD = "hVc" Then Return 101
        If EERSTE_BOD = "hVcc" Then Return 101
        If EERSTE_BOD = "hVd" Then Return 101
        If EERSTE_BOD = "hVk" Then Return 106
        If EERSTE_BOD = "hVkl" Then Return 106
        If EERSTE_BOD = "hVr" Then Return 101
        If EERSTE_BOD = "hVs" Then Return 101
        If EERSTE_BOD = "hVsc" Then Return 101
        If EERSTE_BOD = "hVz" Then Return 102
        If EERSTE_BOD = "hVzc" Then Return 102
        If EERSTE_BOD = "hVzg" Then Return 102
        If EERSTE_BOD = "hVzx" Then Return 102
        If EERSTE_BOD = "hZd20A" Then Return 107
        If EERSTE_BOD = "iVc" Then Return 105
        If EERSTE_BOD = "iVp" Then Return 105
        If EERSTE_BOD = "iVpc" Then Return 105
        If EERSTE_BOD = "iVpg" Then Return 105
        If EERSTE_BOD = "iVpt" Then Return 105
        If EERSTE_BOD = "iVpx" Then Return 105
        If EERSTE_BOD = "iVs" Then Return 105
        If EERSTE_BOD = "iVz" Then Return 105
        If EERSTE_BOD = "iVzg" Then Return 105
        If EERSTE_BOD = "iVzt" Then Return 105
        If EERSTE_BOD = "iVzx" Then Return 105
        If EERSTE_BOD = "iWp" Then Return 105
        If EERSTE_BOD = "iWpc" Then Return 105
        If EERSTE_BOD = "iWpg" Then Return 105
        If EERSTE_BOD = "iWpt" Then Return 105
        If EERSTE_BOD = "iWpx" Then Return 105
        If EERSTE_BOD = "iWz" Then Return 105
        If EERSTE_BOD = "iWzt" Then Return 105
        If EERSTE_BOD = "iWzx" Then Return 105
        If EERSTE_BOD = "kcHn21" Then Return 119
        If EERSTE_BOD = "kgpZg30" Then Return 120
        If EERSTE_BOD = "kHn21" Then Return 119
        If EERSTE_BOD = "kHn21g" Then Return 120
        If EERSTE_BOD = "kHn21x" Then Return 119
        If EERSTE_BOD = "kHn23" Then Return 119
        If EERSTE_BOD = "kHn23x" Then Return 119
        If EERSTE_BOD = "kHn30" Then Return 120
        If EERSTE_BOD = "KK" Then Return 121
        If EERSTE_BOD = "KM" Then Return 121
        If EERSTE_BOD = "kMn43C" Then Return 117
        If EERSTE_BOD = "kMn43Cp" Then Return 117
        If EERSTE_BOD = "kMn43Cpx" Then Return 117
        If EERSTE_BOD = "kMn43Cv" Then Return 118
        If EERSTE_BOD = "kMn43Cwp" Then Return 117
        If EERSTE_BOD = "kMn48C" Then Return 117
        If EERSTE_BOD = "kMn48Cl" Then Return 117
        If EERSTE_BOD = "kMn48Clv" Then Return 118
        If EERSTE_BOD = "kMn48Cv" Then Return 118
        If EERSTE_BOD = "kMn48Cvl" Then Return 118
        If EERSTE_BOD = "kMn48Cw" Then Return 117
        If EERSTE_BOD = "kMn63C" Then Return 117
        If EERSTE_BOD = "kMn63Cp" Then Return 119
        If EERSTE_BOD = "kMn63Cpx" Then Return 119
        If EERSTE_BOD = "kMn63Cv" Then Return 118
        If EERSTE_BOD = "kMn63Cwp" Then Return 119
        If EERSTE_BOD = "kMn68C" Then Return 117
        If EERSTE_BOD = "kMn68Cl" Then Return 117
        If EERSTE_BOD = "kMn68Cv" Then Return 118
        If EERSTE_BOD = "kpZg20A" Then Return 119
        If EERSTE_BOD = "kpZg21" Then Return 119
        If EERSTE_BOD = "kpZg21g" Then Return 120
        If EERSTE_BOD = "kpZg23" Then Return 119
        If EERSTE_BOD = "kpZg23g" Then Return 120
        If EERSTE_BOD = "kpZg23t" Then Return 119
        If EERSTE_BOD = "kpZg23x" Then Return 119
        If EERSTE_BOD = "kpZn21" Then Return 119
        If EERSTE_BOD = "kpZn21g" Then Return 120
        If EERSTE_BOD = "kpZn23" Then Return 119
        If EERSTE_BOD = "kpZn23x" Then Return 119
        If EERSTE_BOD = "KRd1" Then Return 119
        If EERSTE_BOD = "KRd1g" Then Return 120
        If EERSTE_BOD = "KRd7" Then Return 119
        If EERSTE_BOD = "KRd7g" Then Return 120
        If EERSTE_BOD = "KRn1" Then Return 119
        If EERSTE_BOD = "KRn1g" Then Return 120
        If EERSTE_BOD = "KRn2" Then Return 119
        If EERSTE_BOD = "KRn2g" Then Return 120
        If EERSTE_BOD = "KRn2w" Then Return 119
        If EERSTE_BOD = "KRn8" Then Return 119
        If EERSTE_BOD = "KRn8g" Then Return 120
        If EERSTE_BOD = "KS" Then Return 115
        If EERSTE_BOD = "kSn13A" Then Return 119
        If EERSTE_BOD = "kSn13Av" Then Return 119
        If EERSTE_BOD = "kSn13Aw" Then Return 119
        If EERSTE_BOD = "kSn14A" Then Return 119
        If EERSTE_BOD = "kSn14Ap" Then Return 119
        If EERSTE_BOD = "kSn14Av" Then Return 119
        If EERSTE_BOD = "kSn14Aw" Then Return 119
        If EERSTE_BOD = "kSn14Awp" Then Return 119
        If EERSTE_BOD = "KT" Then Return 115
        If EERSTE_BOD = "kVb" Then Return 103
        If EERSTE_BOD = "kVc" Then Return 103
        If EERSTE_BOD = "kVcc" Then Return 103
        If EERSTE_BOD = "kVd" Then Return 103
        If EERSTE_BOD = "kVk" Then Return 106
        If EERSTE_BOD = "kVr" Then Return 103
        If EERSTE_BOD = "kVs" Then Return 103
        If EERSTE_BOD = "kVsc" Then Return 103
        If EERSTE_BOD = "kVz" Then Return 104
        If EERSTE_BOD = "kVzc" Then Return 104
        If EERSTE_BOD = "kVzx" Then Return 104
        If EERSTE_BOD = "kWp" Then Return 104
        If EERSTE_BOD = "kWpg" Then Return 104
        If EERSTE_BOD = "kWpx" Then Return 104
        If EERSTE_BOD = "kWz" Then Return 104
        If EERSTE_BOD = "kWzg" Then Return 104
        If EERSTE_BOD = "kWzx" Then Return 104
        If EERSTE_BOD = "KX" Then Return 115
        If EERSTE_BOD = "kZb21" Then Return 119
        If EERSTE_BOD = "kZb23" Then Return 119
        If EERSTE_BOD = "kZn10A" Then Return 119
        If EERSTE_BOD = "kZn10Av" Then Return 119
        If EERSTE_BOD = "kZn21" Then Return 119
        If EERSTE_BOD = "kZn21g" Then Return 120
        If EERSTE_BOD = "kZn21p" Then Return 119
        If EERSTE_BOD = "kZn21r" Then Return 119
        If EERSTE_BOD = "kZn21w" Then Return 119
        If EERSTE_BOD = "kZn21x" Then Return 119
        If EERSTE_BOD = "kZn23" Then Return 119
        If EERSTE_BOD = "kZn30" Then Return 120
        If EERSTE_BOD = "kZn30A" Then Return 120
        If EERSTE_BOD = "kZn30Ar" Then Return 120
        If EERSTE_BOD = "kZn30x" Then Return 120
        If EERSTE_BOD = "kZn40A" Then Return 119
        If EERSTE_BOD = "kZn40Ap" Then Return 119
        If EERSTE_BOD = "kZn40Av" Then Return 119
        If EERSTE_BOD = "kZn50A" Then Return 119
        If EERSTE_BOD = "kZn50Ap" Then Return 119
        If EERSTE_BOD = "kZn50Ar" Then Return 119
        If EERSTE_BOD = "Ld5" Then Return 121
        If EERSTE_BOD = "Ld5g" Then Return 121
        If EERSTE_BOD = "Ld5m" Then Return 121
        If EERSTE_BOD = "Ld5t" Then Return 121
        If EERSTE_BOD = "Ld6" Then Return 121
        If EERSTE_BOD = "Ld6a" Then Return 121
        If EERSTE_BOD = "Ld6g" Then Return 121
        If EERSTE_BOD = "Ld6k" Then Return 121
        If EERSTE_BOD = "Ld6m" Then Return 121
        If EERSTE_BOD = "Ld6s" Then Return 121
        If EERSTE_BOD = "Ld6t" Then Return 121
        If EERSTE_BOD = "Ldd5" Then Return 121
        If EERSTE_BOD = "Ldd5g" Then Return 121
        If EERSTE_BOD = "Ldd6" Then Return 121
        If EERSTE_BOD = "Ldh5" Then Return 121
        If EERSTE_BOD = "Ldh5g" Then Return 121
        If EERSTE_BOD = "Ldh5t" Then Return 121
        If EERSTE_BOD = "Ldh6" Then Return 121
        If EERSTE_BOD = "Ldh6m" Then Return 121
        If EERSTE_BOD = "lFG" Then Return 114
        If EERSTE_BOD = "lFK" Then Return 121
        If EERSTE_BOD = "lFKk" Then Return 121
        If EERSTE_BOD = "Lh5" Then Return 121
        If EERSTE_BOD = "Lh5g" Then Return 121
        If EERSTE_BOD = "Lh6g" Then Return 121
        If EERSTE_BOD = "Lh6s" Then Return 121
        If EERSTE_BOD = "lKK" Then Return 116
        If EERSTE_BOD = "lKM" Then Return 116
        If EERSTE_BOD = "lKRd7" Then Return 119
        If EERSTE_BOD = "lKS" Then Return 121
        If EERSTE_BOD = "Ln5" Then Return 121
        If EERSTE_BOD = "Ln5g" Then Return 121
        If EERSTE_BOD = "Ln5m" Then Return 121
        If EERSTE_BOD = "Ln5t" Then Return 121
        If EERSTE_BOD = "Ln6a" Then Return 121
        If EERSTE_BOD = "Ln6m" Then Return 121
        If EERSTE_BOD = "Ln6t" Then Return 121
        If EERSTE_BOD = "Lnd5" Then Return 121
        If EERSTE_BOD = "Lnd5g" Then Return 121
        If EERSTE_BOD = "Lnd5m" Then Return 121
        If EERSTE_BOD = "Lnd5t" Then Return 121
        If EERSTE_BOD = "Lnd6" Then Return 121
        If EERSTE_BOD = "Lnd6v" Then Return 121
        If EERSTE_BOD = "Lnh6" Then Return 121
        If EERSTE_BOD = "MA" Then Return 116
        If EERSTE_BOD = "mcY23" Then Return 113
        If EERSTE_BOD = "mcY23x" Then Return 111
        If EERSTE_BOD = "mHd23" Then Return 113
        If EERSTE_BOD = "mHn21x" Then Return 111
        If EERSTE_BOD = "mHn23x" Then Return 111
        If EERSTE_BOD = "MK" Then Return 116
        If EERSTE_BOD = "mKK" Then Return 116
        If EERSTE_BOD = "mKRd7" Then Return 119
        If EERSTE_BOD = "mKX" Then Return 115
        If EERSTE_BOD = "mLd6s" Then Return 121
        If EERSTE_BOD = "mLh6s" Then Return 121
        If EERSTE_BOD = "Mn12A" Then Return 119
        If EERSTE_BOD = "Mn12Ap" Then Return 119
        If EERSTE_BOD = "Mn12Av" Then Return 119
        If EERSTE_BOD = "Mn12Awp" Then Return 119
        If EERSTE_BOD = "Mn15A" Then Return 115
        If EERSTE_BOD = "Mn15Ap" Then Return 119
        If EERSTE_BOD = "Mn15Av" Then Return 118
        If EERSTE_BOD = "Mn15Aw" Then Return 115
        If EERSTE_BOD = "Mn15Awp" Then Return 119
        If EERSTE_BOD = "Mn15C" Then Return 115
        If EERSTE_BOD = "Mn15Clv" Then Return 118
        If EERSTE_BOD = "Mn15Cv" Then Return 118
        If EERSTE_BOD = "Mn15Cw" Then Return 115
        If EERSTE_BOD = "Mn22A" Then Return 119
        If EERSTE_BOD = "Mn22Alv" Then Return 115
        If EERSTE_BOD = "Mn22Ap" Then Return 119
        If EERSTE_BOD = "Mn22Av" Then Return 115
        If EERSTE_BOD = "Mn22Aw" Then Return 119
        If EERSTE_BOD = "Mn22Awp" Then Return 119
        If EERSTE_BOD = "Mn22Ax" Then Return 119
        If EERSTE_BOD = "Mn25A" Then Return 115
        If EERSTE_BOD = "Mn25Alv" Then Return 115
        If EERSTE_BOD = "Mn25Ap" Then Return 119
        If EERSTE_BOD = "Mn25Av" Then Return 118
        If EERSTE_BOD = "Mn25Aw" Then Return 115
        If EERSTE_BOD = "Mn25Awp" Then Return 119
        If EERSTE_BOD = "Mn25C" Then Return 115
        If EERSTE_BOD = "Mn25Cp" Then Return 119
        If EERSTE_BOD = "Mn25Cv" Then Return 118
        If EERSTE_BOD = "Mn25Cw" Then Return 115
        If EERSTE_BOD = "Mn35A" Then Return 116
        If EERSTE_BOD = "Mn35Ap" Then Return 119
        If EERSTE_BOD = "Mn35Av" Then Return 118
        If EERSTE_BOD = "Mn35Aw" Then Return 116
        If EERSTE_BOD = "Mn35Awp" Then Return 119
        If EERSTE_BOD = "Mn35Ax" Then Return 116
        If EERSTE_BOD = "Mn45A" Then Return 117
        If EERSTE_BOD = "Mn45Ap" Then Return 119
        If EERSTE_BOD = "Mn45Av" Then Return 118
        If EERSTE_BOD = "Mn52C" Then Return 119
        If EERSTE_BOD = "Mn52Cp" Then Return 119
        If EERSTE_BOD = "Mn52Cpx" Then Return 119
        If EERSTE_BOD = "Mn52Cwp" Then Return 119
        If EERSTE_BOD = "Mn52Cx" Then Return 119
        If EERSTE_BOD = "Mn56A" Then Return 117
        If EERSTE_BOD = "Mn56Ap" Then Return 119
        If EERSTE_BOD = "Mn56Av" Then Return 118
        If EERSTE_BOD = "Mn56Aw" Then Return 117
        If EERSTE_BOD = "Mn56C" Then Return 117
        If EERSTE_BOD = "Mn56Cp" Then Return 119
        If EERSTE_BOD = "Mn56Cv" Then Return 118
        If EERSTE_BOD = "Mn56Cwp" Then Return 119
        If EERSTE_BOD = "Mn82A" Then Return 119
        If EERSTE_BOD = "Mn82Ap" Then Return 119
        If EERSTE_BOD = "Mn82C" Then Return 119
        If EERSTE_BOD = "Mn82Cp" Then Return 119
        If EERSTE_BOD = "Mn82Cpx" Then Return 119
        If EERSTE_BOD = "Mn82Cwp" Then Return 119
        If EERSTE_BOD = "Mn85C" Then Return 116
        If EERSTE_BOD = "Mn85Clwp" Then Return 119
        If EERSTE_BOD = "Mn85Cp" Then Return 119
        If EERSTE_BOD = "Mn85Cv" Then Return 118
        If EERSTE_BOD = "Mn85Cw" Then Return 116
        If EERSTE_BOD = "Mn85Cwp" Then Return 119
        If EERSTE_BOD = "Mn86A" Then Return 117
        If EERSTE_BOD = "Mn86Al" Then Return 117
        If EERSTE_BOD = "Mn86Av" Then Return 118
        If EERSTE_BOD = "Mn86Aw" Then Return 117
        If EERSTE_BOD = "Mn86C" Then Return 117
        If EERSTE_BOD = "Mn86Cl" Then Return 117
        If EERSTE_BOD = "Mn86Clv" Then Return 117
        If EERSTE_BOD = "Mn86Clw" Then Return 117
        If EERSTE_BOD = "Mn86Clwp" Then Return 119
        If EERSTE_BOD = "Mn86Cp" Then Return 119
        If EERSTE_BOD = "Mn86Cv" Then Return 118
        If EERSTE_BOD = "Mn86Cw" Then Return 117
        If EERSTE_BOD = "Mn86Cwp" Then Return 119
        If EERSTE_BOD = "Mo10A" Then Return 115
        If EERSTE_BOD = "Mo10Av" Then Return 115
        If EERSTE_BOD = "Mo20A" Then Return 115
        If EERSTE_BOD = "Mo20Av" Then Return 115
        If EERSTE_BOD = "Mo50C" Then Return 115
        If EERSTE_BOD = "Mo80A" Then Return 116
        If EERSTE_BOD = "Mo80Ap" Then Return 119
        If EERSTE_BOD = "Mo80Av" Then Return 118
        If EERSTE_BOD = "Mo80C" Then Return 116
        If EERSTE_BOD = "Mo80Cl" Then Return 116
        If EERSTE_BOD = "Mo80Cp" Then Return 119
        If EERSTE_BOD = "Mo80Cv" Then Return 118
        If EERSTE_BOD = "Mo80Cvl" Then Return 118
        If EERSTE_BOD = "Mo80Cw" Then Return 116
        If EERSTE_BOD = "Mo80Cwp" Then Return 119
        If EERSTE_BOD = "MOb12" Then Return 119
        If EERSTE_BOD = "MOb15" Then Return 115
        If EERSTE_BOD = "MOb72" Then Return 119
        If EERSTE_BOD = "MOb75" Then Return 116
        If EERSTE_BOD = "MOo02" Then Return 119
        If EERSTE_BOD = "MOo02v" Then Return 119
        If EERSTE_BOD = "MOo05" Then Return 115
        If EERSTE_BOD = "Mv41C" Then Return 118
        If EERSTE_BOD = "Mv41Cl" Then Return 118
        If EERSTE_BOD = "Mv41Cp" Then Return 118
        If EERSTE_BOD = "Mv41Cv" Then Return 118
        If EERSTE_BOD = "Mv51A" Then Return 118
        If EERSTE_BOD = "Mv51Al" Then Return 118
        If EERSTE_BOD = "Mv51Ap" Then Return 118
        If EERSTE_BOD = "Mv61C" Then Return 118
        If EERSTE_BOD = "Mv61Cl" Then Return 118
        If EERSTE_BOD = "Mv61Cp" Then Return 118
        If EERSTE_BOD = "Mv81A" Then Return 118
        If EERSTE_BOD = "Mv81Al" Then Return 118
        If EERSTE_BOD = "Mv81Ap" Then Return 118
        If EERSTE_BOD = "mY23" Then Return 113
        If EERSTE_BOD = "mY23x" Then Return 111
        If EERSTE_BOD = "mZb23x" Then Return 111
        If EERSTE_BOD = "MZk" Then Return 121
        If EERSTE_BOD = "MZz" Then Return 107
        If EERSTE_BOD = "nAO" Then Return 119
        If EERSTE_BOD = "nkZn21" Then Return 119
        If EERSTE_BOD = "nkZn50Ab" Then Return 119
        If EERSTE_BOD = "nMn15A" Then Return 115
        If EERSTE_BOD = "nMn15Av" Then Return 115
        If EERSTE_BOD = "nMo10A" Then Return 115
        If EERSTE_BOD = "nMo10Av" Then Return 118
        If EERSTE_BOD = "nMo80A" Then Return 116
        If EERSTE_BOD = "nMo80Aw" Then Return 116
        If EERSTE_BOD = "nMv61C" Then Return 118
        If EERSTE_BOD = "npMo50l" Then Return 115
        If EERSTE_BOD = "npMo80l" Then Return 116
        If EERSTE_BOD = "nSn13A" Then Return 113
        If EERSTE_BOD = "nSn13Av" Then Return 113
        If EERSTE_BOD = "nvWz" Then Return 102
        If EERSTE_BOD = "nZn21" Then Return 107
        If EERSTE_BOD = "nZn40A" Then Return 107
        If EERSTE_BOD = "nZn50A" Then Return 107
        If EERSTE_BOD = "nZn50Ab" Then Return 107
        If EERSTE_BOD = "ohVb" Then Return 101
        If EERSTE_BOD = "ohVc" Then Return 101
        If EERSTE_BOD = "ohVk" Then Return 106
        If EERSTE_BOD = "ohVs" Then Return 101
        If EERSTE_BOD = "opVb" Then Return 103
        If EERSTE_BOD = "opVc" Then Return 103
        If EERSTE_BOD = "opVk" Then Return 106
        If EERSTE_BOD = "opVs" Then Return 103
        If EERSTE_BOD = "pKRn1" Then Return 119
        If EERSTE_BOD = "pKRn1g" Then Return 120
        If EERSTE_BOD = "pKRn2" Then Return 119
        If EERSTE_BOD = "pKRn2g" Then Return 120
        If EERSTE_BOD = "pLn5" Then Return 121
        If EERSTE_BOD = "pLn5g" Then Return 121
        If EERSTE_BOD = "pMn52A" Then Return 119
        If EERSTE_BOD = "pMn52C" Then Return 119
        If EERSTE_BOD = "pMn52Cp" Then Return 119
        If EERSTE_BOD = "pMn55A" Then Return 115
        If EERSTE_BOD = "pMn55Av" Then Return 118
        If EERSTE_BOD = "pMn55Aw" Then Return 115
        If EERSTE_BOD = "pMn55C" Then Return 115
        If EERSTE_BOD = "pMn55Cp" Then Return 119
        If EERSTE_BOD = "pMn56C" Then Return 117
        If EERSTE_BOD = "pMn56Cl" Then Return 117
        If EERSTE_BOD = "pMn82A" Then Return 119
        If EERSTE_BOD = "pMn82C" Then Return 119
        If EERSTE_BOD = "pMn85A" Then Return 116
        If EERSTE_BOD = "pMn85Aw" Then Return 116
        If EERSTE_BOD = "pMn85C" Then Return 116
        If EERSTE_BOD = "pMn85Cv" Then Return 118
        If EERSTE_BOD = "pMn86C" Then Return 117
        If EERSTE_BOD = "pMn86Cl" Then Return 117
        If EERSTE_BOD = "pMn86Cv" Then Return 118
        If EERSTE_BOD = "pMn86Cw" Then Return 117
        If EERSTE_BOD = "pMn86Cwl" Then Return 117
        If EERSTE_BOD = "pMo50" Then Return 115
        If EERSTE_BOD = "pMo50l" Then Return 115
        If EERSTE_BOD = "pMo50w" Then Return 115
        If EERSTE_BOD = "pMo80" Then Return 116
        If EERSTE_BOD = "pMo80l" Then Return 116
        If EERSTE_BOD = "pMo80v" Then Return 118
        If EERSTE_BOD = "pMv51" Then Return 118
        If EERSTE_BOD = "pMv81" Then Return 118
        If EERSTE_BOD = "pMv81l" Then Return 118
        If EERSTE_BOD = "pMv81p" Then Return 118
        If EERSTE_BOD = "pRn56p" Then Return 119
        If EERSTE_BOD = "pRn56v" Then Return 118
        If EERSTE_BOD = "pRn56wp" Then Return 119
        If EERSTE_BOD = "pRn59" Then Return 119
        If EERSTE_BOD = "pRn59p" Then Return 119
        If EERSTE_BOD = "pRn59t" Then Return 119
        If EERSTE_BOD = "pRn59w" Then Return 119
        If EERSTE_BOD = "pRn86" Then Return 117
        If EERSTE_BOD = "pRn86p" Then Return 119
        If EERSTE_BOD = "pRn86t" Then Return 117
        If EERSTE_BOD = "pRn86v" Then Return 118
        If EERSTE_BOD = "pRn86w" Then Return 117
        If EERSTE_BOD = "pRn86wp" Then Return 119
        If EERSTE_BOD = "pRn89v" Then Return 118
        If EERSTE_BOD = "pRv81" Then Return 118
        If EERSTE_BOD = "pVb" Then Return 103
        If EERSTE_BOD = "pVc" Then Return 103
        If EERSTE_BOD = "pVcc" Then Return 103
        If EERSTE_BOD = "pVd" Then Return 103
        If EERSTE_BOD = "pVk" Then Return 106
        If EERSTE_BOD = "pVr" Then Return 103
        If EERSTE_BOD = "pVs" Then Return 103
        If EERSTE_BOD = "pVsc" Then Return 103
        If EERSTE_BOD = "pVsl" Then Return 103
        If EERSTE_BOD = "pVz" Then Return 104
        If EERSTE_BOD = "pVzx" Then Return 104
        If EERSTE_BOD = "pZg20A" Then Return 107
        If EERSTE_BOD = "pZg20Ar" Then Return 107
        If EERSTE_BOD = "pZg21" Then Return 109
        If EERSTE_BOD = "pZg21g" Then Return 110
        If EERSTE_BOD = "pZg21r" Then Return 111
        If EERSTE_BOD = "pZg21t" Then Return 111
        If EERSTE_BOD = "pZg21w" Then Return 109
        If EERSTE_BOD = "pZg21x" Then Return 111
        If EERSTE_BOD = "pZg23" Then Return 113
        If EERSTE_BOD = "pZg23g" Then Return 113
        If EERSTE_BOD = "pZg23r" Then Return 113
        If EERSTE_BOD = "pZg23t" Then Return 111
        If EERSTE_BOD = "pZg23w" Then Return 113
        If EERSTE_BOD = "pZg23x" Then Return 111
        If EERSTE_BOD = "pZg30" Then Return 114
        If EERSTE_BOD = "pZg30p" Then Return 114
        If EERSTE_BOD = "pZg30r" Then Return 114
        If EERSTE_BOD = "pZg30x" Then Return 114
        If EERSTE_BOD = "pZn21" Then Return 109
        If EERSTE_BOD = "pZn21g" Then Return 110
        If EERSTE_BOD = "pZn21t" Then Return 111
        If EERSTE_BOD = "pZn21tg" Then Return 109
        If EERSTE_BOD = "pZn21v" Then Return 109
        If EERSTE_BOD = "pZn21x" Then Return 111
        If EERSTE_BOD = "pZn23" Then Return 113
        If EERSTE_BOD = "pZn23g" Then Return 110
        If EERSTE_BOD = "pZn23gx" Then Return 110
        If EERSTE_BOD = "pZn23t" Then Return 111
        If EERSTE_BOD = "pZn23v" Then Return 113
        If EERSTE_BOD = "pZn23w" Then Return 113
        If EERSTE_BOD = "pZn23x" Then Return 111
        If EERSTE_BOD = "pZn23x-F" Then Return 111
        If EERSTE_BOD = "pZn30" Then Return 114
        If EERSTE_BOD = "pZn30g" Then Return 114
        If EERSTE_BOD = "pZn30r" Then Return 114
        If EERSTE_BOD = "pZn30w" Then Return 114
        If EERSTE_BOD = "pZn30x" Then Return 114
        If EERSTE_BOD = "Rd10A" Then Return 119
        If EERSTE_BOD = "Rd10Ag" Then Return 119
        If EERSTE_BOD = "Rd10C" Then Return 119
        If EERSTE_BOD = "Rd10Cg" Then Return 120
        If EERSTE_BOD = "Rd10Cm" Then Return 119
        If EERSTE_BOD = "Rd10Cp" Then Return 119
        If EERSTE_BOD = "Rd90A" Then Return 116
        If EERSTE_BOD = "Rd90C" Then Return 116
        If EERSTE_BOD = "Rd90Cg" Then Return 120
        If EERSTE_BOD = "Rd90Cm" Then Return 116
        If EERSTE_BOD = "Rd90Cp" Then Return 119
        If EERSTE_BOD = "Rn14C" Then Return 117
        If EERSTE_BOD = "Rn15A" Then Return 115
        If EERSTE_BOD = "Rn15C" Then Return 115
        If EERSTE_BOD = "Rn15Cg" Then Return 115
        If EERSTE_BOD = "Rn15Ct" Then Return 115
        If EERSTE_BOD = "Rn15Cw" Then Return 115
        If EERSTE_BOD = "Rn42Cg" Then Return 119
        If EERSTE_BOD = "Rn42Cp" Then Return 119
        If EERSTE_BOD = "Rn44C" Then Return 117
        If EERSTE_BOD = "Rn44Cv" Then Return 118
        If EERSTE_BOD = "Rn44Cw" Then Return 117
        If EERSTE_BOD = "Rn45A" Then Return 117
        If EERSTE_BOD = "Rn46A" Then Return 117
        If EERSTE_BOD = "Rn46Av" Then Return 118
        If EERSTE_BOD = "Rn46Aw" Then Return 117
        If EERSTE_BOD = "Rn47C" Then Return 117
        If EERSTE_BOD = "Rn47Cg" Then Return 120
        If EERSTE_BOD = "Rn47Cp" Then Return 119
        If EERSTE_BOD = "Rn47Cv" Then Return 118
        If EERSTE_BOD = "Rn47Cw" Then Return 117
        If EERSTE_BOD = "Rn47Cwp" Then Return 119
        If EERSTE_BOD = "Rn52A" Then Return 120
        If EERSTE_BOD = "Rn52Ag" Then Return 120
        If EERSTE_BOD = "Rn62C" Then Return 119
        If EERSTE_BOD = "Rn62Cg" Then Return 120
        If EERSTE_BOD = "Rn62Cp" Then Return 119
        If EERSTE_BOD = "Rn62Cwp" Then Return 119
        If EERSTE_BOD = "Rn66A" Then Return 117
        If EERSTE_BOD = "Rn66Av" Then Return 118
        If EERSTE_BOD = "Rn67C" Then Return 117
        If EERSTE_BOD = "Rn67Cg" Then Return 120
        If EERSTE_BOD = "Rn67Cp" Then Return 119
        If EERSTE_BOD = "Rn67Cv" Then Return 118
        If EERSTE_BOD = "Rn67Cwp" Then Return 119
        If EERSTE_BOD = "Rn82A" Then Return 119
        If EERSTE_BOD = "Rn82Ag" Then Return 120
        If EERSTE_BOD = "Rn94C" Then Return 117
        If EERSTE_BOD = "Rn94Cv" Then Return 118
        If EERSTE_BOD = "Rn95A" Then Return 116
        If EERSTE_BOD = "Rn95Av" Then Return 118
        If EERSTE_BOD = "Rn95C" Then Return 116
        If EERSTE_BOD = "Rn95Cg" Then Return 120
        If EERSTE_BOD = "Rn95Cm" Then Return 116
        If EERSTE_BOD = "Rn95Cp" Then Return 119
        If EERSTE_BOD = "Ro40A" Then Return 117
        If EERSTE_BOD = "Ro40Av" Then Return 118
        If EERSTE_BOD = "Ro40C" Then Return 117
        If EERSTE_BOD = "Ro40Cv" Then Return 118
        If EERSTE_BOD = "Ro40Cw" Then Return 117
        If EERSTE_BOD = "Ro60A" Then Return 116
        If EERSTE_BOD = "Ro60C" Then Return 116
        If EERSTE_BOD = "ROb72" Then Return 119
        If EERSTE_BOD = "ROb75" Then Return 116
        If EERSTE_BOD = "Rv01A" Then Return 118
        If EERSTE_BOD = "Rv01C" Then Return 118
        If EERSTE_BOD = "Rv01Cg" Then Return 118
        If EERSTE_BOD = "Rv01Cp" Then Return 118
        If EERSTE_BOD = "saVc" Then Return 101
        If EERSTE_BOD = "saVz" Then Return 102
        If EERSTE_BOD = "sHn21" Then Return 109
        If EERSTE_BOD = "shVz" Then Return 102
        If EERSTE_BOD = "skVc" Then Return 103
        If EERSTE_BOD = "skWz" Then Return 104
        If EERSTE_BOD = "Sn13A" Then Return 113
        If EERSTE_BOD = "Sn13Ap" Then Return 113
        If EERSTE_BOD = "Sn13Av" Then Return 113
        If EERSTE_BOD = "Sn13Aw" Then Return 113
        If EERSTE_BOD = "Sn13Awp" Then Return 113
        If EERSTE_BOD = "Sn14A" Then Return 113
        If EERSTE_BOD = "Sn14Ap" Then Return 113
        If EERSTE_BOD = "Sn14Av" Then Return 113
        If EERSTE_BOD = "spVc" Then Return 103
        If EERSTE_BOD = "spVz" Then Return 104
        If EERSTE_BOD = "sVc" Then Return 101
        If EERSTE_BOD = "sVk" Then Return 106
        If EERSTE_BOD = "sVp" Then Return 102
        If EERSTE_BOD = "sVs" Then Return 101
        If EERSTE_BOD = "svWp" Then Return 102
        If EERSTE_BOD = "svWz" Then Return 102
        If EERSTE_BOD = "svWzt" Then Return 102
        If EERSTE_BOD = "sVz" Then Return 102
        If EERSTE_BOD = "sVzt" Then Return 102
        If EERSTE_BOD = "sVzx" Then Return 102
        If EERSTE_BOD = "tZd21" Then Return 107
        If EERSTE_BOD = "tZd21g" Then Return 110
        If EERSTE_BOD = "tZd21v" Then Return 107
        If EERSTE_BOD = "tZd23" Then Return 113
        If EERSTE_BOD = "Vb" Then Return 101
        If EERSTE_BOD = "Vc" Then Return 101
        If EERSTE_BOD = "Vd" Then Return 101
        If EERSTE_BOD = "Vk" Then Return 106
        If EERSTE_BOD = "Vo" Then Return 101
        If EERSTE_BOD = "Vp" Then Return 102
        If EERSTE_BOD = "Vpx" Then Return 102
        If EERSTE_BOD = "Vr" Then Return 101
        If EERSTE_BOD = "Vs" Then Return 101
        If EERSTE_BOD = "Vsc" Then Return 101
        If EERSTE_BOD = "vWp" Then Return 102
        If EERSTE_BOD = "vWpg" Then Return 102
        If EERSTE_BOD = "vWpt" Then Return 102
        If EERSTE_BOD = "vWpx" Then Return 102
        If EERSTE_BOD = "vWz" Then Return 102
        If EERSTE_BOD = "vWzg" Then Return 102
        If EERSTE_BOD = "vWzr" Then Return 102
        If EERSTE_BOD = "vWzt" Then Return 102
        If EERSTE_BOD = "vWzx" Then Return 102
        If EERSTE_BOD = "Vz" Then Return 102
        If EERSTE_BOD = "Vzc" Then Return 102
        If EERSTE_BOD = "Vzg" Then Return 102
        If EERSTE_BOD = "Vzt" Then Return 102
        If EERSTE_BOD = "Vzx" Then Return 102
        If EERSTE_BOD = "Wg" Then Return 106
        If EERSTE_BOD = "Wgl" Then Return 106
        If EERSTE_BOD = "Wo" Then Return 106
        If EERSTE_BOD = "Wol" Then Return 106
        If EERSTE_BOD = "Wov" Then Return 106
        If EERSTE_BOD = "Y21" Then Return 109
        If EERSTE_BOD = "Y21g" Then Return 110
        If EERSTE_BOD = "Y21x" Then Return 111
        If EERSTE_BOD = "Y23" Then Return 113
        If EERSTE_BOD = "Y23b" Then Return 113
        If EERSTE_BOD = "Y23g" Then Return 110
        If EERSTE_BOD = "Y23x" Then Return 111
        If EERSTE_BOD = "Y30" Then Return 114
        If EERSTE_BOD = "Y30x" Then Return 114
        If EERSTE_BOD = "Zb20A" Then Return 107
        If EERSTE_BOD = "Zb21" Then Return 109
        If EERSTE_BOD = "Zb21g" Then Return 110
        If EERSTE_BOD = "Zb23" Then Return 113
        If EERSTE_BOD = "Zb23g" Then Return 113
        If EERSTE_BOD = "Zb23t" Then Return 111
        If EERSTE_BOD = "Zb23x" Then Return 111
        If EERSTE_BOD = "Zb30" Then Return 114
        If EERSTE_BOD = "Zb30A" Then Return 114
        If EERSTE_BOD = "Zb30g" Then Return 114
        If EERSTE_BOD = "Zd20A" Then Return 107
        If EERSTE_BOD = "Zd20Ab" Then Return 107
        If EERSTE_BOD = "Zd21" Then Return 107
        If EERSTE_BOD = "Zd21g" Then Return 107
        If EERSTE_BOD = "Zd23" Then Return 113
        If EERSTE_BOD = "Zd30" Then Return 114
        If EERSTE_BOD = "Zd30A" Then Return 114
        If EERSTE_BOD = "zEZ21" Then Return 112
        If EERSTE_BOD = "zEZ21g" Then Return 112
        If EERSTE_BOD = "zEZ21t" Then Return 112
        If EERSTE_BOD = "zEZ21w" Then Return 112
        If EERSTE_BOD = "zEZ21x" Then Return 112
        If EERSTE_BOD = "zEZ23" Then Return 112
        If EERSTE_BOD = "zEZ23g" Then Return 112
        If EERSTE_BOD = "zEZ23t" Then Return 112
        If EERSTE_BOD = "zEZ23w" Then Return 112
        If EERSTE_BOD = "zEZ23x" Then Return 112
        If EERSTE_BOD = "zEZ30" Then Return 112
        If EERSTE_BOD = "zEZ30g" Then Return 112
        If EERSTE_BOD = "zEZ30x" Then Return 112
        If EERSTE_BOD = "zgHd30" Then Return 114
        If EERSTE_BOD = "zgMn15C" Then Return 115
        If EERSTE_BOD = "zgMn88C" Then Return 117
        If EERSTE_BOD = "zgY30" Then Return 114
        If EERSTE_BOD = "zHd21" Then Return 108
        If EERSTE_BOD = "zHd21g" Then Return 108
        If EERSTE_BOD = "zHn21" Then Return 108
        If EERSTE_BOD = "zHn23" Then Return 109
        If EERSTE_BOD = "zhVk" Then Return 106
        If EERSTE_BOD = "zKRn1g" Then Return 120
        If EERSTE_BOD = "zKRn2" Then Return 119
        If EERSTE_BOD = "zkVc" Then Return 103
        If EERSTE_BOD = "zkWp" Then Return 104
        If EERSTE_BOD = "zMn15A" Then Return 115
        If EERSTE_BOD = "zMn22Ap" Then Return 119
        If EERSTE_BOD = "zMn25Ap" Then Return 119
        If EERSTE_BOD = "zMn56Cp" Then Return 117
        If EERSTE_BOD = "zMo10A" Then Return 115
        If EERSTE_BOD = "zMv41C" Then Return 118
        If EERSTE_BOD = "zMv61C" Then Return 118
        If EERSTE_BOD = "Zn10A" Then Return 107
        If EERSTE_BOD = "Zn10Ap" Then Return 107
        If EERSTE_BOD = "Zn10Av" Then Return 107
        If EERSTE_BOD = "Zn10Aw" Then Return 107
        If EERSTE_BOD = "Zn10Awp" Then Return 107
        If EERSTE_BOD = "Zn21" Then Return 107
        If EERSTE_BOD = "Zn21-F" Then Return 107
        If EERSTE_BOD = "Zn21g" Then Return 107
        If EERSTE_BOD = "Zn21-H" Then Return 107
        If EERSTE_BOD = "Zn21p" Then Return 107
        If EERSTE_BOD = "Zn21r" Then Return 107
        If EERSTE_BOD = "Zn21t" Then Return 107
        If EERSTE_BOD = "Zn21v" Then Return 107
        If EERSTE_BOD = "Zn21w" Then Return 107
        If EERSTE_BOD = "Zn21x" Then Return 107
        If EERSTE_BOD = "Zn21x-F" Then Return 107
        If EERSTE_BOD = "Zn23" Then Return 113
        If EERSTE_BOD = "Zn23-F" Then Return 113
        If EERSTE_BOD = "Zn23g" Then Return 113
        If EERSTE_BOD = "Zn23g-F" Then Return 113
        If EERSTE_BOD = "Zn23-H" Then Return 113
        If EERSTE_BOD = "Zn23p" Then Return 113
        If EERSTE_BOD = "Zn23r" Then Return 113
        If EERSTE_BOD = "Zn23t" Then Return 111
        If EERSTE_BOD = "Zn23x" Then Return 111
        If EERSTE_BOD = "Zn30" Then Return 114
        If EERSTE_BOD = "Zn30A" Then Return 114
        If EERSTE_BOD = "Zn30Ab" Then Return 114
        If EERSTE_BOD = "Zn30Ag" Then Return 114
        If EERSTE_BOD = "Zn30Ar" Then Return 114
        If EERSTE_BOD = "Zn30g" Then Return 114
        If EERSTE_BOD = "Zn30r" Then Return 114
        If EERSTE_BOD = "Zn30v" Then Return 114
        If EERSTE_BOD = "Zn30x" Then Return 114
        If EERSTE_BOD = "Zn40A" Then Return 107
        If EERSTE_BOD = "Zn40Ap" Then Return 107
        If EERSTE_BOD = "Zn40Ar" Then Return 107
        If EERSTE_BOD = "Zn40Av" Then Return 107
        If EERSTE_BOD = "Zn50A" Then Return 107
        If EERSTE_BOD = "Zn50Ab" Then Return 107
        If EERSTE_BOD = "Zn50Ap" Then Return 107
        If EERSTE_BOD = "Zn50Ar" Then Return 107
        If EERSTE_BOD = "Zn50Aw" Then Return 107
        If EERSTE_BOD = "zpZn23w" Then Return 113
        If EERSTE_BOD = "zRd10A" Then Return 119
        If EERSTE_BOD = "zRn15C" Then Return 115
        If EERSTE_BOD = "zRn47Cwp" Then Return 117
        If EERSTE_BOD = "zRn62C" Then Return 119
        If EERSTE_BOD = "zSn14A" Then Return 113
        If EERSTE_BOD = "zVc" Then Return 105
        If EERSTE_BOD = "zVp" Then Return 105
        If EERSTE_BOD = "zVpg" Then Return 105
        If EERSTE_BOD = "zVpt" Then Return 105
        If EERSTE_BOD = "zVpx" Then Return 105
        If EERSTE_BOD = "zVs" Then Return 105
        If EERSTE_BOD = "zVz" Then Return 105
        If EERSTE_BOD = "zVzg" Then Return 105
        If EERSTE_BOD = "zVzt" Then Return 105
        If EERSTE_BOD = "zVzx" Then Return 105
        If EERSTE_BOD = "zWp" Then Return 105
        If EERSTE_BOD = "zWpg" Then Return 105
        If EERSTE_BOD = "zWpt" Then Return 105
        If EERSTE_BOD = "zWpx" Then Return 105
        If EERSTE_BOD = "zWz" Then Return 105
        If EERSTE_BOD = "zWzg" Then Return 105
        If EERSTE_BOD = "zWzt" Then Return 105
        If EERSTE_BOD = "zWzx" Then Return 105
        If EERSTE_BOD = "zY21" Then Return 108
        If EERSTE_BOD = "zY21g" Then Return 108
        If EERSTE_BOD = "zY23" Then Return 109
        If EERSTE_BOD = "zY30" Then Return 114
        Return 0

    End Function

    Public Function Bod2KleiZandVeen(ByVal EERSTE_BOD As String) As String
        Try
            Dim CapSimCode As Long
            CapSimCode = Bod2Capsim(EERSTE_BOD)
            Select Case CapSimCode
                Case Is = 101
                    Return "veen"
                Case Is = 102
                    Return "veen"
                Case Is = 103
                    Return "veen"
                Case Is = 104
                    Return "veen"
                Case Is = 105
                    Return "veen"
                Case Is = 106
                    Return "veen"
                Case Is = 107
                    Return "zand"
                Case Is = 108
                    Return "zand"
                Case Is = 109
                    Return "zand"
                Case Is = 110
                    Return "zand"
                Case Is = 111
                    Return "zand"
                Case Is = 112
                    Return "zand"
                Case Is = 113
                    Return "zand"
                Case Is = 114
                    Return "zand"
                Case Is = 115
                    Return "klei"
                Case Is = 116
                    Return "klei"
                Case Is = 117
                    Return "klei"
                Case Is = 118
                    Return "klei"
                Case Is = 119
                    Return "klei"
                Case Is = 120
                    Return "klei"
                Case Is = 121
                    Return "klei"
            End Select
            Return "zand"

        Catch ex As Exception
            Return "zand"
        End Try

    End Function

End Class
