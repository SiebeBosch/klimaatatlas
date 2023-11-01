Imports Klimaatatlas.clsGeneralFunctions
Imports Klimaatatlas.SQLiteFunctions
Imports MapWinGIS
Imports GeoLibrary.Geometry
Imports GeoLibrary.IO.Wkb
Imports GeoLibrary.IO.Wkt
Imports GeoLibrary.Model

Public Class clsSpatialFeature
    Dim FeatureType As enmDataType
    Friend WKT As String                   'topography expressed as a WKT string
    Friend WKB As Byte()

    Public Sub New(myType As enmDataType)
        FeatureType = myType
    End Sub

    Public Sub New(myType As enmDataType, myWKT As String)
        FeatureType = myType
        WKT = myWKT
    End Sub


End Class
