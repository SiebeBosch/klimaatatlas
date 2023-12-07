Public Class MessageEventArgs
    Inherits EventArgs

    Public Enum MessageTypes
        [Error]
        Warning
        Message
        Debug
    End Enum

    Public Property Message As String
    Public Property MessageType As MessageTypes

    Public Sub New(ByVal message As String, ByVal messageType As MessageTypes)
        Me.Message = message
        Me.MessageType = messageType
    End Sub
End Class
