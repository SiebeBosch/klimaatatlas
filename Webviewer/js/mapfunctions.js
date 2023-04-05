
async function initializeMap() {
  // console.log("initializing map now");
  L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    maxZoom: 18,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1Ijoic2llYmVib3NjaCIsImEiOiJjazh1NnBqdDIwMW92M3FrZDF6YmNndDMyIn0.7J3-b577pBYCg1XWhmejXA'
  }).addTo(backgroundLayerGroup);
}

async function plotBackgroundMap(bg) {
  console.log("Plotting background map now");
  backgroundLayerGroup.clearLayers();
  if (bg == 'OSM') {
    L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
      attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
      maxZoom: 18,
      id: 'mapbox/streets-v11',
      tileSize: 512,
      zoomOffset: -1,
      accessToken: 'pk.eyJ1Ijoic2llYmVib3NjaCIsImEiOiJjazh1NnBqdDIwMW92M3FrZDF6YmNndDMyIn0.7J3-b577pBYCg1XWhmejXA'
    }).addTo(backgroundLayerGroup);

  } else if (bg == 'LUFO') {
    let wmsLayer = L.tileLayer.wms('https://service.pdok.nl/hwh/luchtfotorgb/wms/v1_0?', {
      layers: 'Actueel_ortho25'
    }).addTo(backgroundLayerGroup);
  } else if (bg == 'OSM_DARK') {
    L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
      attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
      maxZoom: 18,
      id: 'mapbox/dark-v10',
      tileSize: 512,
      zoomOffset: -1,
      accessToken: 'pk.eyJ1Ijoic2llYmVib3NjaCIsImEiOiJjazh1NnBqdDIwMW92M3FrZDF6YmNndDMyIn0.7J3-b577pBYCg1XWhmejXA'
    }).addTo(backgroundLayerGroup);

  }
}


function plotPeilgebieden() {
  let pgStyle = {
    "fillColor": "#15617a",
    "fillOpacity": 0.65,
    "color": "#1b3d47",
    "weight": 1,
    "opacity": 1
  };
  if (document.getElementById("peilgebieden").checked) {
    L.geoJson(subcatchments, { style: pgStyle }).addTo(featureLayerGroup);
  } else {
    featureLayerGroup.clearLayers();
  };
}


function removeLayerFromLayergroup(mylayergroup, id) {
  mylayergroup.eachLayer(function (layer) {
    if (layer._leaflet_id === id) {
      mylayergroup.removeLayer(layer)
    }
  });
}


function plotMarkers() {
  markerLayerGroup.clearLayers();
  pointsAndMarkers.forEach(plotMarker);
}

function plotMarker(item, index) {
  item.marker.addTo(markerLayerGroup);
}

