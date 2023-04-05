
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//          PLOTTING FEATURES
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
async function plotCentroids(depthGradient, meshtsidx) {
  if (Centroids.features) {
    let tsidx = this.getTimestep();
    this.mycentroids = L.geoJson(Centroids, {
      pointToLayer: function (feature, latlng) {
        return L.circleMarker(latlng, {

          // Stroke properties
          color: '#5EA4D4',
          opacity: 1,
          weight: 0,

          // Fill properties
          fillColor: getColor(getWaterDepth(feature.properties.idx, meshtsidx), depthGradient),
          fillOpacity: opacity,

          radius: 2,
          pane: 'mesh'
        });
      }
    }).addTo(markerLayerGroup).on('click',
      function (ev) {
        drawChart2D(ev.layer.feature.properties.i); //generate a chart
      });
  }
}

async function plotMesh() {
  if (Mesh.features) {
    this.mymesh = L.geoJson(Mesh, {
      style: {
        // Stroke properties
        color: '#5EA4D4',
        opacity: 1,
        weight: 0,
        pane: 'mesh',

        // Fill properties
        fillColor: "blue",
        fillOpacity: opacity,

      }
      // MeshStyling
    }).addTo(meshLayerGroup).on('click',
      function (ev) {
        drawChart2D(ev.layer.feature.properties.i); //generate a chart
      });
  }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//          STYLING OF FEATURES
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

async function setCentroidsWaterDepthStyle(depthGradient, meshtsidx) {
  if (Centroids.features) {
    let tsidx = this.getTimestep();
    this.mycentroids.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      layer.setStyle({
        fillColor: getColor(getWaterDepth(cellidx, meshtsidx), depthGradient),
        fillOpacity: opacity
      })
    });
  }
}

async function setCentroidsMaxDepthStyle(depthGradient) {
  if (Centroids.features) {
    this.mycentroids.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.maxd * 100, depthGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}

async function setCentroidsVelocityStyle(velocityGradient, meshtsidx) {
  if (Centroids.features) {
    let tsidx = this.getTimestep();
    this.mycentroids.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      layer.setStyle({
        fillColor: getColor(getVelocity(cellidx, meshtsidx), velocityGradient),
        fillOpacity: opacity
      })
    });
  }
}



async function setCentroidsMaxVelocityStyle(velocityGradient) {
  if (Centroids.features) {
    this.mycentroids.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.maxv * 100, velocityGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}

async function setCentroidsTInundStyle(tinundGradient) {
  if (Centroids.features) {
    this.mycentroids.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.tinu, tinundGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}



async function setMeshWaterDepthStyle(depthGradient, meshtsidx) {
  if (Mesh.features) {
    let tsidx = this.getTimestep();
    this.mymesh.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      layer.setStyle({
        fillColor: getColor(getWaterDepth(cellidx, meshtsidx), depthGradient),
        fillOpacity: opacity
      })
    });
  }
}

async function setMeshMaxDepthStyle(depthGradient) {
  if (Mesh.features) {
    this.mymesh.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.maxd * 100, depthGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}

async function setMeshVelocityStyle(velocityGradient, meshtsidx) {
  if (Mesh.features) {
    let tsidx = this.getTimestep();
    this.mymesh.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      layer.setStyle({
        fillColor: getColor(getVelocity(cellidx, meshtsidx), velocityGradient),
        fillOpacity: opacity
      })
    });
  }
}

