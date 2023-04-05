function drawChart2DForCoordinate(latlng, tsidx) {
  //here we convert the given coordinate into the nearest cell from the Centroids.js file
  let curDist;
  let minDist = 99999999;
  let cellID;

  //one degree longitude is approx 111,139 m.
  //let's allow maximum 50m 
  let maxDistAllowed = 50 / 111139;

  //make sure we don't see the buttons for dambreaks when drawing 2D Depths
  StyleDambreakButtons(undefined, undefined);

  //search for the nearest cell, given our clicked location
  for (let i = 0; i < Centroids.features.length; i++) {
    curDist = Pythagoras(latlng.lat, latlng.lng, Centroids.features[i].geometry.coordinates[1], Centroids.features[i].geometry.coordinates[0]);
    if (curDist < minDist) {
      minDist = curDist;
      cellID = Centroids.features[i].properties.i;
    }
  }

  //draw our chart
  if (cellID && minDist <= maxDistAllowed) {
    populateTable2D(cellID);
    drawChart2DDepth(cellID, tsidx);
  } else {
    clearChart2D();
  }
  return cellID; //return the ID of the selected cell to the calling function
}

function drawChartWQForCoordinate(latlng) {
  //here we convert the given coordinate into the nearest cell from the Centroids.js file
  let curDist;
  let minDist = 99999999;
  let cellID;
  let maxDistAllowed = 100;

  //make sure we don't see the buttons for dambreaks when drawing 2D Depths
  StyleDambreakButtons(undefined, undefined);

  //search for the nearest cell, given our clicked location
  for (let i = 0; i < Centroids.features.length; i++) {
    curDist = Pythagoras(latlng.lat, latlng.lng, Centroids.features[i].geometry.coordinates[1], Centroids.features[i].geometry.coordinates[0]);
    if (curDist < minDist) {
      minDist = curDist;
      cellID = Centroids.features[i].properties.i;
    }
  }

  //draw our chart
  if (cellID && minDist <= maxDistAllowed) {
    drawChartWQ(cellID);
  } else {
    clearChartWQ();
  }
}

function populateTable1D(ModelID, objectType) {
  if (ModelID) {

    //get the object containing observed data for this object
    let myMeas = measurements.locations.find(x => x.ModelID === ModelID);

    //initialize our table for the results statistics
    let table = document.getElementById("stats_table");
    table.innerHTML = "";                                           //clear all existing content
    var header = table.createTHead()
    var row = header.insertRow(0);
    var cell = row.insertCell(0);
    cell.innerHTML = "";

    let ObsMax;
    let SimMax;
    let ObsDates;
    let ObsValues;
    let nScenarios;

    //set the chart's title and populate its statistics section
    switch (objectType) {
      case 'observationpoint':
        nScenarios = Observationpointresults.scenarios.length;
        cell = row.insertCell(1);
        cell.innerHTML = "H.max.sim.";
        cell = row.insertCell(2);
        cell.innerHTML = "H.max.obs.";
        cell = row.insertCell(3);
        cell.innerHTML = "H.max.diff.";
        if (myMeas) {
          ObsMax = Math.max(...myMeas.h.values);
          ObsDates = myMeas.h.dates;
          ObsValues = myMeas.h.values;
        }
        break;
      case 'calculationpoint':
        nScenarios = CalcpntResults.scenarios.length;
        cell = row.insertCell(1);
        cell.innerHTML = "H.max.sim.";
        cell = row.insertCell(2);
        cell.innerHTML = "H.max.obs.";
        cell = row.insertCell(3);
        cell.innerHTML = "H.max.diff.";
        if (myMeas) {
          ObsMax = Math.max(...myMeas.h.values);
          ObsDates = myMeas.h.dates;
          ObsValues = myMeas.h.values;
        }
        break;
      case 'structure':
        nScenarios = StructureResults.scenarios.length;
        cell = row.insertCell(1);
        cell.innerHTML = "Q.max.sim.";
        cell = row.insertCell(2);
        cell.innerHTML = "Q.max.obs.";
        cell = row.insertCell(3);
        cell.innerHTML = "Q.max.diff.";
        if (myMeas) {
          ObsMax = Math.max(...myMeas.Q.values);
          ObsDates = myMeas.Q.dates;
          ObsValues = myMeas.Q.values;
        }
        break;
      case 'wqpoint':
        nScenarios = WQResults.scenarios.length;
        cell = row.insertCell(1);
        cell.innerHTML = "Conc.max.sim.";
        cell = row.insertCell(2);
        cell.innerHTML = "Conc.max.obs.";
        cell = row.insertCell(3);
        cell.innerHTML = "Conc.max.diff.";
        if (myMeas) {
          ObsMax = Math.max(...myMeas.C.values);
          ObsDates = myMeas.C.dates;
          ObsValues = myMeas.C.values;
        }
        break;
      default:
      // code block
    }

    //decide which results to iterate through
    switch (objectType) {
      case 'observationpoint':
        for (let scenarioIdx = 0; scenarioIdx < Observationpointresults.scenarios.length; scenarioIdx++) {

          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario

          scenario = Observationpointresults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          if (myFeature) {
            SimMax = Math.max(...myFeature.waterlevel);
            simSeries = myFeature.waterlevel;
          }

          if (myFeature) {
            row = table.insertRow(1);
            cell = row.insertCell(0);
            cell.innerHTML = scenario.scenario;
            cell = row.insertCell(1);
            cell.innerHTML = RoundNumber(SimMax, 2);
            cell = row.insertCell(2);
            cell.innerHTML = RoundNumber(ObsMax, 2);
            cell = row.insertCell(3);
            if (myMeas) {
              cell.innerHTML = RoundNumber(SimMax - ObsMax, 2);
            }
          }
        }
        break;

      case 'calculationpoint':

        for (let scenarioIdx = 0; scenarioIdx < CalcpntResults.scenarios.length; scenarioIdx++) {

          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario

          scenario = CalcpntResults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          if (myFeature) {
            SimMax = Math.max(...myFeature.waterlevel);
            simSeries = myFeature.waterlevel;
          }

          if (myFeature) {
            row = table.insertRow(1);
            cell = row.insertCell(0);
            cell.innerHTML = scenario.scenario;
            cell = row.insertCell(1);
            cell.innerHTML = RoundNumber(SimMax, 2);
            cell = row.insertCell(2);
            cell.innerHTML = RoundNumber(ObsMax, 2);
            cell = row.insertCell(3);
            if (myMeas) {
              cell.innerHTML = RoundNumber(SimMax - ObsMax, 2);
            }
          }
        }
        break;

      case 'structure':

        for (let scenarioIdx = 0; scenarioIdx < StructureResults.scenarios.length; scenarioIdx++) {

          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario

          scenario = StructureResults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          if (myFeature) {
            SimMax = Math.max(...myFeature.discharge);
            simSeries = myFeature.discharge;
          }

          if (myFeature) {
            row = table.insertRow(1);
            cell = row.insertCell(0);
            cell.innerHTML = scenario.scenario;
            cell = row.insertCell(1);
            cell.innerHTML = RoundNumber(SimMax, 2);
            cell = row.insertCell(2);
            cell.innerHTML = RoundNumber(ObsMax, 2);
            cell = row.insertCell(3);
            if (myMeas) {
              cell.innerHTML = RoundNumber(SimMax - ObsMax, 2);
            }
          }
        }
        break;

      case 'wqpoint':
        for (let scenarioIdx = 0; scenarioIdx < WQResults.scenarios.length; scenarioIdx++) {

          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario

          scenario = WQResults.scenarios[scenarioIdx];
          myFeature = scenario.substances[parameterIdx].features.find(x => x.i === ModelID);

          if (myFeature) {
            SimMax = Math.max(...myFeature.values);
            simSeries = myFeature.values;
          }

          if (myFeature) {

            row = table.insertRow(1);
            cell = row.insertCell(0);
            cell.innerHTML = scenario.scenario;
            cell = row.insertCell(1);
            cell.innerHTML = RoundNumber(SimMax, 2);
            cell = row.insertCell(2);
            cell.innerHTML = RoundNumber(ObsMax, 2);
            cell = row.insertCell(3);
            if (myMeas) {
              cell.innerHTML = RoundNumber(SimMax - ObsMax, 2);
            }
          }
        }
      default:
    }
  }
}

