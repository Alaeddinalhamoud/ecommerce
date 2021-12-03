var i = 0;

function AddNewElement() {
    //Get the reference of the Table's TBODY element.
    var tBody = $("#tblProductSpecification > TBODY")[0];
    //Add Row.
    var row = tBody.insertRow(-1);
    //Add Key cell.
    var cell = $(row.insertCell(-1));
    var inputName = $("<input />");
    inputName.attr("type", "text");
    inputName.attr("class", "form-control form-control-user");
    inputName.attr("name", 'MvProductSpecifications[' + i + '].name');
    inputName.attr("placeholder", "Lable Name");
    inputName.attr("aria-describedby", "Specification Name");
    inputName.attr("required", "true");
    cell.append(inputName);

    //Add Name cell.
    cell = $(row.insertCell(-1));
    var inputValue = $("<input />");
    inputValue.attr("type", "text");
    inputValue.attr("class", "form-control form-control-user");
    inputValue.attr("name", 'MvProductSpecifications[' + i + '].value');
    inputValue.attr("placeholder", "Value");
    inputValue.attr("aria-describedby", "Specification Value");
    inputValue.attr("required", "true");
    cell.append(inputValue);

    //Add Button cell.
    cell = $(row.insertCell(-1));
    var btnRemove = $("<button />");
    btnRemove.attr("type", "button");
    btnRemove.attr("class", "btn btn-danger fas fa-trash-alt");
    btnRemove.attr("onclick", "Remove(this);");
    btnRemove.val("Remove");
    cell.append(btnRemove);

   

    $("#btnDone").addClass("d-none");
    $("#btnAdd").removeClass("d-none");

   
    //Inc i for new item
    i++;

};

function Remove(button) {
    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(0).html();
    if (confirm("Are you sure?")) {
        //Get the reference of the Table.
        var table = $("#tblProductSpecification")[0];
        //Delete the Table row using it's Index.
        table.deleteRow(row[0].rowIndex);
    } 
    i--;

    if (i == 0) {

        $("#btnDone").removeClass("d-none");
        $("#btnAdd").addClass("d-none");
    }
};




 
