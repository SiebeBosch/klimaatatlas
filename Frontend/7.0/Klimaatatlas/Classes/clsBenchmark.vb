Imports Klimaatatlas.clsGeneralFunctions
Imports MapWinGIS
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Class clsBenchmark
    Private Setup As clsKlimaatatlas

    Public Name As String
    Friend FieldNamesPerScenario As Dictionary(Of String, String)
    Friend TransformationPerScenario As Dictionary(Of String, String)
    Friend ClassificationType As clsKlimaatatlas.enmClassificationType

    'classification for discrete values
    Dim Classes As New Dictionary(Of String, Double)

    'classification for continuous values
    Friend ValuesRange As New SortedDictionary(Of Double, Double) 'key= value, value=verdict
    Friend colorScale As clsColorScale ' Set your lower and upper values here

    Public Sub New(ByRef myKlimaatatlas As clsKlimaatatlas, myName As String, myFieldNamesPerScenario As Dictionary(Of String, String), myTransformationPerScenario As Dictionary(Of String, String), myClassificationType As clsKlimaatatlas.enmClassificationType)
        Setup = myKlimaatatlas
        Name = myName
        FieldNamesPerScenario = myFieldNamesPerScenario
        TransformationPerScenario = myTransformationPerScenario
        ClassificationType = myClassificationType
    End Sub

    Public Function getResult(value As Object, ByRef Result As Double) As Boolean
        Result = 0
        If ClassificationType = clsKlimaatatlas.enmClassificationType.Discrete Then
            'cast the value to a string and look up its value in the classes list
            If IsDBNull(value) Then
                If Classes.ContainsKey("NULL") Then
                    Result = Classes.Item("NULL")
                    Return True
                Else
                    Return False
                End If
            End If
            Dim valueString As String = value.ToString
            For i = 0 To Classes.Count - 1
                If Classes.Keys(i) = valueString.Trim.ToUpper Then
                    Result = Classes.Values(i)
                    Return True
                End If
            Next
            Return False
        ElseIf ClassificationType = clsKlimaatatlas.enmClassificationType.Continuous Then
            'interpolate the value in the values range
            If IsDBNull(value) Then Return False
            Dim valueDouble As Double = CDbl(value)
            If valueDouble <= ValuesRange.Keys(0) Then
                Result = ValuesRange.Values(0)
                Return True
            ElseIf valueDouble > ValuesRange.Keys(ValuesRange.Keys.Count - 1) Then
                Result = ValuesRange.Values(ValuesRange.Keys.Count - 1)
                Return True
            Else
                Dim i As Integer = 0
                While valueDouble > ValuesRange.Keys(i)
                    i += 1
                End While
                Dim lowerValue As Double = ValuesRange.Keys(i - 1)
                Dim upperValue As Double = ValuesRange.Keys(i)
                Dim lowerVerdict As Double = ValuesRange.Values(i - 1)
                Dim upperVerdict As Double = ValuesRange.Values(i)
                Result = lowerVerdict + (valueDouble - lowerValue) * (upperVerdict - lowerVerdict) / (upperValue - lowerValue)
                Return True
            End If
        Else
            Return False
        End If
    End Function

    Public Sub SetDiscreteClasses(myClasses As Dictionary(Of String, Double))
        Classes = myClasses
        colorScale = New clsColorScale(Setup, Classes)
    End Sub

    Public Sub SetContinuousClasses(myValuesRange As SortedDictionary(Of Double, Double))
        ValuesRange = myValuesRange
        colorScale = New clsColorScale(Setup, myValuesRange)
    End Sub

End Class