function drawChart1D(ModelID, objectType, parameterIdx) {
  //this function draws the chart for 1D objects
  if (ModelID) {
    switch (objectType) {
      case 'observationpoint':
        drawObservationpointChart(ModelID, parameterIdx, getTimestepIndex());
        break;
      case 'calculationpoint':
        drawChart1DObject(ModelID, objectType, parameterIdx)
        break;
      case 'structure':
        drawStructureChart(ModelID, "discharge", getTimestepIndex());
        // drawChart1DObject(ModelID, objectType, parameterIdx)
        break;
      case 'wqpoint':
        drawChart1DObject(ModelID, objectType, parameterIdx)
        break;
      default:
        break;
    }
  }
}


function drawChart1DObject(ModelID, objectType, parameterIdx) {
  if (ModelID) {

    //prepare a Google datatable for our chart
    let data = new google.visualization.DataTable();
    data.addColumn('date', 'Date');
    data.addColumn('number', 'Gemeten');

    //get the object containing observed data for this object
    let myMeas = measurements.locations.find(x => x.ModelID === ModelID);
    let chartTitle = document.getElementById("chart_title");
    let titleLeader;
    let vAxisTitle;
    let ObsMax;
    let SimMax;
    let ObsDates;
    let ObsValues;
    let nScenarios;

    //create a dictionary to store all dates and values from our scenarios and measurements
    //later we can sort this dictionary by key (date) and migrate all values to our datatable
    let dates = {};

    //set the chart's title and populate its statistics section
    switch (objectType) {
      case 'observationpoint':
        titleLeader = "Verloop waterhoogte ";
        vAxisTitle = "Waterhoogte (m + NAP)"
        nScenarios = Observationpointresults.scenarios.length;
        for (let scenarioIdx = 0; scenarioIdx < Observationpointresults.scenarios.length; scenarioIdx++) {
          data.addColumn('number', Observationpointresults.scenarios[scenarioIdx].scenario);
        }
        if (myMeas) {
          ObsMax = Math.max(...myMeas.h.values);
          ObsDates = myMeas.h.dates;
          ObsValues = myMeas.h.values;
        }
        break;
      case 'calculationpoint':
        titleLeader = "Verloop waterhoogte ";
        vAxisTitle = "Waterhoogte (m + NAP)"
        nScenarios = CalcpntResults.scenarios.length;
        for (let scenarioIdx = 0; scenarioIdx < CalcpntResults.scenarios.length; scenarioIdx++) {
          data.addColumn('number', CalcpntResults.scenarios[scenarioIdx].scenario);
        }
        if (myMeas) {
          ObsMax = Math.max(...myMeas.h.values);
          ObsDates = myMeas.h.dates;
          ObsValues = myMeas.h.values;
        }
        break;
      case 'structure':
        titleLeader = "Verloop debiet ";
        vAxisTitle = "Debiet (m3/s)"
        nScenarios = StructureResults.scenarios.length;
        for (let scenarioIdx = 0; scenarioIdx < StructureResults.scenarios.length; scenarioIdx++) {
          data.addColumn('number', StructureResults.scenarios[scenarioIdx].scenario);
        }
        if (myMeas) {
          ObsMax = Math.max(...myMeas.Q.values);
          ObsDates = myMeas.Q.dates;
          ObsValues = myMeas.Q.values;
        }
        break;
      case 'wqpoint':
        titleLeader = "concentratie " + WQResults.scenarios[0].substances[parameterIdx].substance + " ";
        vAxisTitle = "Concentratie (mg/l)"
        nScenarios = WQResults.scenarios.length;
        for (let scenarioIdx = 0; scenarioIdx < WQResults.scenarios.length; scenarioIdx++) {
          data.addColumn('number', WQResults.scenarios[scenarioIdx].scenario);
        }
        if (myMeas) {
          ObsMax = Math.max(...myMeas.C.values);
          ObsDates = myMeas.C.dates;
          ObsValues = myMeas.C.values;
        }
        break;
      default:
      // code block
    }

    //extend the title for our chart with the observation series' Alias
    if (myMeas) {
      chartTitle.innerText = titleLeader + myMeas.ID + "/" + myMeas.Alias + " (" + myMeas.ModelID + ")";
    } else {
      chartTitle.innerText = titleLeader + "modelobject " + ModelID;
    }


    //decide which results to iterate through
    switch (objectType) {
      case 'observationpoint':
        for (let scenarioIdx = 0; scenarioIdx < Observationpointresults.scenarios.length; scenarioIdx++) {

          let startDate;
          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario
          let ts;

          scenario = Observationpointresults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          ts = scenario.timesteps_second;
          startDate = new Date(Settings.SimulationT0);
          if (myFeature) {
            SimMax = Math.max(...myFeature.waterlevel);
            simSeries = myFeature.waterlevel;
          }

          let year = startDate.getFullYear();
          let month = startDate.getMonth();
          let day = startDate.getDate();
          let hour = startDate.getHours();
          let minute = startDate.getMinutes();
          let second = startDate.getSeconds();

          if (myFeature) {

            //walk through all timesteps, calculate the date and add its date + value to our dictionary            
            for (let i = 0; i < Settings.timesteps_second.length; i++) {
              let curDate = new Date(year, month, day, hour, minute, second + Settings.timesteps_second[i]);

              //check if this date is already existing as a key in our dictionary. If not, add it
              let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss
              if (!(curDateStr in dates)) {
                dates[curDateStr] = {};
              }

              //now check if this timestep also occurs in our results arrays
              //if so, set its value. If not, estimate the result
              let index = ts.indexOf(Settings.timesteps_second[i]);
              if (index >= 0) {
                dates[curDateStr][scenarioIdx + 1] = simSeries[index];   //set the value for this scenario and timestep in our dictionary
              } else {
                // dates[curDateStr][scenarioIdx + 1] = NaN;   //set the value for this scenario and timestep in our dictionary
                //no exact match found so walk backwards through our array with timesteps until we find the last timestep before the requested index value
                for (let j = ts.length - 1; j >= 0; j--) {
                  if (ts[j] <= Settings.timesteps_second[i]) {
                    dates[curDateStr][scenarioIdx + 1] = simSeries[j];   //set the value for this scenario and timestep in our dictionary
                    break;
                  }
                }
              }

            }
          }
        }
        break;

      case 'calculationpoint':

        for (let scenarioIdx = 0; scenarioIdx < CalcpntResults.scenarios.length; scenarioIdx++) {

          let startDate;
          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario
          let ts;

          scenario = CalcpntResults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          ts = scenario.timesteps_second;
          startDate = new Date(scenario.t0);
          if (myFeature) {
            SimMax = Math.max(...myFeature.waterlevel);
            simSeries = myFeature.waterlevel;
          }

          let year = startDate.getFullYear();
          let month = startDate.getMonth();
          let day = startDate.getDate();
          let hour = startDate.getHours();
          let minute = startDate.getMinutes();
          let second = startDate.getSeconds();

          if (myFeature) {
            //walk through all timesteps, calculate the date and add its date + value to our dictionary
            for (let i = 0; i < ts.length; i++) {
              let curDate = new Date(year, month, day, hour, minute, second + ts[i]);
              let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss
              //check if this date is already existing as a key in our dictionary. If not, add it
              if (!(curDateStr in dates)) {
                dates[curDateStr] = {};
              }
              dates[curDateStr][scenarioIdx + 1] = simSeries[i];   //set the value for this scenario and timestep in our dictionary
            }
          }
        }
        break;

      case 'structure':

        for (let scenarioIdx = 0; scenarioIdx < StructureResults.scenarios.length; scenarioIdx++) {

          let startDate;
          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario
          let ts;

          scenario = StructureResults.scenarios[scenarioIdx];
          myFeature = scenario.features.find(x => x.id === ModelID);
          ts = scenario.timesteps_second;
          startDate = new Date(scenario.t0);
          if (myFeature) {
            SimMax = Math.max(...myFeature.discharge);
            simSeries = myFeature.discharge;
          }

          let year = startDate.getFullYear();
          let month = startDate.getMonth();
          let day = startDate.getDate();
          let hour = startDate.getHours();
          let minute = startDate.getMinutes();
          let second = startDate.getSeconds();

          if (myFeature) {
            //walk through all timesteps, calculate the date and add its date + value to our dictionary
            for (let i = 0; i < ts.length; i++) {
              let curDate = new Date(year, month, day, hour, minute, second + ts[i]);
              let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss
              //check if this date is already existing as a key in our dictionary. If not, add it
              if (!(curDateStr in dates)) {
                dates[curDateStr] = {};
              }
              dates[curDateStr][scenarioIdx + 1] = simSeries[i];   //set the value for this scenario and timestep in our dictionary
            }
          }
        }
        break;

      case 'wqpoint':
        for (let scenarioIdx = 0; scenarioIdx < WQResults.scenarios.length; scenarioIdx++) {

          let startDate;
          let myFeature;
          let simSeries;  //the simulated timeseries
          let scenario;   //the simulated scenario
          let ts;

          scenario = WQResults.scenarios[scenarioIdx];
          myFeature = scenario.substances[parameterIdx].features.find(x => x.i === ModelID);
          ts = scenario.timesteps_second;
          startDate = new Date(scenario.SimulationT0);

          if (myFeature) {
            SimMax = Math.max(...myFeature.values);
            simSeries = myFeature.values;
          }

          let year = startDate.getFullYear();
          let month = startDate.getMonth();
          let day = startDate.getDate();
          let hour = startDate.getHours();
          let minute = startDate.getMinutes();
          let second = startDate.getSeconds();

          if (myFeature) {
            //walk through all timesteps, calculate the date and add its date + value to our dictionary
            for (let i = 0; i < ts.length; i++) {
              let curDate = new Date(year, month, day, hour, minute, second + ts[i]);
              let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss

              //check if this date is already existing as a key in our dictionary. If not, add it
              if (!(curDateStr in dates)) {
                dates[curDateStr] = {};
              }
              dates[curDateStr][scenarioIdx + 1] = simSeries[i];   //set the value for this scenario and timestep in our dictionary
            }
          }
        }
        break;

      default:
    }

    //add our measurements, if data exists
    if (myMeas) {
      for (let i = 0; i < ObsDates.length; i++) {
        let curDate = new Date(ObsDates[i]);
        let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss
        if (!(curDateStr in dates)) {
          dates[curDateStr] = {};
        }
        dates[curDateStr][0] = ObsValues[i];           //set the observed value in our dictionary
      }
    }

    data.addRows(Object.keys(dates).length);

    // console.log("added rows is ", Object.keys(dates).length);

    let i = 0;
    Object.keys(dates).forEach(key => {
      // console.log("adding key", key);
      // console.log("adding obs data", dates[key][0]);
      data.setValue(i, 0, new Date(key));     //datum
      data.setValue(i, 1, dates[key][0]);     //meetwaarde
      // console.log("scenarios is ", nScenarios);
      for (let j = 0; j < nScenarios; j++) {
        data.setValue(i, j + 2, dates[key][j + 1]);     //scenario
      }
      i++;
    });

    // console.log("SimMax is ", SimMax);

    // Set chart options
    var options = {
      // 'title': ID,
      fontName: 'Helvetica',
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      chartArea: {
        right: 200,   // set this to adjust the legend width
        left: 60,     // set this eventually, to adjust the left margin
      },
      'width': 600,
      'height': 350,
      vAxis: {
        title: vAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        },
        viewWindow: {
          max: SimMax
        }
      },
      hAxis: {
        title: 'Datum',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
        // viewWindow: {
        //   min: 0
        // }
      },
      seriesType: 'line',
      series: { 0: { type: 'scatter', pointSize: 1 } },
      // series: {1: {type: 'line'}},
	  explorer: {
		actions: ['dragToZoom', 'rightClickToReset'],
		axis: 'horizontal',
		keepInBounds: true,
		maxZoomIn: 4.0
		}
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(data, options);




  }
}

