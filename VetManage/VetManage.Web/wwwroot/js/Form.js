function logSomething() {
    console.log("Hello World");
}

function organizeButtons(action, entityName, title) {
    let readonly;

    if (action === "read") {
        readonly = true;
        $(`#enableEdit${entityName}`).removeClass("d-none");
        $(`#submitEdit${entityName}`).addClass("d-none");
        $(`#delete${entityName}`).addClass("d-none");
        $(`#create${entityName}`).addClass("d-none");
        title.text("Details");
    } else if (action === "edit") {
        readonly = false;
        $(`#submitEdit${entityName}`).removeClass("d-none");
        $(`#enableEdit${entityName}`).addClass("d-none");
        $(`#delete${entityName}`).addClass("d-none");
        $(`#create${entityName}`).addClass("d-none");
        title.text("Edit");
    } else if (action === "new") {
        setReadonly(false);
        $(`#create${entityName}`).removeClass("d-none");
        $(`#enableEdit${entityName}`).addClass("d-none");
        $(`#submitEdit${entityName}`).addClass("d-none");
        $(`#delete${entityName}`).addClass("d-none");
        title.text("New");
    } else {
        readonly = true;
        $(`#delete${entityName}`).removeClass("d-none");
        $(`#enableEdit${entityName}`).addClass("d-none");
        $(`#submitEdit${entityName}`).addClass("d-none");
        $(`#create${entityName}`).addClass("d-none");
        title.text("Delete");
    }

    return readonly;
}

function newOrganizeButtons(entityName, title) {
    $(`#create${entityName}`).removeClass("d-none");
    $(`#enableEdit${entityName}`).addClass("d-none");
    $(`#submitEdit${entityName}`).addClass("d-none");
    $(`#delete${entityName}`).addClass("d-none");
    title.text("New");
}


function populateForm(formInputs, entity) {
    for (let i = 0; i < formInputs.length; i++) {
        formInputs[i].value = entity[`${formInputs[i].name}`];
    }
}

function clearInputs(formInputs) {
    for (let i = 0; i < formInputs.length; i++) {
        if (formInputs[i].nodeName === "SELECT") {
            formInputs[i].selectedIndex = "0";
        } else if (formInputs[i].name === "Id") {
            formInputs[i].value = 0;
            console.log(formInputs[i].name);
        }
            else {
            formInputs[i].value = "";
        }
    }
}

function setReadonly(formInputs, readonly) {
    for (let i = 0; i < formInputs.length; i++) {
        formInputs[i].readOnly = readonly;
    }
}

function enableEdit(entityName, title) {
    $(`#enableEdit${entityName}`).addClass("d-none");
    $(`#submitEdit${entityName}`).removeClass("d-none");
    title.text("Edit");
}