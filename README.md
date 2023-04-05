# klimaatatlas
Klimaatatlas voor het berekenen en publiceren van risico's op slechte waterkwaliteit zoals eutrofiëringshotspots. 

De klimaatatlas bestaat uit een aantal applicaties:

## backend
Voor de backend is een .NET applicatie ontwikkeld. Deze desktopapplicatie werkt met een SQLite-database en kan rekenregels verwerken die in een JSON-bestand worden aangeleverd. Als de database bijvoorbeeld reeksen met temperatuur, stikstof en fosfor bevat, kunnen via de rekenregels deze reeksen worden geanalyseerd op de combinatie van vermestende stoffen en hoge temperatuur. Op basis hiervan kan dan een 'rapportcijfer' worden uitgedeeld.

