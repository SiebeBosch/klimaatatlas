Imports System.Text.Json
''' <summary>
''' This class represents a connection between two nodes in a flowchart.
''' </summary>
Public Class clsFlowchartConnection
    Public Property FromNode As clsFlowchartNode
    Public Property FromEdge As clsGeneralFunctions.enmFlowChartNodeEdge
    Public Property FromItemIdx As Integer                                  'for nodes with multiple inputs/outputs, this is the index of the input/output on the given side
    Public Property ToNode As clsFlowchartNode
    Public Property ToEdge As clsGeneralFunctions.enmFlowChartNodeEdge
    Public Property ToItemIdx As Integer

    Public Sub New(myFromNode As clsFlowchartNode, myFromEdge As clsGeneralFunctions.enmFlowChartNodeEdge, myFromItemIdx As Integer, myToNode As clsFlowchartNode, myToEdge As clsGeneralFunctions.enmFlowChartNodeEdge, myToItemIdx As Integer)
        FromNode = myFromNode
        FromEdge = myFromEdge
        FromItemIdx = myFromItemIdx
        ToNode = myToNode
        ToEdge = myToEdge
        ToItemIdx = myToItemIdx
    End Sub

End Class