function plotDepthTiles(scenarioname, maxNativeZoom, tileidx, opacity) {
  if (typeof ts === 'undefined') { ts = 0 };
  
  //first assign the currently active TileLayerId to be the previous onerror
  prevTileLayerId = activeTileLayerId;  
  tileZIndex ++;	//make sure each next layer is drawn on top of the previous

  let curLayer = L.tileLayer('tiles/' + scenarioname + '/depth/' + String(tileidx) + '/{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
  curLayer.setZIndex(tileZIndex);
  activeTileLayerId = tilesLayerGroup.getLayerId(curLayer);

  //make the previous layer transparent
  fadeOutLayer(prevTileLayerId);

  //here we make sure that only once every x timesteps the old tilemaps are removed from the layergroup. This prevents flickering
  let nTilemaps = tilesLayerGroup.getLayers().length;
  if (nTilemaps > 50){
	//remove all old layers from the layergroup
	tilesLayerGroup.eachLayer(function (layer) {
		if (layer._leaflet_id != prevTileLayerId){
			tilesLayerGroup.removeLayer(layer);
		}
	});		  	  
  }
  
}


function plotVelocityTiles(scenarioname, maxNativeZoom, tileidx, opacity) {
  if (typeof ts === 'undefined') { ts = 0 };
  
  //first assign the currently active TileLayerId to be the previous onerror
  prevTileLayerId = activeTileLayerId;  
  tileZIndex ++;	//make sure each next layer is drawn on top of the previous
  
  L.tileLayer('tiles/' + scenarioname + '/velocity/' + String(tileidx) + '/{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
  curLayer.setZIndex(tileZIndex);
  activeTileLayerId = tilesLayerGroup.getLayerId(curLayer);

  //make the previous layer transparent
  fadeOutLayer(prevTileLayerId);

  //here we make sure that only once every x timesteps the old tilemaps are removed from the layergroup. This prevents flickering
  let nTilemaps = tilesLayerGroup.getLayers().length;
  if (nTilemaps > 25){
	//remove all old layers from the layergroup
	tilesLayerGroup.eachLayer(function (layer) {
		if (layer._leaflet_id != prevTileLayerId){
			tilesLayerGroup.removeLayer(layer);
		}
	});		  	  
  }
  
   
}

function fadeOutLayer(id) {
	tilesLayerGroup.eachLayer(function (layer) {
		if (layer._leaflet_id === id){
			layer.fadeOut();
		}
	});		
}

function removeTileLayer(id) {	
	tilesLayerGroup.eachLayer(function (layer) {
		console.log("we now have layer ", layer._leaflet_id);
		if (layer._leaflet_id === id){
			tilesLayerGroup.removeLayer(layer);
			console.log("Removed tile Layer ", id);			
		}
	});	
}

function plotDepthImageOverlay(scenarioname, ts, opacity) {
  console.log("path is ", 'file:./images/' + scenarioname + '/depth/' + String(ts) + '.png');
  var imageUrl = 'file:./images/' + scenarioname + '/depth/' + String(ts) + '.png',
    imageBounds = [[51.672398775046595, 5.9521634260309284], [52.003791387813912, 6.5528768375832005]];
  L.imageOverlay(imageUrl, imageBounds).addTo(mymap);
}


function plotDepthVectors(depthGradient, forceRerender) {

  if (mymap.getZoom() > zoomthreshold && zoomlevel <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotMesh().then(() => { this.setMeshWaterDepthStyle(depthGradient, getTimestepIndex2D_vector()) });
  } else if (mymap.getZoom() <= zoomthreshold && zoomlevel > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotCentroids(depthGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsWaterDepthStyle(depthGradient, getTimestepIndex2D_vector()) });
  } else if (mymap.getZoom() > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mymesh == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotMesh().then(() => { this.setMeshWaterDepthStyle(depthGradient, getTimestepIndex2D_vector()) });
    } else {
      this.setMeshWaterDepthStyle(depthGradient, getTimestepIndex2D_vector());
    }
  } else if (mymap.getZoom() <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mycentroids == undefined) {
      zoomlevel = mymap.getZoom();
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotCentroids(depthGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsWaterDepthStyle(depthGradient, getTimestepIndex2D_vector()) });
    } else {
      this.setCentroidsWaterDepthStyle(depthGradient, getTimestepIndex2D_vector());
    }
  }
}


function plotMaxDepthTiles(scenarioname, maxNativeZoom, opacity) {
  tilesLayerGroup.clearLayers();
  L.tileLayer('tiles/' + scenarioname + '/maxdepth/' + '{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
}


function plotMaxDepthVectors(depthGradient, forceRerender) {
  if (mymap.getZoom() > zoomthreshold && zoomlevel <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotMesh().then(() => { this.setMeshMaxDepthStyle(depthGradient) });
  } else if (mymap.getZoom() <= zoomthreshold && zoomlevel > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotCentroids(depthGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsMaxDepthStyle(depthGradient) });
  } else if (mymap.getZoom() > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mymesh == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotMesh();
      this.setMeshMaxDepthStyle(depthGradient);
    } else {
      this.setMeshMaxDepthStyle(depthGradient);
    }
  } else if (mymap.getZoom() <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mycentroids == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotCentroids(depthGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsMaxDepthStyle(depthGradient) });
    } else {
      setCentroidsMaxDepthStyle(depthGradient);
    }
  }
}




