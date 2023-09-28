''' <summary>
''' This class represents a node in a flowchart.
''' </summary>
Public MustInherit Class clsFlowchartNode

    Public Enum enmFlowchartNodeType
        Start = 0
        Input = 1
        Decision = 2
        Action = 3
        MultiSelect = 4
        Verdict = 5
    End Enum

    Public Property NodeType As enmFlowchartNodeType
    Public Property Label As String
    Public Property Position As Point

    Public Sub New(myType As enmFlowchartNodeType, myLabel As String, myPosition As Point)
        NodeType = myType
        Label = myLabel
        Position = myPosition
    End Sub

    Public MustOverride Sub Draw(ByRef g As Graphics)
    ' Default drawing logic (if any) for the base class

End Class
