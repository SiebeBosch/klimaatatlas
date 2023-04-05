
function PopulateParametersTable(result) {

  console.log("Result is ", result);

  var table = document.getElementById("parameterstable");
  tableRemoveAllRows(table);

  // helper functions        
  function addCell(tr, text) {
    var td = tr.insertCell();
    td.textContent = text;
    return td;
  }

  function addRow(item, index) {
    row = table.insertRow();
    console.log("item is ", item);
    console.log("index is ", index);
    addCell(row, item.ID);
    addCell(row, item.OriginalValue);
    addCell(row, item.AssignedValue);
    if (item.AssignedValue != item.OriginalValue) {
      table.rows[table.rows.length - 1].cells[1].style.backgroundColor = "orange";
      table.rows[table.rows.length - 1].cells[2].style.backgroundColor = "aquamarine";
    }
  }

  let row;
  result.parameters.forEach(addRow)

}



function populateRulesTable(result) {
  var table = document.getElementById("rulestable");
  tableRemoveAllRows(table);

  // helper function        
  function addCell(tr, text) {
    var td = tr.insertCell();
    td.textContent = text;
    return td;
  }

  // insert data
  let row;
  result.validationresults.forEach(function (item) {
    //   console.log("item is ", item);
	// only add the row if the rule has actually been evaluated
	if (item.successful.toLowerCase() != "n/a"){
		row = table.insertRow();
		addCell(row, item.condition);
		addCell(row, item.left_parameter);
		addCell(row, item.right_parameter);
		addCell(row, item.successful);
	if (item.applied_penalty != 0){
			addCell(row, item.applied_penalty);
		} else {
			addCell(row, "");		
		}
		if (item.successful.toLowerCase() == "false") {
			table.rows[table.rows.length - 1].cells[3].style.backgroundColor = "orange";
		} else if (item.successful.toLowerCase() == "failed") {
			table.rows[table.rows.length - 1].style.backgroundColor = "lightgrey";
		}
		addCell(row, item.subject_parameter);
		addCell(row, item.original_value);
		if (item.original_value != item.assigned_value) {
			addCell(row, item.assigned_value);
			table.rows[table.rows.length - 1].cells[6].style.backgroundColor = "orange";
			table.rows[table.rows.length - 1].cells[7].style.backgroundColor = "aquamarine";
		} else {
			addCell(row,"");
		}

		if (item.successful == "") {
			table.rows[table.rows.length - 1].cells[3].style.backgroundColor = "orange";
		}
		
	}


  });
}
