function drawWeirSchematicFrontView(CanvasName, CanvasWidth, CanvasHeight, TotalWidth, CrestWidth, SurfaceElevation, ShoulderElevation, BedLevel, WinterTargetLevel, SummerTargetLevel, CrestMin, CrestMax, BedLevelInvalid, SurfaceElevationInvalid) {


    //first thing to do is define the relationship between physics coordinates and canvas coordinates
    let PixelsToShoulderLeftSide = CanvasWidth * 0.1;
    let PixelsToShoulderRightSide = CanvasWidth * 0.70;
    let PixelsToShoulder = CanvasHeight * 0.3;
    let PixelsToBedLevel = CanvasHeight * 0.85;
    let bx = PixelsToShoulderLeftSide;
    let ax = (PixelsToShoulderRightSide - bx) / TotalWidth;
    let ay = (PixelsToShoulder - PixelsToBedLevel) / (ShoulderElevation - BedLevel);
    let by = PixelsToBedLevel - ay * BedLevel;

    let x;
    let y;
    let dx;
    let dy;
    let xSec;
    let xStart, yStart, xEnd, yEnd;

    let canvas = document.getElementById(CanvasName);

    //create a color gradient for our sky and draw
    let grd = canvas.getContext("2d").createLinearGradient(0, CanvasWidth * 0.3, 0, 0);
    grd.addColorStop(0, "#6FA8DB");
    grd.addColorStop(1, "#DDEBF7");
    let sky = canvas.getContext("2d");
    sky.fillStyle = grd;
    sky.fillRect(0, 0, CanvasWidth, CanvasHeight);

    //drawing the left rectangle representing ground surface
    x = 0;
    y = ay * SurfaceElevation + by;
    dx = ax * 0 + bx;
    dy = CanvasHeight;
    let leftSurface = canvas.getContext("2d");
    //leftSurface.fillStyle = "#548235";
    leftSurface.fillStyle = "#62983E";
    leftSurface.fillRect(x, y, dx, dy);

    // let ctx = canvas.getContext("2d");
    // let img = document.getElementById("grass");
    // ctx.drawImage(img, 100, 100);

    //drawing the right rectangle representing ground surface
    x = ax * TotalWidth + bx;
    y = ay * SurfaceElevation + by;
    dx = CanvasWidth - x;
    dy = CanvasHeight - y;
    let rightSurface = canvas.getContext("2d");
    rightSurface.fillStyle = "#62983E";
    rightSurface.fillRect(x, y, dx, dy);

    //drawing the bottom
    x = 0;
    y = ay * BedLevel + by;
    dx = CanvasWidth;
    dy = CanvasHeight - y;
    let bottomSurface = canvas.getContext("2d");
    bottomSurface.fillStyle = "62983E";
    bottomSurface.fillRect(x, y, dx, dy);


    // drawing the left triangle
    let leftWing = canvas.getContext("2d");
    leftWing.beginPath();
    x = ax * 0 + bx;
    y = ay * ShoulderElevation + by;
    leftWing.moveTo(x, y);
    x = ax * (TotalWidth - CrestWidth) / 2 + bx;
    y = ay * BedLevel + by;
    leftWing.lineTo(x, y);
    x = ax * 0 + bx;
    y = ay * BedLevel + by;
    leftWing.lineTo(x, y);
    leftWing.fillStyle = "62983E";
    leftWing.fill();

    //drawing the right triangle
    let rightWing = canvas.getContext("2d");
    rightWing.beginPath();
    x = ax * TotalWidth + bx;
    y = ay * ShoulderElevation + by;
    rightWing.moveTo(x, y);
    y = ay * BedLevel + by;
    rightWing.lineTo(x, y);
    x = ax * (TotalWidth - (TotalWidth - CrestWidth) / 2) + bx;
    rightWing.lineTo(x, y);
    rightWing.fillStyle = "62983E";
    rightWing.fill();

    //drawing the left panel
    //note: we must find the x intersection point for panel and slope 
    let leftPanel = canvas.getContext("2d");
    //xSec = interpolate(SurfaceElevation, 0, BedLevel, (TotalWidth - CrestWidth) / 2, ShoulderElevation);
    leftPanel.beginPath();
    x = ax * 0 + bx;
    y = ay * ShoulderElevation + by;
    rightWing.moveTo(x, y);
    x = ax * (TotalWidth - CrestWidth) / 2 + bx;
    rightWing.lineTo(x, y);
    y = ay * BedLevel + by;
    rightWing.lineTo(x, y);
    rightWing.fillStyle = "#9D623D";
    rightWing.fill();

    //drawing the right panel
    let rightPanel = canvas.getContext("2d");
    //xSec = interpolate(SurfaceElevation, TotalWidth, BedLevel, (TotalWidth - (TotalWidth - CrestWidth) / 2), ShoulderElevation);
    rightPanel.beginPath();
    x = ax * TotalWidth + bx;
    y = ay * ShoulderElevation + by;
    rightWing.moveTo(x, y);
    x = ax * (TotalWidth - (TotalWidth - CrestWidth) / 2) + bx;
    rightWing.lineTo(x, y);
    y = ay * BedLevel + by;
    rightWing.lineTo(x, y);
    rightWing.fillStyle = "#9D623D";
    rightWing.fill();

    //drawing the bottom construction, between bed level and minimum crest level
    x = ax * (TotalWidth - CrestWidth) / 2 + bx;
    y = ay * CrestMin + by;
    dx = ax * (TotalWidth - (TotalWidth - CrestWidth) / 2) + bx - x;
    dy = BedLevel * ay + by - y;
    let bottomConstruction = canvas.getContext("2d");
    bottomConstruction.fillStyle = "#9D623D";
    bottomConstruction.fillRect(x, y, dx, dy);

    //drawing the water column between CrestMin and SummerTargetLevel
    let water = canvas.getContext("2d");
    y = ay * SummerTargetLevel + by;
    dy = ay * CrestMin + by - y
    water.fillStyle = "#15A7C9";
    water.fillRect(x, y, dx, dy);

    //drawing the range between maximum crest level and summer target level
    let klep = canvas.getContext("2d");
    y = ay * CrestMax + by;
    dy = ay * SummerTargetLevel + by - y;
    bottomConstruction.globalAlpha = 1;
    bottomConstruction.fillStyle = "#C5C3C3";
    bottomConstruction.fillRect(x, y, dx, dy);


    //drawing the range between minimum crest level and summer target level
    // let minmax = canvas.getContext("2d");
    // y = ay * SummerTargetLevel + by;
    // dy = ay * CrestMin + by - y;
    // bottomConstruction.globalAlpha = 0.5;
    // bottomConstruction.fillStyle = "#9D543D";
    // bottomConstruction.fillRect(x, y, dx, dy);

    // //drawing a line at the max crest level
    // xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    // yStart = ay * CrestMax + by;
    // xEnd = ax * (TotalWidth - (TotalWidth - CrestWidth)/2) + bx;
    // yEnd = yStart;
    // drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 2, "#9D623D", 10, 90, []);


    //add a line measurement and text containing shoulder elevation
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * ShoulderElevation + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);

    let ShoulderTxt = canvas.getContext("2d");
    ShoulderTxt.font = "16px Calibri light";
    ShoulderTxt.globalAlpha = 1;
    ShoulderTxt.fillStyle = "white";
    ShoulderTxt.textAlign = "right";
    ShoulderTxt.fillText("Shoulder elevation: " + RoundNumber(ShoulderElevation,2) + " m + NAP", CanvasWidth - 10, ay * ShoulderElevation + by - 8);   //center our text along the left panel

    //add text containing surface elevation
    let SurfaceTxt = canvas.getContext("2d");
    SurfaceTxt.font = "16px Calibri light";
    SurfaceTxt.fillStyle = "white";
    SurfaceTxt.textAlign = "right";
    SurfaceTxt.fillText("Surface elevation: " + RoundNumber(SurfaceElevation,2) + " m + NAP", CanvasWidth - 10, ay * SurfaceElevation + by - 8);   //center our text along the left panel

    //add a line measurement + text for the total width
    xStart = ax * 0 + bx;
    yStart = CanvasHeight * 0.2;
    xEnd = ax * TotalWidth + bx;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, true, true, 1, 1, "white", 5, 90 ,[]);

    let twTxt = canvas.getContext("2d");
    twTxt.font = "16px Calibri light";
    twTxt.fillStyle = "white";
    twTxt.textAlign = "center";
    twTxt.fillText("Total width: " + RoundNumber(TotalWidth,2) + " m", (TotalWidth) / 2 * ax + bx, CanvasHeight * 0.2 - 10);   //center our text along the left panel

    //add a line measurement + text for the crest width
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = CanvasHeight * 0.87;
    xEnd = ax * (TotalWidth - (TotalWidth - CrestWidth) / 2) + bx;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, true, true, 1, 1, "white", 5, 90, []);

    let cwTxt = canvas.getContext("2d");
    cwTxt.font = "16px Calibri light";
    cwTxt.fillStyle = "white";
    cwTxt.textAlign = "center";
    cwTxt.fillText("Crest width: " + RoundNumber(CrestWidth,2) + " m", (TotalWidth) / 2 * ax + bx, CanvasHeight * 0.87 + 20);   //center our text along the left panel

    //add a line measurement + text for the summer target level
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * SummerTargetLevel + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);


    let zpTxt = canvas.getContext("2d");
    zpTxt.font = "16px Calibri light";
    zpTxt.fillStyle = "white";
    zpTxt.textAlign = "right";
    zpTxt.fillText("Summer Target Level: " + RoundNumber(SummerTargetLevel,2) + " m + NAP", CanvasWidth - 10, ay * SummerTargetLevel + by-8);   //center our text along the left panel


    //add a line measurement + text for the winter target level
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * WinterTargetLevel + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);


    let wpTxt = canvas.getContext("2d");
    wpTxt.font = "16px Calibri light";
    wpTxt.fillStyle = "white";
    wpTxt.textAlign = "right";
    wpTxt.fillText("Winter Target Level: " + RoundNumber(WinterTargetLevel,2) + " m + NAP", CanvasWidth - 10, ay * WinterTargetLevel + by-8);   //center our text along the left panel

    //add a line measurement + text for the minimum crest level
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * CrestMin + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);

    let mcTxt = canvas.getContext("2d");
    mcTxt.font = "16px Calibri light";
    mcTxt.fillStyle = "white";
    mcTxt.textAlign = "right";
    mcTxt.fillText("Minimum crest level: " + RoundNumber(CrestMin,2) + " m + NAP", CanvasWidth - 10, ay * CrestMin + by - 8);   //center our text along the left panel

    //add a line measurement + text for the maximum crest level
    xStart = ax *  (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * CrestMax + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);

    let maxcTxt = canvas.getContext("2d");
    maxcTxt.font = "16px Calibri light";
    maxcTxt.fillStyle = "white";
    maxcTxt.textAlign = "right";
    maxcTxt.fillText("Maximum crest level: " + RoundNumber(CrestMax,2) + " m + NAP", CanvasWidth - 10, ay * CrestMax + by -8);   //center our text along the left panel

    //add a line measurement + text for the  bed level
    xStart = ax * (TotalWidth - CrestWidth) / 2 + bx;
    yStart = ay * BedLevel + by;
    xEnd = CanvasWidth;
    yEnd = yStart;
    drawArrow("myCanvas", xStart, yStart, xEnd, yEnd, false, false, 1, 1, "white", 10, 90, [5, 15]);

    let BedTxt = canvas.getContext("2d");
    BedTxt.font = "16px Calibri light";
    BedTxt.fillStyle = "white";
    BedTxt.textAlign = "right";
    BedTxt.fillText("Bed level: " + RoundNumber(BedLevel,2) + " m + NAP", CanvasWidth - 10, ay * BedLevel + by - 8);   //center our text along the left panel

    //****************************************************************************************************************************************/
    //ADDING VERDICT MARKERS
    //****************************************************************************************************************************************/
    //draw a line along the bottom representing the verdict
    if (BedLevelInvalid) {
        xStart = ax * (TotalWidth - CrestWidth)/2 + bx;
        xEnd = ax * (TotalWidth - (TotalWidth - CrestWidth)/2) + bx;
        yStart = ay * BedLevel + by;
        yEnd = yStart;
        drawArrow("myCanvas", xStart,yStart,xEnd, yEnd, false,false,1,3,"red",0,0,[]);    
    }

    if (SurfaceElevationInvalid) {
        'surface elevation left side'
        xStart = 0;
        xEnd = ax * 0 + bx;
        yStart = ay * SurfaceElevation + by;
        yEnd = yStart;
        drawArrow("myCanvas", xStart,yStart,xEnd, yEnd, false,false,1,3,"red",0,0,[]);    

        'surface elevation right side'
        xStart = ax * TotalWidth + bx;
        xEnd = CanvasWidth;
        yStart = ay * SurfaceElevation + by;
        yEnd = yStart;
        drawArrow("myCanvas", xStart,yStart,xEnd, yEnd, false,false,1,3,"red",0,0,[]);    
    }

}

