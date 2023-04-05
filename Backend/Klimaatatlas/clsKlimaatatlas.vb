Imports System.Data.SQLite
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports NCalc
Imports MathNet.Symbolics

Public Class clsKlimaatatlas
    Private _jsonObj As JObject
    Private _connString As String
    Private _config As JObject

    Public Log As clsLog
    Public SQLiteCon As SQLiteConnection
    Public Generalfunctions As New clsGeneralFunctions()

    Public myProgressBar As ProgressBar
    Public myProgressLabel As Label

    Public Sub New()
    End Sub

    Public Sub SetProgressBar(ByRef pr As ProgressBar, ByRef lb As Label)
        myProgressBar = pr
        myProgressLabel = lb
    End Sub

    Public Sub ReadConfiguration(jsonPath As String)
        Dim jsonString As String = File.ReadAllText(jsonPath)
        _jsonObj = JObject.Parse(jsonString)
        Dim configContent As String = File.ReadAllText(jsonPath)
        _config = JObject.Parse(configContent)
    End Sub

    Public Sub SetDatabaseConnection(path As String)
        _connString = Strings.Replace("Data Source=@;Version=3;", "@", path)
        SQLiteCon = New SQLite.SQLiteConnection(_connString)
    End Sub

    Public Function UpgradeDatabase()
        UpgradeWQTIMESERIESTable(10)
        UpgradeWQDERIVEDSERIESTable(50)
        UpgradeWQINDICATORSTable(90)
        Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Database successfully upgraded.", 0, 100, True)
    End Function

    Public Sub UpgradeWQTIMESERIESTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Upgrading WQTIMESERIES table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("DATASOURCE", New clsSQLiteField("DATASOURCE", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("SUBSTANCE", New clsSQLiteField("SUBSTANCE", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("DATEANDTIME", New clsSQLiteField("DATEANDTIME", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", clsSQLiteField.enmSQLiteDataType.SQLITEREAL, False))
        CreateOrUpdateSQLiteTable(SQLiteCon, "WQTIMESERIES", Fields)
    End Sub

    Public Sub UpgradeWQDERIVEDSERIESTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Upgrading WQDERIVEDSERIES table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("SUBSTANCE", New clsSQLiteField("SUBSTANCE", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("DATEANDTIME", New clsSQLiteField("DATEANDTIME", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", clsSQLiteField.enmSQLiteDataType.SQLITEREAL, False))
        CreateOrUpdateSQLiteTable(SQLiteCon, "WQDERIVEDSERIES", Fields)
    End Sub

    Public Sub UpgradeWQINDICATORSTable(ProgressPercentage As Integer)
        Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Upgrading INDICATORS table...", ProgressPercentage, 100, True)
        Dim Fields As New Dictionary(Of String, clsSQLiteField)
        Fields.Add("SCENARIO", New clsSQLiteField("SCENARIO", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("INDICATOR", New clsSQLiteField("INDICATOR", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("LOCATIONID", New clsSQLiteField("LOCATIONID", clsSQLiteField.enmSQLiteDataType.SQLITETEXT, True))
        Fields.Add("DATAVALUE", New clsSQLiteField("DATAVALUE", clsSQLiteField.enmSQLiteDataType.SQLITEREAL, False))
        CreateOrUpdateSQLiteTable(SQLiteCon, "INDICATORS", Fields)
    End Sub

    Public Function ProcessRules() As Boolean
        Try
            SQLiteCon.Open()
            For Each rule As JObject In _jsonObj("rules")
                If rule("execute").Value(Of Boolean)() Then
                    Select Case rule("operation_type").ToString()
                        Case "timeseries_transformation"
                            ProcessTimeseriesTransformation(rule)
                        Case "timeseries_filter"
                            Dim filterType = rule("filter")("type").ToString()
                            Select Case filterType
                                Case "exceeds_threshold"
                                    ProcessTimeseriesFilter_thresholdExceedance(rule)
                                Case "consecutive_hours_exceeding_threshold"
                                    ProcessTimeseriesFilter_hoursThresholdExceedance(rule)
                                Case Else
                                    Throw New Exception("filterType not yet supported: " & filterType)
                            End Select
                        Case "timeseries_classification"
                            processTimeseriesClassification(rule)
                    End Select
                End If
            Next
            SQLiteCon.Close()
            Return True
        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessRules of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function ProcessTimeseriesTransformation(rule As JObject) As Boolean
        Try
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Executing timeseries transformation """ & rule("name").ToString() & """...", 0, 10, True)

            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim equation = rule("equation").ToString()
            Dim outputParameterName = rule("output")("parameter_name").ToString()

            ' Extract variables from equation
            Dim VariablesList As List(Of String) = ExtractVariablesFromEquation(equation)

            ' Create variables to be used in query
            Dim variablesString As String = String.Join(",", VariablesList.Select(Function(v) $"MAX(CASE WHEN {GetFieldNameByType(inputDataset, "parameter_name")} = '{v}' THEN {GetFieldNameByType(inputDataset, "parameter_value")} ELSE NULL END) AS {v}"))

            ' Get all unique combinations of scenario and location ID
            Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

            ' Process each unique combination
            For i = 0 To UniqueSeries.Count - 1
                Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "", i + 1, UniqueSeries.Count)

                Dim Scenario As String = UniqueSeries(i).Values(0)
                Dim LocationID As String = UniqueSeries(i).Values(1)

                ' Remove old results for the given location ID and scenario
                DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                ' Get data for all variables in a single query
                Dim dt As New DataTable
                Using cmd As New SQLiteCommand(SQLiteCon)
                    cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {variablesString} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' GROUP BY {GetFieldNameByType(inputDataset, "date")} ORDER BY {GetFieldNameByType(inputDataset, "date")};"
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                    End Using
                End Using

                ' Process the data and create resultDataTable
                Dim resultDataTable As New DataTable
                resultDataTable.Columns.Add("Date", GetType(DateTime))
                resultDataTable.Columns.Add("Value", GetType(Double))

                For Each row As DataRow In dt.Rows
                    Dim dateValue As DateTime = row(0)
                    Dim evaluatedEquation As String = equation

                    For Each Variable As String In VariablesList
                        Dim variableValue As Double
                        If Not DBNull.Value.Equals(row(Variable)) Then
                            variableValue = row(Variable)
                        Else
                            variableValue = 0 ' Assign a default value if DBNull is encountered
                        End If
                        evaluatedEquation = evaluatedEquation.Replace("[" & Variable & "]", variableValue.ToString())
                    Next

                    Dim expression As New NCalc.Expression(evaluatedEquation)
                    Dim result As Double = Convert.ToDouble(expression.Evaluate())
                    resultDataTable.Rows.Add(dateValue, result)
                Next

                ' Save results to the output dataset
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

            Next
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessTimeseriesTransformation of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

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
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Executing timeseries filter """ & rule("name").ToString() & """...", 0, 10, True)

            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim filterType = rule("filter")("type").ToString()
            Dim threshold = rule("filter")("args")(0).ToObject(Of Double)()
            Dim valueTrue = rule("filter")("value_true").ToObject(Of Double)()
            Dim valueFalse = rule("filter")("value_false").ToObject(Of Double)()
            Dim inputParameterName = rule("input")("parameter_name").ToString()
            Dim outputParameterName = rule("output")("parameter_name").ToString()

            ' Get all unique combinations of scenario and location ID
            Dim UniqueSeries As List(Of Dictionary(Of String, String)) = GetScenarioLocationCombinations(inputDataset)

            ' Process each unique combination
            For i = 0 To UniqueSeries.Count - 1
                Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "", i + 1, UniqueSeries.Count)

                Dim Scenario As String = UniqueSeries(i).Values(0)
                Dim LocationID As String = UniqueSeries(i).Values(1)

                ' Remove old results for the given location ID and scenario
                DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                ' Get data for the input parameter
                Dim dt As New DataTable
                Using cmd As New SQLiteCommand(SQLiteCon)
                    cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {GetFieldNameByType(inputDataset, "parameter_value")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' AND {GetFieldNameByType(inputDataset, "parameter_name")} = '{inputParameterName}' ORDER BY {GetFieldNameByType(inputDataset, "date")};"
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                    End Using
                End Using

                ' Process the data and create resultDataTable
                Dim resultDataTable As New DataTable
                resultDataTable.Columns.Add("Date", GetType(DateTime))
                resultDataTable.Columns.Add("Value", GetType(Double))

                For Each row As DataRow In dt.Rows
                    Dim dateValue As DateTime = row(0)
                    Dim dataValue As Double = row(1)

                    Dim result As Double = If(dataValue > threshold, valueTrue, valueFalse)
                    resultDataTable.Rows.Add(dateValue, result)
                Next

                ' Save results to the output dataset
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

            Next
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessTimeseriesFilter of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function ProcessTimeseriesFilter_hoursThresholdExceedance(rule As JObject) As Boolean
        Try
            Dim i As Integer, j As Integer

            'update the progress bar
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Executing timeseries filter """ & rule("name").ToString() & """...", 0, 10, True)

            'open the SQLite database
            If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

            'read all relevant rule aspects from our Json object
            Dim inputDataset = GetDatasetById(rule("input")("dataset").ToString())
            Dim outputDataset = GetDatasetById(rule("output")("dataset").ToString())
            Dim filterType = rule("filter")("type").ToString()
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
                Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "", i + 1, UniqueSeries.Count)

                'retrieve the scenario and location ID for this combination
                Dim Scenario As String = UniqueSeries(i).Values(0)
                Dim LocationID As String = UniqueSeries(i).Values(1)

                ' Remove old results for the given location ID and scenario
                DeleteOldResults(outputDataset, Scenario, LocationID, outputParameterName)

                ' Get data for the input parameter
                Dim dt As New DataTable
                Using cmd As New SQLiteCommand(SQLiteCon)
                    cmd.CommandText = $"SELECT {GetFieldNameByType(inputDataset, "date")}, {GetFieldNameByType(inputDataset, "parameter_value")} FROM {inputDataset("tablename")} WHERE {GetFieldNameByType(inputDataset, "scenario")} = '{Scenario}' AND {GetFieldNameByType(inputDataset, "id")} = '{LocationID}' AND {GetFieldNameByType(inputDataset, "parameter_name")} = '{inputParameterName}' ORDER BY {GetFieldNameByType(inputDataset, "date")};"
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                    End Using
                End Using

                'create a results DataTable
                Dim resultDataTable As New DataTable
                resultDataTable.Columns.Add("Date", GetType(DateTime))
                resultDataTable.Columns.Add("Value", GetType(Double))

                Dim BlockActive As Boolean = False
                Dim BlockStartIdx As Integer = 0

                'process the first timestep separately
                Dim dateValue As DateTime = dt.Rows(0)(0)
                Dim dataValue As Double = dt.Rows(0)(1)
                If dataValue > threshold Then
                    BlockActive = True
                    BlockStartIdx = 0
                Else
                    BlockActive = False
                End If

                'loop through the remaining timesteps
                For rowIndex = 1 To dt.Rows.Count - 1
                    dateValue = dt.Rows(rowIndex)(0)
                    dataValue = dt.Rows(rowIndex)(1)
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
                            Dim BlockStartDate As DateTime = dt.Rows(BlockStartIdx)(0)
                            Dim BlockEndDate As DateTime = dt.Rows(rowIndex)(0)
                            Dim BlockDuration As Double = (BlockEndDate - BlockStartDate).TotalHours
                            If BlockDuration >= duration Then
                                'block exceeds minimum duration so write the valueTrue
                                For j = BlockStartIdx To rowIndex
                                    Dim blockDate As DateTime = dt.Rows(j)(0)
                                    resultDataTable.Rows.Add(blockDate, valueTrue)
                                Next
                            Else
                                'block was too short so write the valueFalse
                                For j = BlockStartIdx To rowIndex
                                    Dim blockDate As DateTime = dt.Rows(j)(0)
                                    resultDataTable.Rows.Add(blockDate, valueFalse)
                                Next
                            End If
                        Else
                            'just a regular data point below threshold
                            resultDataTable.Rows.Add(dateValue, valueFalse)
                        End If
                    End If
                Next

                ' Save results to the output dataset
                Using transaction = SQLiteCon.BeginTransaction()
                    For Each row As DataRow In resultDataTable.Rows
                        dateValue = row("Date")
                        dataValue = row("Value")

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

            Next
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "New timeseries " & outputParameterName & " successfully written.", 0, 10, True)

            SQLiteCon.Close()
            Return True

        Catch ex As Exception
            Me.Log.AddError("Error in function ProcessTimeseriesFilter_hoursThresholdExceedance of class clsKlimaatatlas: " & ex.Message)
            Return False
        End Try
    End Function



    Private Function processTimeseriesClassification(rule As JObject) As Boolean
        Try
            Dim i As Integer, j As Integer

            'update the progress bar
            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "Executing timeseries classification """ & rule("name").ToString() & """...", 0, 10, True)

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
                        Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "", i + 1, UniqueSeries.Count)

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


            Me.Generalfunctions.UpdateProgressBar(myProgressBar, myProgressLabel, "New classification " & outputParameterName & " successfully written.", 0, 10, True)

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


End Class