function plotVelocityVectors(forceRerender) {

  if (mymap.getZoom() > zoomthreshold && zoomlevel <= zoomthreshold) {
    console.log("Switching from centroids to mesh");
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotMesh(velocityGradient).then(() => { this.setMeshVelocityStyle(velocityGradient, getTimestepIndex2D_vector()) });
  } else if (mymap.getZoom() <= zoomthreshold && zoomlevel > zoomthreshold) {
    console.log("Switching from mesh to centroids");
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotCentroids(velocityGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsVelocityStyle(velocityGradient, getTimestepIndex2D_vector()) });
  } else if (mymap.getZoom() > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mymesh == undefined) {
      console.log("Redrawing mesh");
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotMesh(velocityGradient).then(() => { this.setMeshVelocityStyle(velocityGradient, getTimestepIndex2D_vector()) });
    } else {
      this.setMeshVelocityStyle(velocityGradient, getTimestepIndex2D_vector());
    }
  } else if (mymap.getZoom() <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mycentroids == undefined) {
      zoomlevel = mymap.getZoom();
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotCentroids(velocityGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsVelocityStyle(velocityGradient, getTimestepIndex2D_vector()) });
    } else {
      this.setCentroidsVelocityStyle(velocityGradient, getTimestepIndex2D_vector());
    }
  }
}


function plotMaxVelocityTiles(scenarioname, maxNativeZoom, opacity) {
  tilesLayerGroup.clearLayers();
  L.tileLayer('tiles/' + scenarioname + '/maxvelocity/' + '{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
}



function plotMaxVelocityVectors(forceRerender) {
  if (mymap.getZoom() > zoomthreshold && zoomlevel <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotMesh(velocityGradient).then(() => { this.setMeshMaxVelocityStyle(velocityGradient) });
  } else if (mymap.getZoom() <= zoomthreshold && zoomlevel > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotCentroids(velocityGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsMaxVelocityStyle(velocityGradient) });
  } else if (mymap.getZoom() > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mymesh == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotMesh();
      this.setMeshMaxVelocityStyle(velocityGradient);
    } else {
      this.setMeshMaxVelocityStyle(velocityGradient);
    }
  } else if (mymap.getZoom() <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mycentroids == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotCentroids(velocityGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsMaxVelocityStyle(velocityGradient) });
    } else {
      setCentroidsMaxVelocityStyle(velocityGradient);
    }
  }
}

function plotTInundTiles(scenarioname, maxNativeZoom, opacity) {
  tilesLayerGroup.clearLayers();
  L.tileLayer('tiles/' + scenarioname + '/tinund/' + '{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
}


function plotTInundVectors(forceRerender) {
  if (mymap.getZoom() > zoomthreshold && zoomlevel <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotMesh();
    this.setMeshTInundStyle(tinundGradient);
  } else if (mymap.getZoom() <= zoomthreshold && zoomlevel > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    markerLayerGroup.clearLayers();
    meshLayerGroup.clearLayers();
    this.plotCentroids(tinundGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsTInundStyle(tinundGradient) });
  } else if (mymap.getZoom() > zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mymesh == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotMesh(tinundGradient).then(() => { this.setMeshTInundStyle(tinundGradient) });
    } else {
      this.setMeshTInundStyle(tinundGradient);
    }
  } else if (mymap.getZoom() <= zoomthreshold) {
    zoomlevel = mymap.getZoom();
    if (forceRerender || this.mycentroids == undefined) {
      markerLayerGroup.clearLayers();
      meshLayerGroup.clearLayers();
      this.plotCentroids(tinundGradient, getTimestepIndex2D_vector()).then(() => { this.setCentroidsTInundStyle(tinundGradient) });
    } else {
      this.setCentroidsTInundStyle(tinundGradient);
    }
  }
}

function plotT20cmTiles(scenarioname, maxNativeZoom, opacity) {
  tilesLayerGroup.clearLayers();
  L.tileLayer('tiles/' + scenarioname + '/t20cm/' + '{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
}

function plotT50cmTiles(scenarioname, opacity) {
  tilesLayerGroup.clearLayers();
  L.tileLayer('tiles/' + scenarioname + '/t50cm/' + '{z}/{x}/{y}.png', {
    opacity: opacity,
    maxNativeZoom: maxNativeZoom,
    tileSize: Settings.tileSize,
    tms: false,
    attribution: 'Generated with HydroToolbox by Hydroconsult'
  }).addTo(tilesLayerGroup);
}


