//let dateOnly;
//let form = $("#appointmentForm");
//let scheduler = $("#schedule");
let startTimePicker = $("#startTimeString");
let endTimePicker = $("#endTimeString");

//setDropdownTimes();

// Set the allowed times for the start time drop down
// Allowed times are from 7:00 to 17:00, in 30 min steps
function setDropdownTimes(startDate, endDate = null) {

    console.log(endDate);

    // initialize the time pickers
    startTimePicker.timepicker({
        'timeFormat': 'H:i',
        'minTime': '7:00',
        'maxTime': '17:30',
        'step': 30,
        'listWidth': 1,
    });

    endTimePicker.timepicker({
        'timeFormat': 'H:i',
        'minTime': '7:00',
        'maxTime': '18:00',
        'step': 30,
        'listWidth': 1,
    });

    startTimePicker.change(onStartTimeChange);
    endTimePicker.change(onEndTimeChange);

    let startTime = new Date(startDate);

    //get the minimum allowed time for the endTimePicker
    let shiftedTime = addMinutes(startTime, 30);;

    let endTime;

    if (endDate === null) {
        endTime = shiftedTime;
    } else {
        endTime = new Date(endDate);
    }
    
    // Set the timePickers times
    startTimePicker.timepicker('setTime', startTime);
    endTimePicker.timepicker('setTime', endTime);

    // set the minimum time for the endTimePicker
    endTimePicker.timepicker('option', { 'minTime': getShortTimeString(shiftedTime) });

    // set the value of the hidden inputs, these values are the ones that will be used in the backEnd
    $("#startTimeHidden").attr('value', getShortTimeString(startTime));
    $("#endTimeHidden").attr('value', getShortTimeString(endTime));
}

// Set the allowed times for the end time drop down
// The end times are chosen based on the selected starttime
function onStartTimeChange() {
    // Get the selected value of the start time and end time
    let selectedTime = startTimePicker.timepicker('getTime');

    // Shift the selected start time by 30 minutes
    let shiftedTime = addMinutes(selectedTime, 30);

    // get the start time and the shifted(end) time in a "hh:mm" formatted string
    let selectedTimeString = getShortTimeString(selectedTime);
    let shiftedTimeString = getShortTimeString(shiftedTime);

    // Set the minimum allowed time for the end time
    endTimePicker.timepicker('option', { 'minTime': shiftedTimeString });

    // set the value of the hidden inputs, these values are the ones that will be used in the backEnd
    $("#startTimeHidden").attr('value', selectedTimeString);
    $("#endTimeHidden").attr('value', shiftedTimeString);

    endTimePicker.timepicker('setTime', shiftedTime);
}

function onEndTimeChange() {
    let selectedTime = endTimePicker.timepicker('getTime');

    $("#endTimeHidden").attr('value', getShortTimeString(selectedTime));
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

// add a specific amount of minutes to a date
function addMinutes(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}

///////// HELPERS /////////
function offsetDateTimezoneToJson(date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toJSON();
}

// (NOT BEING USED)
function offsetDateTimezone(date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
}

// (NOT BEING USED)
// check if a date is not in the past 
function validateDate(currentDate, dateToBeChecked) {
    return dateToBeChecked >= currentDate;
}

// (NOT BEING USED)
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