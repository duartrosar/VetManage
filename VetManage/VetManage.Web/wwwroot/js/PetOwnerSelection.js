$(document).ready(function () {
    // Initialize tables
    let ownersTable = $("#ownersTable").DataTable({
        "pageLength": 5,
    });

    $('#ownersTable tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            ownersTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    // When user clicks the owner selection button
    $("#chooseOwnerBtn").click(function () {
        // owner stuff
        // hide choose button and selected owner table
        // show owners table
        $("#chooseOwnerBtnContainer").addClass("d-none");
        $("#ownerDataContainer").addClass("d-none");
        $("#ownersTableContainer").removeClass("d-none");

        // remove error
        $("#OwnerId-error").remove();
    });

    // When user confirms the owner selection
    $("#confirmOwnerSelection").click(function () {
        let selectedRow = $("#ownerTableBody").find(".selected");
        let ownerId = selectedRow.data("ownerId");
        let owner = selectedRow.data("owner");

        // if the user has actually selected a owner
        if (owner !== undefined) {
            console.log(owner);
            // set the owners values
            $("#ownerImage").css("background-image", `url(${owner.ImageFullPath})`);
            $("#ownerFullName").html(owner.FullName);
            $("#ownerAddress").html(owner.Address);
            $("#ownerDateOfBirth").html(owner.DateOfBirth);

            // show selected owner table body and hide no selection message
            $("#ownerData").removeClass("d-none");
            $("#ownerSelectionMessage").addClass("d-none");

            // hide owners table
            // show selected owner and vet tables
            $("#ownersTableContainer").addClass("d-none");
            $("#ownerDataContainer").removeClass("d-none");

            // show choose button
            $("#chooseOwnerBtnContainer").removeClass("d-none");

            $("#OwnerId").attr('value', owner.Id);
        }
    });

});