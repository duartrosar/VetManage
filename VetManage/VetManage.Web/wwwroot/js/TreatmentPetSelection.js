$(document).ready(function () {
    // Initialize tables
    let petsTable = $("#petsTable").DataTable({
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
        //$("#chooseVetBtnContainer").addClass("d-none");
        //$("#vetDataContainer").addClass("d-none");

        // remove error
        $("#PetId-error").remove();
    });

    // When user confirms the pet selection
    $("#confirmPetSelection").click(function () {
        let selectedRow = $("#petTableBody").find(".selected");
        let petId = selectedRow.data("petId");
        let pet = selectedRow.data("pet");

        // if the user has actually selected a pet
        if (pet !== undefined) {
            console.log(pet);
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
            //$("#vetDataContainer").removeClass("d-none");

            // show choose button
            $("#choosePetBtnContainer").removeClass("d-none");
            //$("#chooseVetBtnContainer").removeClass("d-none");

            $("#PetId").attr('value', pet.Id);
        }
    });

});