function drawArrow(CanvasName, xStart, yStart, xEnd, yEnd, startArrow, endArrow, alpha, lineWidth, strokeColor, ArrowSize, PointAngle, DashArray) {
    //notice: for a solid line: use an empy DashArray

    //first retrieve the angle of our arrow
    let angle = LineAngleDegrees(xStart, yStart, xEnd, yEnd);

    //draw the line
    let canvas = document.getElementById(CanvasName);
    let myArrow = canvas.getContext("2d");
    myArrow.beginPath();
    myArrow.setLineDash(DashArray);
    myArrow.moveTo(xStart, yStart);
    myArrow.lineTo(xEnd, yEnd);
    myArrow.globalAlpha = alpha;
    myArrow.strokeStyle = strokeColor;
    myArrow.lineWidth = lineWidth;
    myArrow.stroke();

    //draw the arrow heads
    let ArrowHead;
    if (startArrow == true) {
        ArrowHead = canvas.getContext("2d");
        ArrowHead.beginPath();
        ArrowHead.globalAlpha = alpha;
        ArrowHead.strokeStyle = strokeColor;
        ArrowHead.lineWidth = lineWidth;

        let xArrow, yArrow;              //define the to-coords
        xArrow = xStart;
        yArrow = yStart + ArrowSize;    //initialize the arrow head to be vertical
        let XYRotated = [xArrow, yArrow]; //put the coordinates in an array so they can be passed byref
        RotatePoint(xArrow, yArrow, xStart, yStart, angle + PointAngle, XYRotated);         //rotate this point angle + 45 degrees
        ArrowHead.moveTo(xStart, yStart);
        ArrowHead.lineTo(XYRotated[0], XYRotated[1]);
        ArrowHead.stroke();

        RotatePoint(xArrow, yArrow, xStart, yStart, angle - PointAngle, XYRotated);         //rotate this point angle + 45 degrees
        ArrowHead.moveTo(xStart, yStart);
        ArrowHead.lineTo(XYRotated[0], XYRotated[1]);
        ArrowHead.stroke();
    }

    if (endArrow == true) {
        ArrowHead = canvas.getContext("2d");
        ArrowHead.beginPath();
        ArrowHead.globalAlpha = alpha;
        ArrowHead.strokeStyle = strokeColor;
        ArrowHead.lineWidth = lineWidth;

        let xArrow, yArrow;              //define the to-coords
        xArrow = xEnd;
        yArrow = yEnd + ArrowSize;    //initialize the arrow head to be vertical
        let XYRotated = [xArrow, yArrow]; //put the coordinates in an array so they can be passed byref
        RotatePoint(xArrow, yArrow, xEnd, yEnd, angle + 180 + PointAngle, XYRotated);         //rotate this point angle + 45 degrees
        ArrowHead.moveTo(xEnd, yEnd);
        ArrowHead.lineTo(XYRotated[0], XYRotated[1]);
        ArrowHead.stroke();

        RotatePoint(xArrow, yArrow, xEnd, yEnd, angle + 180 - PointAngle, XYRotated);         //rotate this point angle + 45 degrees
        ArrowHead.moveTo(xEnd, yEnd);
        ArrowHead.lineTo(XYRotated[0], XYRotated[1]);
        ArrowHead.stroke();
    }
}