# klimaatatlas
Klimaatatlas voor het berekenen en publiceren van risico's op slechte waterkwaliteit zoals botulisme en drijflagen. 

Deze python-applicatie leest twee bestanden in:
* een GeoJSON-bestand, bestaande uit puntlocaties
* een JSON-bestand met voor iedere puntlocatie het tijdsverloop van de concentratie van bepaalde stoffen of fracties

Vervolgens leest het script een JSON-bestand met rekenregels in. Het gebruikt deze rekenregels om voor iedere locatie indicatorwaarden te berekenen omtrent risico's op slechte waterkwaliteit. Te denken valt aan het risico op botulisme en drijflagen.

De indicatorwaarden worden vervolgens ruimtelijk geïnterpoleerd naar een watervlakkenkaart, rekening houdend met barrières.

Het resultaat wordt gepubliceerd naar een shapefile en kan door de gebruiker worden gepubliceerd in bijv. ArcGIS online.

