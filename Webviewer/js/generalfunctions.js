//version 1.01

//--------------------------------------------------------------------------------
//                  GEOMETRIC FUNCTIONS
//--------------------------------------------------------------------------------
function DEG2RAD(angleDegrees) {
    //graden naar radialen
    return angleDegrees / 180 * Math.PI;
}

function RAD2DEG(angleRadians) {
    //radialen naar graden
    return angleRadians * 180 / Math.PI;
}
//--------------------------------------------------------------------------------

//write a function that converts lat/lon map coordinates to the Dutch RD new system
function WGS842RD(Lat, Lon) {
    let phiBesLambdaBes = WGS842BESSEL(Lat, Lon);
    let xy = BESSEL2RD(phiBesLambdaBes[0], phiBesLambdaBes[1]);
    return xy;
}

function PopulateDropdownListFromJSON(Name, ArrayIdx) {
    let dropdown = document.getElementById(Name);
    dropdown.length = 0;

    let defaultOption = document.createElement('option');
    defaultOption.text = 'all';

    dropdown.add(defaultOption);
    dropdown.selectedIndex = 0;

    let option;
    for (let i = 0; i < stochasts.stochasts[ArrayIdx].values.length; i++) {
        option = document.createElement('option');
        option.text = stochasts.stochasts[ArrayIdx].values[i].value;
        option.value = stochasts.stochasts[ArrayIdx].values[i].value;
        dropdown.add(option);
    }
}



function SortByKey(obj) {

    //https://bobbyhadz.com/blog/javascript-sort-object-keys
    //1. use the Object.keys method to retrieve an array of keys
    //2. use the sort method to sort the array
    //3. use the reduce method to get an object with the sorted keys
    const sorted = Object.keys(obj)
        .sort()
        .reduce((accumulator, key) => {
            accumulator[key] = obj[key];

            return accumulator;
        }, {});

    return sorted;

}

function Pythagoras(x1, y1, x2, y2) {
    return Math.sqrt(Math.pow((x1 - x2), 2) + Math.pow((y1 - y2), 2));
}

function fitMapToExtents(minLat, minLon, maxLat, maxLon) {
    let southWest = L.latLng(minLat, minLon);
    let northEast = L.latLng(maxLat, maxLon);
    let bounds = L.latLngBounds(southWest, northEast);
    mymap.fitBounds(bounds);
}


function RotatePoint(Xold, Yold, XOrigin, YOrigin, degrees) {
    let r, theta, dY, dX;
    let result = [];
    //roteert een punt ten opzichte van zijn oorsprong
    dY = (Yold - YOrigin);
    dX = (Xold - XOrigin);
    r = Math.sqrt(Math.pow(dX, 2), Math.pow(dY, 2));
    if (dX === 0) { dX = 0.00000000000001 };
    theta = Math.atan2(dY, dX);
    result[0] = r * Math.cos(theta - DEG2RAD(degrees)) + XOrigin;
    result[1] = r * Math.sin(theta - DEG2RAD(degrees)) + YOrigin;
    return result;
}