function StyleObservationpointsButtons(active_observationpoint_id, active_observationpoint_parameter) {
  let observationpointsdiv = document.getElementById("observationpointscontainer");
  if (!active_observationpoint_id) {
    observationpointsdiv.style.display = 'none';
  } else {
    observationpointsdiv.style.display = 'block';
  }

  let hbutton = document.getElementById("hbutton");
  let qbutton = document.getElementById("qbutton");
  let cbutton = document.getElementById("cbutton");

  // //initialize all buttons to no border
  hbutton.style.borderWidth = '0px';
  hbutton.style.boxShadow = '0px 0px #000000 ';
  qbutton.style.borderWidth = '0px';
  qbutton.style.boxShadow = '0px 0px #000000 ';
  cbutton.style.borderWidth = '0px';
  cbutton.style.boxShadow = '0px 0px #000000 ';

  if (active_observationpoint_parameter == 'observationpoint_waterlevel') {
    hbutton.style.borderWidth = '2px';
    hbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_observationpoint_parameter == 'observationpoint_discharge') {
    qbutton.style.borderWidth = '2px';
    qbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_observationpoint_parameter == 'observationpoint_cumulative_discharge') {
    cbutton.style.borderWidth = '2px';
    cbutton.style.boxShadow = '2px 3px 2px #999999';
  }
}


function StyleDambreakButtons(active_dambreak_id, active_dambreak_parameter) {
  let dambreakdiv = document.getElementById("dambreakcontainer");
  if (!active_dambreak_id) {
    dambreakdiv.style.display = 'none';
  } else {
    dambreakdiv.style.display = 'block';
  }

  let clbutton = document.getElementById("clbutton");
  let cwbutton = document.getElementById("cwbutton");
  let qbutton = document.getElementById("qbutton");
  let headbutton = document.getElementById("headbutton");
  let cumbutton = document.getElementById("cumbutton");
  // let growthbutton = document.getElementById("growthbutton");

  // //initialize all buttons to no border
  clbutton.style.borderWidth = '0px';
  clbutton.style.boxShadow = '0px 0px #000000 ';
  cwbutton.style.borderWidth = '0px';
  cwbutton.style.boxShadow = '0px 0px #000000 ';
  qbutton.style.borderWidth = '0px';
  qbutton.style.boxShadow = '0px 0px #000000 ';
  headbutton.style.borderWidth = '0px';
  headbutton.style.boxShadow = '0px 0px #000000 ';
  cumbutton.style.borderWidth = '0px';
  cumbutton.style.boxShadow = '0px 0px #000000 ';
  // growthbutton.style.borderWidth = '0px';
  // growthbutton.style.boxShadow = '0px 0px #000000 ';

  if (active_dambreak_parameter == 'dambreak_levels') {
    clbutton.style.borderWidth = '2px';
    clbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_dambreak_parameter == 'dambreak_crest_width') {
    cwbutton.style.borderWidth = '2px';
    cwbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_dambreak_parameter == 'dambreak_discharge') {
    qbutton.style.borderWidth = '2px';
    qbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_dambreak_parameter == 'dambreak_cumulative_discharge') {
    cumbutton.style.borderWidth = '2px';
    cumbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_dambreak_parameter == 'dambreak_head') {
    headbutton.style.borderWidth = '2px';
    headbutton.style.boxShadow = '2px 3px 2px #999999';
    // } else if (active_dambreak_parameter == 'dambreak_growth') {
    //   growthbutton.style.borderWidth = '2px';
    //   growthbutton.style.boxShadow = '2px 3px 2px #999999';
  } else if (active_dambreak_parameter == 'dambreak_growth') {
    cumbutton.style.borderWidth = '2px';
    cumbutton.style.boxShadow = '2px 3px 2px #999999';
  }
}

