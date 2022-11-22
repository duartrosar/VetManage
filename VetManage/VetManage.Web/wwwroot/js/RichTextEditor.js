let defaultRTE, formObject, options;
let aspRichtextVal;

function created() {
    options = {
        rules: {
            'RichTextField': {
                required: true
            }
        }
    };
    formObject = new ej.inputs.FormValidator('#formWithRichText', options);
};

window.onload = function () {
    document.getElementById('submitBtn').onclick = function () {
        aspRichTextVal = document.getElementById('richTextVal');
        getValue();
    };
}

function getValue() {
    let isValid = formObject.validate();
    
    if (!isValid) {
        aspRichTextVal.innerHTML = `<span>You must enter this field</span>`
    }
}

function onChange() {
    aspRichTextVal.innerHTML = "";
}

function onDialogOpen(args) {
    args.cancel = true;
}

function onActionBegin(args) {
    if (args.type === 'drop'
        || args.type === 'dragstart'
        || args.requestType === 'Image') {
        args.cancel = true;
    }
}