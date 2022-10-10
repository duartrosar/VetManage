let startHidden;
let endHidden;
let dateOnly;
let form;
let scheduler;

let isEditing;
let inputs = document.querySelectorAll(".form-control");

document.addEventListener('DOMContentLoaded', function () {
    $.validator.setDefaults({
        ignore: ""
    });

    startHidden = $("#startHidden");
    console.log(startHidden);
    endHidden = $("#endHidden");
    form = $("#appointmentForm");
    scheduler = $("#schedule");

    startTime = $("#startTime");
    endTime = $("#endTime");
    setDropdownTimes();
    isEditing = false;


    $("#enableEditAppointment").click(enableEditAppointment);
});


// Set the allowed times for the start time drop down
// Allowed times are from 7:00 to 17:00, in 30 min steps
function setDropdownTimes() {
    startTime.timepicker({
        'timeFormat': 'H:i',
        'minTime': '7:00',
        'maxTime': '17:30',
        'step': 30,
        'listWidth': 1,
    });

    endTime.timepicker({
        'timeFormat': 'H:i',
        'minTime': '7:00',
        'maxTime': '18:00',
        'step': 30,
        'listWidth': 1,
    });

    startTime.change(onStartTimeChange);
    endTime.change(onEndTimeChange);
}

function onPopupOpen(args) {
    console.log(args);
    // cancel the default modal from the scheduler
    args.cancel = true;

    let title = $("#appointmentFormTitle");

    // if the the user clicked an appointment
    if (args.data.Id === undefined) {
        clearInputs(inputs);
        isEditing = false;
        action = "new";
    } else {
        populateForm(inputs, args.data);
        isEditing = true;
        endTime.val(getShortTimeString(args.data.EndTime));
        action = "read";
    }

    // Hide or Show the buttons depending on the action
    let readonly = organizeButtons(action, "Appointment", title);

    // Set the fields to readonly or not depending on the action
    setReadonly(inputs, readonly);

    // save the date the user has selected
    dateOnly = offsetDateTimezone(args.data.StartTime);

    let currentDate = new Date(startHidden[0].dataset.valPastdateNow);

    //if (!validateDate(currentDate, args.data.StartTime)) {
    //    $("#invalidDateModal").modal("show");
    //    return;
    //}

    let selectedTime;

    // if the user clicked a a cell in the monthly view
    if (args.data.StartTime.getHours() === 0) {
        selectedTime = "7:00";
    } else {
        selectedTime = getShortTimeString(args.data.StartTime);
    }

    startTime.val(selectedTime);

    startHidden.val(offsetDateTimezoneToJson(args.data.StartTime));
    endHidden.val(offsetDateTimezoneToJson(args.data.EndTime));

    onStartTimeChange();

    $("#newAppointmentModal").modal("show");
}

// Set the allowed times for the end time drop down
// The end times are chosen based on the selected starttime
function onStartTimeChange() {
    let selectedTime = $('#startTime').timepicker('getTime');
    let shiftedTime = addMinutes(selectedTime, 30);
    let shiftedTimeString = getShortTimeString(shiftedTime);
    
    endTime.timepicker('option', { 'minTime': shiftedTimeString });

    let fixedStartDate = getFormattedDate(dateOnly, selectedTime);
    let fixedEndDate = getFormattedDate(dateOnly, shiftedTime);

    startHidden.val(offsetDateTimezoneToJson(fixedStartDate));

    if (!isEditing) {
        endTime.val(shiftedTimeString);
        endHidden.val(offsetDateTimezoneToJson(fixedEndDate));
    }

}

function onEndTimeChange() {
    let selectedTime = $('#endTime').timepicker('getTime');
    let selectedTimeString = getShortTimeString(selectedTime);

    let fixedEndDate = getFormattedDate(dateOnly, selectedTime);

    endHidden.val(offsetDateTimezoneToJson(fixedEndDate));
}

function enableEditAppointment() {
    let title = $("#appointmentFormTitle");
    enableEdit("Appointment", title);
    setReadonly(inputs, false);
}

function getShortTimeString(date) {
    let hours = date.getHours();
    let minutes = date.getMinutes();

    if (minutes === 0) {
        return `${hours}:${minutes}0`;
    } else {
        return `${hours}:${minutes}`;
    }
}

// Calculate the new date maintaining the date of the cell the user clicked on
// But changing the time
function getFormattedDate(date, time) {
    let year = date.getFullYear();
    let month = date.getMonth();
    let day = date.getDate();
    let hours = time.getHours();
    let minutes = time.getMinutes();


    return new Date(year, month, day, hours, minutes);
}

//function populateForm(appointment) {
//    console.log(appointment);
//}

///////// HELPERS /////////
function offsetDateTimezoneToJson(date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toJSON();
}

function offsetDateTimezone(date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
}

function validateDate(currentDate, dateToBeChecked) {
    return dateToBeChecked >= currentDate;
}

function addMinutes(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}

function onEventClick(args) {
    console.log(args);
}