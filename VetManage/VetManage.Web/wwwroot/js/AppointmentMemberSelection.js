$(document).ready(function () {
    // Initialize tables
    let petsTable = $("#petsTable").DataTable({
        "pageLength": 5,
    });
    let vetsTable = $("#vetsTable").DataTable({
        "pageLength": 5,
    });

    $('#petsTable tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            petsTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    $('#vetsTable tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            vetsTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    // When user clicks the pet selection button
    $("#choosePetBtn").click(function () {
        // pet stuff
        // hide choose button and selected pet table
        // show pets table
        $("#choosePetBtnContainer").addClass("d-none");
        $("#petDataContainer").addClass("d-none");
        $("#petsTableContainer").removeClass("d-none");

        // vet stuff
        // hide choose button and selected pet table
        $("#chooseVetBtnContainer").addClass("d-none");
        $("#vetDataContainer").addClass("d-none");

        // remove error
        $("#PetId-error").remove();
    });

    // When user clicks the vet selection button
    $("#chooseVetBtn").click(function () {
        // vet stuff
        // hide choose button and selected vet table
        // show vets table
        $("#chooseVetBtnContainer").addClass("d-none");
        $("#vetDataContainer").addClass("d-none");
        $("#vetsTableContainer").removeClass("d-none");

        // pet stuff
        // hide choose button and selected pet table
        $("#choosePetBtnContainer").addClass("d-none");
        $("#petDataContainer").addClass("d-none");

        // remove error
        $("#VetId-error").remove();
    });

    // When user confirms the pet selection
    $("#confirmPetSelection").click(function () {
        let selectedRow = $("#petTableBody").find(".selected");
        let petId = selectedRow.data("petId");
        let pet = selectedRow.data("pet");

        // if the user has actually selected a pet
        if (pet !== undefined) {
            // set the pets values
            $("#petImage").css("background-image", `url(${pet.ImageFullPath})`);
            $("#petName").html(pet.Name);
            $("#petType").html(pet.Type);
            $("#petBreed").html(pet.Breed);

            // show selected pet table body and hide no selection message
            $("#petData").removeClass("d-none");
            $("#petSelectionMessage").addClass("d-none");

            // hide pets table
            // show selected pet and vet tables
            $("#petsTableContainer").addClass("d-none");
            $("#petDataContainer").removeClass("d-none");
            $("#vetDataContainer").removeClass("d-none");

            // show choose button
            $("#choosePetBtnContainer").removeClass("d-none");
            $("#chooseVetBtnContainer").removeClass("d-none");

            $("#PetId").attr('value', pet.Id);
        }
    });

    // When user confirms the vet selection
    $("#confirmVetSelection").click(function () {
        let selectedRow = $("#vetTableBody").find(".selected");
        let vet = selectedRow.data("vet");
        // if the user has actually selected a vet
        if (vet !== undefined) {
            // set the vets values
            $("#vetImage").css("background-image", `url(${vet.ImageFullPath})`);
            $("#vetFirstName").html(vet.FirstName);
            $("#vetLastName").html(vet.LastName);
            //$("#vetBreed").html(vet.Breed);  This will be the speciality

            // show selected vet table body and hide no selection message
            $("#vetData").removeClass("d-none");
            $("#vetSelectionMessage").addClass("d-none");

            // hide pets table
            // show selected pet and vet tables
            $("#vetsTableContainer").addClass("d-none");
            $("#vetDataContainer").removeClass("d-none");
            $("#petDataContainer").removeClass("d-none");

            // show choose buttons
            $("#chooseVetBtnContainer").removeClass("d-none");
            $("#choosePetBtnContainer").removeClass("d-none");

            $("#VetId").attr('value', vet.Id);
        }
    });
});