function drawDambreakChart(ID, active_dambreak_parameter, tsidx) {

  if (ID) {

    //prepare a Google datatable for our chart, create a column for the date and set the chart title and the axis title
    // let data = new google.visualization.DataTable();
    let arr = [];
    let header = [];
    let values = [];
    let xAxisTitle = "Datum";

    // data.addColumn('date', 'Date');
    header.push("Date");
    header.push({ role: 'annotation', type: 'string' });

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = "Breslocatie " + ID;
    let vAxisTitle = "titel";
    let dates = {};

    if (active_dambreak_parameter == 'dambreak_levels') {
      vAxisTitle = "Hoogtes (m + NAP)";
    } else if (active_dambreak_parameter == 'dambreak_crest_width') {
      vAxisTitle = "Breedte bres (m)";
    } else if (active_dambreak_parameter == 'dambreak_discharge') {
      vAxisTitle = "Debiet bres (m3/s)";
    } else if (active_dambreak_parameter == 'dambreak_cumulative_discharge') {
      vAxisTitle = "Cumulatief volume (m3)";
    } else if (active_dambreak_parameter == 'dambreak_head') {
      vAxisTitle = "Verval bres (m)";
    } else if (active_dambreak_parameter == 'dambreak_growth') {
      vAxisTitle = "Groeisnelheid bres (m/s)";
    }

    //prepare our table for the results statistics
    let table = document.getElementById("stats_table");
    table.innerHTML = "";                                           //clear all existing content

    //count the number of scenario's wwe have. This will be the number of columns in our datatable
    nScenarios = MeshResults.scenarios.length;
    let nSeries = 0;

    //each scenario gets its own column for the data to be stored in
    let seriesIdx = -1
    for (let myScenarioIdx = 0; myScenarioIdx < MeshResults.scenarios.length; myScenarioIdx++) {

      if (active_dambreak_parameter == 'dambreak_levels') {
        //only plot the currently active scenario because otherwise we have too many lines!
        if (myScenarioIdx == scenarioIdx) {
          seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "kruinhoogte", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_crest_level, seriesIdx);
          nSeries++;
          seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "h bov.str", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_s1up, seriesIdx);
          nSeries++;
          seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "h ben.str", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_s1dn, seriesIdx);
          nSeries++;
        }
      } else if (active_dambreak_parameter == 'dambreak_crest_width') {
        seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "kruinbreedte", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_crest_width, seriesIdx);
        nSeries++;
        // console.log("SeriesIdx is ", seriesIdx);
      } else if (active_dambreak_parameter == 'dambreak_discharge') {
        let max = Math.max(...MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_discharge);
        let min = Math.min(...MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_discharge);
        let multiplier = 1;
        if (Math.abs(min) > max) {
          multiplier = -1;
        }
        seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "debiet", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_discharge, seriesIdx, multiplier);
        nSeries++;
      } else if (active_dambreak_parameter == 'dambreak_cumulative_discharge') {
        seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "volume", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_cumulative_discharge, seriesIdx);
        nSeries++;
      } else if (active_dambreak_parameter == 'dambreak_head') {
        seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "verval", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_head, seriesIdx);
        nSeries++;
      } else if (active_dambreak_parameter == 'dambreak_growth') {
        seriesIdx = addDateTimeSeries(MeshResults.scenarios[myScenarioIdx].scenario, "groei", header, dates, MeshResults.scenarios[myScenarioIdx].dambreaks[0].timesteps_second, MeshResults.scenarios[myScenarioIdx].dambreaks[0].dambreak_growth, seriesIdx);
        nSeries++;
      }

    }

    //add our header to the results array
    arr.push(header);

    //and add all our data to the results array!
    if (xaxisrelative) {

      //set the starttime of our event so we can calculate the difference in hours
      xAxisTitle = "Tijd na aanvang simulatie (uren)"
      EventT0 = new Date(MeshResults.scenarios[0].SimulationT0);
      if (MeshResults.scenarios[0].DambreakT0Seconds) {
        xAxisTitle = "Tijd na aanvang bres (uren)"
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);
      }

      let i = 0;
      Object.keys(dates).forEach(key => {

        let myDate = new Date(key);
        let Hours = getDifferenceBetweenTwoDatesInHours(EventT0, myDate);

        //only plot from two hours before our event
        if (Hours >= 0) {
          values = [];
          values.push(Hours);

          if (i == tsidx) {
            values.push("nu");
          } else {
            values.push(null);
          }

          for (let j = 0; j < nSeries; j++) {
            values.push(dates[key][j + 1]);
          }
          arr.push(values);
        }
        i++;
      });

    } else {
      let i = 0;
      Object.keys(dates).forEach(key => {

        values = [];
        values.push(new Date(key));

        if (i == tsidx) {
          values.push("nu");
        } else {
          values.push(null);
        }

        for (let j = 0; j < nSeries; j++) {
          values.push(dates[key][j + 1]);
        }
        i++;
        arr.push(values);
      });
    }





    // Set chart options
    var options = {
      // 'title': ID,
      annotations: {
        stem: {
          color: '#097138'
        },
        style: 'line'
      },
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      chartArea: {
        right: 200,   // set this to adjust the legend width
        left: 60,     // set this eventually, to adjust the left margin
      },
      'width': 600,
      'height': 350,
      vAxis: {
        title: vAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
      },
      hAxis: {
        title: xAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
        // viewWindow: {
        //   min: 0
        // }
      },
      seriesType: 'line',
      // series: { 0: { type: 'scatter', pointSize: 1 } },
      // series: {1: {type: 'line'}},
	  explorer: {
		actions: ['dragToZoom', 'rightClickToReset'],
		axis: 'horizontal',
		keepInBounds: true,
		maxZoomIn: 4.0
		}
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(google.visualization.arrayToDataTable(arr), options);

  }
}


