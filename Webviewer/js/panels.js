
  function openLeftPanel() {
    document.getElementById("leftpanel").style.width = "250px";
    document.getElementById("legendcontainer").style.top = "10px";
    document.getElementById("legendcontainer").style.left = "260px";
    document.getElementById("openLeftPanel").style.visibility = "hidden";
  }
  function closeLeftPanel() {
    document.getElementById("leftpanel").style.width = "0";
    document.getElementById("legendcontainer").style.left = "10px";
    document.getElementById("legendcontainer").style.top = "65px";
    document.getElementById("openLeftPanel").style.visibility = "visible";
  }
  function openRightPanel() {
    document.getElementById("rightpanel").classList.remove('closed');
  }
  function closeRightPanel() {
    document.getElementById("rightpanel").classList.add('closed');
  }


  var coll = document.getElementsByClassName("collapsible");
  var i;

  for (i = 0; i < coll.length; i++) {
  coll[i].addEventListener("click", function() {
    this.classList.toggle("active");
    var content = this.nextElementSibling;
    if (content.style.display === "block") {
      content.style.display = "none";
    } else {
      content.style.display = "block";
    }
  });
}