function getColorFromGradient(value, Gradient) {
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

function getDifferenceBetweenTwoDatesInHours(refdate, comparedate) {
    let diffInMilliseconds = comparedate.getTime() - refdate.getTime();
    let diffInHours = diffInMilliseconds / (1000 * 60 * 60);  // 6
    return diffInHours;
}

//------------------------------------------------------------------------------------------------------------------------------
//                      Writing SVG ICONS
//------------------------------------------------------------------------------------------------------------------------------
function SVGICONHEADER(ScaleFactor) { return "<svg width='" + 20 * ScaleFactor + "' height='" + ScaleFactor * 20 + "' xmlns='http://www.w3.org/2000/svg'>" }
function SVGICONFOOTER() { return "</svg>" }
function SVGSHAPEPILLARBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M 0 0 l " + 3 * ScaleFactor + " " + 6 * ScaleFactor + " l 0 " + -4 * ScaleFactor + " l " + 6 * ScaleFactor + " 0 l 0 " + 8 * ScaleFactor + " l " + 2 * ScaleFactor + " 0 " + " l 0 " + -8 * ScaleFactor + " l " + 6 * ScaleFactor + " 0 l " + " 0 " + 4 * ScaleFactor + " l " + 3 * ScaleFactor + " " + -6 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGSHAPEABUTMENTBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M 0 0 l " + 3 * ScaleFactor + " " + 6 * ScaleFactor + " l 0 " + -4 * ScaleFactor + " l " + 14 * ScaleFactor + " " + 0 + " l " + " 0 " + 4 * ScaleFactor + " l " + 3 * ScaleFactor + " " + -6 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGABUTMENTBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEABUTMENTBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGPILLARBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEPILLARBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGSHAPECIRCLE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<circle cx='" + 10 * ScaleFactor + "' cy='" + 5 * ScaleFactor + "' r='" + 3 * ScaleFactor + "' fill-opacity='0.5' fill='kleur' stroke='kleur' stroke-width='" + StrokeWidth + "'/>" }
function SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M 0 0 l " + 5 * ScaleFactor + " " + 10 * ScaleFactor + " l " + 10 * ScaleFactor + " 0 " + " l " + 5 * ScaleFactor + " " + -10 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGTRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGSHAPETRAPEZIUMBLUNT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M " + 1 * ScaleFactor + " 0 l 0 " + 2 * ScaleFactor + " l " + 4 * ScaleFactor + " " + 8 * ScaleFactor + " l " + 10 * ScaleFactor + " " + 0 + " l " + 4 * ScaleFactor + " " + -8 * ScaleFactor + " l " + 0 + " " + -2 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGTRAPEZIUMBLUNT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMBLUNT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGCULVERT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMBLUNT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPECIRCLE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGPILLARBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEPILLARBRIDGE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGSHAPERECTANGULARWEIR(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M " + 2 * ScaleFactor + " " + 4 * ScaleFactor + " l " + 3 * ScaleFactor + " " + 6 * ScaleFactor + " l " + 10 * ScaleFactor + " 0 l " + 3 * ScaleFactor + " " + -6 * ScaleFactor + " l " + -5 * ScaleFactor + " 0 l 0 " + 3 * ScaleFactor + " l " + -6 * ScaleFactor + " 0 l 0 " + -3 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 7 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 13 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGRECTANGULARWEIR(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPERECTANGULARWEIR(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGORIFICE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEORIFICE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGPUMP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMBLUNT(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPECIRCLE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEPUMP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGFISH(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGSHAPEFISH(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGTRAPEZIUM(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPETRAPEZIUMSHARP(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }
function SVGSHAPEYZ(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return "<path d='M " + 1 * ScaleFactor + " 0 " + " l " + 0 * ScaleFactor + " " + 3 * ScaleFactor + " l " + 1 * ScaleFactor + " " + 3 * ScaleFactor + " l " + 1 * ScaleFactor + " " + 2 * ScaleFactor + " l " + 2 * ScaleFactor + " " + 2 * ScaleFactor + " l " + 10 * ScaleFactor + " 0 " + " l " + 2 * ScaleFactor + " " + -2 * ScaleFactor + " l " + 1 * ScaleFactor + " " + -2 * ScaleFactor + " l " + 1 * ScaleFactor + " " + -3 * ScaleFactor + " l " + 0 * ScaleFactor + " " + -3 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" }
function SVGYZ(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) { return SVGICONHEADER(ScaleFactor) + SVGSHAPEYZ(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) + SVGICONFOOTER() }


function SVGSHAPEPUMP(ScaleFactor, StrokeColor, StrokeWidth) {
    //this function draws the blades of our pump shape in bézier curves
    //definitie van een Bézier curve in SVG: C X1 Y1 X2 Y2 x y waarbij: X1,Y1 = handle van het startpunt, X2,Y2 = handle van het eindpunt, x,y = eindpunt
    let i;
    let SVGSTRING = "";

    for (i = 0; i < 6; i++) {
        //define the points that define our bézier curve
        XYstart = [10 * ScaleFactor, 5 * ScaleFactor];
        XYend = [(10 + 3) * ScaleFactor, 5 * ScaleFactor];
        XYstartHandle = [XYstart[0] + ScaleFactor / 2, XYstart[1] - ScaleFactor / 2];
        XYendHandle = [XYend[0] - ScaleFactor / 2, XYend[1] - ScaleFactor / 2];
        let svg = SVGBEZIERCURVE(360 / 6 * i, XYstart, XYend, XYstartHandle, XYendHandle, StrokeColor, StrokeWidth);
        SVGSTRING = SVGSTRING + svg;
    }
    return SVGSTRING
}

function SVGBEZIERCURVE(RotationDegrees, XYstart, XYend, XYstartHandle, XYendHandle, StrokeColor, StrokeWidth) {
    //first rotate the endpoint and the handles, if required
    let XYEndRotated = RotatePoint(XYend[0], XYend[1], XYstart[0], XYstart[1], RotationDegrees);
    let XY1Rotated = RotatePoint(XYstartHandle[0], XYstartHandle[1], XYstart[0], XYstart[1], RotationDegrees);
    let XY2Rotated = RotatePoint(XYendHandle[0], XYendHandle[1], XYstart[0], XYstart[1], RotationDegrees);
    return "<path d='M " + XYstart[0] + " " + XYstart[1] + " C " + XY1Rotated[0] + " " + XY1Rotated[1] + " " + XY2Rotated[0] + " " + XY2Rotated[1] + " " + XYEndRotated[0] + " " + XYEndRotated[1] + " ' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "' fill='transparent'/>";
    //return "<path d='M " + XYstart[0] + " " + XYstart[1] + " C " + XYstartHandle[0] + " " + XYstartHandle[1] + " " + XYendHandle[0] + " " + XYendHandle[1] + " " + XYend[0] + " " + XYend[1] + " ' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "' fill='transparent'/>"
}

function SVGSHAPEFISH(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) {
    let SVGSTRING;
    let Xstart, Xend, Ystart, Yend, X1, X2, Y1, Y2;

    //buik
    Xstart = 5 * ScaleFactor
    Ystart = (5 + 2) * ScaleFactor
    X1 = (5 + 1) * ScaleFactor
    Y1 = 5 * ScaleFactor
    Xend = 15 * ScaleFactor
    Yend = 5 * ScaleFactor
    X2 = (15 - 2) * ScaleFactor
    Y2 = (5 - 4) * ScaleFactor
    SVGSTRING = "<path d='M " + Xstart + " " + Ystart + " C " + X1 + " " + Y1 + " " + X2 + " " + Y2 + " " + Xend + " " + Yend

    //rug
    Xstart = 5 * ScaleFactor
    Ystart = (5 - 2) * ScaleFactor
    X1 = (5 + 1) * ScaleFactor
    Y1 = 5 * ScaleFactor
    Xend = 15 * ScaleFactor
    Yend = 5 * ScaleFactor
    X2 = (15 - 2) * ScaleFactor
    Y2 = (5 + 4) * ScaleFactor
    SVGSTRING = SVGSTRING + " C " + X2 + " " + Y2 + " " + X1 + " " + Y1 + " " + Xstart + " " + Ystart + " Z'" + " stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "'/>"
    return SVGSTRING;
}

function SVGSHAPEORIFICE(ScaleFactor, FillColor, FillOpacity, StrokeColor, StrokeWidth) {
    //we schematize an orifice using a shape and a rectangle
    let SVGSTRING;
    SVGSTRING = "<path d='M 0 0 l " + 7 * ScaleFactor + " " + 0 + " l 0 " + 8 * ScaleFactor + " l " + 6 * ScaleFactor + " 0 l 0 " + -8 * ScaleFactor + " l " + 7 * ScaleFactor + " 0 l " + -5 * ScaleFactor + " " + 10 * ScaleFactor + " l " + -10 * ScaleFactor + " 0 " + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 7 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 13 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>"
    return SVGSTRING + "<path d='M " + 7 * ScaleFactor + " 0 l 0 " + 4 * ScaleFactor + " l " + 6 * ScaleFactor + " 0 l 0 " + -4 * ScaleFactor + " Z' fill-opacity='" + FillOpacity + "' fill='" + FillColor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 7 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>" + "<path d='M " + 13 * ScaleFactor + " " + 5 * ScaleFactor + " l 0 " + 5 * ScaleFactor + "' stroke='" + StrokeColor + "' stroke-width='" + StrokeWidth + "'/>"
}




//------------------------------------------------------------------------------------------------------------------------------

function WGS842BESSEL(PhiWGS, LamWGS) {
    let dphi, dlam, phicor, lamcor;

    dphi = PhiWGS - 52;
    dlam = LamWGS - 5;
    phicor = (-96.862 - dphi * 11.714 - dlam * 0.125) * 0.00001;
    lamcor = (dphi * 0.329 - 37.902 - dlam * 14.667) * 0.00001;
    let phi = PhiWGS - phicor;
    let lambda = LamWGS - lamcor;
    return [phi, lambda];
}

function BESSEL2RD(phiBes, lamBes) {

    //converteert Lat/Long van een Bessel-functie naar X en Y in RD
    //code is geheel gebaseerd op de routines van Ejo Schrama's software:
    //schrama@geo.tudelft.nl
    let d_1, d_2, r, sa, ca, cpsi, spsi;
    let b, dl, w, q;
    let dq, phi, lambda, s2psihalf, cpsihalf, spsihalf;
    let tpsihalf;

    let x0 = 155000;
    let y0 = 463000;
    let k = 0.9999079;
    let bigr = 6382644.571;
    let m = 0.003773953832;
    let n = 1.00047585668;

    let pi = Math.PI;
    let lambda0 = pi * 0.0299313271611111;
    let phi0 = pi * 0.289756447533333;
    let l0 = pi * 0.0299313271611111;
    let b0 = pi * 0.289561651383333;

    let e = 0.08169683122;
    let a = 6377397.155;

    phi = phiBes / 180 * pi;
    lambda = lamBes / 180 * pi;
    q = Math.log(Math.tan(phi / 2 + pi / 4));
    dq = e / 2 * Math.log((e * Math.sin(phi) + 1) / (1 - e * Math.sin(phi)));
    q = q - dq;
    w = n * q + m;
    b = Math.atan(Math.pow(Math.exp(1), w)) * 2 - pi / 2;
    dl = n * (lambda - lambda0);
    d_1 = Math.sin((b - b0) / 2);
    d_2 = Math.sin(dl / 2);
    s2psihalf = d_1 * d_1 + d_2 * d_2 * Math.cos(b) * Math.cos(b0);
    cpsihalf = Math.sqrt(1 - s2psihalf);
    spsihalf = Math.sqrt(s2psihalf);
    tpsihalf = spsihalf / cpsihalf;
    spsi = spsihalf * 2 * cpsihalf;
    cpsi = 1 - s2psihalf * 2;
    sa = Math.sin(dl) * Math.cos(b) / spsi;
    ca = (Math.sin(b) - Math.sin(b0) * cpsi) / (Math.cos(b0) * spsi);
    r = k * 2 * bigr * tpsihalf;
    let X = Math.round(r * sa + x0);
    let Y = Math.round(r * ca + y0);
    return [X, Y];
}

function getAngle(latLng1, latLng2, coef) {
    var dy = latLng2.lat - latLng1.lat;
    var dx = Math.cos(Math.PI / 180 * latLng1.lat) * (latLng2.lng - latLng1.lng);
    var ang = ((Math.atan2(dy, dx) / Math.PI) * 180 * coef);
    return (ang).toFixed(2);
}

function distanceTo(p1, p2) {
    var x = p2.x - p1.x,
        y = p2.y - p1.y;

    return Math.sqrt(x * x + y * y);
}

function toPoint(x, y, round) {
    if (x instanceof Point) {
        return x;
    }
    if (x.isArray()) {
        return new Point(x[0], x[1]);
    }
    if (x === undefined || x === null) {
        return x;
    }
    if (typeof x === 'object' && 'x' in x && 'y' in x) {
        return new Point(x.x, x.y);
    }
    return new Point(x, y, round);
}

function tableRemoveAllRows(table) {
    let tableHeaderRowCount = 1;
    let rowCount = table.rows.length;
    for (var i = tableHeaderRowCount; i < rowCount; i++) {
        table.deleteRow(tableHeaderRowCount);
    }
}

function RoundNumber(value, nDecimals) {
    return Math.round((value + Number.EPSILON) * Math.pow(10, nDecimals)) / Math.pow(10, nDecimals);
}

function Point(x, y, round) {
    this.x = (round ? Math.round(x) : x);
    this.y = (round ? Math.round(y) : y);
}

function interpolate(x1, y1, x2, y2, x3) {
    //this function interpolates between two xy value pairs. 
    //note: it does NOT extrapolate
    if (x3 <= Math.min(x1, x2)) {
        return y1;
    } else if (x3 >= Math.max(x1, x2)) {
        return y2;
    } else {
        let teller1 = (Number(y2) - Number(y1));
        let teller2 = (Number(x3) - Number(x1));
        let noemer = (Number(x2) - Number(x1));
        return Number(y1) + teller1 * teller2 / noemer;
    }
}

function interpolateRGB(x1, R1, G1, B1, x2, R2, G2, B2, x3) {
    let R = Math.round(interpolate(x1, R1, x2, R2, x3), 0);
    let G = Math.round(interpolate(x1, G1, x2, G2, x3), 0);
    let B = Math.round(interpolate(x1, B1, x2, B2, x3), 0);
    return 'rgb(' + R + ',' + G + ',' + B + ')';
}

function RGBfromValues(R, G, B) {
    return 'rgb(' + R + ',' + G + ',' + B + ')';
}

//#########################################################################//
//snippet from https://gist.github.com/IceCreamYou/6ffa1b18c4c8f6aeaad2    //
// Returns the value at a given percentile in a sorted numeric array.
// "Linear interpolation between closest ranks" method
//#########################################################################//
function percentile(arr, p) {
    if (arr.length === 0) return 0;
    arr.sort(function (a, b) { return a - b });	//sorting numeric in ascending order
    if (typeof p !== 'number') throw new TypeError('p must be a number');
    if (p <= 0) return arr[0];
    if (p >= 1) return arr[arr.length - 1];

    var index = (arr.length - 1) * p,
        lower = Math.floor(index),
        upper = lower + 1,
        weight = index % 1;

    if (upper >= arr.length) return arr[lower];
    return arr[lower] * (1 - weight) + arr[upper] * weight;
}

// Returns the percentile of the given value in a sorted numeric array.
function percentRank(arr, v) {
    if (typeof v !== 'number') throw new TypeError('v must be a number');
    for (var i = 0, l = arr.length; i < l; i++) {
        if (v <= arr[i]) {
            while (i < l && v === arr[i]) i++;
            if (i === 0) return 0;
            if (v !== arr[i - 1]) {
                i += (v - arr[i - 1]) / (arr[i] - arr[i - 1]);
            }
            return i / l;
        }
    }
    return 1;
}
//#########################################################################//



function LineAngleDegrees(X1, Y1, X2, Y2) {
    //berekent de hoek van een lijn tussen twee xy coordinaten
    let dx, dy;
    dX = Math.abs(X2 - X1)
    dY = Math.abs(Y2 - Y1)

    if (dX === 0) {
        if (dY === 0) {
            return 0;
        } else if (Y2 > Y1) {
            return 0;
        } else if (Y2 < Y1) {
            return 180;
        }
    } else if (dY === 0) {
        if (dX > 0) {
            return 90;
        } else if (dX < 0) {
            return 270;
        }
    } else {
        if (X2 > X1 && Y2 > Y1) {
            //eerste kwadrant
            return R2D(Math.atan(dX / dY));
        } else if (X2 > X1 && Y2 < Y1) {
            //tweede kwadrant
            return 90 + R2D(Math.atan(dY / dX));
        } else if (X2 < X1 && Y2 < Y1) {
            //derde kwadrant
            return 180 + R2D(Math.atan(dY / dX));
        } else {
            return 270 + R2D(Math.atan(dY / dX));
        }
    }
}

function R2D(angle) {
    //radialen naar graden
    return angle * 180 / Math.PI;
}


function D2R(angle) {
    //graden naar radialen
    return angle / 180 * Math.PI
}



// function RotatePoint(Xold, Yold, Xorigin, Yorigin, Degrees, XYNew) {
//     //rotating a point around a given origin
//     let r, dy, dx;

//     dy = (Yold - Yorigin);
//     dx = (Xold - Xorigin);
//     r = Math.sqrt(Math.pow(dx, 2) + Math.pow(dy, 2));

//     let curangle = LineAngleDegrees(Xorigin, Yorigin, Xold, Yold);
//     let newangle = curangle + Degrees;

//     dx = Math.sin(D2R(newangle)) * r;
//     dy = Math.cos(D2R(newangle)) * r;

//     XYNew[0] = Xorigin + dx;
//     XYNew[1] = Yorigin + dy;
//     return true;
// }