async function setMeshMaxVelocityStyle(velocityGradient) {
  if (Mesh.features) {
    this.mymesh.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.maxv * 100, velocityGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}

async function setMeshTInundStyle(tinundGradient) {
  if (Mesh.features) {
    this.mymesh.eachLayer(function (layer) {
      let cellidx = layer.feature.properties.idx;
      let props = getProperties(cellidx);
      if (props) {
        layer.setStyle({
          fillColor: getColor(props.tinu, tinundGradient),
          fillOpacity: opacity
        })
      }
    });
  }
}




//////////////////////////////////////////////////////////////////////////////////////////////////////////////////



function getProperties(cellidx) {
  // 202202-27: replaced the find function by a direct refer to the index number (saving time)
  return MeshResults.scenarios[scenarioIdx].features[cellidx];
  // return MeshResults.scenarios[scenarioIdx].features.find(feature => (feature.i === cellidx))
}

function getVelocity(cellidx, meshtsidx) {
  let props = getProperties(cellidx);
  let ts;
  let velocities;
  // let ts = MeshResults.scenarios[scenarioIdx].features.find(feature => feature.i == cellidx);

  if (props != null) {
    ts = props.t;
    velocities = props.v;
  } else {
    return 0;
  }

  if (ts.length > 1) {
    for (let i = 1; i < ts.length; i++) {
      if (ts[i] > meshtsidx) { return velocities[i - 1]; }    //as soon as we overshoot, take the previous value
    }
    return velocities[ts.length - 1];                     //if we end up here, we take the last value
  } else if (ts.length === 1) {
    return velocities[0];
  } else {
    return 0;
  }
}

function getWaterDepth(cellidx, meshtsidx) {
  let props = getProperties(cellidx);
  let ts;
  let depths;

  if (props != null) {
    ts = props.t;
    depths = props.d;
  } else {
    return 0;
  }

  if (ts.length > 1) {
    for (let i = 1; i < ts.length; i++) {
      if (ts[i] > meshtsidx) { return depths[i - 1]; }    //as soon as we overshoot, take the previous value
    }
    return depths[ts.length - 1];                     //if we end up here, we take the last value
  } else if (ts.length === 1) {
    return depths[0];
  } else {
    return 0;
  }
}

function getColor(value, Gradient) {
  if (Gradient.fromValue < value && value < Gradient.toValue) {
    return interpolateRGB(Gradient.fromValue, Gradient.fromColor.R, Gradient.fromColor.G, Gradient.fromColor.B, Gradient.toValue, Gradient.toColor.R, Gradient.toColor.G, Gradient.toColor.B, value)
  } else if (value < Gradient.fromValue) {
    if (Gradient.transparentBelowLowest == true) { return 'rgba(0,0,255,0)' } else { return RGBfromValues(Gradient.fromColor.R, Gradient.fromColor.G, Gradient.fromColor.B) };
  } else if (value == Gradient.fromValue) {
    if (Gradient.transparentAtLowest == true) { return "rgba(0,0,255,0)" } else { return RGBfromValues(Gradient.fromColor.R, Gradient.fromColor.G, Gradient.fromColor.B) };
  } else if (value == Gradient.toValue) {
    if (Gradient.transparentAtHighest) { return "rgba(0,0,255,0)" } else { return RGBfromValues(Gradient.toColor.R, Gradient.toColor.G, Gradient.toColor.B) };
  } else if (value > Gradient.toValue) {
    if (Gradient.transparentAboveHighest == true) { return "rgba(0,0,255,0)" } else { return RGBfromValues(Gradient.toColor.R, Gradient.toColor.G, Gradient.toColor.B) };
  }
}

function fitMapToFeatures() {
  //a home-made algorithm to zoom to the data extent
  let minLat = 9E99;
  let minLon = 9E99;
  let maxLat = -9E99;
  let maxLon = -9E99;
  Centroids.features.forEach((feature => {
    if (feature.geometry.coordinates[0] < minLon) { minLon = feature.geometry.coordinates[0] };
    if (feature.geometry.coordinates[0] > maxLon) { maxLon = feature.geometry.coordinates[0] };
    if (feature.geometry.coordinates[1] < minLat) { minLat = feature.geometry.coordinates[1] };
    if (feature.geometry.coordinates[1] > maxLat) { maxLat = feature.geometry.coordinates[1] };
  }))

  var southWest = L.latLng(minLat, minLon),
    northEast = L.latLng(maxLat, maxLon),
    bounds = L.latLngBounds(southWest, northEast);
  mymap.fitBounds(bounds);
}
