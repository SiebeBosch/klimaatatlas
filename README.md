# klimaatatlas
Klimaatatlas voor het berekenen en publiceren van risico's op slechte waterkwaliteit zoals eutrofiëringshotspots. 

De klimaatatlas bestaat uit een aantal applicaties:

## backend
De backend bestaat uit twee delen: een desktop-applicatie en diverse python scripts.

### desktop-applicatie
Voor de backend is een .NET applicatie ontwikkeld. Deze desktopapplicatie werkt met een SQLite-database en kan rekenregels verwerken die in een JSON-bestand worden aangeleverd. Als de database bijvoorbeeld reeksen met temperatuur, stikstof en fosfor bevat, kunnen via de rekenregels deze reeksen worden geanalyseerd op de combinatie van vermestende stoffen en hoge temperatuur. Op basis hiervan kan dan een 'rapportcijfer' worden uitgedeeld.

### python scripts
Het python script voert diverse voorbewerkingen uit op de gegevens uit SOBEK en de ZICHT-database. Het gaat hier met name om ruimtelijke interpolaties van de waterkwaliteitsparameters. Het script maakt gebruik van de shapefile met watervlakken en de database met SOBEK-resultaten. De SOBEK-resultaten worden ruimtelijk geïnterpoleerd naar iedere polygoon van de watervlakkenkaart en het resultaat teruggeschreven naar de SQLite database.

* sobek_statistics_to_polygons.py leest de SOBEK-tijdreeksen uit een SQLite database (kan uit SOBEK worden geëxporteerd m.b.v. HydroToolbox van Hydroconsult) en bepaalt percentielwaarden voor iedere combinatie van klimaatscenario en parameter (stof). De percentielwaarden worden weer naar de database geschreven.
* sobek_timeserie_to_polygons.py leest de SOBEK-tijdreeksen uit een SQLite database (kan uit SOBEK worden geëxporteerd m.b.v. HydroToolbox van Hydroconsult) en schrijft de reeksen naar de database.
* db_setup.py bevat ondersteunende functies voor de database.

## frontend
Net als de backend bestaat de frontend uit diverse applicaties. Zo is er voor de specialisten van het waterschap de interactieve webviewer waarmee we de uitkomsten op de kaart plotten. Voor communicatie naar bestuurders en het grote publiek zijn er de ESRI Storymaps.

### webviewer
Om de uitkomsten van de analyses te kunnen tonen op een interactieve kaart is een webviewer ontwikkeld in HTML/CSS/Javascript.
Deze webviewer wordt op dit moment alleen nog gevuld met puntgegevens.

### ESRI Storymaps

### PowerBI



