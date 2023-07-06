Imports Klimaatatlas.clsGeneralFunctions

Public Class clsRating
    Friend Method As enmRatingMethod
    Friend FieldType As enmFieldType
    Friend classificationId As String          'refers to the classification table to be applied
    Friend lookuptableId As String             'refers to the lookup table to be applied
    Friend Penalty As Double
    Friend resultText As String
    Friend ApplyDataTransformation As Boolean = False               'whether or not to apply a data transformation prior to any classification or lookup
    Friend transformation_function As enmTransformationFunction     'an (optional) reference to a data transformation function
End Class