function drawObservationpointChart(ID, parameterIdx, tsidx) {

  if (ID) {

    //prepare a Google datatable for our chart, create a column for the date and set the chart title and the axis title
    // let data = new google.visualization.DataTable();
    let arr = [];
    let header = [];
    let values = [];

    let EventT0 = new Date(Settings.SimulationT0);
    let dambreaktsidx = -1;

    // data.addColumn('date', 'Date');
    header.push("Date");
    header.push({ role: 'annotation', type: 'string' });  //annotation for current timestep
    header.push({ role: 'annotation', type: 'string' });  //annotation for time breach

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = ID;
    let vAxisTitle = "titel";
    let dates = {};

    if (parameterIdx == 0) {
      vAxisTitle = "Waterhoogte (m + NAP)";
    } else if (parameterIdx == 1) {
      vAxisTitle = "Debiet (m3/s)";
    } else if (parameterIdx == 2) {
      vAxisTitle = "Cum. Volume (m3)";
    }

    //count the number of scenario's wwe have. This will be the number of columns in our datatable
    nScenarios = Observationpointresults.scenarios.length;
    let nSeries = 0;

    //each scenario gets its own column for the data to be stored in
    let seriesIdx = -1
    for (let myScenarioIdx = 0; myScenarioIdx < Observationpointresults.scenarios.length; myScenarioIdx++) {

      //find the feature we're dealing with!
      console.log("Finding feature ", ID);
      let myFeature = Observationpointresults.scenarios[myScenarioIdx].features.find(x => x.id === ID);

      if (parameterIdx == 0) {
        //only plot the currently active scenario because otherwise we have too many lines!
        seriesIdx = addDateTimeSeries(Observationpointresults.scenarios[myScenarioIdx].scenario, "waterhoogte", header, dates, Observationpointresults.scenarios[myScenarioIdx].timesteps_second, myFeature.waterlevel, seriesIdx);
        nSeries++;
      } else if (parameterIdx == 1) {
        seriesIdx = addDateTimeSeries(Observationpointresults.scenarios[myScenarioIdx].scenario, "debiet", header, dates, Observationpointresults.scenarios[myScenarioIdx].timesteps_second, myFeature.discharge, seriesIdx);
        nSeries++;
      } else if (parameterIdx == 2) {
        seriesIdx = addDateTimeSeries(Observationpointresults.scenarios[myScenarioIdx].scenario, "volume", header, dates, Observationpointresults.scenarios[myScenarioIdx].timesteps_second, myFeature.discharge, seriesIdx, 1, true);
        nSeries++;
      }
    }

    //finally add our observed data
    let myObserved = measurements.locations.find(x => x.ModelID === ID);
    if (myObserved) {

      if (parameterIdx == 0 && myObserved.h) {
        seriesIdx = addObservedSeries(header, dates, myObserved.h.dates, myObserved.h.values, seriesIdx);
        nSeries++;
      } else if (parameterIdx == 1 && myObserved.Q) {
        seriesIdx = addObservedSeries(header, dates, myObserved.Q.dates, myObserved.Q.values, seriesIdx);
        nSeries++;
      } else if (parameterIdx == 2 && myObserved.Q) {
        seriesIdx = addObservedSeries(header, dates, myObserved.Q.dates, myObserved.Q.values, seriesIdx, true);
        nSeries++;
      } else {
        console.log("No observed data found for ", ID);
      }
    } else {
      console.log("No observed data found for ", ID);
    }

    //add our header to the results array
    arr.push(header);

    //and add all our data to the results array!
    let xAxisTitle = "Datum";
    if (xaxisrelative) {

      //set the starttime of our event so we can calculate the difference in hours
      xAxisTitle = "Tijd na aanvang simulatie (uren)"
      if (MeshResults.scenarios[0].DambreakT0Seconds) {
        xAxisTitle = "Tijd na aanvang bres (uren)"
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      let i = 0;
      Object.keys(dates).forEach(key => {

        let myDate = new Date(key);
        let Hours = getDifferenceBetweenTwoDatesInHours(EventT0, myDate);

        //only plot from two hours before our event
        if (Hours >= 0) {
          values = [];
          values.push(Hours);

          if (i == tsidx) {
            values.push("nu");
          } else {
            values.push(null);
          }

          if (dambreaktsidx >= 0 && i == dambreaktsidx) {
            values.push("bres")
          } else {
            values.push(null);
          }

          for (let j = 0; j < nSeries; j++) {
            values.push(dates[key][j + 1]);
          }
          arr.push(values);
        }
        i++;
      });

    } else {
      let i = 0;

      //set our dambreak timestep index, if present
      if (MeshResults.scenarios.length > 0 && MeshResults.scenarios[0].DambreakT0Seconds) {
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);  //set EventT0 equal to the start of our simulation
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      Object.keys(dates).forEach(key => {

        values = [];
        values.push(new Date(key));

        if (i == tsidx) {
          values.push("nu");
        } else {
          values.push(null);
        }

        if (dambreaktsidx >= 0 && i == dambreaktsidx) {
          values.push("bres")
        } else {
          values.push(null);
        }

        for (let j = 0; j < nSeries; j++) {
          values.push(dates[key][j + 1]);
        }
        i++;
        arr.push(values);
      });
    }

    // Set chart options
    var options = {
      // 'title': ID,
      annotations: {
        stem: {
          color: '#097138'
        },
        style: 'line'
      },
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      chartArea: {
        right: 200,   // set this to adjust the legend width
        left: 60,     // set this eventually, to adjust the left margin
      },
      'width': 600,
      'height': 350,
      vAxis: {
        title: vAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
      },
      hAxis: {
        title: xAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
        // viewWindow: {
        //   min: 0
        // }
      },
      seriesType: 'line',
      // series: { 0: { type: 'scatter', pointSize: 1 } },
      // series: {1: {type: 'line'}},
	  explorer: {
		actions: ['dragToZoom', 'rightClickToReset'],
		axis: 'horizontal',
		keepInBounds: true,
		maxZoomIn: 4.0
		}
    };

    console.log("plotting now");

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(google.visualization.arrayToDataTable(arr), options);

  }
}


function drawStructureChart(ID, parameter, tsidx) {

  if (ID) {

    //prepare a Google datatable for our chart, create a column for the date and set the chart title and the axis title
    // let data = new google.visualization.DataTable();
    let arr = [];
    let header = [];
    let values = [];

    let EventT0 = new Date(Settings.SimulationT0);
    let dambreaktsidx = -1;

    // data.addColumn('date', 'Date');
    header.push("Date");
    header.push({ role: 'annotation', type: 'string' });  //annotation for current timestep
    header.push({ role: 'annotation', type: 'string' });  //annotation for time breach

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = ID;
    let vAxisTitle = "titel";
    let dates = {};

    if (parameter == 'discharge') {
      vAxisTitle = "Debiet (m3/s)";
    }

    //count the number of scenario's wwe have. This will be the number of columns in our datatable
    nScenarios = Settings.scenarios.length;
    let nSeries = 0;

    //each scenario gets its own column for the data to be stored in
    let seriesIdx = -1
    for (let myScenarioIdx = 0; myScenarioIdx < StructureResults.scenarios.length; myScenarioIdx++) {

      //find the feature we're dealing with!
      let myFeature = StructureResults.scenarios[myScenarioIdx].features.find(x => x.id === ID);

      if (parameter == 'discharge') {
        //only plot the currently active scenario because otherwise we have too many lines!
        seriesIdx = addDateTimeSeries(StructureResults.scenarios[myScenarioIdx].scenario, "discharge", header, dates, StructureResults.scenarios[myScenarioIdx].timesteps_second, myFeature.discharge, seriesIdx);
        nSeries++;
      }
    }

    //finally add our observed data
    let myObserved = measurements.locations.find(x => x.ModelID === ID);
    if (myObserved) {
      seriesIdx = addObservedSeries(header, dates, myObserved.Q.dates, myObserved.Q.values, seriesIdx);
      nSeries++;
    } else {
      console.log("No observed data found for ", ID);
    }

    //add our header to the results array
    arr.push(header);

    //and add all our data to the results array!
    let xAxisTitle = "Datum";
    if (xaxisrelative) {

      //set the starttime of our event so we can calculate the difference in hours
      xAxisTitle = "Tijd na aanvang simulatie (uren)"
      if (MeshResults.scenarios[0].DambreakT0Seconds) {
        xAxisTitle = "Tijd na aanvang bres (uren)"
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      let i = 0;
      Object.keys(dates).forEach(key => {

        let myDate = new Date(key);
        let Hours = getDifferenceBetweenTwoDatesInHours(EventT0, myDate);

        //only plot from two hours before our event
        if (Hours >= 0) {
          values = [];
          values.push(Hours);

          if (i == tsidx) {
            values.push("nu");
          } else {
            values.push(null);
          }

          if (dambreaktsidx >= 0 && i == dambreaktsidx) {
            values.push("bres")
          } else {
            values.push(null);
          }

          for (let j = 0; j < nSeries; j++) {
            values.push(dates[key][j + 1]);
          }
          arr.push(values);
        }
        i++;
      });

    } else {
      let i = 0;

      //set our dambreak timestep index, if present
      if (MeshResults.scenarios.length > 0 && MeshResults.scenarios[0].DambreakT0Seconds) {
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);  //set EventT0 equal to the start of our simulation
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      Object.keys(dates).forEach(key => {

        values = [];
        values.push(new Date(key));

        if (i == tsidx) {
          values.push("nu");
        } else {
          values.push(null);
        }

        if (dambreaktsidx >= 0 && i == dambreaktsidx) {
          values.push("bres")
        } else {
          values.push(null);
        }

        for (let j = 0; j < nSeries; j++) {
          values.push(dates[key][j + 1]);
        }
        i++;
        arr.push(values);
      });
    }

    // Set chart options
    var options = {
      // 'title': ID,
      annotations: {
        stem: {
          color: '#097138'
        },
        style: 'line'
      },
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      chartArea: {
        right: 200,   // set this to adjust the legend width
        left: 60,     // set this eventually, to adjust the left margin
      },
      'width': 600,
      'height': 350,
      vAxis: {
        title: vAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
      },
      hAxis: {
        title: xAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
        // viewWindow: {
        //   min: 0
        // }
      },
      seriesType: 'line',
      // series: { 0: { type: 'scatter', pointSize: 1 } },
      // series: {1: {type: 'line'}},
	  explorer: {
		actions: ['dragToZoom', 'rightClickToReset'],
		axis: 'horizontal',
		keepInBounds: true,
		maxZoomIn: 4.0
		}
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(google.visualization.arrayToDataTable(arr), options);

  }
}



function GetDambreakTimestepIndex(dates, EventT0) {
  //figure out which timestep index best matches the start of our dambreak so we can add a vertical annotation line there
  let i = 0;
  let minDif = 9E99;
  Object.keys(dates).forEach(key => {
    let myDate = new Date(key);
    let Hours = getDifferenceBetweenTwoDatesInHours(EventT0, myDate);
    if (Math.abs(Hours) < minDif) {
      minDif = Math.abs(Hours);
      dambreaktsidx = i
    }
    i++;
  })
  return dambreaktsidx;
}

function drawChart2DDepth(ID, tsidx) {

  if (ID) {

    //prepare a Google datatable for our chart, create a column for the date and set the chart title and the axis title
    let arr = [];
    let header = [];
    let values = [];
    let xAxisTitle = "Datum"

    let EventT0 = new Date(Settings.SimulationT0);
    let dambreaktsidx = -1;

    if (xaxisrelative) {
      header.push("Number");
    } else {
      header.push("Date");
    }

    header.push({ role: 'annotation', type: 'string' });
    header.push({ role: 'annotation', type: 'string' });

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = "Overstromingsdiepte";
    let vAxisTitle = "Waterdiepte (m)";

    let dates = {};

    //count the number of scenario's wwe have. This will be the number of columns in our datatable
    nScenarios = MeshResults.scenarios.length;
    let nSeries = 0;

    //each scenario gets its own column for the data to be stored in
    let seriesIdx = -1
    for (let myScenarioIdx = 0; myScenarioIdx < MeshResults.scenarios.length; myScenarioIdx++) {

      //first find the feature we're dealing with here and then add the incremental timeseries for this feature to our chart data
      let myFeature = MeshResults.scenarios[myScenarioIdx].features.find(x => x.i === ID);

      seriesIdx = addDateTimeSeriesFromIncrementalData(MeshResults.scenarios[myScenarioIdx].scenario, "waterdiepte", header, dates, MeshResults.scenarios[myScenarioIdx].timesteps_second, myFeature.t, myFeature.d, seriesIdx, 0.01);
      nSeries++;
    }

    //add our header to the results array
    arr.push(header);

    //and add all our data to the results array!
    if (xaxisrelative) {

      //set the starttime of our event so we can calculate the difference in hours
      xAxisTitle = "Tijd na aanvang simulatie (uren)"

      //set our dambreak timestep index, if present
      if (MeshResults.scenarios[0].DambreakT0Seconds) {
        xAxisTitle = "Tijd na aanvang bres (uren)"
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);  //set EventT0 equal to the start of our breach
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      let i = 0;
      Object.keys(dates).forEach(key => {

        let myDate = new Date(key);
        let Hours = getDifferenceBetweenTwoDatesInHours(EventT0, myDate);

        //only plot from two hours before our event
        if (Hours >= 0) {
          values = [];
          values.push(Hours);

          if (i == tsidx) {
            values.push("nu");
          } else {
            values.push(null);
          }

          if (i == dambreaktsidx) {
            values.push("bres");
          } else {
            values.push(null);
          }

          for (let j = 0; j < nSeries; j++) {
            values.push(dates[key][j + 1]);
          }
          arr.push(values);
        }
        i++;
      });

    } else {

      //set our dambreak timestep index, if present
      if (MeshResults.scenarios[0].DambreakT0Seconds) {
        EventT0.setSeconds(EventT0.getSeconds() + MeshResults.scenarios[0].DambreakT0Seconds);  //set EventT0 equal to the start of our simulation
        dambreaktsidx = GetDambreakTimestepIndex(dates, EventT0);
      }

      //populate the array with all chart values: date + annotation1 + annotation2 + result + result etc.
      let i = 0;
      Object.keys(dates).forEach(key => {
        values = [];
        values.push(new Date(key));

        if (i == tsidx) {
          values.push("nu");
        } else {
          values.push(null);
        }

        if (i == dambreaktsidx) {
          values.push("bres");
        } else {
          values.push(null);
        }

        for (let j = 0; j < nSeries; j++) {
          values.push(dates[key][j + 1]);
        }
        i++;
        arr.push(values);
      });
    }

    // Set chart options
    var options = {
      annotations: {
        stem: {
          color: '#097138'
        },
        style: 'line'
      },
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      chartArea: {
        right: 200,   // set this to adjust the legend width
        left: 60,     // set this eventually, to adjust the left margin
      },
      'width': 600,
      'height': 350,
      vAxis: {
        title: vAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
      },
      hAxis: {
        title: xAxisTitle,
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
        //title: 'Datum',
        // viewWindow: {
        //   min: 0
        // }
      },
      seriesType: 'line',
      // series: { 0: { type: 'scatter', pointSize: 1 } },
      // series: {1: {type: 'line'}},
	  explorer: {
		actions: ['dragToZoom', 'rightClickToReset'],
		axis: 'horizontal',
		keepInBounds: true,
		maxZoomIn: 4.0
		}
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(google.visualization.arrayToDataTable(arr), options);
  }
}


function addDateTimeSeries(scenarioName, parameter, header, dates, timesteps_second, values, seriesIdx, multiplier = 1, cumulative = false) {
  console.log("cumulative is ", cumulative);
  seriesIdx++;
  header.push(scenarioName + ' ' + parameter);

  let startDate = new Date(Settings.SimulationT0);

  let year = startDate.getFullYear();
  let month = startDate.getMonth();
  let day = startDate.getDate();
  let hour = startDate.getHours();
  let minute = startDate.getMinutes();
  let second = startDate.getSeconds();
  let lastval = 0;

  //walk through all timesteps as specified in the Settings.js file and set the value for the current timestep + scenario in the datatable
  for (let tsidx = 0; tsidx < Settings.timesteps_second.length; tsidx++) {

    //set the date
    let curDate = new Date(year, month, day, hour, minute, second + Settings.timesteps_second[tsidx]);
    let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss

    //check if this date is already existing as a key in our dictionary. If not, add it
    if (!(curDateStr in dates)) {
      dates[curDateStr] = {};
    }

    //so first look up the corresponding index for the given timestep in our timeseries
    let series_tsindex = timesteps_second.indexOf(Settings.timesteps_second[tsidx]);

    if (series_tsindex >= 0) {
      //yes. our series actually contains the exact timestep from the settings.timesteps_second array
      if (cumulative) {
        if (tsidx == 0) {
          //cumulatives always start with zero on the first timestep
          dates[curDateStr][seriesIdx + 1] = 0;   //set the value for this scenario and timestep in our dictionary      
          lastval = dates[curDateStr][seriesIdx + 1];
        } else {
          //we will multiply the discharge from the previous timestep with the timestep size and add that to our cumulative
          //however this can only be done if the timestep index in our series is > 0
          if (series_tsindex > 0) {
            dates[curDateStr][seriesIdx + 1] = lastval + multiplier * (Settings.timesteps_second[tsidx] - Settings.timesteps_second[tsidx - 1]) * values[series_tsindex - 1];   //set the value for this scenario and timestep in our dictionary      
            lastval = dates[curDateStr][seriesIdx + 1];
          } else {
            //apparently the index number found in our timeseries does not exceed 0 so we'll have to stick with 0 for our cumulative
            dates[curDateStr][seriesIdx + 1] = 0;   //set the value for this scenario and timestep in our dictionary      
            lastval = dates[curDateStr][seriesIdx + 1];
          }
        }
      } else {
        dates[curDateStr][seriesIdx + 1] = multiplier * values[series_tsindex];   //set the value for this scenario and timestep in our dictionary      
      }
      lastval = dates[curDateStr][seriesIdx + 1];
    } else {
      //unfortunately our series does not contain the exact timestep that we're looking for from the settings.timesteps_second array
      //therefore we will walk backwards in that array until we find the first value before the timestep we were looking for
      if (cumulative) {
        for (let j = timesteps_second.length - 1; j >= 0; j--) {
          if (timesteps_second[j] <= Settings.timesteps_second[tsidx]) {
              //we will multiply the discharge from the previous timestep with the timestep size and add that to our cumulative
              //however this can only be done if the timestep index found in our series is also > 0
              if (j > 0) {
                dates[curDateStr][seriesIdx + 1] = lastval + multiplier * (Settings.timesteps_second[tsidx] - Settings.timesteps_second[tsidx - 1]) * values[j];   //set the value for this scenario and timestep in our dictionary
                lastval = dates[curDateStr][seriesIdx + 1];
              } else {
                //apparently the index number found in our timeseries does not exceed 0 so we'll have to stick with 0 for our cumulative
                dates[curDateStr][seriesIdx + 1] = 0;   //set the value for this scenario and timestep in our dictionary      
                lastval = dates[curDateStr][seriesIdx + 1];
              }
            break;
          }
        }
      } else {
        for (let j = timesteps_second.length - 1; j >= 0; j--) {
          if (timesteps_second[j] <= Settings.timesteps_second[tsidx]) {
            dates[curDateStr][seriesIdx + 1] = multiplier * values[j];   //set the value for this scenario and timestep in our dictionary
            break;
          }
        }
      }

      // dates[curDateStr][seriesIdx + 1] = NaN;   //set the value for this scenario and timestep in our dictionary      
    }
  }
  return seriesIdx;
}



function addObservedSeries(header, dates, ResultDates, ResultValues, seriesIdx, cumulative = false) {
  seriesIdx++;
  header.push("gemeten");

  //walk through all timesteps as specified in the Settings.js file and set the value for the current timestep + scenario in the datatable
  let startDate = new Date(Settings.SimulationT0);
  let year = startDate.getFullYear();
  let month = startDate.getMonth();
  let day = startDate.getDate();
  let hour = startDate.getHours();
  let minute = startDate.getMinutes();
  let second = startDate.getSeconds();
  let lastval = 0;

  let timesteps_template = Settings.timesteps_second;   //the timesteps specified in Settings form the basis for all our charts
  for (let tsidx = 0; tsidx < timesteps_template.length; tsidx++) {

    //set the date
    let curDate = new Date(year, month, day, hour, minute, second + timesteps_template[tsidx]);
    let curDateStr = curDate.toISOString()   //.substring(0, 24)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss.sssZ

    //now look up the index number for this date in our observed data array
    let obsIdx = ResultDates.indexOf(curDateStr);
    if (obsIdx >= 0) {
      //yess we actually found our desired timestep in the observed series
      let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss
      if (cumulative) {
        if (tsidx == 0) {
          //cumulative results always start at 0
          dates[curDateStr][seriesIdx + 1] = 0;                                    //add our observed value
          lastval = dates[curDateStr][seriesIdx + 1];
        } else {
          //we use the result of the previous timestep value found and multiply it with the timestep size
          //however this can only be done when obsIdx is also > 0
          if (obsIdx > 0) {
            dates[curDateStr][seriesIdx + 1] = lastval + (timesteps_template[tsidx] - timesteps_template[tsidx - 1]) * ResultValues[obsIdx - 1];                              //add our observed value
            lastval = dates[curDateStr][seriesIdx + 1];
          } else {
            //we have to stick to 0 for now
            dates[curDateStr][seriesIdx + 1] = 0
            lastval = dates[curDateStr][seriesIdx + 1];
          }
        }
      } else {
        dates[curDateStr][seriesIdx + 1] = ResultValues[obsIdx];                              //add our observed value
      }

    } else {
      if (cumulative) {
        //unfortunately our series does not contain the exact timestep from the settings.timesteps_second array
        //therefore we will walk backwards in that array until we find the first value before the timestep we were looking for
        for (let j = ResultDates.length - 1; j >= 0; j--) {
          if (ResultDates[j] <= curDate) {
            //found in our series the first timestep before the one we are looking for for our chart
            if (tsidx == 0) {
              //cumulatives always start with zero on the first timestep
              dates[curDateStr][seriesIdx + 1] = 0;   //set the value for this scenario and timestep in our dictionary
              lastval = dates[curDateStr][seriesIdx + 1]
            } else {
              //we will multiply the discharge from the first value just before the current timestep and add that to our cumulative
              dates[curDateStr][seriesIdx + 1] = lastval + multiplier * (Settings.timesteps_second[tsidx] - Settings.timesteps_second[tsidx - 1]) * ResultValues[j];   //set the value for this scenario and timestep in our dictionary
              lastval = dates[curDateStr][seriesIdx + 1];
            }
            break;
          }
        }
      } else {
        dates[curDateStr.substring(0, 19)][seriesIdx + 1] = NaN;
      }
    }
  }
  return seriesIdx;
}



function addDateTimeSeriesFromIncrementalData(scenarioName, parameter, header, dates, timesteps_second, tsidx_incremental, values_incremental, seriesIdx, multiplier = 1) {
  //this function is a little more complicated than addDateTimeSeries since it involves an extra step in retrieving the requested data per timestep
  seriesIdx++;
  header.push(scenarioName);

  let startDate = new Date(Settings.SimulationT0);
  let timesteps_template = Settings.timesteps_second;   //the timesteps specified in Settings form the basis for all our charts

  let year = startDate.getFullYear();
  let month = startDate.getMonth();
  let day = startDate.getDate();
  let hour = startDate.getHours();
  let minute = startDate.getMinutes();
  let second = startDate.getSeconds();

  //walk through all timesteps as specified in the Settings.js file and set the value for the current timestep + scenario in the datatable
  for (let tsidx = 0; tsidx < timesteps_template.length; tsidx++) {

    //set the date
    let curDate = new Date(year, month, day, hour, minute, second + timesteps_template[tsidx]);
    let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss

    //check if this date is already existing as a key in our dictionary. If not, add it
    if (!(curDateStr in dates)) {
      dates[curDateStr] = {};
    }

    //so first look up the corresponding index for thit timestep in our vector data's timesteps array
    let series_tsindex = timesteps_second.indexOf(timesteps_template[tsidx]);

    if (series_tsindex >= 0) {
      //we found the corresponding index in the vector data's timesteps array: now look up the last incremental value before this timestep
      for (let incidx = tsidx_incremental.length - 1; incidx >= 0; incidx--) {
        if (tsidx_incremental[incidx] <= series_tsindex) {
          //we have found the corresponding index number in the incremental timesteps array
          dates[curDateStr][seriesIdx + 1] = multiplier * values_incremental[incidx];   //set the value for this scenario and timestep in our dictionary 
          break;
        }
      }
    } else {
      //this timestep has not been found in our vector result's timesteps array. 
      //We must find the last timestep before this one so we can retrieve the corresponding incremental value
      for (let series_tsindex = timesteps_second.length - 1; series_tsindex >= 0; series_tsindex--) {
        if (timesteps_second[series_tsindex] <= timesteps_template[tsidx]) {

          //finally search our incremental index
          for (let incidx = tsidx_incremental.length - 1; incidx >= 0; incidx--) {
            if (tsidx_incremental[incidx] <= series_tsindex) {
              //we have found the corresponding index number in the incremental timesteps array
              dates[curDateStr][seriesIdx + 1] = multiplier * values_incremental[incidx];   //set the value for this scenario and timestep in our dictionary 
              break;
            }
          }
          break;
        }
      }
    }
  }
  return seriesIdx;
}


function clearChart2D() {
  let table = document.getElementById("stats_table");
  table.innerHTML = "";                                           //clear all existing content
  var header = table.createTHead()
  var row = header.insertRow(0);
  var cell = row.insertCell(0);
  cell.innerHTML = "";
  cell = row.insertCell(1);
  cell.innerHTML = "Max. diepte (m)";
  cell = row.insertCell(2);
  cell.innerHTML = "Max. str.snelh (m/s)";
  cell = row.insertCell(3);
  cell.innerHTML = "Aankomsttijd (uur)";

  var chart = document.getElementById('chart_div');
  chart.innerHTML = "";

}

function clearChartWQ() {
  let table = document.getElementById("stats_table");
  table.innerHTML = "";                                           //clear all existing content
  var header = table.createTHead()
  var row = header.insertRow(0);
  var cell = row.insertCell(0);
  cell.innerHTML = "";
  cell = row.insertCell(1);
  cell.innerHTML = "Botulisme";
  cell = row.insertCell(2);
  cell.innerHTML = "Drijflagen";
  cell = row.insertCell(3);
  cell.innerHTML = "Zuurstofgebrek";

}


function addRelativeTimeSeries(seriesIdx) {
  seriesIdx++;

  // //add a column to our datatable for this scenario's result
  // // data.addColumn('number', MeshResults.scenarios[scenarioIdx].scenario);
  // header.push(scenario.scenario + ' ' + parameter);

  // let startDate = new Date(scenario.SimulationT0);
  // let ts = scenario.dambreaks[0].timesteps_second;

  // let year = startDate.getFullYear();
  // let month = startDate.getMonth();
  // let day = startDate.getDate();
  // let hour = startDate.getHours();
  // let minute = startDate.getMinutes();
  // let second = startDate.getSeconds();

  // //walk through all timesteps and set the value for the current timestep + scenario in the datatable
  // for (let tsidx = 0; tsidx < ts.length; tsidx++) {

  //   //set the date
  //   let curDate = new Date(year, month, day, hour, minute, second + scenario.dambreaks[0].timesteps_second[tsidx]);
  //   let curDateStr = curDate.toISOString().substring(0, 19)                      //convert our date to the ISO 8601 format, only keep YYYY-MM-DDTHH:mm:ss

  //   //check if this date is already existing as a key in our dictionary. If not, add it
  //   if (!(curDateStr in dates)) {
  //     dates[curDateStr] = {};
  //   }
  //   dates[curDateStr][seriesIdx + 1] = multiplier * timeseries[tsidx];   //set the value for this scenario and timestep in our dictionary
  // }
  return seriesIdx;
}


function populateTable2D(cellID) {

  let DambreakT0Seconds;

  if (MeshResults.scenarios) {
    DambreakT0Seconds = MeshResults.scenarios[scenarioIdx].DambreakT0Seconds;
  } else {
    DambreakT0Seconds = 0;
  }

  if (cellID) {

    //set some generic variables that are valid for all paths
    let startDate = new Date(Settings.SimulationT0);
    let ts = MeshResults.scenarios[scenarioIdx].timesteps_second;
    let curDate = startDate;

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = "Verloop overstroming cel " + cellID;

    //populate our table for the results statistics
    let table = document.getElementById("stats_table");
    table.innerHTML = "";                                           //clear all existing content
    var header = table.createTHead()
    var row = header.insertRow(0);
    var cell = row.insertCell(0);
    cell.innerHTML = "";
    cell = row.insertCell(1);
    cell.innerHTML = "Max. diepte (m)";
    cell = row.insertCell(2);
    cell.innerHTML = "Max. str.snelh (m/s)";
    cell = row.insertCell(3);
    cell.innerHTML = "Aankomsttijd (uur)";

    //and write the results statistics for each of the scenario's
    for (let scenidx = 0; scenidx < MeshResults.scenarios.length; scenidx++) {

      //use a find statement here since the feature index difference between scenarios
      let myFeature = MeshResults.scenarios[scenidx].features.find(x => x.i === cellID);

      let maxd = myFeature.maxd;
      let maxv = myFeature.maxv;
      let tinu = myFeature.tinu;

      row = table.insertRow(1);
      cell = row.insertCell(0);
      cell.innerHTML = MeshResults.scenarios[scenidx].scenario;
      cell = row.insertCell(1);
      cell.innerHTML = RoundNumber(maxd, 2);
      cell = row.insertCell(2);
      cell.innerHTML = RoundNumber(maxv, 2);
      cell = row.insertCell(3);
      cell.innerHTML = RoundNumber(tinu, 2);
    }
  }
}



function drawChartWQ(cellID) {

  let DambreakT0Seconds;

  if (MeshResults.scenarios) {
    DambreakT0Seconds = MeshResults.scenarios[scenarioIdx].DambreakT0Seconds;
  }
  let Hours;

  if (cellID) {

    let chartTitle = document.getElementById("chart_title");
    chartTitle.innerText = "Waterkwaliteitsindicator cel " + cellID;

    //create our table for the results statistics
    let table = document.getElementById("stats_table");
    table.innerHTML = "";                                           //clear all existing content
    var header = table.createTHead()
    var row = header.insertRow(0);
    var cell = row.insertCell(0);
    cell.innerHTML = "";
    cell = row.insertCell(1);
    cell.innerHTML = "Botulisme.";
    cell = row.insertCell(2);
    cell.innerHTML = "Drijflagen.";
    cell = row.insertCell(3);
    cell.innerHTML = "Zuurstofgebrek.";

    //store our results for this location in a nested array. First item is the scenario, second contains the depths
    //so it's results[scenarioIdx].depths[tsidx]
    let data = new google.visualization.DataTable();
    data.addColumn('number', 'Tijdstap (uren)');

    let depths = [];
    let timesteps = [];

    //and write the results statistics for each of the scenario's
    for (let scenidx = 0; scenidx < MeshResults.scenarios.length; scenidx++) {

      //use a find statement here since the feature index difference between scenarios
      let myFeature = MeshResults.scenarios[scenidx].features.find(x => x.i === cellID);

      let maxd = myFeature.maxd;
      let maxv = myFeature.maxv;
      let tinu = myFeature.tinu;

      // let maxd = MeshResults.scenarios[scenidx].features[cellidx].maxd;
      // let maxv = MeshResults.scenarios[scenidx].features[cellidx].maxv;
      // let tinu = MeshResults.scenarios[scenidx].features[cellidx].tinu;
      row = table.insertRow(1);
      cell = row.insertCell(0);
      cell.innerHTML = MeshResults.scenarios[scenidx].scenario;
      cell = row.insertCell(1);
      cell.innerHTML = RoundNumber(maxd, 2);
      cell = row.insertCell(2);
      cell.innerHTML = RoundNumber(maxv, 2);
      cell = row.insertCell(3);
      cell.innerHTML = RoundNumber(tinu, 2);

      depths[scenidx] = myFeature.d;
      timesteps[scenidx] = myFeature.t;
      data.addColumn('number', MeshResults.scenarios[scenidx].scenario + ' (cm)');

    }


    //create our datatable by stepping through each timestep
    for (let tsidx = 0; tsidx < MeshResults.scenarios[scenarioIdx].timesteps_second.length; tsidx++) {

      //only process timesteps after the dambreak
      // Hours = (MeshResults.scenarios[scenarioIdx].timesteps_second[tsidx] - DambreakT0Seconds) / 3600;

      Hours = (MeshResults.scenarios[scenarioIdx].timesteps_second[tsidx]) / 3600;

      if (Hours >= 0) {
        let row = [Hours];                            //create a new row for our datatable
        for (let k = 0; k < depths.length; k++) {     //loop trough the waterdepts for each scenario

          //find the last changed value just before or at the current timestep index tsidx
          let lastidx = 0;
          for (let i = 0; i < timesteps[k].length; i++) {
            if (timesteps[k][i] <= tsidx) {
              lastidx = i;
            }
          }
          row.push(depths[k][lastidx]);

        }
        data.addRows([row]);
      }
    }


    // Set chart options
    var options = {
      legend: {
        position: 'right',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        }
      },
      vAxis: {
        title: "Waterdiepte (cm)",
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        }
      },
      hAxis: {
        title: 'Tijd na aanvang gebeurtenis (uren)',
        textStyle: {
          fontName: 'Helvetica',
          fontSize: 14,
        },
        titleTextStyle: {
          fontName: 'Helvetica',
          fontSize: 16,
        },
        viewWindow: {
          min: 0
        }
      }
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
    chart.draw(data, options);
  }
}
