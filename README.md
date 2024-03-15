# Klimaatatlas
Klimaatatlas is een applicatie voor het berekenen en publiceren van risico's op slechte waterkwaliteit zoals risico op:
* drijflagen
* blauwalg
* botulisme
* zuurstofloosheid
* kroos

De klimaatatlas bestaat uit de volgende applicaties:

## Frontend

## desktop-applicatie
Via een desktop-applicatie, getiteld 'Klimaatatlas', kan de gebruiker maatlatten en rekenregels configureren en doorrekenen en daarnaast het resultaat publiceren in de interactieve webviewer.
Klimaatatlas is geschreven in VB.NET, in het .NET framework 7.0. Het programma hanteert twee types invoerbestanden: een Geopackage (.gpkg) en een Json-bestand (.json). 

Bij de toepassing voor Hoogheemraadschap van Rijnland bevat de Geopakckage alle bijna 100.000 watervlakken van het Hoogheemraadschap.
De JSON bevat een beschrijving van de rekenregels die moeten worden toegepast op data uit de geopackage.

De volgende zaken komen voor rekening van de desktop-applicatie:
* Verwijzen naar een watervlakkenkaart (geopackage; .gpkg) met alle relevante gegevensvelden (bijv. waterdiepte, breedte, doodlopendheid, oriëntatie tov de wind, temperatuur die met een bepaalde frequentie voorkomt)
* Verwijzen naar de gebruikte SQLite-database (optioneel)
* Opstellen van 'maatlatten' die worden gebruikt in de rekenregels
* Opstellen van rekenregels waarlangs het risico op slechte waterkwaliteit wordt berekend. Rekenregels zijn een optelsom van de individuele maatlatten, met een wegingsfactor per maatlat.
* Doorrekenen van de rekenregels
* Wegschrijven van het resultaat naar de onderhavige geopackage.

Voorbeeld van een maatlat: 
* bij grote waterdiepte (> 1.5m): risicowaarde t.a.v. kroos = 0
* bij geringe waterdiepte (< 0.2m): risicowaarde t.a.v. kroos = 1.

Voorbeeld van een rekenregel: 
* risico op kroos = (f1 * risico_ondiep_water + f2 * doodlopendheid_watergang + f3 * belasting_vermestende_stoffen) / (f1 + f2 + f3)

Doordat na de optelling weer gedeeld wordt door de som van de factoren ligt het risico altijd tussen 0 (geen risico) en 1 (hoog risico).
De applicatie doorloopt alle features uit de watervlakkenkaart, berekent het resultaat voor iedere maatlat en op basis daarvan, aan de hand van de rekenregels, het risico.
Het risico wordt weggeschreven naar een kopie van de watervlakkenkaart, tezamen met de relatieve bijdrage van elke maatlat bij de totstandkoming van het risico.

## backend
De backend bestaat uit diverse python scripts.

### python scripts
Het python script voert diverse voorbewerkingen uit op de gegevens uit SOBEK en de ZICHT-database. Het gaat hier met name om ruimtelijke interpolaties van de waterkwaliteitsparameters. Het script maakt gebruik van de shapefile met watervlakken en de database met SOBEK-resultaten. De SOBEK-resultaten worden ruimtelijk geïnterpoleerd naar iedere polygoon van de watervlakkenkaart en het resultaat teruggeschreven naar de SQLite database.

* sobek_statistics_to_polygons.py leest de SOBEK-tijdreeksen uit een SQLite database (kan uit SOBEK worden geëxporteerd m.b.v. HydroToolbox van Hydroconsult) en bepaalt percentielwaarden voor iedere combinatie van klimaatscenario en parameter (stof). De percentielwaarden worden weer naar de database geschreven.
* sobek_timeserie_to_polygons.py leest de SOBEK-tijdreeksen uit een SQLite database (kan uit SOBEK worden geëxporteerd m.b.v. HydroToolbox van Hydroconsult) en schrijft de reeksen naar de database.
* db_setup.py bevat ondersteunende functies voor de database.

### Visualisatie en publicatie
De verrijkte geopackage kan door de gebruiker weer opgepakt worden als bron voor een interactieve visualisatie in bijvoorbeeld ESRI StoryMaps of ArcGIS Online.





