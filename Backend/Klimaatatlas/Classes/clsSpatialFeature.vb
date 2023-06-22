Imports Klimaatatlas.clsGeneralFunctions
Imports Klimaatatlas.SQLiteFunctions
Imports MapWinGIS
Public Class clsSpatialFeature
    Dim FeatureType As enmDataType
    Dim WKT As String                   'topography expressed as a WKT string

    Public Sub New(myType As enmDataType, myWKT As String)
        FeatureType = myType
        WKT = myWKT
    End Sub

End Class
