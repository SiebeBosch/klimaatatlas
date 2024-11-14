Imports Klimaatatlas.clsGeneralFunctions
Public Class clsFilter
    Friend filterFieldType As enmFieldType
    Friend filterFieldName As String
    Friend filterOperator As String
    Friend filterOperand As Object

    Public Function ParseFromString(filterStr As String) As Boolean
        Try
            ' Remove any whitespace first
            filterStr = filterStr.Trim()

            ' Extract field name (text between square brackets)
            If filterStr.Contains("[") AndAlso filterStr.Contains("]") Then
                filterFieldName = filterStr.Substring(filterStr.IndexOf("[") + 1, filterStr.IndexOf("]") - filterStr.IndexOf("[") - 1)
            Else
                Return False
            End If

            ' Remove the [fieldname] part from the string
            filterStr = filterStr.Substring(filterStr.IndexOf("]") + 1).Trim()

            ' Find the operator
            ' We'll check for two-character operators first (>=, <=, <>)
            If filterStr.Length >= 2 Then
                Dim possibleOperator = filterStr.Substring(0, 2)
                If possibleOperator = ">=" OrElse possibleOperator = "<=" OrElse possibleOperator = "<>" Then
                    filterOperator = possibleOperator
                    filterStr = filterStr.Substring(2)
                Else
                    ' Check for single-character operators (>, <, =)
                    filterOperator = filterStr.Substring(0, 1)
                    If filterOperator = ">" OrElse filterOperator = "<" OrElse filterOperator = "=" Then
                        filterStr = filterStr.Substring(1)
                    Else
                        Return False
                    End If
                End If
            Else
                Return False
            End If

            ' The remaining string should be the operand
            filterStr = filterStr.Trim()

            ' Try to convert the operand to a number if possible
            Dim numericOperand As Double
            If Double.TryParse(filterStr, numericOperand) Then
                filterOperand = numericOperand
            Else
                filterOperand = filterStr
            End If

            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
